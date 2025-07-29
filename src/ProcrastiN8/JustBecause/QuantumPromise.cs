using System.Diagnostics;

using ProcrastiN8.JustBecause.CollapseBehaviors;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// A promise that exists in a state of superposition until observed. Observation may collapse the value,
/// throw an exception, or result in existential expiration. Do not use in production. Ever.
/// </summary>
/// <typeparam name="T">The type of the value encapsulated by the promise.</typeparam>
/// <param name="lazyInitializer">A function to lazily initialize the promise's value.</param>
/// <param name="schrodingerWindow">The time window during which the promise remains in superposition.</param>
/// <param name="delayStrategy">An optional strategy for introducing delays during observation.</param>
/// <param name="timeProvider">An optional provider for time-related operations.</param>
/// <param name="randomProvider">An optional provider for randomness.</param>
public sealed class QuantumPromise<T>(Func<Task<T>> lazyInitializer, TimeSpan schrodingerWindow, IDelayStrategy? delayStrategy = null, ITimeProvider? timeProvider = null, IRandomProvider? randomProvider = null) : IQuantumPromise<T>, ICopenhagenCollapsible<T>
{
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.JustBecause.QuantumPromise");

    private readonly Lazy<Task<T>> _lazyInitializer = new Lazy<Task<T>>(lazyInitializer) ?? throw new ArgumentNullException(nameof(lazyInitializer));
    private readonly ITimeProvider _timeProvider = timeProvider ?? new SystemTimeProvider();
    private readonly DateTimeOffset _creationTime = (timeProvider ?? new SystemTimeProvider()).GetUtcNow();
    private readonly IDelayStrategy _delayStrategy = delayStrategy ?? new DefaultDelayStrategy();
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;

    public DateTimeOffset CreationTime => _creationTime;

    private readonly TimeSpan _schrodingerWindow = schrodingerWindow;
    private readonly object _lock = new();

    private bool _isObserved = false;
    private Task<T>? _evaluationTask;
    private Exception? _collapseFailure;

    // Minimum milliseconds after creation before observation is allowed (quantum instability window)
    private static readonly TimeSpan MinObservationDelayMs = TimeSpan.FromMilliseconds(200);
    // Maximum milliseconds after creation before observation is allowed (quantum instability window)
    private static readonly TimeSpan MaxObservationDelayMs = TimeSpan.FromMilliseconds(1000);
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
    /// <exception cref="CollapseTooEarlyException">Thrown if the promise is observed too early.</exception>
    /// <exception cref="CollapseTooLateException">Thrown if the promise is observed after the Schrödinger window expires.</exception>
    /// <exception cref="CollapseToVoidException">Thrown if the promise collapses to void.</exception>
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

        using var activity = ActivitySource.StartActivity("ProcrastiN8.QuantumPromise.Observe", ActivityKind.Internal);

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

            _evaluationTask = CollapseAndStoreAsync(activity, cancellationToken);
            return _evaluationTask;
        }
    }

    private bool CheckCollapseTiming(double delay, Activity? activity)
    {
        var timeSinceCreation = _timeProvider.GetUtcNow() - _creationTime;

        if (timeSinceCreation < TimeSpan.FromMilliseconds(delay))
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

        return true;
    }

    private async Task<T> CollapseAndStoreAsync(Activity? activity, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        await _delayStrategy.DelayAsync(MinObservationDelayMs, MaxObservationDelayMs, delay => CheckCollapseTiming(delay, activity), cancellationToken); // simulate quantum weirdness

        // Check for void collapse
        if (_randomProvider.GetDouble() < VoidCollapseProbability)
        {
            _collapseFailure = new CollapseToVoidException("The promise collapsed to void.");
            QuantumPromiseMetrics.Failures.Add(1, new KeyValuePair<string, object?>("reason", "VoidCollapse"));
            activity?.SetTag("collapse.status", "failure");
            activity?.SetTag("collapse.reason", "VoidCollapse");
            throw _collapseFailure;
        }

        var result = await _lazyInitializer.Value;

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
    /// <exception cref="InvalidOperationException">Thrown if the promise has already been resolved.</exception>
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

        await _delayStrategy.DelayAsync(minDelay: TimeSpan.FromMilliseconds(CollapseConsensusMinDelayMs), maxDelay: TimeSpan.FromMilliseconds(CollapseConsensusMaxDelayMs), cancellationToken: cancellationToken);
        QuantumPromiseMetrics.Collapses.Add(1);
    }

    /// <summary>
    /// Entangles this promise with other promises, using specified collapse behavior and random provider.
    /// </summary>
    /// <param name="behavior">The collapse behavior to use for the entanglement.</param>
    /// <param name="randomProvider">The random provider to use for the entanglement.</param>
    /// <param name="others">The promises to entangle with this promise.</param>
    /// <remarks>
    /// This method allows full customization of the entanglement registry, including collapse behavior and randomness.
    /// </remarks>
    public void Entangle(ICollapseBehavior<T>? behavior, IRandomProvider? randomProvider, params QuantumPromise<T>[] others)
    {
        var registry = QuantumEntanglementRegistry<T>.Create(behavior, randomProvider);
        registry.Entangle(this);
        foreach (var other in others)
        {
            registry.Entangle(other);
        }
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