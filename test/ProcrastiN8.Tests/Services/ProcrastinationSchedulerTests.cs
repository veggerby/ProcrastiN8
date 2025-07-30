using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class ProcrastinationSchedulerTests
{
    [Fact]
    public async Task Schedule_MovingTarget_CompletesTask()
    {
        // Arrange
        var taskCompleted = false;
        Func<Task> task = () => { taskCompleted = true; return Task.CompletedTask; };
        var excuseProvider = new MockExcuseProvider();
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();

        // Simulate a small random delay increase to ensure the task completes quickly
        randomProvider.GetDouble().Returns(0.1);

        // Act
        await ProcrastinationScheduler.Schedule(
            task,
            TimeSpan.FromMilliseconds(10), // Reduced initial delay
            ProcrastinationMode.MovingTarget,
            excuseProvider,
            delayStrategy,
            randomProvider);

        // Assert
        taskCompleted.Should().BeTrue("Task should complete in MovingTarget mode.");
    }

    [Fact]
    public async Task Schedule_InfiniteEstimation_CancelsTask()
    {
        // Arrange
        var taskCompleted = false;
        Func<Task> task = () => { taskCompleted = true; return Task.CompletedTask; };
        var excuseProvider = new MockExcuseProvider();
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500)); // Reduced timeout for faster test

        // Act
        await ProcrastinationScheduler.Schedule(
            task,
            TimeSpan.Zero,
            ProcrastinationMode.InfiniteEstimation,
            excuseProvider,
            delayStrategy,
            randomProvider,
            cancellationToken: cts.Token);

        // Assert
        taskCompleted.Should().BeFalse("Task should not complete in InfiniteEstimation mode.");
    }

    [Fact]
    public async Task Schedule_WeekendFallback_CompletesTaskOnSaturday()
    {
        // Arrange
        var taskCompleted = false;
        Func<Task> task = () => { taskCompleted = true; return Task.CompletedTask; };
        var excuseProvider = new MockExcuseProvider();
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();

        // Mock ITimeProvider to simulate a Saturday after 3 PM
        var mockTimeProvider = Substitute.For<ITimeProvider>();
        mockTimeProvider.GetUtcNow().Returns(new DateTime(2025, 8, 2, 15, 0, 0, DateTimeKind.Utc));

        // Act
        await ProcrastinationScheduler.Schedule(
            task,
            TimeSpan.Zero,
            ProcrastinationMode.WeekendFallback,
            excuseProvider,
            delayStrategy,
            randomProvider,
            mockTimeProvider);

        // Assert
        taskCompleted.Should().BeTrue("Task should complete in WeekendFallback mode.");
    }

    [Fact]
    public async Task MovingTargetStrategy_CompletesTask()
    {
        // Arrange
        var taskCompleted = false;
        Func<Task> task = () => { taskCompleted = true; return Task.CompletedTask; };
        var excuseProvider = new MockExcuseProvider();
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1);
        var strategy = new MovingTargetStrategy();

        // Act
        await strategy.ExecuteAsync(task, TimeSpan.FromMilliseconds(10), excuseProvider, delayStrategy, randomProvider, SystemTimeProvider.Default, CancellationToken.None);

        // Assert
        taskCompleted.Should().BeTrue("Task should complete in MovingTarget mode.");
    }

    [Fact]
    public async Task InfiniteEstimationStrategy_CancelsTask()
    {
        // Arrange
        var taskCompleted = false;
        Func<Task> task = () => { taskCompleted = true; return Task.CompletedTask; };
        var excuseProvider = new MockExcuseProvider();
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        var strategy = new InfiniteEstimationStrategy();
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));

        // Act
        await strategy.ExecuteAsync(task, TimeSpan.Zero, excuseProvider, delayStrategy, randomProvider, SystemTimeProvider.Default, cts.Token);

        // Assert
        taskCompleted.Should().BeFalse("Task should not complete in InfiniteEstimation mode.");
    }

    [Fact]
    public async Task WeekendFallbackStrategy_CompletesTaskOnSaturday()
    {
        // Arrange
        var taskCompleted = false;
        Func<Task> task = () => { taskCompleted = true; return Task.CompletedTask; };
        var excuseProvider = new MockExcuseProvider();
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        var mockTimeProvider = Substitute.For<ITimeProvider>();
        mockTimeProvider.GetUtcNow().Returns(new DateTime(2025, 8, 2, 15, 0, 0, DateTimeKind.Utc));
        var strategy = new WeekendFallbackStrategy();

        // Act
        await strategy.ExecuteAsync(task, TimeSpan.Zero, excuseProvider, delayStrategy, randomProvider, mockTimeProvider, CancellationToken.None);

        // Assert
        taskCompleted.Should().BeTrue("Task should complete in WeekendFallback mode.");
    }

    [Fact]
    public async Task ProcrastinationScheduler_DelegatesToCorrectStrategy()
    {
        // Arrange
        var taskCompleted = false;
        Func<Task> task = () => { taskCompleted = true; return Task.CompletedTask; };
        var excuseProvider = new MockExcuseProvider();
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        var mockTimeProvider = Substitute.For<ITimeProvider>();
        mockTimeProvider.GetUtcNow().Returns(new DateTime(2025, 8, 2, 15, 0, 0, DateTimeKind.Utc));

        // Act & Assert
        await ProcrastinationScheduler.Schedule(task, TimeSpan.Zero, ProcrastinationMode.MovingTarget, excuseProvider, delayStrategy, randomProvider, mockTimeProvider);
        taskCompleted.Should().BeTrue("ProcrastinationScheduler should delegate to MovingTargetStrategy.");

        taskCompleted = false;
        await ProcrastinationScheduler.Schedule(task, TimeSpan.Zero, ProcrastinationMode.InfiniteEstimation, excuseProvider, delayStrategy, randomProvider, mockTimeProvider);
        taskCompleted.Should().BeFalse("ProcrastinationScheduler should delegate to InfiniteEstimationStrategy.");

        taskCompleted = false;
        await ProcrastinationScheduler.Schedule(task, TimeSpan.Zero, ProcrastinationMode.WeekendFallback, excuseProvider, delayStrategy, randomProvider, mockTimeProvider);
        taskCompleted.Should().BeTrue("ProcrastinationScheduler should delegate to WeekendFallbackStrategy.");
    }

    private class MockExcuseProvider : IExcuseProvider
    {
        public Task<string> GetExcuseAsync()
        {
            return Task.FromResult("Mock excuse");
        }
    }
}
