using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

/// <summary>
/// Abstract base providing common instrumentation and helper methods for procrastination strategies.
/// </summary>
public abstract class ProcrastinationStrategyBase : IResultReportingProcrastinationStrategy
{
    private readonly ProcrastinationResult _result = new();
    private DateTimeOffset _startUtc;
    private IProcrastinationExecutionControl? _control;
    private IReadOnlyList<IProcrastinationObserver> _observers = Array.Empty<IProcrastinationObserver>();
    private IExecutionSafetyOptions _safety = DefaultExecutionSafetyOptions.Instance; // default safety options.
    private static IExecutionSafetyOptions _ambientSafety = DefaultExecutionSafetyOptions.Instance;

    /// <summary>Exposes the mutable context from the attached control, if available.</summary>
    protected ProcrastinationContext? ControlContext => _control?.Context;
    /// <summary>UTC time scheduling began (for derived elapsed calculations).</summary>
    protected DateTimeOffset StartUtc => _startUtc;

    /// <inheritdoc />
    public ProcrastinationResult LastResult => _result;

    /// <inheritdoc />
    public async Task ExecuteAsync(
        Func<Task> task,
        TimeSpan initialDelay,
        IExcuseProvider? excuseProvider,
        IDelayStrategy delayStrategy,
        IRandomProvider randomProvider,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        _startUtc = timeProvider.GetUtcNow();
        _result.StartedUtc = _startUtc;
        if (_result.CorrelationId == Guid.Empty)
        {
            _result.CorrelationId = Guid.NewGuid();
        }
        if (_safety == DefaultExecutionSafetyOptions.Instance && _ambientSafety != DefaultExecutionSafetyOptions.Instance)
        {
            _safety = _ambientSafety; // adopt ambient if not explicitly overridden
        }
        _result.Executed = false;
        _result.ExcuseCount = 0;
        _result.Cycles = 0;
        _control?.MarkStatus(ProcrastinationStatus.Deferring);
        await ExecuteCoreAsync(task, initialDelay, excuseProvider, delayStrategy, randomProvider, timeProvider, cancellationToken);
        var end = timeProvider.GetUtcNow();
        _result.TotalDeferral = end - _startUtc;
        _result.CompletedUtc = end;
        if (_result.Executed)
        {
            await NotifyExecutedAsync(_result, cancellationToken);
        }
    }

    /// <summary>
    /// Marks the underlying task as executed.
    /// </summary>
    protected void MarkExecuted() => _result.Executed = true;

    /// <summary>
    /// Records a delay cycle increment.
    /// </summary>
    protected void IncrementCycle() => _result.Cycles++;

    /// <summary>
    /// Invokes the excuse provider (if any) and increments counters.
    /// </summary>
    protected async Task InvokeExcuseAsync(IExcuseProvider? excuseProvider)
    {
        if (excuseProvider is null) { return; }
        _result.ExcuseCount++;
        if (_control != null)
        {
            _control.Context.Excuses = _result.ExcuseCount;
        }
        await excuseProvider.GetExcuseAsync();
        await NotifyExcuseAsync(_control?.Context, CancellationToken.None);
    }

    /// <summary>
    /// Attaches execution control surface (invoked by the scheduler).
    /// </summary>
    internal void AttachControl(IProcrastinationExecutionControl control)
    {
        _control = control;
    }

    internal void AttachObservers(IEnumerable<IProcrastinationObserver>? observers)
    {
        if (observers is null)
        {
            _observers = Array.Empty<IProcrastinationObserver>();
            return;
        }
        _observers = observers.ToArray();
    }

    /// <summary>
    /// Checks for external trigger/abandon hints and acts accordingly.
    /// </summary>
    protected async Task<bool> CheckForExternalOverrideAsync(Func<Task> task)
    {
        if (_control == null)
        {
            return false;
        }
        if (_control.AbandonRequested)
        {
            MarkAbandoned();
            await NotifyAbandonedAsync(_control.Context, CancellationToken.None);
            return true; // end silently
        }
        if (_control.TriggerNowRequested && !_result.Executed)
        {
            await task();
            MarkExecuted();
            MarkTriggered();
            _control.MarkStatus(ProcrastinationStatus.Executed);
            await NotifyTriggeredAsync(_control.Context, CancellationToken.None);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks safety caps to avoid runaway loops; returns true if execution should terminate.
    /// </summary>
    protected bool SafetyCapReached()
    {
        return _result.Cycles >= _safety.MaxCycles;
    }

    protected async Task NotifyCycleAsync(ProcrastinationContext? context, CancellationToken ct)
    {
        var evt = new ProcrastinationObserverEvent(_result.CorrelationId, _result.Mode, "cycle", _result.Cycles, _result.ExcuseCount, _result.Triggered, _result.Abandoned, DateTimeOffset.UtcNow);
        if (context != null && _observers.Count > 0)
        {
            foreach (var o in _observers)
            {
                await o.OnCycleAsync(context, ct);
                await o.OnEventAsync(evt, ct);
            }
        }
        Diagnostics.ProcrastinationDiagnostics.RecordEvent(evt);
    }

    private async Task NotifyExcuseAsync(ProcrastinationContext? context, CancellationToken ct)
    {
        var evt = new ProcrastinationObserverEvent(_result.CorrelationId, _result.Mode, "excuse", _result.Cycles, _result.ExcuseCount, _result.Triggered, _result.Abandoned, DateTimeOffset.UtcNow);
        if (context != null && _observers.Count > 0)
        {
            foreach (var o in _observers)
            {
                await o.OnExcuseAsync(context, ct);
                await o.OnEventAsync(evt, ct);
            }
        }
        Diagnostics.ProcrastinationDiagnostics.RecordEvent(evt);
    }

    private async Task NotifyTriggeredAsync(ProcrastinationContext? context, CancellationToken ct)
    {
        var evt = new ProcrastinationObserverEvent(_result.CorrelationId, _result.Mode, "triggered", _result.Cycles, _result.ExcuseCount, true, _result.Abandoned, DateTimeOffset.UtcNow);
        if (context != null && _observers.Count > 0)
        {
            foreach (var o in _observers)
            {
                await o.OnTriggeredAsync(context, ct);
                await o.OnEventAsync(evt, ct);
            }
        }
        Diagnostics.ProcrastinationDiagnostics.RecordEvent(evt);
    }

    private async Task NotifyAbandonedAsync(ProcrastinationContext? context, CancellationToken ct)
    {
        var evt = new ProcrastinationObserverEvent(_result.CorrelationId, _result.Mode, "abandoned", _result.Cycles, _result.ExcuseCount, _result.Triggered, true, DateTimeOffset.UtcNow);
        if (context != null && _observers.Count > 0)
        {
            foreach (var o in _observers)
            {
                await o.OnAbandonedAsync(context, ct);
                await o.OnEventAsync(evt, ct);
            }
        }
        Diagnostics.ProcrastinationDiagnostics.RecordEvent(evt);
    }

    private async Task NotifyExecutedAsync(ProcrastinationResult result, CancellationToken ct)
    {
        var evt = new ProcrastinationObserverEvent(result.CorrelationId, result.Mode, "executed", result.Cycles, result.ExcuseCount, result.Triggered, result.Abandoned, DateTimeOffset.UtcNow);
        if (_observers.Count > 0)
        {
            foreach (var o in _observers)
            {
                await o.OnExecutedAsync(result, ct);
                await o.OnEventAsync(evt, ct);
            }
        }
        Diagnostics.ProcrastinationDiagnostics.RecordEvent(evt);
    }

    /// <summary>
    /// Strategy-specific logic implementation.
    /// </summary>
    protected abstract Task ExecuteCoreAsync(
        Func<Task> task,
        TimeSpan initialDelay,
        IExcuseProvider? excuseProvider,
        IDelayStrategy delayStrategy,
        IRandomProvider randomProvider,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken);

    /// <summary>Marks that execution was externally triggered.</summary>
    protected void MarkTriggered() => _result.Triggered = true;

    /// <summary>Marks that execution was abandoned.</summary>
    protected void MarkAbandoned() => _result.Abandoned = true;

    /// <summary>Allows derived strategies or factories to override safety options.</summary>
    public void ConfigureSafety(IExecutionSafetyOptions safety) => _safety = safety ?? DefaultExecutionSafetyOptions.Instance;

    /// <summary>Sets global ambient safety applied to strategies that have not been explicitly configured.</summary>
    public static void SetAmbientSafety(IExecutionSafetyOptions safety) => _ambientSafety = safety ?? DefaultExecutionSafetyOptions.Instance;
    /// <summary>Resets ambient safety to defaults (test isolation helper).</summary>
    public static void ResetAmbientSafety() => _ambientSafety = DefaultExecutionSafetyOptions.Instance;
}
