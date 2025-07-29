using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class DefaultDelayStrategyTests
{
    [Fact]
    public async Task DelayAsync_Uses_Custom_RandomProvider()
    {
        // arrange
        var randomProvider = Substitute.For<ProcrastiN8.JustBecause.IRandomProvider>();
        randomProvider.GetDouble().Returns(0);
        var strategy = new DefaultDelayStrategy(
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(200),
            randomProvider: randomProvider);

        // act
        await strategy.DelayAsync();

        // assert
        randomProvider.Received(1).GetDouble();
    }

    [Fact]
    public async Task DelayAsync_Respects_Cancellation()
    {
        var strategy = new DefaultDelayStrategy(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
        var cts = new CancellationTokenSource();
        cts.Cancel();
        await Assert.ThrowsAsync<TaskCanceledException>(() => strategy.DelayAsync(Arg.Any<TimeSpan?>(), Arg.Any<TimeSpan?>(), Arg.Any<Func<double, bool>?>(), cts.Token));
    }
}