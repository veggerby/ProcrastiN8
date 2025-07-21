using System.Collections.Concurrent;
using System.Diagnostics;

using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// Manages entangled QuantumPromise instances. Collapsing one may trigger collapse or decay in others.
/// Entanglement is non-local and non-consensual.
/// </summary>
public sealed class QuantumEntanglementRegistry<T>
{
    // Holds all entangled quantum promises
    private readonly ConcurrentBag<QuantumPromise<T>> _entangled = [];

    // Random number generator for all quantum effects
    private static readonly Random _rng = new();
    // Activity source for tracing entanglement operations
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.JustBecause.QuantumEntanglement");

    // Minimum delay (ms) for ripple collapse propagation
    private const int RippleCollapseMinDelayMs = 300;
    // Maximum delay (ms) for ripple collapse propagation
    private const int RippleCollapseMaxDelayMs = 1300;
    // Probability that a ripple collapse will occur (50%)
    private const double RippleCollapseProbability = 0.5;
    // Minimum delay (ms) for ripple entropy propagation
    private const int RippleEntropyMinDelayMs = 100;
    // Maximum delay (ms) for ripple entropy propagation
    private const int RippleEntropyMaxDelayMs = 700;
    // Probability that a ripple entropy event will occur (25%)
    private const double RippleEntropyProbability = 0.25;

    /// <summary>
    /// Adds a quantum promise to the entangled set.
    /// </summary>
    public void Entangle(QuantumPromise<T> quantum)
    {
        if (quantum == null)
        {
            throw new ArgumentNullException(nameof(quantum));
        }

        _entangled.Add(quantum);
        QuantumEntanglementMetrics.Entanglements.Add(1);
    }

    /// <summary>
    /// Collapses one randomly chosen entangled promise and ripples to others.
    /// This function is completely unfair and does not respect promise autonomy.
    /// </summary>
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
            T result = await chosen.ObserveAsync(cancellationToken);

            foreach (var other in array.Where(p => !ReferenceEquals(p, chosen)))
            {
                QuantumEntanglementRegistry<T>.RippleCollapse(other, cancellationToken);
            }

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
            QuantumEntanglementRegistry<T>.RippleEntropy(array, cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Attempts to collapse another promise after a random delay, with a chance of success.
    /// </summary>
    private static void RippleCollapse(QuantumPromise<T> other, CancellationToken cancellationToken)
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

    /// <summary>
    /// Attempts to propagate entropy to all promises after a random delay, with a chance of success.
    /// </summary>
    private static void RippleEntropy(IEnumerable<QuantumPromise<T>> others, CancellationToken cancellationToken)
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

    /// <summary>
    /// Returns a string representation of the entangled set.
    /// </summary>
    public override string ToString() => $"[Entangled Set: {_entangled.Count} promises]";
}