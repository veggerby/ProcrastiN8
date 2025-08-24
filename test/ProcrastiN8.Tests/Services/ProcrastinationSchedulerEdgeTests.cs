using System;

using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class ProcrastinationSchedulerEdgeTests
{
    private sealed class NoOpDelayStrategy : IDelayStrategy
    {
        public Task DelayAsync(TimeSpan? minDelay = null, TimeSpan? maxDelay = null, Func<double, bool>? beforeCallback = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class SafetyAbortStrategy : ProcrastinationStrategyBase
    {
        protected override async Task ExecuteCoreAsync(Func<Task> task, TimeSpan initialDelay, IExcuseProvider? excuseProvider, IDelayStrategy delayStrategy, IRandomProvider randomProvider, ITimeProvider timeProvider, CancellationToken cancellationToken)
        {
            // Spin until safety cap forces termination; do NOT execute task.
            while (!cancellationToken.IsCancellationRequested && !SafetyCapReached())
            {
                IncrementCycle();
                await NotifyCycleAsync(ControlContext, cancellationToken);
            }
            // exit without MarkExecuted; theatrical non-performance.
        }
    }

    private sealed class SafetyOverrideFactory : IProcrastinationStrategyFactory
    {
        private readonly int _maxCycles;
        public SafetyOverrideFactory(int maxCycles) { _maxCycles = maxCycles; }
        public IProcrastinationStrategy Create(ProcrastinationMode mode)
        {
            var s = new SafetyAbortStrategy();
            s.ConfigureSafety(new LocalSafety(_maxCycles));
            return s;
        }
        private sealed class LocalSafety(int maxCycles) : IExecutionSafetyOptions { public int MaxCycles { get; } = maxCycles; }
    }

    [Fact]
    public async Task Abandon_InfiniteEstimation_Sets_Abandoned_Flag()
    {
        // arrange
        var handle = ProcrastinationScheduler.ScheduleWithHandle(
            () => Task.CompletedTask,
            TimeSpan.Zero,
            ProcrastinationMode.InfiniteEstimation,
            delayStrategy: new NoOpDelayStrategy(),
            randomProvider: RandomProvider.Default);

        // act
        handle.Abandon();
        var result = await handle.Completion;

        // assert
        result.Executed.Should().BeFalse("abandon skips execution");
        result.Abandoned.Should().BeTrue();
        result.Triggered.Should().BeFalse();
    }

    [Fact]
    public async Task TriggerNow_InfiniteEstimation_Sets_Triggered_Flag()
    {
        // arrange
        var executed = false;
        Func<Task> task = () => { executed = true; return Task.CompletedTask; };
        var handle = ProcrastinationScheduler.ScheduleWithHandle(
            task,
            TimeSpan.Zero,
            ProcrastinationMode.InfiniteEstimation,
            delayStrategy: new NoOpDelayStrategy(),
            randomProvider: RandomProvider.Default);

        // act
        handle.TriggerNow();
        var result = await handle.Completion;

        // assert
        executed.Should().BeTrue();
        result.Executed.Should().BeTrue();
        result.Triggered.Should().BeTrue();
        result.Abandoned.Should().BeFalse();
    }

    [Fact]
    public async Task PerStrategy_Safety_Override_Stops_At_MaxCycles()
    {
        // arrange
        var factory = new SafetyOverrideFactory(maxCycles: 5);
        var scheduler = ProcrastinationSchedulerBuilder.Create()
            .WithFactory(factory)
            .WithDelayStrategy(new NoOpDelayStrategy())
            .Build();

        // act
        var result = await scheduler.ScheduleWithResult(() => Task.CompletedTask, TimeSpan.Zero, ProcrastinationMode.MovingTarget);

        // assert
        result.Executed.Should().BeFalse("strategy intentionally never executes underlying task");
        result.Cycles.Should().Be(5, "safety override should terminate at configured MaxCycles");
        result.ProductivityIndex.Should().Be(0.0);
        result.CorrelationId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Underlying_Task_Exception_Propagates_With_Provisional_Result_Available()
    {
        // arrange
        ProcrastinationResult? before = null;
        var captureMw = new TestCaptureMiddleware(r => before = r);
        var scheduler = ProcrastinationSchedulerBuilder.Create()
            .AddMiddleware(captureMw)
            .Build();

        // act
        var ex = await Record.ExceptionAsync(async () => await scheduler.ScheduleWithResult(() => throw new InvalidOperationException("boom"), TimeSpan.Zero, ProcrastinationMode.MovingTarget));

        // assert
        ex.Should().BeOfType<InvalidOperationException>();
        before.Should().NotBeNull("middleware receives provisional result even when task faults");
        before!.Executed.Should().BeFalse();
    }

    private sealed class TestCaptureMiddleware : IProcrastinationMiddleware
    {
        private readonly Action<ProcrastinationResult?> _capture;
        public TestCaptureMiddleware(Action<ProcrastinationResult?> capture) { _capture = capture; }
        public async Task InvokeAsync(ProcrastinationExecutionContext context, Func<Task> next, CancellationToken cancellationToken)
        {
            _capture(context.Result);
            await next();
        }
    }
}