using ProcrastiN8.JustBecause;

namespace ProcrastiN8.LazyTasks;

/// <summary>
/// A deadline distortion strategy that causes deadlines to move away each time they are observed.
/// </summary>
/// <remarks>
/// This strategy implements the quantum observer effect for deadlines: each observation pushes
/// the deadline further into the future, similar to how a watched pot never boils. The more you
/// check the deadline, the more it recedes. Eventually, the deadline may move so far that it
/// creates a temporal paradox, at which point it collapses back to its original position out of spite.
/// </remarks>
public sealed class QuantumObserverDeadlineStrategy : IDeadlineDistortionStrategy
{
    private readonly IRandomProvider _randomProvider;
    private readonly TimeSpan _shiftPerObservation;
    private readonly int _paradoxThreshold;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuantumObserverDeadlineStrategy"/> class.
    /// </summary>
    /// <param name="shiftPerObservation">
    /// The amount of time the deadline shifts into the future per observation.
    /// Defaults to 1 hour, though procrastinators often prefer larger values.
    /// </param>
    /// <param name="paradoxThreshold">
    /// The number of observations before a paradox is triggered and the deadline collapses.
    /// Defaults to 10 observations.
    /// </param>
    /// <param name="randomProvider">Optional random provider for observation jitter. If null, uses default.</param>
    public QuantumObserverDeadlineStrategy(
        TimeSpan? shiftPerObservation = null,
        int paradoxThreshold = 10,
        IRandomProvider? randomProvider = null)
    {
        _shiftPerObservation = shiftPerObservation ?? TimeSpan.FromHours(1);
        _paradoxThreshold = paradoxThreshold;
        _randomProvider = randomProvider ?? JustBecause.RandomProvider.Default;
    }

    /// <inheritdoc />
    public DateTimeOffset DistortDeadline(DateTimeOffset originalDeadline, DateTimeOffset currentTime, int observationCount)
    {
        // If we've exceeded the paradox threshold, collapse back to original deadline
        if (observationCount >= _paradoxThreshold)
        {
            Metrics.TemporalMetrics.ParadoxCount.Add(1, 
                new KeyValuePair<string, object?>("strategy", "QuantumObserver"),
                new KeyValuePair<string, object?>("reason", "ObservationOverload"));
            
            // Collapse with slight randomness to seem natural
            var collapseJitter = TimeSpan.FromMinutes(_randomProvider.GetDouble() * 30 - 15);
            return originalDeadline.Add(collapseJitter);
        }

        // Calculate shift based on observation count
        var totalShift = TimeSpan.FromTicks(_shiftPerObservation.Ticks * observationCount);
        
        // Add quantum jitter (±10% randomness per observation)
        var jitter = _randomProvider.GetDouble() * 0.2 - 0.1;
        totalShift = TimeSpan.FromTicks((long)(totalShift.Ticks * (1.0 + jitter)));

        var distortedDeadline = originalDeadline.Add(totalShift);

        // Record the shift magnitude for metrics
        var shiftHours = (distortedDeadline - originalDeadline).TotalHours;
        Metrics.TemporalMetrics.DeadlineShift.Record(Math.Abs(shiftHours), 
            new KeyValuePair<string, object?>("strategy", "QuantumObserver"));

        return distortedDeadline;
    }

    /// <inheritdoc />
    public bool IsParadoxical(DateTimeOffset originalDeadline, DateTimeOffset distortedDeadline)
    {
        // A paradox occurs if the deadline has shifted more than a year
        // or if it has somehow moved backward in time
        var shift = distortedDeadline - originalDeadline;
        
        return shift < TimeSpan.Zero || shift > TimeSpan.FromDays(365);
    }
}
