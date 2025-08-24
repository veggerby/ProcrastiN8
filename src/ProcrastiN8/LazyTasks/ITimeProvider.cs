namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Provides an abstraction for current UTC time retrieval, enabling testable and mockable time passage in delay strategies.
/// </summary>
/// <remarks>
/// Implementations can simulate, accelerate, or otherwise manipulate the perception of time for stalling operations.
/// </remarks>
public interface ITimeProvider
{
    /// <summary>Returns the current UTC timestamp (mockable).</summary>
    DateTimeOffset GetUtcNow();
}