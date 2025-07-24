using System.Diagnostics;

using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// A promise that exists in a state of superposition until observed. Observation may collapse the value,
/// throw an exception, or result in existential expiration. Do not use in production. Ever.
/// </summary>
public sealed class QuantumPromise<T>(Func<Task<T>> lazyInitializer, TimeSpan schrodingerWindow) : IQuantumPromise<T>
{
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.JustBecause.QuantumPromise");

    private readonly Func<Task<T>> _lazyInitializer = lazyInitializer ?? throw new ArgumentNullException(nameof(lazyInitializer));
    private readonly DateTimeOffset _creationTime = DateTimeOffset.UtcNow;
    private readonly TimeSpan _schrodingerWindow = schrodingerWindow;
    private readonly object _lock = new();

    private bool _isObserved = false;
    private Task<T>? _evaluationTask;
    private T? _value;
    private Exception? _collapseFailure;

    private static readonly Random _rng = new();

    // Minimum milliseconds after creation before observation is allowed (quantum instability window)
    private const int MinObservationDelayMs = 200;
    // Maximum milliseconds after creation before observation is allowed (quantum instability window)
    private const int MaxObservationDelayMs = 1000;
    // Probability that the promise collapses to void (evaporates)
    private const double VoidCollapseProbability = 0.1;

    /// <summary>
    /// Observes the value, triggering collapse. May throw if observed too soon, too late, or from the wrong dimension.
    /// </summary>
    public Task<T> ObserveAsync(CancellationToken cancellationToken = default)
    {
        QuantumPromiseMetrics.Observations.Add(1);

        using var activity = ActivitySource.StartActivity("QuantumPromise.Observe", ActivityKind.Internal);

        lock (_lock)
        {
            if (_isObserved)
            {
                if (_collapseFailure != null)
                {
                    QuantumPromiseMetrics.Failures.Add(1, new KeyValuePair<string, object?>("reason", _collapseFailure.GetType().Name));
                    activity?.SetTag("collapse.status", "failure");
                    activity?.SetTag("collapse.reason", _collapseFailure.GetType().Name);
                    throw _collapseFailure;
                }

                activity?.SetTag("collapse.status", "already-resolved");
                return _evaluationTask!;
            }

            _isObserved = true;

            var timeSinceCreation = DateTimeOffset.UtcNow - _creationTime;

            if (timeSinceCreation < TimeSpan.FromMilliseconds(_rng.Next(MinObservationDelayMs, MaxObservationDelayMs)))
            {
                _collapseFailure = new CollapseTooEarlyException("You peeked too soon. Quantum instability triggered.");
                QuantumPromiseMetrics.Failures.Add(1, new KeyValuePair<string, object?>("reason", "TooEarly"));
                activity?.SetTag("collapse.status", "failure");
                activity?.SetTag("collapse.reason", "TooEarly");
                throw _collapseFailure;
            }

            if (timeSinceCreation > _schrodingerWindow)
            {
                _collapseFailure = new CollapseTooLateException("Observation window expired. The promise has decayed.");
                QuantumPromiseMetrics.Failures.Add(1, new KeyValuePair<string, object?>("reason", "TooLate"));
                activity?.SetTag("collapse.status", "failure");
                activity?.SetTag("collapse.reason", "TooLate");
                throw _collapseFailure;
            }

            if (_rng.NextDouble() < VoidCollapseProbability)
            {
                _collapseFailure = new CollapseToVoidException("The promise evaporated into existential nothingness.");
                QuantumPromiseMetrics.Failures.Add(1, new KeyValuePair<string, object?>("reason", "VoidCollapse"));
                activity?.SetTag("collapse.status", "failure");
                activity?.SetTag("collapse.reason", "VoidCollapse");
                throw _collapseFailure;
            }

            _evaluationTask = CollapseAndStoreAsync(activity, cancellationToken);
            return _evaluationTask;
        }
    }

    private async Task<T> CollapseAndStoreAsync(Activity? activity, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        await Task.Delay(_rng.Next(100, 1000), cancellationToken); // simulate quantum weirdness

        var result = await _lazyInitializer.Invoke();

        sw.Stop();
        QuantumPromiseMetrics.CollapseDuration.Record(sw.Elapsed.TotalMilliseconds);
        QuantumPromiseMetrics.Collapses.Add(1);

        activity?.SetTag("collapse.status", "success");
        activity?.SetTag("collapse.duration_ms", sw.Elapsed.TotalMilliseconds);

        _value = result;
        return result;
    }

    public override string ToString()
    {
        return _isObserved
            ? _collapseFailure is not null ? "[QuantumPromise ✖️ collapsed (failure)]" : "[QuantumPromise ✅ resolved]"
            : "[QuantumPromise 🌀 unobserved]";
    }
}