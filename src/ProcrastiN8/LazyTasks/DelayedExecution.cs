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

    // Minimum allowed delay for delayed execution (ms)
    private const int MinDelayMs = 500;

    /// <summary>
    /// Executes a synchronous action after a delay, possibly with a snooze buffer and existential commentary.
    /// </summary>
    /// <param name="delay">How long to pretend to be busy before executing.</param>
    /// <param name="action">The action to eventually, reluctantly run.</param>
    /// <param name="snooze">Optional additional delay to hit before the main delay. For when even starting the delay feels premature.</param>
    /// <param name="logger">Optional logger for progress reports nobody asked for.</param>
    /// <param name="commentaryService">Optional commentary service for unsolicited observations. If not provided, a default is used.</param>
    /// <param name="cancellationToken">Token to cancel if productivity threatens to break out.</param>
    public static async Task RunAfterThinkingAboutIt(
        TimeSpan delay,
        Action action,
        TimeSpan? snooze = null,
        IProcrastiLogger? logger = null,
        ICommentaryService? commentaryService = null,
        CancellationToken cancellationToken = default)
    {
        if (delay < TimeSpan.FromMilliseconds(MinDelayMs))
        {
            throw new ArgumentOutOfExcusesException("Whoa, not so fast. This is ProcrastiN8, not ExecuteNow.");
        }

        logger ??= new DefaultLogger();
        commentaryService ??= new CommentaryService();
        var excuse = ExcuseService.GenerateExcuse();

        using var activity = ActivitySource.StartActivity("ProcrastiN8.DelayedExecution.Sync", ActivityKind.Internal);
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

        commentaryService.LogRandomRemark();

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
    /// <param name="delay">How long to stall before executing the async operation.</param>
    /// <param name="action">The async action to run, eventually.</param>
    /// <param name="snooze">Optional extra buffer before the main delay, for the indecisive procrastinator.</param>
    /// <param name="logger">Optional logger for earnest status updates.</param>
    /// <param name="commentaryService">Optional commentary service for mid-wait philosophical interjections. If not provided, a default is used.</param>
    /// <param name="cancellationToken">Token to cancel before any work is done. Recommended.</param>
    public static async Task RunWhenYouFeelLikeIt(
        TimeSpan delay,
        Func<Task> action,
        TimeSpan? snooze = null,
        IProcrastiLogger? logger = null,
        ICommentaryService? commentaryService = null,
        CancellationToken cancellationToken = default)
    {
        if (delay < TimeSpan.FromMilliseconds(MinDelayMs))
        {
            throw new ArgumentOutOfExcusesException("This task is trying way too hard to be on time.");
        }

        logger ??= new DefaultLogger();
        commentaryService ??= new CommentaryService();
        var excuse = ExcuseService.GenerateExcuse();

        using var activity = ActivitySource.StartActivity("ProcrastiN8.DelayedExecution.Async", ActivityKind.Internal);
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

        commentaryService.LogRandomRemark();

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