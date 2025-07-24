using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause.CollapseBehaviors;

public sealed class RandomUnfairCollapseBehavior<T> : ICollapseBehavior<T>
{
    private static readonly Random _rng = new();

    private const int RippleCollapseMinDelayMs = 300;
    private const int RippleCollapseMaxDelayMs = 1300;
    private const double RippleCollapseProbability = 0.5;

    private const int RippleEntropyMinDelayMs = 100;
    private const int RippleEntropyMaxDelayMs = 700;
    private const double RippleEntropyProbability = 0.25;

    public async Task<T?> CollapseAsync(IEnumerable<IQuantumPromise<T>> entangled, CancellationToken cancellationToken)
    {
        var array = entangled.ToArray();
        var chosen = array[_rng.Next(array.Length)];

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

    private static void RippleCollapse(IQuantumPromise<T> other, CancellationToken cancellationToken)
    {
        QuantumEntanglementMetrics.RippleAttempts.Add(1);

        Task.Run(async () =>
        {
            await Task.Delay(_rng.Next(RippleCollapseMinDelayMs, RippleCollapseMaxDelayMs), cancellationToken);

            try
            {
                if (_rng.NextDouble() < RippleCollapseProbability)
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

    private static void RippleEntropy(IEnumerable<IQuantumPromise<T>> others, CancellationToken cancellationToken)
    {
        foreach (var promise in others)
        {
            Task.Run(async () =>
            {
                await Task.Delay(_rng.Next(RippleEntropyMinDelayMs, RippleEntropyMaxDelayMs), cancellationToken);

                try
                {
                    if (_rng.NextDouble() < RippleEntropyProbability)
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