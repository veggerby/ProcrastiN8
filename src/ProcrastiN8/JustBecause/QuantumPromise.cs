using System.Diagnostics;
using ProcrastiN8.LazyTasks;

using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// A promise that exists in a state of superposition until observed. Observation may collapse the value,
/// throw an exception, or result in existential expiration. Do not use in production. Ever.
/// </summary>
public sealed class QuantumPromise<T>(Func<Task<T>> lazyInitializer, TimeSpan schrodingerWindow, IRandomProvider? randomProvider = null, ITimeProvider? timeProvider = null) : IQuantumPromise<T>, ICopenhagenCollapsible<T>
{
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.JustBecause.QuantumPromise");

    private readonly Func<Task<T>> _lazyInitializer = lazyInitializer ?? throw new ArgumentNullException(nameof(lazyInitializer));
    private readonly ITimeProvider _timeProvider = timeProvider ?? new SystemTimeProvider();
    private readonly DateTimeOffset _creationTime = (timeProvider ?? new SystemTimeProvider()).GetUtcNow();

    public DateTimeOffset CreationTime => _creationTime;

    private readonly TimeSpan _schrodingerWindow = schrodingerWindow;
    private readonly object _lock = new();

    private bool _isObserved = false;
    private Task<T>? _evaluationTask;
    private Exception? _collapseFailure;
    private readonly IRandomProvider _randomProvider = randomProvider ?? new RandomProvider();

    // Minimum milliseconds after creation before observation is allowed (quantum instability window)
    private const int MinObservationDelayMs = 200;
    // Maximum milliseconds after creation before observation is allowed (quantum instability window)
    private const int MaxObservationDelayMs = 1000;
    // Probability that the promise collapses to void (evaporates)
    private const double VoidCollapseProbability = 0.1;
    // Minimum milliseconds to simulate collapse latency for consensus
    private const int CollapseConsensusMinDelayMs = 50;
    // Maximum milliseconds to simulate collapse latency for consensus
    private const int CollapseConsensusMaxDelayMs = 150;

    private IQuantumEntanglementRegistry<T>? _entanglementRegistry;

    /// <summary>
    /// Requests observation of this quantum promise. If this promise is entangled in a registry, the registry will coordinate the collapse of all entangled promises according to its behavior.
    /// If not entangled, this promise will collapse independently.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the observation.</param>
    /// <returns>The observed value, or throws if collapse fails.</returns>
    public async Task<T> ObserveAsync(CancellationToken cancellationToken = default)
    {
        // If this promise is entangled in a registry, delegate observation to the registry
        if (_entanglementRegistry is not null)
        {
            // The registry will coordinate the collapse and return the value for this promise
            return await _entanglementRegistry.ObserveAsync(this, cancellationToken);
        }

        // Otherwise, collapse independently (legacy, discouraged)
        return await ObserveInternalAsync(cancellationToken);
    }

    // Internal observation logic, only used if not entangled
    internal Task<T> ObserveInternalAsync(CancellationToken cancellationToken)
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

            var timeSinceCreation = _timeProvider.GetUtcNow() - _creationTime;

            if (timeSinceCreation < TimeSpan.FromMilliseconds(_randomProvider.Next(MinObservationDelayMs, MaxObservationDelayMs)))
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

            if (_randomProvider.NextDouble() < VoidCollapseProbability)
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

        await Task.Delay(_randomProvider.Next(100, 1000), cancellationToken); // simulate quantum weirdness

        var result = await _lazyInitializer.Invoke();

        sw.Stop();
        QuantumPromiseMetrics.CollapseDuration.Record(sw.Elapsed.TotalMilliseconds);
        QuantumPromiseMetrics.Collapses.Add(1);

        activity?.SetTag("collapse.status", "success");
        activity?.SetTag("collapse.duration_ms", sw.Elapsed.TotalMilliseconds);

        return result;
    }

    // Explicit interface implementation: only accessible via ICopenhagenCollapsible<T>
    Task ICopenhagenCollapsible<T>.CollapseToValueAsync(T value, CancellationToken cancellationToken)
        => CollapseToValueCoreAsync(value, cancellationToken);

    /// <summary>
    /// Forcibly collapses this quantum promise to a specific value. Only accessible via ICopenhagenCollapsible.
    /// </summary>
    /// <param name="value">The value to collapse to.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <remarks>
    /// This method is intentionally not public. Only the registry, via a collapse behavior, may invoke it.
    /// </remarks>
    private async Task CollapseToValueCoreAsync(T value, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (_isObserved)
            {
                return; // Already collapsed, do nothing
            }

            _isObserved = true;
            _collapseFailure = null;
            _evaluationTask = Task.FromResult(value);
        }

        // Simulate collapse latency for optics
        await Task.Delay(_randomProvider.Next(CollapseConsensusMinDelayMs, CollapseConsensusMaxDelayMs), cancellationToken);
        QuantumPromiseMetrics.Collapses.Add(1);
    }

    // Called by the registry to set the entanglement context
    internal void SetEntanglementRegistry(IQuantumEntanglementRegistry<T> registry)
    {
        _entanglementRegistry = registry;
    }

    public T Value
    {
        get
        {
            if (_evaluationTask == null || !_evaluationTask.IsCompletedSuccessfully)
            {
                throw new InvalidOperationException("The promise has not been resolved yet.");
            }

            return _evaluationTask.Result;
        }
    }

    public override string ToString()
    {
        return _isObserved
            ? _collapseFailure is not null ? "[QuantumPromise ✖️ collapsed (failure)]" : "[QuantumPromise ✅ resolved]"
            : "[QuantumPromise 🌀 unobserved]";
    }
}