using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.JustBecause;

public class QuantumAbortTokenTests
{
    [Fact]
    public void Token_Is_Not_Cancelled_Initially()
    {
        // arrange + act
        using var qat = new QuantumAbortToken();

        // assert
        qat.IsAborted.Should().BeFalse("the token should not abort itself at construction — that would defeat the point");
    }

    [Fact]
    public void ObserveImportance_With_FullProbability_Always_Cancels()
    {
        // arrange — 100% probability means even moderate importance triggers cancellation
        var deterministicRandom = Substitute.For<IRandomProvider>();
        deterministicRandom.GetDouble().Returns(0.0); // always below any threshold

        using var qat = new QuantumAbortToken(baseCancellationProbability: 1.0, deterministicRandom);

        // act
        qat.ObserveImportance(importance: 1.0);

        // assert
        qat.IsAborted.Should().BeTrue("a 100% probability guarantee means the universe always cancels at the worst moment");
    }

    [Fact]
    public void ObserveImportance_With_ZeroProbability_NeverCancels()
    {
        // arrange — 0% probability means the universe is having an unusually cooperative day
        var deterministicRandom = Substitute.For<IRandomProvider>();
        deterministicRandom.GetDouble().Returns(0.99); // always above any threshold

        using var qat = new QuantumAbortToken(baseCancellationProbability: 0.0, deterministicRandom);

        // act — observe many times; nothing should happen
        for (var i = 0; i < 10; i++)
        {
            qat.ObserveImportance(importance: 100.0);
        }

        // assert
        qat.IsAborted.Should().BeFalse("a 0% probability means the task was mercifully spared");
    }

    [Fact]
    public void AbortImmediately_Cancels_Without_Observation()
    {
        // arrange
        using var qat = new QuantumAbortToken();

        // act
        qat.AbortImmediately();

        // assert
        qat.IsAborted.Should().BeTrue("AbortImmediately cancels unconditionally — no fate consultation required");
        qat.Token.IsCancellationRequested.Should().BeTrue();
    }

    [Fact]
    public void Token_Is_Usable_With_CancellationToken_APIs()
    {
        // arrange
        var deterministicRandom = Substitute.For<IRandomProvider>();
        deterministicRandom.GetDouble().Returns(0.0);

        using var qat = new QuantumAbortToken(baseCancellationProbability: 1.0, deterministicRandom);
        qat.ObserveImportance();

        // act + assert — the token integrates seamlessly with standard BCL cancellation
        var token = qat.Token;
        token.IsCancellationRequested.Should().BeTrue("the quantum abort token is a standard CancellationToken in a trenchcoat");
    }

    [Fact]
    public void InvalidProbability_Throws_ArgumentOutOfRangeException()
    {
        // arrange + act
        Action act = () => _ = new QuantumAbortToken(baseCancellationProbability: 1.5);

        // assert
        act.Should().Throw<ArgumentOutOfRangeException>("probability must stay within the confines of ordinary mathematics");
    }
}
