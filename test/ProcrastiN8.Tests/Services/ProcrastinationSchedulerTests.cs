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
    public async Task ScheduleWithResult_ReportsExecutionMetrics()
    {
        // arrange
        Func<Task> task = () => Task.CompletedTask;
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
    randomProvider.GetDouble().Returns(0.2);
        delayStrategy.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<TimeSpan?>(), Arg.Any<Func<double, bool>>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // act
        var result = await ProcrastinationScheduler.ScheduleWithResult(
            task,
            TimeSpan.FromMilliseconds(5),
            ProcrastinationMode.MovingTarget,
            excuseProvider: null,
            delayStrategy: delayStrategy,
            randomProvider: randomProvider);

        // assert
        result.Mode.Should().Be(ProcrastinationMode.MovingTarget);
        result.Executed.Should().BeTrue("MovingTarget eventually executes the task");
        result.Cycles.Should().BeGreaterThan(0, "At least one cycle of deferment is expected for optics");
    }

    [Fact]
    public async Task CompositeStrategy_ExecutesFinalPhaseOnly()
    {
        // arrange
        var executed = 0;
        Func<Task> realTask = () => { executed++; return Task.CompletedTask; };
        var delayStrategy = Substitute.For<IDelayStrategy>();
        delayStrategy.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<TimeSpan?>(), Arg.Any<Func<double, bool>>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.05);
        var composite = new CompositeProcrastinationStrategy(new MovingTargetStrategy(), new MovingTargetStrategy());

        // act
        await composite.ExecuteAsync(realTask, TimeSpan.FromMilliseconds(5), null, delayStrategy, randomProvider, SystemTimeProvider.Default, CancellationToken.None);

        // assert
        executed.Should().Be(1, "only the final phase should run the real task");
    }

    [Fact]
    public async Task ConditionalStrategy_SelectsBranch()
    {
        // arrange
        var chosenMoving = false;
        var moving = new MovingTargetStrategy();
        var weekend = new WeekendFallbackStrategy();
        Func<ITimeProvider, bool> predicate = _ => { chosenMoving = true; return true; };
        var conditional = new ConditionalProcrastinationStrategy(moving, weekend, predicate);
        var delayStrategy = Substitute.For<IDelayStrategy>();
        delayStrategy.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<TimeSpan?>(), Arg.Any<Func<double, bool>>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.05);
        var executed = false;
        Func<Task> task = () => { executed = true; return Task.CompletedTask; };

        // act
        await conditional.ExecuteAsync(task, TimeSpan.FromMilliseconds(5), null, delayStrategy, randomProvider, SystemTimeProvider.Default, CancellationToken.None);

        // assert
        chosenMoving.Should().BeTrue();
        executed.Should().BeTrue();
    }

    private class TestFactory : IProcrastinationStrategyFactory
    {
        public IProcrastinationStrategy Create(ProcrastinationMode mode)
        {
            return new MovingTargetStrategy();
        }
    }

    [Fact]
    public async Task Schedule_UsesCustomFactory()
    {
        // arrange
        var factory = new TestFactory();
        var executed = false;
        Func<Task> task = () => { executed = true; return Task.CompletedTask; };

        // act
        await ProcrastinationScheduler.Schedule(
            task,
            TimeSpan.Zero,
            ProcrastinationMode.InfiniteEstimation, // overridden by factory returning MovingTargetStrategy
            factory,
            excuseProvider: null,
            delayStrategy: Substitute.For<IDelayStrategy>(),
            randomProvider: Substitute.For<IRandomProvider>(),
            timeProvider: SystemTimeProvider.Default,
            cancellationToken: CancellationToken.None);

        // assert
        executed.Should().BeTrue("Factory forces a strategy that executes the task");
    }

    [Fact]
    public async Task Handle_TriggerNow_ForcesExecutionOnInfiniteEstimation()
    {
        // arrange
        var executed = false;
        Func<Task> task = () => { executed = true; return Task.CompletedTask; };
        var triggerDelayStrategy = Substitute.For<IDelayStrategy>();
        triggerDelayStrategy.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<TimeSpan?>(), Arg.Any<Func<double, bool>>(), Arg.Any<CancellationToken>())
            .Returns(async _ => await Task.Yield());
        var handle = ProcrastinationScheduler.ScheduleWithHandle(
            task,
            TimeSpan.Zero,
            ProcrastinationMode.InfiniteEstimation,
            delayStrategy: triggerDelayStrategy,
            randomProvider: Substitute.For<IRandomProvider>());

        // act
        handle.TriggerNow();
        var result = await handle.Completion;

        // assert
        executed.Should().BeTrue("TriggerNow should override infinite deferral");
    result.Executed.Should().BeTrue();
    result.Triggered.Should().BeTrue("Result should record that execution was externally triggered");
    result.Abandoned.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Abandon_SkipsExecution()
    {
        // arrange
        var executed = false;
        Func<Task> task = () => { executed = true; return Task.CompletedTask; };
        var handle = ProcrastinationScheduler.ScheduleWithHandle(
            task,
            TimeSpan.Zero,
            ProcrastinationMode.MovingTarget,
            delayStrategy: Substitute.For<IDelayStrategy>(),
            randomProvider: Substitute.For<IRandomProvider>());

        // act
        handle.Abandon();
        var result = await handle.Completion;

        // assert
        executed.Should().BeFalse("Abandon should prevent task execution");
        result.Executed.Should().BeFalse();
        result.Abandoned.Should().BeTrue("Result should record abandonment");
        result.Triggered.Should().BeFalse();
    }

    [Fact]
    public async Task Builder_Schedules_With_CustomFactory_And_Observers()
    {
        // arrange
        var executed = false;
        Func<Task> task = () => { executed = true; return Task.CompletedTask; };
        var observer = Substitute.For<IProcrastinationObserver>();
        var factory = Substitute.For<IProcrastinationStrategyFactory>();
        factory.Create(Arg.Any<ProcrastinationMode>()).Returns(new MovingTargetStrategy());

        var scheduler = ProcrastinationSchedulerBuilder
            .Create()
            .WithFactory(factory)
            .AddObserver(observer)
            .Build();

        // act
        var result = await scheduler.ScheduleWithResult(task, TimeSpan.FromMilliseconds(5), ProcrastinationMode.MovingTarget);

        // assert
        executed.Should().BeTrue();
        result.Executed.Should().BeTrue();
        await observer.ReceivedWithAnyArgs().OnExecutedAsync(result, Arg.Any<CancellationToken>());
    factory.Received(1).Create(ProcrastinationMode.MovingTarget);
    }

    [Fact]
    public async Task Schedule_InfiniteEstimation_CancelsTask()
    {
        // Arrange
        var taskCompleted = false;
        Func<Task> task = () => { taskCompleted = true; return Task.CompletedTask; };
        var excuseProvider = new MockExcuseProvider();
        var delayStrategy = Substitute.For<IDelayStrategy>();
        delayStrategy.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<TimeSpan?>(), Arg.Any<Func<double, bool>>(), Arg.Any<CancellationToken>())
            .Returns(async _ => await Task.Yield());
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
        delayStrategy.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<TimeSpan?>(), Arg.Any<Func<double, bool>>(), Arg.Any<CancellationToken>())
            .Returns(async _ => await Task.Yield());
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
    public async Task WeekendFallbackStrategy_CompletesAfter72HoursIfNoWeekendWindow()
    {
        // arrange
        var taskCompleted = false;
        Func<Task> task = () => { taskCompleted = true; return Task.CompletedTask; };
        var excuseProvider = new MockExcuseProvider();
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        var mockTimeProvider = Substitute.For<ITimeProvider>();

        // Simulate monotonically advancing time in 24h steps, never hitting Saturday 15:00
        var baseTime = new DateTime(2025, 8, 4, 9, 0, 0, DateTimeKind.Utc); // Monday
        var call = 0;
        mockTimeProvider.GetUtcNow().Returns(ci => baseTime.AddHours(call++ * 24));

        // Each delay of one hour advances our simulated clock by 24h above via GetUtcNow sequence.
        // This relies on delayStrategy.DelayAsync being awaited repeatedly.
        delayStrategy
            .DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<TimeSpan?>(), Arg.Any<Func<double, bool>>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.CompletedTask);

        var strategy = new WeekendFallbackStrategy();

        // act
        await strategy.ExecuteAsync(task, TimeSpan.Zero, excuseProvider, delayStrategy, randomProvider, mockTimeProvider, CancellationToken.None);

        // assert
        taskCompleted.Should().BeTrue("After 72 hours of theatrical deferral the task must finally capitulate.");
    }

    private class MockExcuseProvider : IExcuseProvider
    {
        public Task<string> GetExcuseAsync()
        {
            return Task.FromResult("Mock excuse");
        }
    }
}
