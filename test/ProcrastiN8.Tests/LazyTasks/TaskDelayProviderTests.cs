using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class TaskDelayProviderTests
{
    [Fact]
    public async Task DelayAsync_WaitsForSpecifiedDuration()
    {
        // arrange
        var mockTimeProvider = Substitute.For<ITimeProvider>();
        var provider = new TaskDelayProvider(mockTimeProvider);
        var delay = TimeSpan.FromMilliseconds(10);

        // act
        await provider.DelayAsync(delay);

        // assert
        await mockTimeProvider.Received(1).DelayAsync(delay, Arg.Any<CancellationToken>());
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