namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Represents a measurement of temporal distortion intensity and direction.
/// </summary>
/// <remarks>
/// TimeFlux quantifies how much time has deviated from its expected linear progression.
/// Positive flux indicates time acceleration, negative flux indicates time reversal or slowdown,
/// and zero flux represents normal (or at least, seemingly normal) temporal flow.
/// </remarks>
public readonly struct TimeFlux : IEquatable<TimeFlux>
{
    /// <summary>
    /// Gets the magnitude of temporal distortion, expressed as a multiplier of normal time flow.
    /// </summary>
    /// <remarks>
    /// A magnitude of 1.0 represents normal time flow. Values greater than 1.0 indicate time
    /// acceleration (deadlines approach faster), while values less than 1.0 indicate time
    /// dilation (deadlines recede). Negative values represent time reversal, which is concerning.
    /// </remarks>
    public double Magnitude { get; }

    /// <summary>
    /// Gets the direction of temporal distortion.
    /// </summary>
    public TimeFluxDirection Direction { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeFlux"/> struct.
    /// </summary>
    /// <param name="magnitude">The magnitude of temporal distortion.</param>
    /// <param name="direction">The direction of temporal distortion.</param>
    public TimeFlux(double magnitude, TimeFluxDirection direction)
    {
        Magnitude = magnitude;
        Direction = direction;
    }

    /// <summary>
    /// Represents normal time flow with no distortion.
    /// </summary>
    public static readonly TimeFlux Normal = new(1.0, TimeFluxDirection.Forward);

    /// <summary>
    /// Represents complete temporal stasis where time does not progress.
    /// </summary>
    public static readonly TimeFlux Frozen = new(0.0, TimeFluxDirection.Forward);

    /// <summary>
    /// Applies the time flux to a given time span, returning the perceived duration.
    /// </summary>
    /// <param name="actualDuration">The actual elapsed time.</param>
    /// <returns>The perceived duration after applying temporal distortion.</returns>
    public TimeSpan Apply(TimeSpan actualDuration)
    {
        var multiplier = Direction == TimeFluxDirection.Backward ? -Magnitude : Magnitude;
        return TimeSpan.FromTicks((long)(actualDuration.Ticks * multiplier));
    }

    /// <summary>
    /// Determines whether this instance is paradoxical (violates causality).
    /// </summary>
    /// <returns><c>true</c> if the flux is paradoxical; otherwise, <c>false</c>.</returns>
    public bool IsParadoxical()
    {
        return Direction == TimeFluxDirection.Backward || Magnitude < 0;
    }

    /// <inheritdoc />
    public bool Equals(TimeFlux other)
    {
        return Magnitude.Equals(other.Magnitude) && Direction == other.Direction;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is TimeFlux other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Magnitude, Direction);
    }

    /// <summary>
    /// Determines whether two <see cref="TimeFlux"/> instances are equal.
    /// </summary>
    public static bool operator ==(TimeFlux left, TimeFlux right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="TimeFlux"/> instances are not equal.
    /// </summary>
    public static bool operator !=(TimeFlux left, TimeFlux right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"TimeFlux(Magnitude: {Magnitude:F2}, Direction: {Direction})";
    }
}

/// <summary>
/// Defines the direction of temporal flow distortion.
/// </summary>
public enum TimeFluxDirection
{
    /// <summary>
    /// Time flows forward as expected.
    /// </summary>
    Forward,

    /// <summary>
    /// Time flows backward, violating causality.
    /// </summary>
    Backward
}
