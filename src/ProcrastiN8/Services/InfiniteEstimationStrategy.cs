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

        var absoluteDeadline = StartUtc + TimeSpan.FromSeconds(2);
        while (!cancellationToken.IsCancellationRequested)
        {
            if (CheckForExternalOverride(task)) { return; }
            await InvokeExcuseAsync(excuseProvider);
            IncrementCycle();
            await Task.Yield();
            // Use a tiny synthetic delay in library context to avoid real multi-minute waits.
            await delayStrategy.DelayAsync(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10), cancellationToken: cancellationToken);
            await NotifyCycleAsync(ControlContext, cancellationToken);
            if (SafetyCapReached() || timeProvider.GetUtcNow() >= absoluteDeadline) { return; }
        }
    }
}
