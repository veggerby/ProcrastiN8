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
}
