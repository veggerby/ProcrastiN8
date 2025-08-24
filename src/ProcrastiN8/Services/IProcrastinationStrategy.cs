using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

/// <summary>
/// Defines the contract for a procrastination execution strategy.
/// </summary>
/// <remarks>
/// Implementations orchestrate elaborate deferral patterns prior to the optional commencement of meaningful work.
/// </remarks>
public interface IProcrastinationStrategy
{
    /// <summary>
    /// Executes the procrastination workflow and optionally the supplied task according to the strategy semantics.
    /// </summary>
    /// <param name="task">The task to eventually (or never) execute.</param>
    /// <param name="initialDelay">An initial delay hint; semantics vary per strategy.</param>
    /// <param name="excuseProvider">Optional provider of ceremonial justifications.</param>
    /// <param name="delayStrategy">Delay strategy abstraction for time deferral.</param>
    /// <param name="randomProvider">Randomness source for stochastic hesitation patterns.</param>
    /// <param name="timeProvider">Time provider used for deterministic temporal logic.</param>
    /// <param name="cancellationToken">Cancellation token to abort further deferral.</param>
    Task ExecuteAsync(
        Func<Task> task,
        TimeSpan initialDelay,
        IExcuseProvider? excuseProvider,
        IDelayStrategy delayStrategy,
        IRandomProvider randomProvider,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken);
}
