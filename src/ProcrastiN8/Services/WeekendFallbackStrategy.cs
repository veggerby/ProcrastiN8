using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

/// <summary>
/// A strategy that defers execution until a culturally convenient weekend window or a hard 72 hour cap elapses.
/// </summary>
/// <remarks>
/// This is meant to simulate the rationalization process of deferring tasks to a mythical "later" that is always Saturday afternoon.
/// </remarks>
public class WeekendFallbackStrategy : ProcrastinationStrategyBase
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
        if (task is null)
        {
            throw new ArgumentNullException(nameof(task));
        }
        if (delayStrategy is null)
        {
            throw new ArgumentNullException(nameof(delayStrategy));
        }
        if (timeProvider is null)
        {
            throw new ArgumentNullException(nameof(timeProvider));
        }

        var start = timeProvider.GetUtcNow();

        while (!cancellationToken.IsCancellationRequested)
        {
            if (await CheckForExternalOverrideAsync(task)) { return; }
            var now = timeProvider.GetUtcNow();
            var elapsed = now - start;

            var weekendWindow = now.DayOfWeek == DayOfWeek.Saturday && now.Hour >= 15;
            var maxElapsedReached = elapsed >= TimeSpan.FromHours(72);

            if (weekendWindow || maxElapsedReached)
            {
                await task();
                MarkExecuted();
                return;
            }

            await InvokeExcuseAsync(excuseProvider);
            IncrementCycle();
            await delayStrategy.DelayAsync(TimeSpan.FromHours(1), cancellationToken: cancellationToken);
            await NotifyCycleAsync(ControlContext, cancellationToken);
            if (SafetyCapReached()) { return; }
        }
    }
}
