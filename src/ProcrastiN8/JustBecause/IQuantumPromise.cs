namespace ProcrastiN8.JustBecause;

/// <summary>
/// This interface exists solely to appease unit test frameworks.
/// It defeats the purpose of quantum uncertainty, but here we are.
/// </summary>
public interface IQuantumPromise<T>
{
    /// <summary>
    /// Gets the time when the promise was created.
    /// </summary>
    DateTimeOffset CreationTime { get; }

    /// <summary>
    /// Gets the value of the promise, if resolved.
    /// </summary>
    T Value { get; }

    Task<T> ObserveAsync(CancellationToken cancellationToken = default);
}