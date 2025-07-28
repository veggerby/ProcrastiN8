using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class DefaultDelayStrategy_AbstractionTests
{
    [Fact]
    public async Task DelayAsync_UsesInjectedDelayProvider()
    {
        var delayProvider = Substitute.For<IDelayProvider>();
        var strategy = new DefaultDelayStrategy(
            minDelay: TimeSpan.FromMilliseconds(100),
            maxDelay: TimeSpan.FromMilliseconds(100),
            delayProvider: delayProvider
        );

        await strategy.DelayAsync();

        await delayProvider.Received(1)
            .DelayAsync(TimeSpan.FromMilliseconds(100), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DelayAsync_UsesRandomizedDelayFromProvider()
    {
        var delayProvider = Substitute.For<IDelayProvider>();
        var strategy = new DefaultDelayStrategy(
            minDelay: TimeSpan.FromMilliseconds(50),
            maxDelay: TimeSpan.FromMilliseconds(150),
            randomFunc: _ => 42, // Always returns 42
            delayProvider: delayProvider
        );

        await strategy.DelayAsync();

        // 50 + 42 = 92
        await delayProvider.Received(1)
            .DelayAsync(TimeSpan.FromMilliseconds(92), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DelayAsync_ActuallyDelays_WhenUsingTaskDelayProvider()
    {
        var strategy = new DefaultDelayStrategy(
            minDelay: TimeSpan.FromMilliseconds(1),
            maxDelay: TimeSpan.FromMilliseconds(1),
            delayProvider: new TaskDelayProvider()
        );
        var before = DateTime.UtcNow;
        await strategy.DelayAsync();
        var after = DateTime.UtcNow;
        (after - before).TotalMilliseconds.Should().BeGreaterThanOrEqualTo(1, "even the smallest delay is a victory for procrastination");
    }
}
