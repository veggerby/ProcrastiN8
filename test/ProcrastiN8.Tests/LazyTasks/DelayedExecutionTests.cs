using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class DelayedExecutionTests
{
    [Fact]
    public async Task RunAfterThinkingAboutIt_ExecutesAction()
    {
        // Arrange
        bool called = false;

        // Act
        await DelayedExecution.RunAfterThinkingAboutIt(TimeSpan.FromMilliseconds(600), () => called = true);

        // Assert
        called.Should().BeTrue();
    }

    [Fact]
    public async Task RunAfterThinkingAboutIt_ThrowsIfDelayTooShort()
    {
        // Arrange
        // (no setup needed)

        // Act
        Func<Task> act = () => DelayedExecution.RunAfterThinkingAboutIt(TimeSpan.FromMilliseconds(100), () => { });

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfExcusesException>();
    }

    [Fact]
    public async Task RunWhenYouFeelLikeIt_ThrowsIfDelayTooShort()
    {
        // Arrange
        // (no setup needed)

        // Act
        Func<Task> act = () => DelayedExecution.RunWhenYouFeelLikeIt(TimeSpan.FromMilliseconds(100), async () => await Task.CompletedTask);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfExcusesException>();
    }
}