using ProcrastiN8.TODOFramework;

namespace ProcrastiN8.Tests.TODOFramework;

public class NeverDoExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsTaskThatNeverCompletes()
    {
        // arrange
        var executor = new NeverDoExecutor();
        var action = Substitute.For<Func<Task>>();
        var cts = new CancellationTokenSource();

        // act
        var task = executor.NeverAsync(action, cancellationToken: cts.Token);

        // assert
        // The NeverAsync implementation currently completes after an optional delay, but the test expects a task that never completes.
        // To pass this test, NeverAsync must return a Task that never completes (e.g., by returning TaskCompletionSource.Task that is never set).
        // For now, we check that the task does not complete within a reasonable time window.
        var completed = await Task.WhenAny(task, Task.Delay(100));
        completed.Should().NotBe(task, "the whole point is to never actually do the work");
        // Just to be sure, try to wait a moment
        await Task.Delay(50, CancellationToken.None);
        task.IsCompleted.Should().BeFalse("even after a brief moment of hope, nothing should be accomplished");
    }

    [Fact]
    public void ExecuteAsync_ThrowsIfActionIsNull()
    {
        // arrange
        var executor = new NeverDoExecutor();
        var cts = new CancellationTokenSource();

        // act
        var act = () => executor.NeverAsync(null!, cancellationToken: cts.Token);

        // assert
        act.Should().ThrowAsync<ArgumentNullException>("even procrastinators need something to avoid");
    }
}
// These tests guarantee that NeverDoExecutor is the gold standard for inaction.