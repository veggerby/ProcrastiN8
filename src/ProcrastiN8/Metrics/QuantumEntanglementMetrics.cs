using System.Diagnostics.Metrics;

namespace ProcrastiN8.Metrics;

/// <summary>
/// Metrics for entangled promise behavior across distributed uncertainty.
/// </summary>
internal static class QuantumEntanglementMetrics
{
    /// <summary>
    /// The OpenTelemetry Meter for QuantumEntanglement metrics.
    /// </summary>
    private static readonly Meter Meter = new("ProcrastiN8.JustBecause.QuantumEntanglement", "1.0.0");

    /// <summary>
    /// Counts the number of entangled promise registrations.
    /// </summary>
    public static readonly Counter<long> Entanglements =
        Meter.CreateCounter<long>("quantum_entanglement.entangled", unit: "count", description: "Entangled promise registrations");

    /// <summary>
    /// Counts the number of collapse events triggered in entangled promises.
    /// </summary>
    public static readonly Counter<long> Collapses =
        Meter.CreateCounter<long>("quantum_entanglement.collapses", unit: "count", description: "Collapses triggered");

    /// <summary>
    /// Counts the number of triggered ripple collapse attempts.
    /// </summary>
    public static readonly Counter<long> RippleAttempts =
        Meter.CreateCounter<long>("quantum_entanglement.ripple_attempts", unit: "count", description: "Triggered ripple collapses");

    /// <summary>
    /// Counts the number of failed ripple observations.
    /// </summary>
    public static readonly Counter<long> RippleFailures =
        Meter.CreateCounter<long>("quantum_entanglement.ripple_failures", unit: "count", description: "Failed ripple observations");

    /// <summary>
    /// Records the latency of primary collapse events, in milliseconds.
    /// </summary>
    public static readonly Histogram<double> CollapseLatency =
        Meter.CreateHistogram<double>("quantum_entanglement.collapse_latency", unit: "ms", description: "Latency of primary collapse");
}