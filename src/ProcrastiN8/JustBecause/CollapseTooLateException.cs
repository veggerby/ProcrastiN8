namespace ProcrastiN8.JustBecause;

/// <summary>
/// Exception thrown when a quantum promise is observed after the observation window has expired.
/// </summary>
public sealed class CollapseTooLateException(string message) : CollapseException(message)
{
}