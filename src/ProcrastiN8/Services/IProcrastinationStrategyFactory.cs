namespace ProcrastiN8.Services;

/// <summary>
/// Factory for creating procrastination strategies based on a mode.
/// </summary>
public interface IProcrastinationStrategyFactory
{
    /// <summary>
    /// Creates an appropriate strategy for the specified <paramref name="mode"/>.
    /// </summary>
    IProcrastinationStrategy Create(ProcrastinationMode mode);
}