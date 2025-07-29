namespace ProcrastiN8.JustBecause;

public static class RandomProviderExtensions
{
    /// <summary>
    /// Returns a random integer within the specified range.
    /// </summary>
    public static int GetRandom(this IRandomProvider randomProvider, int minValue, int maxValue) =>
        minValue + (int)Math.Floor(randomProvider.GetDouble() * (maxValue - minValue));



    public static int GetRandom(this IRandomProvider randomProvider, int maxValue) => randomProvider.GetRandom(0, maxValue);
}