namespace ProcrastiN8.Services;

/// <summary>
/// Provides an interactive control surface for a scheduled procrastination workflow.
/// </summary>
public sealed class ProcrastinationHandle : IProcrastinationExecutionControl
{
    private readonly TaskCompletionSource<ProcrastinationResult> _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private volatile bool _triggerNow;
    private volatile bool _abandon;
    private volatile ProcrastinationStatus _status = ProcrastinationStatus.Pending;

    internal ProcrastinationHandle(ProcrastinationMode mode)
    {
        Mode = mode;
    }

    /// <summary>The mode originally requested.</summary>
    public ProcrastinationMode Mode { get; }

    /// <summary>Gets the current status.</summary>
    public ProcrastinationStatus Status => _status;

    /// <summary>Gets a task that completes when the workflow finishes.</summary>
    public Task<ProcrastinationResult> Completion => _tcs.Task;

    /// <summary>Current evolving context snapshot.</summary>
    public ProcrastinationContext Context { get; } = new();

    /// <summary>Requests immediate execution at the next safe checkpoint.</summary>
    public void TriggerNow()
    {
        _triggerNow = true;
        MarkStatus(ProcrastinationStatus.Triggered);
    }

    /// <summary>Requests abandoning the workflow without executing the underlying task.</summary>
    public void Abandon()
    {
        _abandon = true;
        MarkStatus(ProcrastinationStatus.Abandoned);
    }

    internal void Complete(ProcrastinationResult result)
    {
        if (!_tcs.Task.IsCompleted)
        {
            _tcs.TrySetResult(result);
        }
    }

    internal void Cancel()
    {
        if (!_tcs.Task.IsCompleted)
        {
            _tcs.TrySetCanceled();
        }
    }

    bool IProcrastinationExecutionControl.TriggerNowRequested => _triggerNow;
    bool IProcrastinationExecutionControl.AbandonRequested => _abandon;

    void IProcrastinationExecutionControl.MarkStatus(ProcrastinationStatus status)
    {
        MarkStatus(status);
    }

    private void MarkStatus(ProcrastinationStatus status)
    {
        _status = status;
    }
}
