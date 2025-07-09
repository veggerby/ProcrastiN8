using System.Diagnostics.Metrics;

namespace ProcrastiN8.Metrics;

public static class ProcrastinationMetrics
{
    private static readonly Meter Meter = new("ProcrastiN8.Core", "1.0.0");

    // Total time spent procrastinating (in seconds)
    public static readonly Counter<long> TotalTimeProcrastinated =
        Meter.CreateCounter<long>("procrastin8_total_time_seconds", description: "Total time spent procrastinating");

    // Total number of generated excuses
    public static readonly Counter<long> ExcusesGenerated =
        Meter.CreateCounter<long>("procrastin8_excuses_total", description: "Total number of excuses generated");

    // Number of delayed tasks
    public static readonly Counter<long> DelaysTotal =
        Meter.CreateCounter<long>("procrastin8_delays_total", description: "Total number of task delays");

    // Histogram of snooze durations (in seconds)
    public static readonly Histogram<double> SnoozeDurations =
        Meter.CreateHistogram<double>("procrastin8_snooze_duration_seconds", description: "Duration of task snoozes");

    // Number of witty commentary messages emitted
    public static readonly Counter<long> CommentaryTotal =
        Meter.CreateCounter<long>("procrastin8_commentary_total", description: "Total number of commentary remarks logged");

    // Number of retry attempts before acting
    public static readonly Counter<long> RetryAttempts =
        Meter.CreateCounter<long>("procrastin8_retry_attempts_total", description: "Number of retries before performing a task");

    // Tasks eventually completed
    public static readonly Counter<long> TasksCompleted =
        Meter.CreateCounter<long>("procrastin8_tasks_completed_total", description: "Total number of tasks completed");

    // Tasks that were never completed
    public static readonly Counter<long> TasksNeverDone =
        Meter.CreateCounter<long>("procrastin8_tasks_never_done_total", description: "Tasks that were completely abandoned");
}