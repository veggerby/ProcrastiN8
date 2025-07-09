using System.Diagnostics.Metrics;

namespace ProcrastiN8.Metrics;

/// <summary>
/// Exposes OpenTelemetry metrics for QuantumUndecider decision logic.
/// </summary>
internal static class QuantumUndeciderMetrics
{
    private static readonly Meter Meter = new("ProcrastiN8.JustBecause.QuantumUndecider", "1.0.0");

    public static readonly Counter<long> Observations =
        Meter.CreateCounter<long>("quantum_undecider.observations", unit: "count", description: "Total observation attempts");

    public static readonly Counter<long> Failures =
        Meter.CreateCounter<long>("quantum_undecider.failures", unit: "count", description: "Failed or unstable observations");

    public static readonly Counter<long> Outcomes =
        Meter.CreateCounter<long>("quantum_undecider.outcomes", unit: "count", description: "Recorded outcome state");

    public static readonly Histogram<double> DecisionLatency =
        Meter.CreateHistogram<double>("quantum_undecider.latency", unit: "ms", description: "Time spent observing decisions");
}