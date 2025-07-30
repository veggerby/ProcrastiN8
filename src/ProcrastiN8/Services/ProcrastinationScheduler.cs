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

public interface IProcrastinationStrategy
{
    Task ExecuteAsync(
        Func<Task> task,
        TimeSpan initialDelay,
        IExcuseProvider? excuseProvider,
        IDelayStrategy delayStrategy,
        IRandomProvider randomProvider,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken);
}

public class MovingTargetStrategy : IProcrastinationStrategy
{
    public async Task ExecuteAsync(
        Func<Task> task,
        TimeSpan initialDelay,
        IExcuseProvider? excuseProvider,
        IDelayStrategy delayStrategy,
        IRandomProvider randomProvider,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var delay = initialDelay;

        while (!cancellationToken.IsCancellationRequested)
        {
            await delayStrategy.DelayAsync(delay, cancellationToken: cancellationToken);
            delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * (1 + randomProvider.GetDouble() * 0.25));

            if (excuseProvider != null)
            {
                await excuseProvider.GetExcuseAsync();
            }

            if (delay.TotalMilliseconds > TimeSpan.FromHours(1).TotalMilliseconds)
            {
                break;
            }
        }

        await task();
    }
}

public class InfiniteEstimationStrategy : IProcrastinationStrategy
{
    public async Task ExecuteAsync(
        Func<Task> task,
        TimeSpan initialDelay,
        IExcuseProvider? excuseProvider,
        IDelayStrategy delayStrategy,
        IRandomProvider randomProvider,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (excuseProvider != null)
            {
                await excuseProvider.GetExcuseAsync();
            }
            await delayStrategy.DelayAsync(TimeSpan.FromMinutes(5), cancellationToken: cancellationToken);
        }
    }
}

public class WeekendFallbackStrategy : IProcrastinationStrategy
{
    public async Task ExecuteAsync(
        Func<Task> task,
        TimeSpan initialDelay,
        IExcuseProvider? excuseProvider,
        IDelayStrategy delayStrategy,
        IRandomProvider randomProvider,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var now = timeProvider.GetUtcNow();
            if (now.DayOfWeek == DayOfWeek.Saturday && now.Hour >= 15 ||
                now - timeProvider.GetUtcNow() >= TimeSpan.FromHours(72))
            {
                await task();
                return;
            }

            if (excuseProvider != null)
            {
                await excuseProvider.GetExcuseAsync();
            }
            await delayStrategy.DelayAsync(TimeSpan.FromHours(1), cancellationToken: cancellationToken);
        }
    }
}
