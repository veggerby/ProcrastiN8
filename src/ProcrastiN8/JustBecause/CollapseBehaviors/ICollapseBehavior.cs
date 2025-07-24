namespace ProcrastiN8.JustBecause.CollapseBehaviors;

/// <summary>
/// Defines a contract for collapse behaviors that dictate how entangled quantum promises are resolved.
/// </summary>
public interface ICollapseBehavior<T>
{
    /// <summary>
    /// Collapses a set of entangled quantum promises according to the behavior's interpretation of quantum ethics.
    /// </summary>
    /// <param name="entangled">The entangled quantum promises to collapse.</param>
    /// <param name="cancellationToken">A token to cancel the collapse operation.</param>
    /// <returns>The collapsed value, or default if collapse fails or is forbidden by quantum law.</returns>
    Task<T?> CollapseAsync(IEnumerable<IQuantumPromise<T>> entangled, CancellationToken cancellationToken);
}