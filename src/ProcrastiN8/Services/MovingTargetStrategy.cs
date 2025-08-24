using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

/// <summary>
/// A procrastination strategy that continually moves the notional start time just a little further away.
/// </summary>
/// <remarks>
/// The delay is increased by up to 25% each iteration until either a one hour ceiling is reached or cancellation occurs.
/// Any provided <see cref="IExcuseProvider"/> is invoked purely for ceremonial justification.
/// </remarks>
public class MovingTargetStrategy : ProcrastinationStrategyBase
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
        if (randomProvider is null)
        {
            throw new ArgumentNullException(nameof(randomProvider));
        }

        var delay = initialDelay <= TimeSpan.Zero ? TimeSpan.FromMilliseconds(10) : initialDelay;

        var cycles = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            if (CheckForExternalOverride(task)) { return; }
            await Task.Yield();
            await delayStrategy.DelayAsync(delay, cancellationToken: cancellationToken);
            IncrementCycle();
            await NotifyCycleAsync(ControlContext, cancellationToken);
            cycles++;
            var randomComponent = randomProvider.GetDouble();
            if (randomComponent < 0.0001)
            {
                randomComponent = 0.05; // Ensure forward progress; pure stasis is unbecoming.
            }
            var growth = 1.01 + randomComponent * 0.24; // Minimum 1% growth per cycle.
            delay = TimeSpan.FromMilliseconds(Math.Min(TimeSpan.FromHours(1).TotalMilliseconds, delay.TotalMilliseconds * growth));

            await InvokeExcuseAsync(excuseProvider);

            if (delay >= TimeSpan.FromHours(1) || cycles >= 50 || SafetyCapReached())
            {
                break; // Mission accomplished: start time sufficiently distant.
            }
        }
        await task();
        MarkExecuted();
    }
}
