using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class DefaultDelayStrategyTests
{
    [Fact]
    public async Task DelayAsync_Uses_Custom_RandomFunc()
    {
        var called = false;
        var strategy = new DefaultDelayStrategy(
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(100),
            _ => { called = true; return 0; });
        await strategy.DelayAsync();
        called.Should().BeTrue("even delays can be overengineered");
    }

    [Fact]
    public async Task DelayAsync_Respects_Cancellation()
    {
        var strategy = new DefaultDelayStrategy(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
        var cts = new CancellationTokenSource();
        cts.Cancel();
        await Assert.ThrowsAsync<TaskCanceledException>(() => strategy.DelayAsync(cts.Token));
    }
}
