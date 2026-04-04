using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class DefaultDelayStrategyTests
{
    [Fact]
    public void Substitute_Works_Correctly()
    {
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0D);

        var value = randomProvider.GetDouble();

        Assert.Equal(0D, value);
    }

    [Fact]
    public async Task DelayAsync_Uses_Custom_RandomProvider()
    {
        // arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetRandom(Arg.Any<int>(), Arg.Any<int>()).Returns(0);
        var strategy = new DefaultDelayStrategy(
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(200),
            randomProvider: randomProvider);

        // act
        await strategy.DelayAsync();

        // assert — the strategy should consult GetRandom for its procrastination arithmetic
        randomProvider.Received(1).GetRandom(Arg.Any<int>(), Arg.Any<int>());
    }

    [Fact]
    public async Task DelayAsync_Respects_Cancellation()
    {
        // Arrange
        var strategy = new DefaultDelayStrategy(
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(100));
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => strategy.DelayAsync(null, null, null, cts.Token));
    }
}