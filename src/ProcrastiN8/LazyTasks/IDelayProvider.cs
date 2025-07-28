namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Provides an abstraction for introducing delays, decoupling the concept of stalling from its implementation.
/// </summary>
/// <remarks>
/// Implementations may use <see cref="Task.Delay"/>, custom schedulers, or quantum uncertainty.
/// </remarks>
public interface IDelayProvider
{
    /// <summary>
    /// Delays execution for the specified duration.
    /// </summary>
    /// <param name="delay">The duration to delay.</param>
    /// <param name="cancellationToken">A token to cancel the delay.</param>
    /// <returns>A task representing the delay.</returns>
    Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken = default);
}
