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

    /// <summary>
    /// Returns a random integer within the specified range.
    /// </summary>
    int Next(int minValue, int maxValue);

    /// <summary>
    /// Returns a random double between 0.0 and 1.0.
    /// </summary>
    double NextDouble();
}