using ProcrastiN8.Services;

namespace ProcrastiN8.Services.Diagnostics;

/// <summary>
/// Observer that forwards structured procrastination events to <see cref="ProcrastinationDiagnostics"/> for metric aggregation.
/// </summary>
public sealed class MetricsObserver : IProcrastinationObserver
{
    public Task OnCycleAsync(ProcrastinationContext context, CancellationToken ct) => Task.CompletedTask;
    public Task OnExcuseAsync(ProcrastinationContext context, CancellationToken ct) => Task.CompletedTask;
    public Task OnTriggeredAsync(ProcrastinationContext context, CancellationToken ct) => Task.CompletedTask;
    public Task OnAbandonedAsync(ProcrastinationContext context, CancellationToken ct) => Task.CompletedTask;
    public Task OnExecutedAsync(ProcrastinationResult result, CancellationToken ct) => Task.CompletedTask;
    public Task OnEventAsync(ProcrastinationObserverEvent evt, CancellationToken ct)
    {
        ProcrastinationDiagnostics.RecordEvent(evt);
        return Task.CompletedTask;
    }
}
