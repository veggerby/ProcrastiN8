using System.Diagnostics.Metrics;

namespace ProcrastiN8.Metrics;

/// <summary>
/// Exposes OpenTelemetry metrics for temporal distortion and time travel behavior.
/// </summary>
internal static class TemporalMetrics
{
    /// <summary>
    /// The OpenTelemetry Meter for temporal metrics.
    /// </summary>
    private static readonly Meter Meter = new("ProcrastiN8.LazyTasks.Temporal", "1.0.0");

    /// <summary>
    /// Counts the number of temporal paradoxes detected.
    /// </summary>
    public static readonly Counter<long> ParadoxCount =
        Meter.CreateCounter<long>("temporal.paradoxes", unit: "count", description: "Detected causality violations and temporal paradoxes");

    /// <summary>
    /// Counts the number of timeline divergence events.
    /// </summary>
    public static readonly Counter<long> TimelineDivergences =
        Meter.CreateCounter<long>("temporal.timeline_divergences", unit: "count", description: "Timeline branch or divergence events");

    /// <summary>
    /// Records the current timeline divergence index (how far from baseline reality).
    /// </summary>
    public static readonly ObservableGauge<double> TimelineDivergenceIndex =
        Meter.CreateObservableGauge<double>("temporal.divergence_index", 
            () => _currentDivergenceIndex, 
            unit: "index", 
            description: "Current distance from baseline timeline");

    /// <summary>
    /// Counts the number of Mercury retrograde rewind events.
    /// </summary>
    public static readonly Counter<long> RetrogradeRewinds =
        Meter.CreateCounter<long>("temporal.retrograde_rewinds", unit: "count", description: "Mercury retrograde time rewind events");

    /// <summary>
    /// Counts the number of temporal whiplash exceptions thrown.
    /// </summary>
    public static readonly Counter<long> TemporalWhiplashEvents =
        Meter.CreateCounter<long>("temporal.whiplash_events", unit: "count", description: "Temporal whiplash exception events");

    /// <summary>
    /// Records the magnitude of relativistic time dilation near deadlines.
    /// </summary>
    public static readonly Histogram<double> DilationMagnitude =
        Meter.CreateHistogram<double>("temporal.dilation_magnitude", unit: "factor", description: "Relativistic time dilation multiplier");

    /// <summary>
    /// Counts the number of deadline distortions applied.
    /// </summary>
    public static readonly Counter<long> DeadlineDistortions =
        Meter.CreateCounter<long>("temporal.deadline_distortions", unit: "count", description: "Deadline distortion strategy applications");

    /// <summary>
    /// Records how far deadlines have shifted from their original positions.
    /// </summary>
    public static readonly Histogram<double> DeadlineShift =
        Meter.CreateHistogram<double>("temporal.deadline_shift", unit: "hours", description: "Deadline shift magnitude in hours");

    private static double _currentDivergenceIndex = 0.0;

    /// <summary>
    /// Updates the current timeline divergence index.
    /// </summary>
    /// <param name="divergenceIndex">The new divergence index value.</param>
    internal static void UpdateDivergenceIndex(double divergenceIndex)
    {
        _currentDivergenceIndex = divergenceIndex;
    }
}
