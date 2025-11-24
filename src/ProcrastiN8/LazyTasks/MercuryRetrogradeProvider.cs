using ProcrastiN8.JustBecause;

namespace ProcrastiN8.LazyTasks;

/// <summary>
/// A time provider that randomly rewinds time, simulating Mercury retrograde chaos.
/// </summary>
/// <remarks>
/// During Mercury retrograde periods, time becomes unreliable and may spontaneously reverse,
/// skip forward, or oscillate unpredictably. This provider captures that essential chaos,
/// making it impossible to rely on temporal consistency. Deadlines become suggestions,
/// schedules become fiction, and causality becomes optional.
/// </remarks>
public sealed class MercuryRetrogradeProvider : ITimeProvider
{
    private readonly ITimeProvider _baseTimeProvider;
    private readonly IRandomProvider _randomProvider;
    private readonly double _retrogradeProbability;
    private readonly TimeSpan _maxRewindDuration;
    private DateTimeOffset? _lastObservedTime;
    private int _consecutiveRewinds;

    /// <summary>
    /// Initializes a new instance of the <see cref="MercuryRetrogradeProvider"/> class.
    /// </summary>
    /// <param name="retrogradeProbability">
    /// The probability (0.0 to 1.0) that time will rewind on any given observation.
    /// Typical Mercury retrograde conditions suggest 0.3 (30% chaos).
    /// </param>
    /// <param name="maxRewindDuration">
    /// The maximum duration that time can rewind in a single retrograde event.
    /// Defaults to 1 hour, though Mercury's influence is theoretically unbounded.
    /// </param>
    /// <param name="baseTimeProvider">The underlying time provider to wrap. If null, uses <see cref="SystemTimeProvider"/>.</param>
    /// <param name="randomProvider">Optional random provider for retrograde chaos. If null, uses default.</param>
    public MercuryRetrogradeProvider(
        double retrogradeProbability = 0.3,
        TimeSpan? maxRewindDuration = null,
        ITimeProvider? baseTimeProvider = null,
        IRandomProvider? randomProvider = null)
    {
        if (retrogradeProbability < 0.0 || retrogradeProbability > 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(retrogradeProbability),
                "Retrograde probability must be between 0.0 and 1.0. Even Mercury has limits.");
        }

        _baseTimeProvider = baseTimeProvider ?? SystemTimeProvider.Default;
        _randomProvider = randomProvider ?? JustBecause.RandomProvider.Default;
        _retrogradeProbability = retrogradeProbability;
        _maxRewindDuration = maxRewindDuration ?? TimeSpan.FromHours(1);
        _lastObservedTime = null;
        _consecutiveRewinds = 0;
    }

    /// <summary>
    /// Returns the current UTC timestamp, potentially rewound due to Mercury retrograde effects.
    /// </summary>
    /// <returns>A timestamp that may be earlier than previous observations.</returns>
    /// <remarks>
    /// Each observation has a chance of triggering a retrograde event, causing time to rewind
    /// by a random amount. Multiple consecutive rewinds are possible but increasingly unlikely,
    /// preventing infinite temporal regression (usually). Observers may experience temporal whiplash.
    /// </remarks>
    /// <exception cref="TemporalWhiplashException">
    /// Thrown when consecutive rewinds exceed safe thresholds (more than 5 in a row).
    /// </exception>
    public DateTimeOffset GetUtcNow()
    {
        var realNow = _baseTimeProvider.GetUtcNow();
        
        // Initialize last observed time if this is the first observation
        if (_lastObservedTime == null)
        {
            _lastObservedTime = realNow;
            _consecutiveRewinds = 0;
            return realNow;
        }

        // Check if Mercury retrograde triggers
        var shouldRewind = _randomProvider.GetDouble() < _retrogradeProbability;

        if (shouldRewind)
        {
            _consecutiveRewinds++;

            // Safety check: too many consecutive rewinds causes temporal whiplash
            if (_consecutiveRewinds > 5)
            {
                _consecutiveRewinds = 0;
                throw new TemporalWhiplashException(
                    $"Mercury retrograde caused {_consecutiveRewinds} consecutive rewinds. Temporal stability compromised.");
            }

            // Rewind by a random amount, up to maxRewindDuration
            var rewindAmount = TimeSpan.FromSeconds(_randomProvider.GetDouble() * _maxRewindDuration.TotalSeconds);
            var rewindTime = realNow - rewindAmount;

            // Ensure we don't rewind before the last observation (that would be too chaotic)
            if (rewindTime < _lastObservedTime.Value)
            {
                rewindTime = _lastObservedTime.Value.AddSeconds(-_randomProvider.GetDouble() * 60);
            }

            _lastObservedTime = rewindTime;
            return rewindTime;
        }
        else
        {
            // No retrograde: time proceeds normally
            _consecutiveRewinds = 0;
            _lastObservedTime = realNow;
            return realNow;
        }
    }

    /// <summary>
    /// Gets the number of consecutive retrograde rewinds that have occurred.
    /// </summary>
    public int ConsecutiveRewinds => _consecutiveRewinds;

    /// <summary>
    /// Determines whether Mercury is currently in retrograde (i.e., last observation triggered a rewind).
    /// </summary>
    /// <returns><c>true</c> if the last observation was a retrograde event; otherwise, <c>false</c>.</returns>
    public bool IsCurrentlyRetrograde()
    {
        return _consecutiveRewinds > 0;
    }

    /// <summary>
    /// Forces a reset of the retrograde state, clearing consecutive rewind counters.
    /// </summary>
    /// <remarks>
    /// This method is useful for recovering from temporal whiplash or resetting the provider
    /// to a stable state. It does not prevent future retrograde events.
    /// </remarks>
    public void ResetRetrogradeState()
    {
        _consecutiveRewinds = 0;
        _lastObservedTime = _baseTimeProvider.GetUtcNow();
    }
}
