namespace ProcrastiN8.JustBecause;

/// <summary>
/// Exception thrown when a quantum promise collapses to existential nothingness.
/// </summary>
public sealed class CollapseToVoidException(string message) : CollapseException(message)
{
}