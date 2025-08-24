namespace ProcrastiN8.Services;

/// <summary>
/// Provides an interactive control surface for a scheduled procrastination workflow.
/// </summary>
public sealed class ProcrastinationHandle : IProcrastinationExecutionControl
{
    private readonly TaskCompletionSource<ProcrastinationResult> _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _triggerNow; // 0/1
    private int _abandon;    // 0/1
    private int _status = (int)ProcrastinationStatus.Pending;

    internal ProcrastinationHandle(ProcrastinationMode mode)
    {
        Mode = mode;
    }

    /// <summary>The mode originally requested.</summary>
    public ProcrastinationMode Mode { get; }

    /// <summary>Gets the current status.</summary>
    public ProcrastinationStatus Status => (ProcrastinationStatus)_status;
    /// <summary>Raised when status changes.</summary>
    public event EventHandler<ProcrastinationStatusChangedEventArgs>? StatusChanged;

    /// <summary>Gets a task that completes when the workflow finishes.</summary>
    public Task<ProcrastinationResult> Completion => _tcs.Task;

    /// <summary>Current evolving context snapshot.</summary>
    public ProcrastinationContext Context { get; } = new();

    /// <summary>Requests immediate execution at the next safe checkpoint.</summary>
    public void TriggerNow()
    {
        TryTriggerNow();
    }

    /// <summary>Requests abandoning the workflow without executing the underlying task.</summary>
    public void Abandon()
    {
        if (Interlocked.CompareExchange(ref _abandon, 1, 0) == 0)
        {
            MarkStatus(ProcrastinationStatus.Abandoned);
        }
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

    bool IProcrastinationExecutionControl.TriggerNowRequested => _triggerNow == 1;
    bool IProcrastinationExecutionControl.AbandonRequested => _abandon == 1;

    void IProcrastinationExecutionControl.MarkStatus(ProcrastinationStatus status)
    {
        MarkStatus(status);
    }

    private void MarkStatus(ProcrastinationStatus status)
    {
        var previous = (ProcrastinationStatus)Interlocked.Exchange(ref _status, (int)status);
        if (previous != status)
        {
            StatusChanged?.Invoke(this, new ProcrastinationStatusChangedEventArgs(previous, status));
        }
    }

    /// <summary>Attempts to trigger execution if neither executed nor abandoned yet.</summary>
    public bool TryTriggerNow()
    {
        if (Status is ProcrastinationStatus.Executed or ProcrastinationStatus.Abandoned || _abandon == 1)
        {
            return false;
        }
        if (Interlocked.CompareExchange(ref _triggerNow, 1, 0) == 0)
        {
            MarkStatus(ProcrastinationStatus.Triggered);
            return true;
        }
        return false;
    }
}

/// <summary>Status change event args.</summary>
public sealed class ProcrastinationStatusChangedEventArgs(ProcrastinationStatus previous, ProcrastinationStatus current) : EventArgs
{
    /// <summary>Previous status.</summary>
    public ProcrastinationStatus Previous { get; } = previous;
    /// <summary>Current status.</summary>
    public ProcrastinationStatus Current { get; } = current;
}