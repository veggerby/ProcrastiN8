using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause.CollapseBehaviors;

/// <summary>
/// Implements the Copenhagen interpretation: observation collapses all entangled promises, but only the chosen one determines the returned value.
/// All other entangled promises are forcibly collapsed to the same value, bypassing quantum uncertainty.
/// </summary>
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
        if (array.Length == 0)
        {
            return default;
        }

        var chosenIndex = new Random().Next(array.Length);
        var chosen = array[chosenIndex];
        QuantumEntanglementMetrics.Collapses.Add(1);

        // Observe the chosen promise
        var value = await chosen.ObserveAsync(cancellationToken);

        // Collapse all other entangled promises to the same value, if possible
        var collapseTasks = array
            .Where((p, i) => i != chosenIndex)
            .Select(async p =>
            {
                if (p is ICopenhagenCollapsible<T> copenhagen)
                {
                    await copenhagen.CollapseToValueAsync(value, cancellationToken);
                }
                else
                {
                    try { await p.ObserveAsync(cancellationToken); } catch { /* Suppress exceptions for quantum harmony */ }
                }
            });
        await Task.WhenAll(collapseTasks);

        return value;
    }
}