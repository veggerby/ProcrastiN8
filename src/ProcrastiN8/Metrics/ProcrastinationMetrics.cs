using System.Diagnostics.Metrics;

namespace ProcrastiN8.Metrics;

public static class ProcrastinationMetrics
{
    /// <summary>
    /// Version number for ProcrastinationMetrics.
    /// </summary>
    private const double ProcrastinationMetricsVersion = 1.0;

    /// <summary>
    /// The OpenTelemetry Meter for ProcrastinationMetrics.
    /// </summary>
    private static readonly Meter Meter = new("ProcrastiN8.Core", $"{ProcrastinationMetricsVersion}.0");

    /// <summary>
    /// Counts the total time spent procrastinating (in seconds).
    /// </summary>
    public static readonly Counter<long> TotalTimeProcrastinated =
        Meter.CreateCounter<long>("procrastin8_total_time_seconds", description: "Total time spent procrastinating");

    /// <summary>
    /// Counts the total number of generated excuses.
    /// </summary>
    public static readonly Counter<long> ExcusesGenerated =
        Meter.CreateCounter<long>("procrastin8_excuses_total", description: "Total number of excuses generated");

    /// <summary>
    /// Counts the total number of task delays.
    /// </summary>
    public static readonly Counter<long> DelaysTotal =
        Meter.CreateCounter<long>("procrastin8_delays_total", description: "Total number of task delays");

    /// <summary>
    /// Records the duration of task snoozes (in seconds).
    /// </summary>
    public static readonly Histogram<double> SnoozeDurations =
        Meter.CreateHistogram<double>("procrastin8_snooze_duration_seconds", description: "Duration of task snoozes");

    /// <summary>
    /// Counts the total number of commentary remarks logged.
    /// </summary>
    public static readonly Counter<long> CommentaryTotal =
        Meter.CreateCounter<long>("procrastin8_commentary_total", description: "Total number of commentary remarks logged");

    /// <summary>
    /// Counts the number of retries before performing a task.
    /// </summary>
    public static readonly Counter<long> RetryAttempts =
        Meter.CreateCounter<long>("procrastin8_retry_attempts_total", description: "Number of retries before performing a task");

    /// <summary>
    /// Counts the total number of tasks completed.
    /// </summary>
    public static readonly Counter<long> TasksCompleted =
        Meter.CreateCounter<long>("procrastin8_tasks_completed_total", description: "Total number of tasks completed");

    /// <summary>
    /// Counts the number of tasks that were completely abandoned.
    /// </summary>
    public static readonly Counter<long> TasksNeverDone =
        Meter.CreateCounter<long>("procrastin8_tasks_never_done_total", description: "Tasks that were completely abandoned");
}