using System.Diagnostics;

using ProcrastiN8.Metrics;
using ProcrastiN8.Services;

namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Provides methods to execute asynchronous actions after a random delay, simulating procrastination with logging, tracing, and metrics.
/// </summary>
public static class Eventually
{
    // Default maximum delay for 'eventually' procrastination (seconds)
    private const int DefaultMaxDelaySeconds = 30;

    /// <summary>
    /// Activity source for distributed tracing of procrastination events.
    /// </summary>
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.LazyTasks.Eventually");

    /// <summary>
    /// Service for introducing artificial delays with procrastination flavor.
    /// </summary>
    private static readonly DelayService DelayService = new();
    /// <summary>
    /// Service for generating excuses for procrastination.
    /// </summary>
    private static readonly ExcuseService ExcuseService = new();
    /// <summary>
    /// Service for generating and logging random commentary during procrastination.
    /// </summary>
    private static readonly CommentaryService CommentaryService = new();

    /// <summary>
    /// Executes an async action after a random delay, with logging, tracing, and metrics support.
    /// </summary>
    /// <param name="action">The asynchronous action to execute after the delay.</param>
    /// <param name="within">The maximum delay before execution. Defaults to 30 seconds if not specified.</param>
    /// <param name="excuse">Optional excuse for the delay. If not provided, one will be generated.</param>
    /// <param name="logger">Logger for progress and commentary. If not provided, a default logger is used.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    public static async Task Do(
        Func<Task> action,
        TimeSpan? within = null,
        string? excuse = null,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        logger ??= new DefaultLogger();

        var maxDelay = within ?? TimeSpan.FromSeconds(DefaultMaxDelaySeconds);
        var delay = GetDelay(maxDelay);

        excuse ??= ExcuseService.GenerateExcuse(); // Now tracked in metrics
        ProcrastinationMetrics.DelaysTotal.Add(1);
        ProcrastinationMetrics.TotalTimeProcrastinated.Add((long)delay.TotalSeconds,
            KeyValuePair.Create<string, object?>("reason", excuse));

        using var activity = ActivitySource.StartActivity("Eventually.Do", ActivityKind.Internal);
        activity?.AddTag("eventually.delay.ms", delay.TotalMilliseconds);
        activity?.AddTag("eventually.excuse", excuse);

        logger.Info("Eventually scheduled in {DelaySeconds:0.0}s. Reason: {Excuse}", delay.TotalSeconds, excuse);

        var chatter = ProcrastinationChatter(delay, logger, cancellationToken);

        try
        {
            await DelayService.DelayWithProcrastinationAsync(excuse, delay, cancellationToken);
            await action();

            ProcrastinationMetrics.TasksCompleted.Add(1);
            logger.Info("Eventually completed the task.");
        }
        catch (Exception ex)
        {
            ProcrastinationMetrics.TasksNeverDone.Add(1);
            logger.Error(ex, "Eventually failed to complete the task.");
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        finally
        {
            chatter.Dispose();
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
    }

    /// <summary>
    /// Returns a random delay up to the specified maximum.
    /// </summary>
    /// <param name="max">The maximum delay.</param>
    /// <returns>A random TimeSpan between 0 and max.</returns>
    private static TimeSpan GetDelay(TimeSpan max)
    {
        var jitter = new Random().NextDouble() * max.TotalMilliseconds;
        return TimeSpan.FromMilliseconds(jitter);
    }

    /// <summary>
    /// Starts a timer that logs random commentary during the procrastination delay.
    /// </summary>
    /// <param name="delay">The total delay duration.</param>
    /// <param name="logger">Logger for commentary.</param>
    /// <param name="cancellationToken">Token to cancel commentary.</param>
    /// <returns>A Timer that can be disposed to stop commentary.</returns>
    private static Timer ProcrastinationChatter(TimeSpan delay, IProcrastiLogger logger, CancellationToken cancellationToken)
    {
        var timer = new Timer(_ =>
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                CommentaryService.LogRandomRemark(); // Also tracks metric
            }
        }, null, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5));

        return timer;
    }
}