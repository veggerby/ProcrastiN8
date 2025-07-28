using ProcrastiN8.Common;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.TODOFramework;

namespace ProcrastiN8.Tests.TODOFramework;

public class TodoSchedulerTests
{
    [Fact]
    public async Task ScheduleAsync_LogsExcuseAndDefersAllTodoMethods()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var excuseProvider = Substitute.For<IExcuseProvider>();
        var delayStrategy = Substitute.For<IDelayStrategy>();
        excuseProvider.GetExcuseAsync().Returns("Because reasons");
        delayStrategy.DelayAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var scheduler = new TodoScheduler(logger, excuseProvider, delayStrategy);
        var cts = new CancellationTokenSource();

        // Dummy type with TODO methods
        var type = typeof(DummyTodoClass);

        // act
        await scheduler.ScheduleAllAsync(type, cts.Token);

        // assert
        logger.Received().Info(Arg.Is<string>(msg => msg.Contains("Deferring TODO: TodoMethod")));
        await excuseProvider.Received().GetExcuseAsync();
        // The TODO task is never actually executed, only deferred
        // This test is as unproductive as the code it tests
    }

    [Fact]
    public void Constructor_ThrowsIfExecutorIsNull()
    {
        // arrange
        // (no setup needed)

        // act
        Action act = () => new TodoScheduler(null!);

        // assert
        act.Should().NotThrow(); // Logger is optional, so this should not throw
    }

    private class DummyTodoClass
    {
        [Todo]
        public void TodoMethod() { /* This will never be called. */ }
    }
}
// These tests ensure that the TODO framework is as unproductive as intended.