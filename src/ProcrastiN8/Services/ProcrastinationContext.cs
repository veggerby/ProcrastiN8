namespace ProcrastiN8.Services;

/// <summary>
/// Mutable context updated during procrastination cycles; exposed read-only to consumers via the handle.
/// </summary>
public sealed class ProcrastinationContext
{
    internal DateTimeOffset StartUtc { get; } = DateTimeOffset.UtcNow;
    internal TimeSpan AccumulatedDelay { get; set; }
    internal int Cycles { get; set; }
    internal int Excuses { get; set; }

    /// <summary>Total elapsed since scheduling began.</summary>
    public TimeSpan Elapsed => DateTimeOffset.UtcNow - StartUtc;

    /// <summary>Number of delay cycles performed.</summary>
    public int DelayCycles => Cycles;

    /// <summary>Number of excuses retrieved so far.</summary>
    public int ExcuseCount => Excuses;
}