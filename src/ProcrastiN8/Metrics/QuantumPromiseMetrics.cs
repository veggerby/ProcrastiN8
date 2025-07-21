using System.Diagnostics.Metrics;

namespace ProcrastiN8.Metrics;

/// <summary>
/// Exposes OpenTelemetry metrics for quantum promise behavior.
/// </summary>
internal static class QuantumPromiseMetrics
{
    /// <summary>
    /// The OpenTelemetry Meter for QuantumPromise metrics.
    /// </summary>
    private static readonly Meter Meter = new("ProcrastiN8.JustBecause.QuantumPromise", "1.0.0");

    /// <summary>
    /// Counts the number of observation attempts on quantum promises.
    /// </summary>
    public static readonly Counter<long> Observations =
        Meter.CreateCounter<long>("quantum_promise.observations", unit: "count", description: "Observation attempts");

    /// <summary>
    /// Counts the number of successful collapses of quantum promises.
    /// </summary>
    public static readonly Counter<long> Collapses =
        Meter.CreateCounter<long>("quantum_promise.collapses", unit: "count", description: "Successful collapses");

    /// <summary>
    /// Counts the number of failed collapse attempts on quantum promises.
    /// </summary>
    public static readonly Counter<long> Failures =
        Meter.CreateCounter<long>("quantum_promise.failures", unit: "count", description: "Failed collapses");

    /// <summary>
    /// Records the execution time of quantum promise collapses, in milliseconds.
    /// </summary>
    public static readonly Histogram<double> CollapseDuration =
        Meter.CreateHistogram<double>("quantum_promise.collapse_duration", unit: "ms", description: "Collapse execution time");
}