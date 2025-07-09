using System.Diagnostics;

using ProcrastiN8.Metrics;
using ProcrastiN8.Services;

namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Retries an action until it succeeds... or gets cancelled... or loses the will to keep trying.
/// </summary>
public static class RetryUntilCancelled
{
    private static readonly Random Rng = new();
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.LazyTasks.RetryUntilCancelled");

    private static readonly ExcuseService ExcuseService = new();
    private static readonly DelayService DelayService = new();

    /// <summary>
    /// Keeps retrying the action until it works or someone stops the madness.
    /// </summary>
    public static async Task RunForever(
        Func<Task> action,
        TimeSpan? initialDelay = null,
        int? maxRetries = null,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        initialDelay ??= TimeSpan.FromMilliseconds(500);
        logger ??= new DefaultLogger();
        var attempt = 0;

        using var activity = ActivitySource.StartActivity("RetryUntilCancelled.RunForever", ActivityKind.Internal);
        activity?.SetTag("retry.initialDelay.ms", initialDelay.Value.TotalMilliseconds);
        activity?.SetTag("retry.maxRetries", maxRetries ?? -1);

        while (!cancellationToken.IsCancellationRequested)
        {
            attempt++;
            ProcrastinationMetrics.RetryAttempts.Add(1);
            logger.Info("[Retry] Attempt #{Attempt}...", attempt);

            try
            {
                await action();
                logger.Info("[Retry] 🎉 Success on attempt #{Attempt}!", attempt);
                ProcrastinationMetrics.TasksCompleted.Add(1);
                activity?.SetStatus(ActivityStatusCode.Ok);
                return;
            }
            catch (Exception ex)
            {
                var excuse = ExcuseService.GenerateExcuse();
                logger.Error(ex, "[Retry] Attempt #{Attempt} failed. {Excuse}", attempt, excuse);

                if (maxRetries.HasValue && attempt >= maxRetries.Value)
                {
                    logger.Error(ex, "[Retry] 😵 Gave up after {Attempt} attempts.", attempt);
                    ProcrastinationMetrics.TasksNeverDone.Add(1);
                    activity?.SetStatus(ActivityStatusCode.Error, "Max retries exceeded.");
                    throw new RetryExhaustedException(attempt, ex);
                }

                var delay = GetBackoffDelay(initialDelay.Value, attempt);
                logger.Debug("[Retry] Waiting {DelayMs:0}ms before next attempt...", delay.TotalMilliseconds);

                await DelayService.DelayWithProcrastinationAsync("retry-backoff", delay, cancellationToken);
            }
        }

        logger.Info("[Retry] Operation cancelled. Probably for the best.");
        activity?.SetStatus(ActivityStatusCode.Ok, "Cancelled before success");
    }

    private static TimeSpan GetBackoffDelay(TimeSpan initial, int attempt)
    {
        // Exponential backoff with jitter
        var backoff = Math.Min(initial.TotalMilliseconds * Math.Pow(2, attempt - 1), 60_000);
        var jitter = Rng.NextDouble() * 500;
        return TimeSpan.FromMilliseconds(backoff + jitter);
    }

    public class RetryExhaustedException(int attempts, Exception lastError) : Exception($"All {attempts} retry attempts failed.", lastError)
    {
        public int Attempts { get; } = attempts;
    }
}