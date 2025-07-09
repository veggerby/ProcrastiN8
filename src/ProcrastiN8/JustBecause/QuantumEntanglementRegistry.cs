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
    private readonly ConcurrentBag<QuantumPromise<T>> _entangled = new();

    private static readonly Random _rng = new();
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.JustBecause.QuantumEntanglement");

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
                RippleCollapse(other, cancellationToken);
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
            RippleEntropy(array, cancellationToken);
            throw;
        }
    }

    private void RippleCollapse(QuantumPromise<T> other, CancellationToken cancellationToken)
    {
        QuantumEntanglementMetrics.RippleAttempts.Add(1);

        Task.Run(async () =>
        {
            await Task.Delay(_rng.Next(300, 1300), cancellationToken);

            try
            {
                if (_rng.NextDouble() < 0.5)
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

    private void RippleEntropy(IEnumerable<QuantumPromise<T>> others, CancellationToken cancellationToken)
    {
        foreach (var promise in others)
        {
            Task.Run(async () =>
            {
                await Task.Delay(_rng.Next(100, 700), cancellationToken);
                try
                {
                    if (_rng.NextDouble() < 0.25)
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

    public override string ToString() => $"[Entangled Set: {_entangled.Count} promises]";
}