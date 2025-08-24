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
    // New structured payload callbacks (optional override)
    Task OnEventAsync(ProcrastinationObserverEvent evt, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

/// <summary>Immutable observer event payload.</summary>
public sealed record ProcrastinationObserverEvent(
    Guid CorrelationId,
    ProcrastinationMode Mode,
    string EventType,
    int Cycles,
    int Excuses,
    bool Triggered,
    bool Abandoned,
    DateTimeOffset Timestamp);
