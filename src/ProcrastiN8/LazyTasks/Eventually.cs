using System.Diagnostics;

using ProcrastiN8.Metrics;
using ProcrastiN8.Services;

namespace ProcrastiN8.LazyTasks;

public static class Eventually
{
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.LazyTasks.Eventually");

    private static readonly DelayService DelayService = new();
    private static readonly ExcuseService ExcuseService = new();
    private static readonly CommentaryService CommentaryService = new();

    /// <summary>
    /// Executes an async action after a random delay, with logging, tracing, and metrics support.
    /// </summary>
    public static async Task Do(
        Func<Task> action,
        TimeSpan? within = null,
        string? excuse = null,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        logger ??= new DefaultLogger();

        var maxDelay = within ?? TimeSpan.FromSeconds(30);
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

    private static TimeSpan GetDelay(TimeSpan max)
    {
        var jitter = new Random().NextDouble() * max.TotalMilliseconds;
        return TimeSpan.FromMilliseconds(jitter);
    }

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