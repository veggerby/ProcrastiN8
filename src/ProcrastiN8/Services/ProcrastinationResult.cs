namespace ProcrastiN8.Services;

/// <summary>
/// Represents the outcome and metrics of a procrastination scheduling session.
/// </summary>
public sealed class ProcrastinationResult
{
    /// <summary>Gets or sets the mode used.</summary>
    public ProcrastinationMode Mode { get; internal set; }

    /// <summary>Gets or sets a value indicating whether the underlying task was ultimately executed.</summary>
    public bool Executed { get; internal set; }

    /// <summary>Gets or sets total wall-clock deferral duration.</summary>
    public TimeSpan TotalDeferral { get; internal set; }

    /// <summary>Gets or sets the number of excuses retrieved.</summary>
    public int ExcuseCount { get; internal set; }

    /// <summary>Gets or sets the number of delay cycles performed.</summary>
    public int Cycles { get; internal set; }

    /// <summary>Gets or sets a value indicating whether execution was forced early via a trigger.</summary>
    public bool Triggered { get; internal set; }

    /// <summary>Gets or sets a value indicating whether the workflow was abandoned prior to execution.</summary>
    public bool Abandoned { get; internal set; }

    /// <summary>UTC timestamp when scheduling began.</summary>
    public DateTimeOffset StartedUtc { get; internal set; }

    /// <summary>UTC timestamp when scheduling concluded (execution, trigger, abandon, or cancellation).</summary>
    public DateTimeOffset CompletedUtc { get; internal set; }

    /// <summary>Gets a unique correlation identifier for tracing.</summary>
    public Guid CorrelationId { get; internal set; } = Guid.NewGuid();

    /// <summary>Derived metric: cycles per second of deferral (rounded to 3 decimals).</summary>
    public double? CyclesPerSecond => TotalDeferral.TotalSeconds > 0 ? Math.Round(Cycles / TotalDeferral.TotalSeconds, 3) : null;

    /// <summary>
    /// A tongue-in-cheek productivity index (MPI) giving diminishing credit for execution after excessive excuses and cycles.
    /// </summary>
    /// <remarks>
    /// Defined as Executed ? 1 / (1 + ExcuseCount + Cycles) : 0. Lower is more theatrically elaborate.
    /// </remarks>
    public double ProductivityIndex => Executed ? Math.Round(1.0 / (1 + ExcuseCount + Cycles), 4) : 0.0;
}