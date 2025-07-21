namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Exception thrown when all possible excuses for procrastination are exhausted.
/// </summary>
public class ArgumentOutOfExcusesException(string message) : Exception(message)
{
}