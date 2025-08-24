namespace ProcrastiN8.Services;

/// <summary>Ergonomic extension methods for common scheduling modes.</summary>
public static class ProcrastinationSchedulerExtensions
{
    public static Task ScheduleMovingTarget(this IProcrastinationScheduler scheduler, Func<Task> task, TimeSpan initial, CancellationToken ct = default)
        => scheduler.Schedule(task, initial, ProcrastinationMode.MovingTarget, ct);

    public static Task<ProcrastinationResult> ScheduleMovingTargetWithResult(this IProcrastinationScheduler scheduler, Func<Task> task, TimeSpan initial, CancellationToken ct = default)
        => scheduler.ScheduleWithResult(task, initial, ProcrastinationMode.MovingTarget, ct);
}