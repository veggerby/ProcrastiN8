using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

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
