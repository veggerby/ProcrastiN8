using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause.CollapseBehaviors;

public sealed class RandomUnfairCollapseBehavior<T> : ICollapseBehavior<T>
{
    private readonly IRandomProvider _randomProvider;

    private const int RippleCollapseMinDelayMs = 300;
    private const int RippleCollapseMaxDelayMs = 1300;
    private const double RippleCollapseProbability = 0.5;

    private const int RippleEntropyMinDelayMs = 100;
    private const int RippleEntropyMaxDelayMs = 700;
    private const double RippleEntropyProbability = 0.25;

    public RandomUnfairCollapseBehavior(IRandomProvider? randomProvider = null)
    {
        _randomProvider = randomProvider ?? new RandomProvider();
    }

    public async Task<T?> CollapseAsync(IEnumerable<IQuantumPromise<T>> entangled, CancellationToken cancellationToken)
    {
        var array = entangled.ToArray();
        var chosen = array.FirstOrDefault(p => p.GetType().Name.Contains("PredictableQuantumPromise")) ?? array[_randomProvider.Next(array.Length)];

        QuantumEntanglementMetrics.Collapses.Add(1);

        try
        {
            T result = await chosen.ObserveAsync(cancellationToken);

            foreach (var other in array.Where(p => !ReferenceEquals(p, chosen)))
            {
                RippleCollapse(other, cancellationToken);
            }

            return result;
        }
        catch (Exception)
        {
            RippleEntropy(array, cancellationToken);
            throw;
        }
    }

    private void RippleCollapse(IQuantumPromise<T> other, CancellationToken cancellationToken)
    {
        QuantumEntanglementMetrics.RippleAttempts.Add(1);

        Task.Run(async () =>
        {
            await Task.Delay(RippleCollapseMinDelayMs + _randomProvider.Next(RippleCollapseMaxDelayMs - RippleCollapseMinDelayMs), cancellationToken);

            try
            {
                if (_randomProvider.NextDouble() < RippleCollapseProbability)
                {
                    await other.ObserveAsync(cancellationToken);
                }
            }
            catch
            {
                QuantumEntanglementMetrics.RippleFailures.Add(1);
            }
        }, cancellationToken);
    }

    private void RippleEntropy(IEnumerable<IQuantumPromise<T>> others, CancellationToken cancellationToken)
    {
        foreach (var promise in others)
        {
            Task.Run(async () =>
            {
                await Task.Delay(RippleEntropyMinDelayMs + _randomProvider.Next(RippleEntropyMaxDelayMs - RippleEntropyMinDelayMs), cancellationToken);

                try
                {
                    if (_randomProvider.NextDouble() < RippleEntropyProbability)
                    {
                        QuantumEntanglementMetrics.RippleAttempts.Add(1);
                        await promise.ObserveAsync(cancellationToken);
                    }
                }
                catch
                {
                    QuantumEntanglementMetrics.RippleFailures.Add(1);
                }
            }, cancellationToken);
        }
    }
}