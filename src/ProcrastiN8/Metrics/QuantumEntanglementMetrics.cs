using System.Diagnostics.Metrics;

namespace ProcrastiN8.Metrics;

/// <summary>
/// Metrics for entangled promise behavior across distributed uncertainty.
/// </summary>
internal static class QuantumEntanglementMetrics
{
    private static readonly Meter Meter = new("ProcrastiN8.JustBecause.QuantumEntanglement", "1.0.0");

    public static readonly Counter<long> Entanglements =
        Meter.CreateCounter<long>("quantum_entanglement.entangled", unit: "count", description: "Entangled promise registrations");

    public static readonly Counter<long> Collapses =
        Meter.CreateCounter<long>("quantum_entanglement.collapses", unit: "count", description: "Collapses triggered");

    public static readonly Counter<long> RippleAttempts =
        Meter.CreateCounter<long>("quantum_entanglement.ripple_attempts", unit: "count", description: "Triggered ripple collapses");

    public static readonly Counter<long> RippleFailures =
        Meter.CreateCounter<long>("quantum_entanglement.ripple_failures", unit: "count", description: "Failed ripple observations");

    public static readonly Histogram<double> CollapseLatency =
        Meter.CreateHistogram<double>("quantum_entanglement.collapse_latency", unit: "ms", description: "Latency of primary collapse");
}