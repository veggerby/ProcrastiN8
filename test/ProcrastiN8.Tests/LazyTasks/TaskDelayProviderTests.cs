using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class TaskDelayProviderTests
{
    [Fact]
    public async Task DelayAsync_WaitsForSpecifiedDuration()
    {
        var provider = new TaskDelayProvider();
        var before = DateTime.UtcNow;
        await provider.DelayAsync(TimeSpan.FromMilliseconds(10));
        var after = DateTime.UtcNow;
        (after - before).TotalMilliseconds.Should().BeGreaterThanOrEqualTo(10, "even a trivial delay is a triumph of stalling");
    }

    [Fact]
    public async Task DelayAsync_RespectsCancellationToken()
    {
        var provider = new TaskDelayProvider();
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await provider.DelayAsync(TimeSpan.FromSeconds(1), cts.Token));
    }
}
