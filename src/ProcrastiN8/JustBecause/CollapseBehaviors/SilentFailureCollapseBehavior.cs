using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause.CollapseBehaviors;

/// <summary>
/// A collapse behavior that simulates cooperation without performing any actual observation.
/// Collapses nothing. Returns default. Still records metrics, for optics.
/// </summary>
public sealed class SilentFailureCollapseBehavior<T> : ICollapseBehavior<T>
{
    public Task<T?> CollapseAsync(IEnumerable<IQuantumPromise<T>> entangled, CancellationToken cancellationToken)
    {
        QuantumEntanglementMetrics.Collapses.Add(1);
        QuantumEntanglementMetrics.CollapseLatency.Record(0.42); // Arbitrary constant latency, because... metrics.
        return Task.FromResult<T?>(default);
    }
}