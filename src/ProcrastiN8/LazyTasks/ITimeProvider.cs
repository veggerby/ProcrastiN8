namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Provides an abstraction for time measurement, enabling testable and mockable time passage in delay strategies.
/// </summary>
/// <remarks>
/// Implementations can simulate, accelerate, or otherwise manipulate the perception of time for stalling operations.
/// </remarks>
public interface ITimeProvider
{
    /// <summary>
    /// Asynchronously waits for the specified duration, honoring cancellation.
    /// </summary>
    /// <param name="delay">The duration to wait.</param>
    /// <param name="cancellationToken">A token to cancel the wait.</param>
    DateTimeOffset GetUtcNow();
}