using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause.CollapseBehaviors;

/// <summary>
/// In the spirit of the Many-Worlds Interpretation, this behavior collapses one promise
/// and then forks multiple parallel universes (i.e. background tasks) that each recurse independently.
/// Great for burning CPU and confusing stakeholders.
/// </summary>
public sealed class ForkingCollapseBehavior<T>(IRandomProvider? randomProvider = null) : ICollapseBehavior<T>
{
    private readonly IRandomProvider _randomProvider = randomProvider ?? new RandomProvider();
    private const int MaxForks = 3;
    private const int MaxForkDepth = 2;

    public async Task<T?> CollapseAsync(IEnumerable<IQuantumPromise<T>> entangled, CancellationToken cancellationToken)
    {
        var array = entangled.ToArray();
        var chosen = array.FirstOrDefault(p => p.GetType().Name.Contains("PredictableQuantumPromise")) ?? array[_randomProvider.Next(array.Length)];

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

        int forkCount = 1 + _randomProvider.Next(MaxForks);
        QuantumEntanglementMetrics.Forks.Add(forkCount);

        for (int i = 0; i < forkCount; i++)
        {
            Task.Run(async () =>
            {
                await Task.Delay(100 + _randomProvider.Next(300), cancellationToken);

                try
                {
                    var forkedBehavior = new ForkingCollapseBehavior<T>(_randomProvider);
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