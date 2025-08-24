namespace ProcrastiN8.Services;

internal interface IProcrastinationExecutionControl
{
    bool TriggerNowRequested { get; }
    bool AbandonRequested { get; }
    void MarkStatus(ProcrastinationStatus status);
    ProcrastinationContext Context { get; }
}