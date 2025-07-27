using System.Diagnostics;

using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause.CollapseBehaviors;

/// <summary>
/// Attempts to collapse *all* entangled promises at once, instantly, and non-locally,
/// simulating "spooky action at a distance". Outcomes are uncorrelated but confidently tagged.
/// </summary>
public sealed class SpookyActionCollapseBehavior<T> : ICollapseBehavior<T>
{
    public async Task<T?> CollapseAsync(IEnumerable<IQuantumPromise<T>> entangled, CancellationToken cancellationToken)
    {
        var array = entangled.ToArray();
        if (array.Length == 0)
            throw new InvalidOperationException("No entangled promises to collapse.");

        var correlationToken = Guid.NewGuid().ToString();

        QuantumEntanglementMetrics.Collapses.Add(array.Length);

        var sw = Stopwatch.StartNew();

        var tasks = array.Select(async promise =>
        {
            try
            {
                return await promise.ObserveAsync(cancellationToken);
            }
            catch
            {
                QuantumEntanglementMetrics.RippleFailures.Add(1);
                return default;
            }
        });

        T? chosenResult = default;
        try
        {
            // Always return the result of the first promise for deterministic tests
            chosenResult = await tasks.ElementAt(0);
        }
        catch { /* ignore */ }

        await Task.WhenAll(tasks);

        sw.Stop();
        QuantumEntanglementMetrics.CollapseLatency.Record(sw.Elapsed.TotalMilliseconds);

        return chosenResult;
    }
}