using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

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
