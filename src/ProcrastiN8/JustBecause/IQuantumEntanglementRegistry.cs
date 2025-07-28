namespace ProcrastiN8.JustBecause;

/// <summary>
/// Defines a contract for a quantum entanglement registry that coordinates the observation and collapse of entangled quantum promises.
/// </summary>
public interface IQuantumEntanglementRegistry<T>
{
    /// <summary>
    /// Observes the specified quantum promise, coordinating the collapse of all entangled promises according to the registry's behavior.
    /// </summary>
    /// <param name="promise">The quantum promise to observe.</param>
    /// <param name="cancellationToken">A token to cancel the observation.</param>
    /// <returns>The observed value of the specified promise.</returns>
    Task<T> ObserveAsync(QuantumPromise<T> promise, CancellationToken cancellationToken = default);
}