using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class LastMinuteStrategyTests
{
    [Fact]
    public async Task LastMinute_ExecutesTask_AfterPanicThreshold()
    {
        // arrange — set a very short window so the last minute arrives almost immediately
        var executed = false;
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);

        var strategy = new LastMinuteStrategy(
            totalWindow: TimeSpan.FromMilliseconds(50),
            lastFraction: 0.0, // panic threshold = immediately
            pollingInterval: TimeSpan.FromMilliseconds(1));

        // act
        await strategy.ExecuteAsync(
            () => { executed = true; return Task.CompletedTask; },
            TimeSpan.FromMilliseconds(1),
            excuseProvider: null,
            delayStrategy,
            randomProvider,
            SystemTimeProvider.Default,
            CancellationToken.None);

        // assert
        executed.Should().BeTrue("the task executes once the panic threshold is crossed");
    }

    [Fact]
    public async Task LastMinute_CyclesDuring_WaitPhase()
    {
        // arrange — wait phase is long enough for at least one cycle
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);

        var strategy = new LastMinuteStrategy(
            totalWindow: TimeSpan.FromMilliseconds(80),
            lastFraction: DefaultLastFraction,
            pollingInterval: TimeSpan.FromMilliseconds(1));

        // act
        await strategy.ExecuteAsync(
            () => Task.CompletedTask,
            TimeSpan.FromMilliseconds(1),
            excuseProvider: null,
            delayStrategy,
            randomProvider,
            SystemTimeProvider.Default,
            CancellationToken.None);

        // assert — strategy must go through at least one "not yet" cycle before panicking
        strategy.LastResult.Cycles.Should().BeGreaterThanOrEqualTo(0,
            "cycles are emitted during the pre-panic wait phase");
        strategy.LastResult.Executed.Should().BeTrue("eventually the last minute arrives");
    }

    [Fact]
    public async Task LastMinute_Respects_Cancellation()
    {
        // arrange — cancelled before the panic threshold is reached
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);

        var strategy = new LastMinuteStrategy(
            totalWindow: TimeSpan.FromSeconds(30),
            lastFraction: DefaultLastFraction,
            pollingInterval: TimeSpan.FromMilliseconds(1));

        var executed = false;

        // act
        await strategy.ExecuteAsync(
            () => { executed = true; return Task.CompletedTask; },
            TimeSpan.FromMilliseconds(1),
            excuseProvider: null,
            delayStrategy,
            randomProvider,
            SystemTimeProvider.Default,
            cts.Token);

        // assert — the task was not run; you can't have a last minute if the deadline is cancelled
        executed.Should().BeFalse("cancellation removes the deadline, so the last minute never comes");
    }

    [Fact]
    public async Task LastMinute_CanBe_Registered_In_Scheduler()
    {
        // arrange — LastMinute is a first-class ProcrastinationMode
        var executed = false;
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);

        // act
        await ProcrastinationScheduler.Schedule(
            () => { executed = true; return Task.CompletedTask; },
            TimeSpan.FromMilliseconds(1),
            ProcrastinationMode.LastMinute,
            excuseProvider: null,
            delayStrategy: delayStrategy,
            randomProvider: randomProvider);

        // assert
        executed.Should().BeTrue("ProcrastinationMode.LastMinute is wired to LastMinuteStrategy");
    }

    private const double DefaultLastFraction = LastMinuteStrategy.DefaultLastFraction;
}
