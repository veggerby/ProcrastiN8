using ProcrastiN8.Services;

namespace ProcrastiN8.Services.Diagnostics;

/// <summary>
/// Observer placeholder for metrics pipelines where an observer instance is expected.
/// Base strategy instrumentation already records counters directly.
/// </summary>
public sealed class MetricsObserver : IProcrastinationObserver
{
    public Task OnCycleAsync(ProcrastinationContext context, CancellationToken ct) => Task.CompletedTask;
    public Task OnExcuseAsync(ProcrastinationContext context, CancellationToken ct) => Task.CompletedTask;
    public Task OnTriggeredAsync(ProcrastinationContext context, CancellationToken ct) => Task.CompletedTask;
    public Task OnAbandonedAsync(ProcrastinationContext context, CancellationToken ct) => Task.CompletedTask;
    public Task OnExecutedAsync(ProcrastinationResult result, CancellationToken ct) => Task.CompletedTask;

    // Event counters are recorded by ProcrastinationStrategyBase to avoid duplication.
    public Task OnEventAsync(ProcrastinationObserverEvent evt, CancellationToken ct) => Task.CompletedTask;
}
