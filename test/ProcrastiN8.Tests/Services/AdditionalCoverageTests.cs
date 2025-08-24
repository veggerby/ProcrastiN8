using System.Diagnostics.Metrics;
using ProcrastiN8.Services;
using ProcrastiN8.Services.Diagnostics;
using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.Services;

/// <summary>
/// Supplemental coverage for composite/conditional strategies, metrics counters, and builder conveniences.
/// Treats frivolous behaviors with grave seriousness.
/// </summary>
public class AdditionalCoverageTests
{
    private sealed class NoOpDelayStrategy : IDelayStrategy
    {
        public Task DelayAsync(TimeSpan? minDelay = null, TimeSpan? maxDelay = null, Func<double, bool>? beforeCallback = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class FakeCycleStrategy(int cycles, bool markExecuted) : ProcrastinationStrategyBase
    {
        protected override async Task ExecuteCoreAsync(
            Func<Task> task,
            TimeSpan initialDelay,
            IExcuseProvider? excuseProvider,
            IDelayStrategy delayStrategy,
            IRandomProvider randomProvider,
            ITimeProvider timeProvider,
            CancellationToken cancellationToken)
        {
            for (int i = 0; i < cycles; i++)
            {
                IncrementCycle();
                await NotifyCycleAsync(ControlContext, cancellationToken);
            }
            if (markExecuted)
            {
                await task();
                MarkExecuted();
            }
        }
    }

    private sealed class CounterCapture : IDisposable
    {
        private readonly MeterListener _listener;
        public readonly Dictionary<string, int> Counts = new();

        public CounterCapture()
        {
            _listener = new MeterListener();
            _listener.InstrumentPublished = (instrument, listener) =>
            {
                if (instrument.Meter.Name == ProcrastinationDiagnostics.MeterName && instrument is Counter<int>)
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            };
            _listener.SetMeasurementEventCallback<int>((instrument, measurement, tags, state) =>
            {
                lock (Counts)
                {
                    Counts[instrument.Name] = Counts.TryGetValue(instrument.Name, out var existing)
                        ? existing + measurement
                        : measurement;
                }
            });
            _listener.Start();
        }

        public void Dispose() => _listener.Dispose();
    }

    [Fact]
    public async Task CompositeStrategy_Aggregates_Cycles()
    {
        // arrange
        var phase1 = new FakeCycleStrategy(3, false);
        var phase2 = new FakeCycleStrategy(5, true); // only final executes underlying task
        var composite = new CompositeProcrastinationStrategy(phase1, phase2);
        var executed = false;
        Func<Task> task = () => { executed = true; return Task.CompletedTask; };
        var delay = new NoOpDelayStrategy();
        var random = Substitute.For<IRandomProvider>();
        random.GetDouble().Returns(0.01);

        // act
        await composite.ExecuteAsync(task, TimeSpan.Zero, null, delay, random, SystemTimeProvider.Default, CancellationToken.None);
        var result = composite.LastResult;

        // assert
        executed.Should().BeTrue("final phase must execute original task");
        result.Cycles.Should().Be(8, "composite should sum cycles from all phases");
        result.Executed.Should().BeTrue();
    }

    [Fact]
    public async Task ConditionalStrategy_FalseBranch_Selected()
    {
        // arrange
        var weekend = new WeekendFallbackStrategy();
        var moving = new MovingTargetStrategy();
        Func<ITimeProvider, bool> predicate = _ => false; // force false branch
        var conditional = new ConditionalProcrastinationStrategy(moving, weekend, predicate);
        var delay = new NoOpDelayStrategy();
        var random = Substitute.For<IRandomProvider>();
        random.GetDouble().Returns(0.02);
        var mockTime = Substitute.For<ITimeProvider>();
        mockTime.GetUtcNow().Returns(new DateTime(2025, 8, 2, 16, 0, 0, DateTimeKind.Utc)); // Saturday after 15:00
        var executed = false;
        Func<Task> task = () => { executed = true; return Task.CompletedTask; };

        // act
        await conditional.ExecuteAsync(task, TimeSpan.Zero, null, delay, random, mockTime, CancellationToken.None);
        var res = conditional.LastResult;

        // assert
        executed.Should().BeTrue();
        res.Executed.Should().BeTrue();
    }

    [Fact]
    public async Task MetricsCounters_MovingTarget_Record_Lifecycle()
    {
        // arrange
        using var capture = new CounterCapture();
        Func<Task> task = () => Task.CompletedTask;
        var delay = new NoOpDelayStrategy();
        var random = Substitute.For<IRandomProvider>();
        random.GetDouble().Returns(0.05);

        // act
        var result = await ProcrastinationScheduler.ScheduleWithResult(task, TimeSpan.FromMilliseconds(5), ProcrastinationMode.MovingTarget, delayStrategy: delay, randomProvider: random);

        // assert
        result.Executed.Should().BeTrue();
    capture.Counts.Should().ContainKey("procrastination.cycles");
    // Excuses may not increment if no excuse provider was supplied; cycles & executions are mandatory.
    capture.Counts.Should().ContainKey("procrastination.executions");
        capture.Counts["procrastination.executions"].Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task MetricsCounters_Triggered_RecordsTriggeredEvent()
    {
        // arrange
        using var capture = new CounterCapture();
        var handle = ProcrastinationScheduler.ScheduleWithHandle(() => Task.CompletedTask, TimeSpan.Zero, ProcrastinationMode.InfiniteEstimation, delayStrategy: new NoOpDelayStrategy(), randomProvider: Substitute.For<IRandomProvider>());

        // act
        handle.TriggerNow();
        var result = await handle.Completion;

        // assert
        result.Triggered.Should().BeTrue();
        capture.Counts.Should().ContainKey("procrastination.triggered");
        capture.Counts["procrastination.triggered"].Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task MetricsCounters_Abandoned_RecordsAbandonedEvent()
    {
        // arrange
        using var capture = new CounterCapture();
        var handle = ProcrastinationScheduler.ScheduleWithHandle(() => Task.CompletedTask, TimeSpan.Zero, ProcrastinationMode.MovingTarget, delayStrategy: new NoOpDelayStrategy(), randomProvider: Substitute.For<IRandomProvider>());

        // act
        handle.Abandon();
        var result = await handle.Completion;

        // assert
        result.Abandoned.Should().BeTrue();
        capture.Counts.Should().ContainKey("procrastination.abandoned");
        capture.Counts["procrastination.abandoned"].Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task Builder_WithMetrics_AttachesObserver_And_RecordsEvents()
    {
        // arrange
        using var capture = new CounterCapture();
        var scheduler = ProcrastinationSchedulerBuilder
            .Create()
            .WithMetrics()
            .Build();

        // act
        var result = await scheduler.ScheduleWithResult(() => Task.CompletedTask, TimeSpan.FromMilliseconds(5), ProcrastinationMode.MovingTarget);

        // assert
        result.Executed.Should().BeTrue();
        capture.Counts.Should().ContainKey("procrastination.executions");
        capture.Counts["procrastination.executions"].Should().BeGreaterThanOrEqualTo(1);
    }
}
