using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

/// <summary>
/// Chooses between two procrastination strategies based on a predicate evaluated at scheduling time.
/// </summary>
public sealed class ConditionalProcrastinationStrategy(
    IProcrastinationStrategy ifTrue,
    IProcrastinationStrategy ifFalse,
    Func<ITimeProvider, bool> predicate) : ProcrastinationStrategyBase
{
    private readonly IProcrastinationStrategy _ifTrue = ifTrue ?? throw new ArgumentNullException(nameof(ifTrue));
    private readonly IProcrastinationStrategy _ifFalse = ifFalse ?? throw new ArgumentNullException(nameof(ifFalse));
    private readonly Func<ITimeProvider, bool> _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

    protected override async Task ExecuteCoreAsync(
        Func<Task> task,
        TimeSpan initialDelay,
        IExcuseProvider? excuseProvider,
        IDelayStrategy delayStrategy,
        IRandomProvider randomProvider,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var chosen = _predicate(timeProvider) ? _ifTrue : _ifFalse;
        await chosen.ExecuteAsync(task, initialDelay, excuseProvider, delayStrategy, randomProvider, timeProvider, cancellationToken);
        if (chosen is IResultReportingProcrastinationStrategy r)
        {
            if (r.LastResult.Executed)
            {
                MarkExecuted();
                for (int c = 0; c < r.LastResult.Cycles; c++) { IncrementCycle(); }
                // Do not re-invoke excuses to avoid double counting.
            }
        }
        else
        {
            MarkExecuted();
        }
    }
}