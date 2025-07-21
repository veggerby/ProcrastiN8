namespace ProcrastiN8.JustBecause;

/// <summary>
/// Base class for exceptions related to quantum promise collapse.
/// </summary>
public abstract class CollapseException(string message) : Exception(message)
{
}
