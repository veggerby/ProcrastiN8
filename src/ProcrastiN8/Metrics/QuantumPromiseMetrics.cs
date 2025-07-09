using System.Diagnostics.Metrics;

namespace ProcrastiN8.Metrics;

/// <summary>
/// Exposes OpenTelemetry metrics for quantum promise behavior.
/// </summary>
internal static class QuantumPromiseMetrics
{
    private static readonly Meter Meter = new("ProcrastiN8.JustBecause.QuantumPromise", "1.0.0");

    public static readonly Counter<long> Observations =
        Meter.CreateCounter<long>("quantum_promise.observations", unit: "count", description: "Observation attempts");

    public static readonly Counter<long> Collapses =
        Meter.CreateCounter<long>("quantum_promise.collapses", unit: "count", description: "Successful collapses");

    public static readonly Counter<long> Failures =
        Meter.CreateCounter<long>("quantum_promise.failures", unit: "count", description: "Failed collapses");

    public static readonly Histogram<double> CollapseDuration =
        Meter.CreateHistogram<double>("quantum_promise.collapse_duration", unit: "ms", description: "Collapse execution time");
}