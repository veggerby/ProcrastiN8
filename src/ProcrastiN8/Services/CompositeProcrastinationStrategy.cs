using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

/// <summary>
/// Executes a sequence of procrastination strategies in order; only the final phase is allowed to run the real task.
/// </summary>
/// <remarks>
/// Earlier phases are passed a no-op task to preserve their ceremonial delay rituals without prematurely achieving productivity.
/// </remarks>
public sealed class CompositeProcrastinationStrategy(params IProcrastinationStrategy[] phases) : ProcrastinationStrategyBase
{
    private readonly IReadOnlyList<IProcrastinationStrategy> _phases = phases?.Length > 0 ? phases : throw new ArgumentException("At least one phase required", nameof(phases));

    protected override async Task ExecuteCoreAsync(
        Func<Task> task,
        TimeSpan initialDelay,
        IExcuseProvider? excuseProvider,
        IDelayStrategy delayStrategy,
        IRandomProvider randomProvider,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        for (var i = 0; i < _phases.Count; i++)
        {
            var phase = _phases[i];
            var isLast = i == _phases.Count - 1;
            var phaseTask = isLast ? task : static () => Task.CompletedTask;
            await phase.ExecuteAsync(phaseTask, initialDelay, excuseProvider, delayStrategy, randomProvider, timeProvider, cancellationToken);

            if (phase is IResultReportingProcrastinationStrategy r)
            {
                // Aggregate metrics
                for (int c = 0; c < r.LastResult.Cycles; c++) { IncrementCycle(); }
                // Avoid artificially inflating excuse counts; no-op increment removed.
                if (isLast && r.LastResult.Executed)
                {
                    MarkExecuted();
                }
            }
        }

        if (!_phases.OfType<IResultReportingProcrastinationStrategy>().Any(p => p.LastResult.Executed))
        {
            // Ensure execution is recorded if last phase did not mark it (e.g., non-reporting strategy)
            MarkExecuted();
        }
    }
}