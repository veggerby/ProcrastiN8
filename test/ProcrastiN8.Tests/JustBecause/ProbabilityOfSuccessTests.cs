using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.JustBecause;

public class ProbabilityOfSuccessTests
{
    [Fact]
    public async Task ExecuteAsync_WithFullProbability_AlwaysSucceeds()
    {
        // arrange
        var deterministicRandom = Substitute.For<IRandomProvider>();
        deterministicRandom.GetDouble().Returns(0.0); // always below threshold → always executes

        var executed = false;

        // act
        await ProbabilityOfSuccess.ExecuteAsync(
            () => { executed = true; return Task.CompletedTask; },
            successProbability: 1.0,
            randomProvider: deterministicRandom);

        // assert
        executed.Should().BeTrue("a 100% success probability means the universe cooperates — just this once");
    }

    [Fact]
    public async Task ExecuteAsync_WithZeroProbability_AlwaysThrows()
    {
        // arrange
        var deterministicRandom = Substitute.For<IRandomProvider>();
        deterministicRandom.GetDouble().Returns(0.99); // always >= threshold → always fails

        // act
        Func<Task> act = () => ProbabilityOfSuccess.ExecuteAsync(
            () => Task.CompletedTask,
            successProbability: 0.0,
            randomProvider: deterministicRandom);

        // assert
        await act.Should().ThrowAsync<QuantumUncertaintyException>("a 0% success probability is fate's way of saying 'absolutely not'");
    }

    [Fact]
    public async Task ExecuteAsync_Typed_ReturnsResult_OnSuccess()
    {
        // arrange
        var deterministicRandom = Substitute.For<IRandomProvider>();
        deterministicRandom.GetDouble().Returns(0.0); // always succeeds

        // act
        var result = await ProbabilityOfSuccess.ExecuteAsync(
            () => Task.FromResult(42),
            successProbability: 1.0,
            randomProvider: deterministicRandom);

        // assert
        result.Should().Be(42, "when fate cooperates, the correct value is returned");
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsQuantumUncertaintyException_WithCorrectProbability()
    {
        // arrange
        var deterministicRandom = Substitute.For<IRandomProvider>();
        deterministicRandom.GetDouble().Returns(0.99); // always fails

        const double offeredProbability = 0.5;

        // act
        Func<Task> act = () => ProbabilityOfSuccess.ExecuteAsync(
            () => Task.CompletedTask,
            successProbability: offeredProbability,
            randomProvider: deterministicRandom);

        // assert
        var exception = await act.Should().ThrowAsync<QuantumUncertaintyException>();
        exception.Which.OfferedProbability.Should().Be(offeredProbability, "the exception records the probability that the universe declined");
    }

    [Fact]
    public async Task ExecuteAsync_InvalidProbability_ThrowsArgumentOutOfRange()
    {
        // arrange + act
        Func<Task> act = () => ProbabilityOfSuccess.ExecuteAsync(
            () => Task.CompletedTask,
            successProbability: 1.5);

        // assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>("probability must remain earthbound, unlike the feature roadmap");
    }

    [Fact]
    public async Task ExecuteAsync_RespectsCancellationToken()
    {
        // arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // act
        Func<Task> act = () => ProbabilityOfSuccess.ExecuteAsync(
            () => Task.CompletedTask,
            successProbability: 1.0,
            cancellationToken: cts.Token);

        // assert — cancellation is respected before fate is consulted
        await act.Should().ThrowAsync<OperationCanceledException>("even fate respects cancellation tokens");
    }
}
