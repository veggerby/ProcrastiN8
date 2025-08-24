using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

/// <summary>
/// A strategy that continuously re-estimates work in fixed five minute increments without ever executing the task.
/// </summary>
/// <remarks>
/// This intentionally never invokes the supplied task, affirming the purity of unstarted work.
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
