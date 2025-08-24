using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

/// <summary>
/// A strategy that conceptually re-estimates start time in perpetual five-minute increments without ever executing the task.
/// </summary>
/// <remarks>
/// In production metaphors it would wait real minutes; in this implementation it uses micro-delays (~10ms) plus an absolute
/// deadline to remain test-friendly and deterministic. The underlying task is only invoked if externally forced via a control
/// handle trigger. This affirms the purity of unstarted work while still terminating promptly under test safety caps.
/// </remarks>
public class InfiniteEstimationStrategy : ProcrastinationStrategyBase
{
    /// <summary>Default absolute deadline offset (keeps strategy from looping forever in test environments).</summary>
    public static readonly TimeSpan DefaultAbsoluteDeadlineOffset = TimeSpan.FromSeconds(2);
    /// <summary>Tiny synthetic per-iteration delay to avoid busy looping while preserving rapid test execution.</summary>
    public static readonly TimeSpan DefaultMicroDelay = TimeSpan.FromMilliseconds(10);

    private readonly TimeSpan _absoluteDeadlineOffset;
    private readonly TimeSpan _microDelay;

    /// <summary>
    /// Creates an infinite estimation strategy with optional overrides for deterministic test or production tuning.
    /// </summary>
    public InfiniteEstimationStrategy(TimeSpan? absoluteDeadlineOffset = null, TimeSpan? microDelay = null)
    {
        _absoluteDeadlineOffset = absoluteDeadlineOffset ?? DefaultAbsoluteDeadlineOffset;
        _microDelay = microDelay ?? DefaultMicroDelay;
    }
    protected override async Task ExecuteCoreAsync(
        Func<Task> task,
        TimeSpan initialDelay,
        IExcuseProvider? excuseProvider,
        IDelayStrategy delayStrategy,
        IRandomProvider randomProvider,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        if (delayStrategy is null)
        {
            throw new ArgumentNullException(nameof(delayStrategy));
        }
        if (excuseProvider is null)
        {
            // Still acceptable; proceed without ceremonial justification.
        }

    var absoluteDeadline = StartUtc + _absoluteDeadlineOffset;
        while (!cancellationToken.IsCancellationRequested)
        {
            if (CheckForExternalOverride(task)) { return; }
            await InvokeExcuseAsync(excuseProvider);
            IncrementCycle();
            await Task.Yield();
            // Use a tiny synthetic delay in library context to avoid real multi-minute waits.
            await delayStrategy.DelayAsync(_microDelay, _microDelay, cancellationToken: cancellationToken);
            await NotifyCycleAsync(ControlContext, cancellationToken);
            if (SafetyCapReached() || timeProvider.GetUtcNow() >= absoluteDeadline) { return; }
        }
    }
}