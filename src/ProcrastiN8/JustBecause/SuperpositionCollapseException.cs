namespace ProcrastiN8.JustBecause;

/// <summary>
/// Thrown when an observation is too forceful and prematurely collapses a quantum decision state.
/// </summary>
public sealed class SuperpositionCollapseException(string message) : Exception(message)
{
}