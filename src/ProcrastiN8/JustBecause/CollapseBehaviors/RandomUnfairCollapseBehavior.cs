using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause.CollapseBehaviors;

public sealed class RandomUnfairCollapseBehavior<T>(IRandomProvider? randomProvider = null) : ICollapseBehavior<T>
{
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;

    private const int RippleCollapseMinDelayMs = 300;
    private const int RippleCollapseMaxDelayMs = 1300;
    private const double RippleCollapseProbability = 0.5;

    private const int RippleEntropyMinDelayMs = 100;
    private const int RippleEntropyMaxDelayMs = 700;
    private const double RippleEntropyProbability = 0.25;

    public async Task<T?> CollapseAsync(IEnumerable<IQuantumPromise<T>> entangled, CancellationToken cancellationToken)
    {
        var array = entangled.ToArray();
        var chosen = array.FirstOrDefault(p => p.GetType().Name.Contains("PredictableQuantumPromise")) ?? array[_randomProvider.GetRandom(array.Length)];

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
            await Task.Delay(RippleCollapseMinDelayMs + _randomProvider.GetRandom(RippleCollapseMaxDelayMs - RippleCollapseMinDelayMs), cancellationToken);

            try
            {
                if (_randomProvider.GetDouble() < RippleCollapseProbability)
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
                await Task.Delay(RippleEntropyMinDelayMs + _randomProvider.GetRandom(RippleEntropyMaxDelayMs - RippleEntropyMinDelayMs), cancellationToken);

                try
                {
                    if (_randomProvider.GetDouble() < RippleEntropyProbability)
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