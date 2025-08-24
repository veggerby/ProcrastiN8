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
    private readonly IPacingGrowthPolicy _growth;
    private readonly IDelayCeilingPolicy _ceiling;
    private readonly Random _rng;

    public MovingTargetStrategy(
        IPacingGrowthPolicy? growth = null,
        IDelayCeilingPolicy? ceiling = null,
        Random? rng = null)
    {
        _growth = growth ?? DefaultPacingGrowthPolicy.Instance;
        _ceiling = ceiling ?? DefaultDelayCeilingPolicy.Instance;
        _rng = rng ?? new Random();
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

        var delay = initialDelay <= TimeSpan.Zero ? TimeSpan.FromMilliseconds(5) : initialDelay;
        var absoluteDeadline = StartUtc + TimeSpan.FromSeconds(2); // hard cap to keep tests short

        var cycles = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            if (CheckForExternalOverride(task)) { return; }
            await Task.Yield();
            var bounded = delay > TimeSpan.FromMilliseconds(50) ? TimeSpan.FromMilliseconds(50) : delay;
            await delayStrategy.DelayAsync(bounded, bounded, cancellationToken: cancellationToken);
            IncrementCycle();
            await NotifyCycleAsync(ControlContext, cancellationToken);
            cycles++;
            delay = _growth.Next(delay, cycles - 1, _rng);
            if (delay > TimeSpan.FromHours(1))
            {
                delay = TimeSpan.FromHours(1);
            }

            await InvokeExcuseAsync(excuseProvider);

            var now = timeProvider.GetUtcNow();
            if (_ceiling.ShouldCease(delay, cycles, now - StartUtc) || SafetyCapReached() || now >= absoluteDeadline)
            {
                break; // Mission accomplished: start time sufficiently distant.
            }
        }
        await task();
        MarkExecuted();
    }
}