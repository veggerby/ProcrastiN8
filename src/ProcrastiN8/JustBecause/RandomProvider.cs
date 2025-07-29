namespace ProcrastiN8.JustBecause;

/// <summary>
/// Default implementation of <see cref="IRandomProvider"/> using <see cref="System.Random"/>.
/// </summary>
/// <remarks>
/// This provider ensures testability and consistency in randomness across ProcrastiN8 components.
/// </remarks>
public class RandomProvider : IRandomProvider
{
    private readonly Random _random = new();

    public static readonly IRandomProvider Default = new RandomProvider();

    private RandomProvider() { }

    public int GetRandom(int minValue, int maxValue) => _random.Next(minValue, maxValue);

    public double GetDouble() => _random.NextDouble();
}