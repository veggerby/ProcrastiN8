namespace ProcrastiN8.JustBecause;

/// <summary>
/// Provides a source of randomness for testability.
/// </summary>
public interface IRandomProvider
{
    /// <summary>
    /// Returns a non-negative random integer less than the specified maximum.
    /// </summary>
    int Next(int maxValue);
}