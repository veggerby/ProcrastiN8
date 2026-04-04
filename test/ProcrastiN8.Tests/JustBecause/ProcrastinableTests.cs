using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.JustBecause;

public class ProcrastinableTests
{
    [Fact]
    public async Task EvaluateAsync_ExecutesFactory_And_ReturnsResult()
    {
        // arrange — after deliberation, the factory is called
        var delayProvider = Substitute.For<IDelayProvider>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetRandom(Arg.Any<int>(), Arg.Any<int>()).Returns(10);

        var procrastinable = new Procrastinable<int>(
            () => Task.FromResult(99),
            delayProvider,
            randomProvider);

        // act
        var result = await procrastinable.EvaluateAsync();

        // assert — eventually, something gets done
        result.Should().Be(99, "the factory is invoked after deliberation, not instead of it");
        procrastinable.IsEvaluated.Should().BeTrue("the result has been obtained and cached");
    }

    [Fact]
    public async Task EvaluateAsync_SecondCall_ReturnsCachedResult()
    {
        // arrange
        var callCount = 0;
        var delayProvider = Substitute.For<IDelayProvider>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetRandom(Arg.Any<int>(), Arg.Any<int>()).Returns(10);

        var procrastinable = new Procrastinable<int>(
            () => { callCount++; return Task.FromResult(7); },
            delayProvider,
            randomProvider);

        // act — evaluate twice
        var first = await procrastinable.EvaluateAsync();
        var second = await procrastinable.EvaluateAsync();

        // assert — factory called only once; result is reused without further deliberation
        first.Should().Be(7);
        second.Should().Be(7, "the second call returns cached results — we already went through all that deliberation once");
        callCount.Should().Be(1, "the factory is only invoked once regardless of how many times EvaluateAsync is called");
    }

    [Fact]
    public void Value_BeforeEvaluation_Throws()
    {
        // arrange — attempting to access the value before evaluating is impatient and wrong
        var procrastinable = new Procrastinable<string>(() => Task.FromResult("someday"));

        // act
        Action act = () => _ = procrastinable.Value;

        // assert
        act.Should().Throw<InvalidOperationException>("accessing the value before evaluation is the eager anti-pattern");
    }

    [Fact]
    public async Task Value_AfterEvaluation_ReturnsResult()
    {
        // arrange
        var delayProvider = Substitute.For<IDelayProvider>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetRandom(Arg.Any<int>(), Arg.Any<int>()).Returns(5);

        var procrastinable = new Procrastinable<string>(
            () => Task.FromResult("eventually"),
            delayProvider,
            randomProvider);

        await procrastinable.EvaluateAsync();

        // act + assert
        procrastinable.Value.Should().Be("eventually", "after evaluation, the value is accessible synchronously");
    }

    [Fact]
    public void NullFactory_ThrowsArgumentNullException()
    {
        // arrange + act
        Action act = () => _ = new Procrastinable<int>(null!);

        // assert
        act.Should().Throw<ArgumentNullException>("even ProcrastiN8 has limits — null factories are not a valid excuse");
    }
}
