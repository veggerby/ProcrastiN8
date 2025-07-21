namespace ProcrastiN8.JustBecause;

/// <summary>
/// Exception thrown when a quantum promise is observed too early, causing quantum instability.
/// </summary>
public sealed class CollapseTooEarlyException(string message) : CollapseException(message)
{
}