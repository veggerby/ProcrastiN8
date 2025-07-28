using System.Collections.Concurrent;
using System.Diagnostics;

using ProcrastiN8.JustBecause.CollapseBehaviors;
using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// Manages a non-local, non-consensual registry of entangled <see cref="QuantumPromise{T}"/> instances.
/// Collapsing one may trigger collapse or decay in others, with no regard for fairness or quantum ethics.
/// </summary>
/// <remarks>
/// <para>
/// This class simulates quantum entanglement with the same rigor as a TED Talk given by a blockchain startup.
/// It is not suitable for physics, security, or any application with consequences.
/// </para>
/// <para>
/// Thread safety is provided by <see cref="ConcurrentBag{QuantumPromise}"/>, but the quantum effects are entirely untrustworthy.
/// </para>
/// <para>
/// All operations are traced for metrics and activity, in case auditors wish to observe the collapse of productivity in real time.
/// </para>
/// </remarks>
internal sealed class QuantumEntanglementRegistry<T>(ICollapseBehavior<T>? behavior = null) : IQuantumEntanglementRegistry<T>
{
    // Holds all entangled quantum promises
    private readonly ConcurrentBag<QuantumPromise<T>> _entangled = [];
    // Behavior for collapsing quantum promises
    private readonly ICollapseBehavior<T> _collapseBehavior = behavior ?? CollapseBehaviorFactory.Create<T>(QuantumComplianceLevel.Entanglish);

    // Random number generator for all quantum effects
    private static readonly Random _rng = new();
    // Activity source for tracing entanglement operations
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.JustBecause.QuantumEntanglement");

    /// <summary>
    /// Adds a <see cref="QuantumPromise{T}"/> to the entangled set.
    /// </summary>
    /// <param name="quantum">The quantum promise to entangle.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="quantum"/> is null.</exception>
    public void Entangle(QuantumPromise<T> quantum)
    {
        ArgumentNullException.ThrowIfNull(quantum);

        quantum.SetEntanglementRegistry(this);
        _entangled.Add(quantum);
        QuantumEntanglementMetrics.Entanglements.Add(1);
    }

    /// <summary>
    /// Observes the specified quantum promise, coordinating the collapse of all entangled promises according to the registry's behavior.
    /// </summary>
    /// <param name="promise">The quantum promise to observe. Must be entangled in this registry.</param>
    /// <param name="cancellationToken">A token to cancel the observation.</param>
    /// <returns>The observed value, or throws if collapse fails.</returns>
    public async Task<T> ObserveAsync(QuantumPromise<T> promise, CancellationToken cancellationToken = default)
    {
        if (!_entangled.Contains(promise))
        {
            throw new InvalidOperationException("Promise is not entangled in this registry.");
        }

        // Collapse all entangled promises via the configured behavior
        var result = await _collapseBehavior.CollapseAsync(_entangled.Cast<IQuantumPromise<T>>(), cancellationToken);
        // Return the value for the requested promise (may be null if collapse failed)
        return result is not null ? result : throw new InvalidOperationException("Collapse yielded no value.");
    }

    /// <summary>
    /// Collapses one randomly chosen entangled promise and ripples to others.
    /// This function is completely unfair and does not respect promise autonomy.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the collapse operation.</param>
    /// <returns>The result of the collapse, or null if collapse yields nothing.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no entangled promises are available for collapse.</exception>
    /// <exception cref="Exception">Propagates any exception thrown during collapse behavior.</exception>
    public async Task<T?> CollapseOneAsync(CancellationToken cancellationToken = default)
    {
        if (_entangled.IsEmpty)
        {
            throw new InvalidOperationException("No entangled promises available for collapse.");
        }

        using var activity = ActivitySource.StartActivity("QuantumEntanglementRegistry.CollapseOne", ActivityKind.Internal);
        var sw = Stopwatch.StartNew();

        var array = _entangled.ToArray();
        var chosen = array[_rng.Next(array.Length)];

        QuantumEntanglementMetrics.Collapses.Add(1);
        activity?.SetTag("entangled.count", array.Length);

        try
        {
            var result = await _collapseBehavior.CollapseAsync(_entangled, cancellationToken);

            sw.Stop();
            QuantumEntanglementMetrics.CollapseLatency.Record(sw.Elapsed.TotalMilliseconds);
            activity?.SetTag("collapse.status", "success");
            activity?.SetTag("collapse.duration_ms", sw.Elapsed.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            activity?.SetTag("collapse.status", "failure");
            activity?.SetTag("collapse.error", ex.GetType().Name);
            throw;
        }
    }

    /// <summary>
    /// Returns a string representation of the entangled set, suitable for existential dread.
    /// </summary>
    /// <returns>A string describing the number of entangled promises.</returns>
    public override string ToString() => $"[Entangled Set: {_entangled.Count} promises]";
}