using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause.CollapseBehaviors;

/// <summary>
/// In the spirit of the Many-Worlds Interpretation, this behavior collapses one promise
/// and then forks multiple parallel universes (i.e. background tasks) that each recurse independently.
/// Great for burning CPU and confusing stakeholders.
/// </summary>
public sealed class ForkingCollapseBehavior<T> : ICollapseBehavior<T>
{
    private static readonly Random _rng = new();
    private const int MaxForks = 3;
    private const int MaxForkDepth = 2;

    public async Task<T?> CollapseAsync(IEnumerable<IQuantumPromise<T>> entangled, CancellationToken cancellationToken)
    {
        var array = entangled.ToArray();
        var chosen = array.FirstOrDefault(p => p.GetType().Name.Contains("PredictableQuantumPromise")) ?? array[_rng.Next(array.Length)];

        QuantumEntanglementMetrics.Collapses.Add(1);

        try
        {
            T result = await chosen.ObserveAsync(cancellationToken);

            // Begin parallel forking
            ForkUniverses(entangled, depth: 1, cancellationToken);

            return result;
        }
        catch (Exception)
        {
            ForkUniverses(entangled, depth: 1, cancellationToken);
            throw;
        }
    }

    private void ForkUniverses(IEnumerable<IQuantumPromise<T>> entangled, int depth, CancellationToken cancellationToken)
    {
        if (depth > MaxForkDepth)
            return;

        int forkCount = _rng.Next(1, MaxForks + 1);
        QuantumEntanglementMetrics.Forks.Add(forkCount);

        for (int i = 0; i < forkCount; i++)
        {
            Task.Run(async () =>
            {
                await Task.Delay(_rng.Next(100, 400), cancellationToken);

                try
                {
                    var forkedBehavior = new ForkingCollapseBehavior<T>();
                    await forkedBehavior.CollapseAsync(entangled, cancellationToken);
                }
                catch
                {
                    // Silent fail — some universes just aren't meant to be.
                    QuantumEntanglementMetrics.ForkFailures.Add(1);
                }
            }, cancellationToken);
        }
    }
}