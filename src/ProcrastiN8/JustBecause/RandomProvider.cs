namespace ProcrastiN8.JustBecause;

/// <summary>
/// Default implementation of IRandomProvider using System.Random.
/// </summary>
public class RandomProvider : IRandomProvider
{
    private readonly Random _random = new();

    public int Next(int maxValue) => _random.Next(maxValue);

    public int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);

    public double NextDouble() => _random.NextDouble();
}