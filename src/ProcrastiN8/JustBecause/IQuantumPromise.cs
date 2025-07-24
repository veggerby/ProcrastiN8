namespace ProcrastiN8.JustBecause;

/// <summary>
/// This interface exists solely to appease unit test frameworks.
/// It defeats the purpose of quantum uncertainty, but here we are.
/// </summary>
public interface IQuantumPromise<T>
{
    Task<T> ObserveAsync(CancellationToken cancellationToken = default);
}
