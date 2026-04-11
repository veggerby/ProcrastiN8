namespace ProcrastiN8.JustBecause;

/// <summary>
/// Default implementation of <see cref="IRandomProvider"/> using <see cref="System.Random"/>.
/// </summary>
/// <remarks>
/// This provider ensures testability and consistency in randomness across ProcrastiN8 components.
/// Uses <see cref="Random.Shared"/>, which is thread-safe for concurrent access.
/// </remarks>
public class RandomProvider : IRandomProvider
{
    public static readonly IRandomProvider Default = new RandomProvider();

    private RandomProvider() { }

    public double GetDouble() => Random.Shared.NextDouble();
}