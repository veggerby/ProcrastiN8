namespace ProcrastiN8.JustBecause;

public sealed class CollapseTooLateException(string message) : Exception(message)
{
}