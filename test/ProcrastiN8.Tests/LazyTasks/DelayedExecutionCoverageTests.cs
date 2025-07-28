using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class DelayedExecutionCoverageTests
{
    [Fact]
    public async Task RunAfterThinkingAboutIt_LogsSnoozeAndExecutesAction()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        bool called = false;
        var snooze = TimeSpan.FromMilliseconds(700);

        // act
        await DelayedExecution.RunAfterThinkingAboutIt(
            TimeSpan.FromMilliseconds(800),
            () => called = true,
            snooze,
            logger,
            CancellationToken.None);

        // assert
        called.Should().BeTrue("the action should be executed after the delay");
        logger.Received().Info(Arg.Any<string>(), Arg.Any<object[]>());
    }

    [Fact]
    public async Task RunAfterThinkingAboutIt_LogsAndHandlesException()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var ex = new InvalidOperationException("Oops");

        // act
        Func<Task> act = () => DelayedExecution.RunAfterThinkingAboutIt(
            TimeSpan.FromMilliseconds(600),
            () => throw ex,
            null,
            logger,
            CancellationToken.None);

        // assert
        (await act.Should().ThrowAsync<InvalidOperationException>()).WithMessage("Oops");
        logger.Received().Error(ex, Arg.Is<string>(m => m.Contains("Task failed")), Arg.Any<object[]>());
    }

    [Fact]
    public async Task RunWhenYouFeelLikeIt_ExecutesAsyncAction()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        bool called = false;

        // act
        await DelayedExecution.RunWhenYouFeelLikeIt(
            TimeSpan.FromMilliseconds(600),
            async () => { called = true; await Task.Yield(); },
            null,
            logger,
            CancellationToken.None);

        // assert
        called.Should().BeTrue("the async action should be executed after the delay");
        logger.Received().Info(Arg.Any<string>(), Arg.Any<object[]>());
    }

    [Fact]
    public async Task RunWhenYouFeelLikeIt_LogsAndHandlesAsyncException()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var ex = new InvalidOperationException("Async fail");

        // act
        Func<Task> act = () => DelayedExecution.RunWhenYouFeelLikeIt(
            TimeSpan.FromMilliseconds(600),
            async () => { await Task.Yield(); throw ex; },
            null,
            logger,
            CancellationToken.None);

        // assert
        (await act.Should().ThrowAsync<InvalidOperationException>()).WithMessage("Async fail");
        logger.Received().Error(ex, Arg.Is<string>(m => m.Contains("Async task derailed")), Arg.Any<object[]>());
    }
    [Fact]
    public async Task RunAfterThinkingAboutIt_ThrowsIfDelayTooShort()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();

        // act
        Func<Task> act = () => DelayedExecution.RunAfterThinkingAboutIt(
            TimeSpan.FromMilliseconds(100),
            () => { },
            null,
            logger,
            CancellationToken.None);

        // assert
        (await act.Should().ThrowAsync<ArgumentOutOfExcusesException>())
            .WithMessage("Whoa, not so fast. This is ProcrastiN8, not ExecuteNow.");
    }

    [Fact]
    public async Task RunWhenYouFeelLikeIt_ThrowsIfDelayTooShort()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();

        // act
        Func<Task> act = () => DelayedExecution.RunWhenYouFeelLikeIt(
            TimeSpan.FromMilliseconds(100),
            async () => await Task.CompletedTask,
            null,
            logger,
            CancellationToken.None);

        // assert
        (await act.Should().ThrowAsync<ArgumentOutOfExcusesException>())
            .WithMessage("This task is trying way too hard to be on time.");
    }

    [Fact]
    public async Task RunAfterThinkingAboutIt_RespectsCancellationToken()
    {
        // arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // act
        Func<Task> act = () => DelayedExecution.RunAfterThinkingAboutIt(
            TimeSpan.FromMilliseconds(600),
            () => { },
            null,
            null,
            cts.Token);

        // assert
        await act.Should().ThrowAsync<TaskCanceledException>("cancellation should be respected");
    }

    [Fact]
    public async Task RunWhenYouFeelLikeIt_RespectsCancellationToken()
    {
        // arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // act
        Func<Task> act = () => DelayedExecution.RunWhenYouFeelLikeIt(
            TimeSpan.FromMilliseconds(600),
            async () => await Task.CompletedTask,
            null,
            null,
            cts.Token);

        // assert
        await act.Should().ThrowAsync<TaskCanceledException>("cancellation should be respected");
    }
}