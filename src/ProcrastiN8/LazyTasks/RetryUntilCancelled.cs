using System.Diagnostics;

using ProcrastiN8.Metrics;
using ProcrastiN8.Services;

namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Retries an action until it succeeds, is cancelled, or exhausts retry attempts.
/// </summary>
public static class RetryUntilCancelled
{
    // Random number generator for retry jitter
    private static readonly Random Rng = new();
    // Activity source for tracing retry operations
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.LazyTasks.RetryUntilCancelled");

    // Service for generating excuses (used in retry commentary)
    private static readonly ExcuseService ExcuseService = new();
    // Service for delaying with procrastination
    private static readonly DelayService DelayService = new();

    // Default initial delay for retry logic (ms)
    private const int DefaultInitialDelayMs = 500;
    // Maximum backoff delay (ms) for exponential backoff
    private const int MaxBackoffDelayMs = 60_000;
    // Maximum jitter to add to backoff (ms)
    private const int MaxJitterMs = 500;

    /// <summary>
    /// Keeps retrying the action until it works, is cancelled, or max retries is reached.
    /// </summary>
    public static async Task RunForever(
        Func<Task> action,
        TimeSpan? initialDelay = null,
        int? maxRetries = null,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        logger ??= new DefaultLogger();
        initialDelay ??= TimeSpan.FromMilliseconds(DefaultInitialDelayMs);
        int attempt = 0;
        using var activity = ActivitySource.StartActivity("ProcrastiN8.RetryUntilCancelled.RunForever", ActivityKind.Internal);

        activity?.SetTag("retry.initialDelay.ms", initialDelay.Value.TotalMilliseconds);
        activity?.SetTag("retry.maxRetries", maxRetries ?? -1);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                attempt++;
                ProcrastinationMetrics.RetryAttempts.Add(1);
                logger.Info("[Retry] Attempt #{Attempt}...", attempt);

                await action();
                logger.Info("[Retry] 🎉 Success on attempt #{Attempt}!", attempt);
                ProcrastinationMetrics.TasksCompleted.Add(1);
                activity?.SetStatus(ActivityStatusCode.Ok, $"Succeeded after {attempt} attempts");
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
                    activity?.SetStatus(ActivityStatusCode.Error, $"Failed after {attempt} attempts");
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

    /// <summary>
    /// Calculates exponential backoff delay with random jitter.
    /// </summary>
    private static TimeSpan GetBackoffDelay(TimeSpan initial, int attempt)
    {
        // Exponential backoff with jitter
        var backoff = Math.Min(initial.TotalMilliseconds * Math.Pow(2, attempt - 1), MaxBackoffDelayMs);
        var jitter = Rng.NextDouble() * MaxJitterMs;
        return TimeSpan.FromMilliseconds(backoff + jitter);
    }

    /// <summary>
    /// Exception thrown when all retry attempts are exhausted.
    /// </summary>
    public class RetryExhaustedException(int attempts, Exception lastError) : Exception($"All {attempts} retry attempts failed.", lastError)
    {
        /// <summary>
        /// Number of attempts made before giving up.
        /// </summary>
        public int Attempts { get; } = attempts;
    }
}