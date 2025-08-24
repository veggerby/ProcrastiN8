using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

public static class ProcrastinationScheduler
{
    public static async Task Schedule(
        Func<Task> task,
        TimeSpan initialDelay,
        ProcrastinationMode mode,
        IExcuseProvider? excuseProvider = null,
        IDelayStrategy? delayStrategy = null,
        IRandomProvider? randomProvider = null,
        ITimeProvider? timeProvider = null,
        CancellationToken cancellationToken = default)
    {
        delayStrategy ??= new DefaultDelayStrategy();
        randomProvider ??= RandomProvider.Default;
        timeProvider ??= SystemTimeProvider.Default;

        IProcrastinationStrategy strategy;

        switch (mode)
        {
            case ProcrastinationMode.MovingTarget:
                strategy = new MovingTargetStrategy();
                break;
            case ProcrastinationMode.InfiniteEstimation:
                strategy = new InfiniteEstimationStrategy();
                break;
            case ProcrastinationMode.WeekendFallback:
                strategy = new WeekendFallbackStrategy();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }

        await strategy.ExecuteAsync(task, initialDelay, excuseProvider, delayStrategy, randomProvider, timeProvider, cancellationToken);
    }
}
