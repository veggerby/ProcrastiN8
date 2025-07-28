namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Defines a strategy for introducing arbitrary, configurable delays.
/// </summary>
/// <remarks>
/// Implementations should provide testable, injectable delay logic for stalling execution.
/// </remarks>
public interface IDelayStrategy
{
    /// <summary>
    /// Delays execution according to the strategy's configuration.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the delay.</param>
    Task DelayAsync(CancellationToken cancellationToken = default);
}