namespace ProcrastiN8.JustBecause;

/// <summary>
/// Provides a source of randomness for testability.
/// </summary>
public interface IRandomProvider
{
    /// <summary>
    /// Returns a random double between 0.0 and 1.0.
    /// </summary>
    double GetDouble();
}