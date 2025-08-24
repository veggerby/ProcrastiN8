using ProcrastiN8.Common;

namespace ProcrastiN8.Services;

/// <summary>
/// Instance-based abstraction over the static <see cref="ProcrastinationScheduler"/> to facilitate dependency injection without external packages.
/// </summary>
public interface IProcrastinationScheduler
{
    /// <summary>Schedules a task with the specified procrastination mode; completes when the underlying task (eventually) executes.</summary>
    Task Schedule(Func<Task> task, TimeSpan initialDelay, ProcrastinationMode mode, CancellationToken cancellationToken = default);
    /// <summary>Schedules a task and returns a detailed result containing metrics and flags (Triggered / Abandoned).</summary>
    Task<ProcrastinationResult> ScheduleWithResult(Func<Task> task, TimeSpan initialDelay, ProcrastinationMode mode, CancellationToken cancellationToken = default);
    /// <summary>Schedules a task and returns a handle permitting external intervention (TriggerNow / Abandon).</summary>
    ProcrastinationHandle ScheduleWithHandle(Func<Task> task, TimeSpan initialDelay, ProcrastinationMode mode, CancellationToken cancellationToken = default);
}