using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause.CollapseBehaviors;

public sealed class CopenhagenCollapseBehavior<T>(IObserverContext? context = null) : ICollapseBehavior<T>
{
    private readonly IObserverContext _observerContext = context ?? new DefaultObserverContext();

    public async Task<T?> CollapseAsync(IEnumerable<IQuantumPromise<T>> entangled, CancellationToken cancellationToken)
    {
        if (!_observerContext.IsObserved("QuantumCollapse"))
        {
            QuantumEntanglementMetrics.Collapses.Add(0);
            return default;
        }

        var array = entangled.ToArray();
        var chosen = array[new Random().Next(array.Length)];
        QuantumEntanglementMetrics.Collapses.Add(1);

        return await chosen.ObserveAsync(cancellationToken);
    }
}