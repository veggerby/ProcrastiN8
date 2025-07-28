namespace ProcrastiN8.JustBecause;

/// <summary>
/// Internal interface for quantum promises that can be forcibly collapsed to a specific value.
/// Only the CopenhagenCollapseBehavior is permitted to invoke this method.
/// </summary>
/// <typeparam name="T">The type of the quantum value.</typeparam>
internal interface ICopenhagenCollapsible<T> : IQuantumPromise<T>
{
    /// <summary>
    /// Collapses this quantum promise to a specific value, forcibly resolving its state.
    /// </summary>
    /// <param name="value">The value to collapse to.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task CollapseToValueAsync(T value, CancellationToken cancellationToken = default);
}