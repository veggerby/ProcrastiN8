namespace ProcrastiN8.JustBecause;

/// <summary>
/// Extension methods for <see cref="IRandomProvider"/>.
/// </summary>
/// <remarks>
/// These overloads are preserved for backwards compatibility and delegate to the
/// default interface methods declared on <see cref="IRandomProvider"/>.
/// </remarks>
public static class RandomProviderExtensions
{
    /// <summary>
    /// Returns a random integer within the specified range.
    /// </summary>
    [Obsolete("Use IRandomProvider.GetRandom(int, int) instead.")]
    public static int GetRandom(this IRandomProvider randomProvider, int minValue, int maxValue) =>
        randomProvider.GetRandom(minValue, maxValue);

    /// <summary>
    /// Returns a random integer between 0 (inclusive) and <paramref name="maxValue"/> (exclusive).
    /// </summary>
    [Obsolete("Use IRandomProvider.GetRandom(int) instead.")]
    public static int GetRandom(this IRandomProvider randomProvider, int maxValue) =>
        randomProvider.GetRandom(maxValue);
}