using ProcrastiN8.Common;

namespace ProcrastiN8.Services;

/// <summary>
/// Logs earnest commentary about procrastination lifecycle events.
/// </summary>
public sealed class LoggingProcrastinationObserver(IProcrastiLogger? logger = null) : IProcrastinationObserver
{
    private readonly IProcrastiLogger? _logger = logger;

    public Task OnCycleAsync(ProcrastinationContext context, CancellationToken cancellationToken = default)
    {
        _logger?.Debug($"Cycle {context.DelayCycles} elapsed={context.Elapsed.TotalMilliseconds:n0}ms excuses={context.ExcuseCount}");
        return Task.CompletedTask;
    }

    public Task OnExcuseAsync(ProcrastinationContext context, CancellationToken cancellationToken = default)
    {
        _logger?.Info($"Excuse #{context.ExcuseCount} solemnly recorded.");
        return Task.CompletedTask;
    }

    public Task OnTriggeredAsync(ProcrastinationContext context, CancellationToken cancellationToken = default)
    {
        _logger?.Info("External trigger requested acceleration of inevitable execution.");
        return Task.CompletedTask;
    }

    public Task OnAbandonedAsync(ProcrastinationContext context, CancellationToken cancellationToken = default)
    {
        _logger?.Warn("Procrastination session abandoned prior to achieving narrative closure.");
        return Task.CompletedTask;
    }

    public Task OnExecutedAsync(ProcrastinationResult result, CancellationToken cancellationToken = default)
    {
        _logger?.Info($"Task executed after {result.Cycles} cycles and {result.ExcuseCount} excuses.");
        return Task.CompletedTask;
    }
}