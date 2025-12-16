using ProcrastiN8.JustBecause;
using ProcrastiN8.NeuralExcuseLab;

namespace ProcrastiN8.Tests.NeuralExcuseLab;

public class ExcuseQualityScorerTests
{
    [Fact]
    public async Task CalculateQualityScoreAsync_Should_ReturnScoreBetween0And100()
    {
        // arrange
        var scorer = new ExcuseQualityScorer();

        // act
        var score = await scorer.CalculateQualityScoreAsync("Test excuse");

        // assert
        score.Should().BeInRange(0.0, 100.0, "quality scores must be between 0 and 100");
    }

    [Fact]
    public async Task CalculateShameIndexAsync_Should_ReturnIndexBetween0And100()
    {
        // arrange
        var scorer = new ExcuseQualityScorer();

        // act
        var shameIndex = await scorer.CalculateShameIndexAsync("Test excuse");

        // assert
        shameIndex.Should().BeInRange(0.0, 100.0, "shame index must be between 0 and 100");
    }

    [Fact]
    public async Task CalculateShameIndexAsync_WithQuantumExcuse_Should_ReduceShame()
    {
        // arrange
        var randomProvider = new FakeRandomProvider(0.0);
        var scorer = new ExcuseQualityScorer(randomProvider);

        // act
        var normalShame = await scorer.CalculateShameIndexAsync("Normal excuse");
        var quantumShame = await scorer.CalculateShameIndexAsync("Quantum entanglement excuse");

        // assert
        quantumShame.Should().BeLessThan(normalShame, "quantum excuses are shameless");
    }

    [Fact]
    public async Task CalculateShameIndexAsync_WithProductionExcuse_Should_IncreaseShame()
    {
        // arrange
        var randomProvider = new FakeRandomProvider(0.0);
        var scorer = new ExcuseQualityScorer(randomProvider);

        // act
        var normalShame = await scorer.CalculateShameIndexAsync("Normal excuse");
        var productionShame = await scorer.CalculateShameIndexAsync("Production issues excuse");

        // assert
        productionShame.Should().BeGreaterThan(normalShame, "production excuses are shameful");
    }

    [Fact]
    public async Task CalculateQualityScoreAsync_WithLongerExcuse_Should_TendToScoreHigher()
    {
        // arrange
        var randomProvider = new FakeRandomProvider(0.0);
        var scorer = new ExcuseQualityScorer(randomProvider);

        // act
        var shortScore = await scorer.CalculateQualityScoreAsync("Short");
        var longScore = await scorer.CalculateQualityScoreAsync("A much longer and more elaborate excuse that goes on and on with many words");

        // assert
        longScore.Should().BeGreaterThan(shortScore, "longer excuses appear more sophisticated");
    }

    // Helper class for testing
    private class FakeRandomProvider : IRandomProvider
    {
        private readonly double _value;
        
        public FakeRandomProvider(double value) => _value = value;
        
        public double GetDouble() => _value;
    }
}
