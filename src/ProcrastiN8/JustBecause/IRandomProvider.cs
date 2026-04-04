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
    int GetRandom(int maxValue)
    {
        if (maxValue <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxValue), "maxValue must be greater than zero.");
        }

        var sample = Math.Clamp(GetDouble(), 0.0d, 1.0d);
        return Math.Min((int)Math.Floor(sample * maxValue), maxValue - 1);
    }

    /// <summary>
    /// Returns a random integer between <paramref name="minValue"/> (inclusive) and <paramref name="maxValue"/> (exclusive).
    /// </summary>
    int GetRandom(int minValue, int maxValue)
    {
        if (maxValue <= minValue)
        {
            throw new ArgumentOutOfRangeException(nameof(maxValue), "maxValue must be greater than minValue.");
        }

        var sample = Math.Clamp(GetDouble(), 0.0d, 1.0d);
        var range = maxValue - minValue;
        return Math.Min(minValue + (int)Math.Floor(sample * range), maxValue - 1);
    }
}