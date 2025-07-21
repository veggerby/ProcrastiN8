using System.Diagnostics;

using ProcrastiN8.Metrics;
using ProcrastiN8.Services;

namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Provides methods to delay execution in a dramatically overthought and lazy way.
/// </summary>
public static class DelayedExecution
{
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.LazyTasks.DelayedExecution");

    private static readonly ExcuseService ExcuseService = new();
    private static readonly DelayService DelayService = new();
    private static readonly CommentaryService CommentaryService = new();

    // Minimum allowed delay for delayed execution (ms)
    private const int MinDelayMs = 500;

    /// <summary>
    /// Executes a synchronous action after a delay, possibly with a snooze buffer and existential commentary.
    /// </summary>
    public static async Task RunAfterThinkingAboutIt(
        TimeSpan delay,
        Action action,
        TimeSpan? snooze = null,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        if (delay < TimeSpan.FromMilliseconds(MinDelayMs))
        {
            throw new ArgumentOutOfExcusesException("Whoa, not so fast. This is ProcrastiN8, not ExecuteNow.");
        }

        logger ??= new DefaultLogger();
        var excuse = ExcuseService.GenerateExcuse();

        using var activity = ActivitySource.StartActivity("DelayedExecution.Sync", ActivityKind.Internal);
        activity?.SetTag("procrastination.delay.ms", delay.TotalMilliseconds);
        activity?.SetTag("procrastination.snooze.ms", snooze?.TotalMilliseconds);
        activity?.SetTag("procrastination.excuse", excuse);

        logger.Info("[DelayedExecution] Preparing to ignore the task for {DelaySeconds:0.0}s.", delay.TotalSeconds);
        logger.Info("[DelayedExecution] Reason for delay: {Excuse}", excuse);

        if (snooze is not null)
        {
            logger.Info("[DelayedExecution] Hitting the snooze button for {SnoozeSeconds:0.0}s...", snooze.Value.TotalSeconds);
            await DelayService.DelayWithProcrastinationAsync("snooze", snooze.Value, cancellationToken);
        }

        CommentaryService.LogRandomRemark();

        await DelayService.DelayWithProcrastinationAsync(excuse, delay, cancellationToken);

        try
        {
            action();
            logger.Info("[DelayedExecution] Task executed. It wasn't that bad, was it?");
            ProcrastinationMetrics.TasksCompleted.Add(1);
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "[DelayedExecution] Task failed — but honestly, you kind of expected that.");
            ProcrastinationMetrics.TasksNeverDone.Add(1);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Executes an asynchronous action after a delay, possibly with a snooze buffer and spiritual detachment.
    /// </summary>
    public static async Task RunWhenYouFeelLikeIt(
        TimeSpan delay,
        Func<Task> action,
        TimeSpan? snooze = null,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        if (delay < TimeSpan.FromMilliseconds(MinDelayMs))
        {
            throw new ArgumentOutOfExcusesException("This task is trying way too hard to be on time.");
        }

        logger ??= new DefaultLogger();
        var excuse = ExcuseService.GenerateExcuse();

        using var activity = ActivitySource.StartActivity("DelayedExecution.Async", ActivityKind.Internal);
        activity?.SetTag("procrastination.delay.ms", delay.TotalMilliseconds);
        activity?.SetTag("procrastination.snooze.ms", snooze?.TotalMilliseconds);
        activity?.SetTag("procrastination.excuse", excuse);

        logger.Info("[DelayedExecution] Putting off task execution for {DelaySeconds:0.0}s.", delay.TotalSeconds);
        logger.Info("[DelayedExecution] Reason for delay: {Excuse}", excuse);

        if (snooze is not null)
        {
            logger.Debug("[DelayedExecution] Snoozing for {SnoozeSeconds:0.0}s...", snooze.Value.TotalSeconds);
            await DelayService.DelayWithProcrastinationAsync("snooze", snooze.Value, cancellationToken);
        }

        CommentaryService.LogRandomRemark();

        await DelayService.DelayWithProcrastinationAsync(excuse, delay, cancellationToken);

        try
        {
            await action();
            logger.Info("[DelayedExecution] Async task completed. No alarms, no surprises.");
            ProcrastinationMetrics.TasksCompleted.Add(1);
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "[DelayedExecution] Async task derailed. That's tomorrow's problem.");
            ProcrastinationMetrics.TasksNeverDone.Add(1);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}