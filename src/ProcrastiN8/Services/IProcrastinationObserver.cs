namespace ProcrastiN8.Services;

/// <summary>
/// Observer for procrastination lifecycle events.
/// </summary>
public interface IProcrastinationObserver
{
    Task OnCycleAsync(ProcrastinationContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;
    Task OnExcuseAsync(ProcrastinationContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;
    Task OnTriggeredAsync(ProcrastinationContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;
    Task OnAbandonedAsync(ProcrastinationContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;
    Task OnExecutedAsync(ProcrastinationResult result, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
