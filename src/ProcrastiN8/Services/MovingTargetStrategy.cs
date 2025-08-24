using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

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
