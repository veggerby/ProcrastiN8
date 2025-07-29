namespace ProcrastiN8.JustBecause;

/// <summary>
/// Default implementation of IRandomProvider using System.Random.
/// </summary>
public class RandomProvider : IRandomProvider
{
    private readonly Random _random = new();

    public static readonly IRandomProvider Default = new RandomProvider();

    private RandomProvider() { }

    public int GetRandom(int minValue, int maxValue) => _random.Next(minValue, maxValue);

    public double GetDouble() => _random.NextDouble();
}