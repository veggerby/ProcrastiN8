namespace ProcrastiN8.Services;

/// <summary>
/// Represents the current status of a scheduled procrastination workflow.
/// </summary>
public enum ProcrastinationStatus
{
    Pending,
    Deferring,
    Triggered,
    Executed,
    Abandoned,
    Cancelled
}