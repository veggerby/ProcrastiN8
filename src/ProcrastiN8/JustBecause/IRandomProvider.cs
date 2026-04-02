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

    /// <summary>
    /// Returns a random integer between 0 (inclusive) and <paramref name="maxValue"/> (exclusive).
    /// </summary>
    int GetRandom(int maxValue) => (int)Math.Floor(GetDouble() * maxValue);

    /// <summary>
    /// Returns a random integer between <paramref name="minValue"/> (inclusive) and <paramref name="maxValue"/> (exclusive).
    /// </summary>
    int GetRandom(int minValue, int maxValue) => minValue + (int)Math.Floor(GetDouble() * (maxValue - minValue));
}