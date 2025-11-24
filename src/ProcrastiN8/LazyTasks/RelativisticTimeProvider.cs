using ProcrastiN8.JustBecause;

namespace ProcrastiN8.LazyTasks;

/// <summary>
/// A time provider that simulates relativistic time dilation effects near deadlines.
/// </summary>
/// <remarks>
/// As the current time approaches a configured deadline, this provider causes time to slow down
/// exponentially, creating the illusion of more available time. This behavior mimics the subjective
/// experience of procrastinators who believe they have infinite time until the moment they don't.
/// The closer to the deadline, the slower time appears to flow, asymptotically approaching temporal stasis.
/// </remarks>
public sealed class RelativisticTimeProvider : ITimeProvider
{
    private readonly ITimeProvider _baseTimeProvider;
    private readonly DateTimeOffset _deadline;
    private readonly double _dilationFactor;
    private readonly IRandomProvider _randomProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelativisticTimeProvider"/> class.
    /// </summary>
    /// <param name="deadline">The deadline that causes relativistic time dilation effects.</param>
    /// <param name="dilationFactor">
    /// The factor controlling the intensity of time dilation. Higher values cause more extreme slowdown.
    /// Typical values range from 0.5 (mild dilation) to 5.0 (severe temporal distortion).
    /// </param>
    /// <param name="baseTimeProvider">The underlying time provider to wrap. If null, uses <see cref="SystemTimeProvider"/>.</param>
    /// <param name="randomProvider">Optional random provider for introducing quantum jitter. If null, uses default.</param>
    public RelativisticTimeProvider(
        DateTimeOffset deadline,
        double dilationFactor = 2.0,
        ITimeProvider? baseTimeProvider = null,
        IRandomProvider? randomProvider = null)
    {
        _baseTimeProvider = baseTimeProvider ?? SystemTimeProvider.Default;
        _deadline = deadline;
        _dilationFactor = dilationFactor;
        _randomProvider = randomProvider ?? JustBecause.RandomProvider.Default;
    }

    /// <summary>
    /// Returns the current UTC timestamp with relativistic time dilation applied.
    /// </summary>
    /// <returns>A dilated timestamp that appears to progress more slowly as the deadline approaches.</returns>
    /// <remarks>
    /// The dilation effect is calculated using an exponential decay function based on proximity to the deadline.
    /// Time appears to slow down asymptotically, never quite reaching the deadline even when it has passed
    /// in real time. This creates a comfortable illusion of infinite procrastination potential.
    /// </remarks>
    public DateTimeOffset GetUtcNow()
    {
        var realNow = _baseTimeProvider.GetUtcNow();
        
        // If we're past the deadline, causality is violated - but we pretend we're still before it
        if (realNow >= _deadline)
        {
            // Asymptotically approach the deadline but never reach it
            var overshoot = (realNow - _deadline).TotalSeconds;
            var asymptote = _deadline.AddSeconds(-0.001 - (overshoot * 0.01));
            return asymptote;
        }

        // Calculate distance to deadline in seconds
        var distanceToDeadline = (_deadline - realNow).TotalSeconds;
        
        // Apply exponential time dilation: the closer we get, the slower time flows
        // When near the deadline, dilation factor increases the perceived distance
        var perceivedDistance = distanceToDeadline * (1.0 + _dilationFactor / Math.Max(distanceToDeadline, 1.0));
        
        // Add quantum jitter (±5% randomness) to simulate observer effects
        var jitter = 1.0 + (_randomProvider.GetDouble() - 0.5) * 0.1;
        perceivedDistance *= jitter;

        // Apply dilation to time progression - we're further from deadline than we think
        var dilatedNow = _deadline.AddSeconds(-perceivedDistance);

        return dilatedNow;
    }

    /// <summary>
    /// Calculates the current time flux magnitude near the deadline.
    /// </summary>
    /// <returns>A <see cref="TimeFlux"/> representing the current temporal distortion.</returns>
    public TimeFlux GetCurrentFlux()
    {
        var realNow = _baseTimeProvider.GetUtcNow();
        var distanceToDeadline = (_deadline - realNow).TotalSeconds;

        if (distanceToDeadline <= 0)
        {
            // Past the deadline - time is severely distorted
            return new TimeFlux(0.01, TimeFluxDirection.Forward);
        }

        var dilation = 1.0 / (1.0 + (_dilationFactor / Math.Max(distanceToDeadline, 1.0)));
        return new TimeFlux(dilation, TimeFluxDirection.Forward);
    }
}
