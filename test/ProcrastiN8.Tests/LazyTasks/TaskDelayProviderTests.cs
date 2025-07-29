using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class TaskDelayProviderTests
{
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