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

    /// <summary>
    /// Counts the number of quantum fork events, representing attempts to split entangled states for no good reason.
    /// </summary>
    public static readonly Counter<long> Forks =
        Meter.CreateCounter<long>("quantum_entanglement.forks", unit: "count", description: "Quantum fork events");

    /// <summary>
    /// Counts the number of failed quantum fork attempts, which are as inevitable as they are pointless.
    /// </summary>
    public static readonly Counter<long> ForkFailures =
        Meter.CreateCounter<long>("quantum_entanglement.fork_failures", unit: "count", description: "Failed quantum fork attempts");
}