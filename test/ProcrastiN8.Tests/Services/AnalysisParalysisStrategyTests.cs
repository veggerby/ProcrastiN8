using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class AnalysisParalysisStrategyTests
{
    [Fact]
    public async Task AnalysisParalysis_ExecutesTask_AfterDeadline()
    {
        // arrange — analysis never concludes on its own; we wait for the absolute deadline
        var executed = false;
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);

        var strategy = new AnalysisParalysisStrategy(
            analysisDuration: TimeSpan.FromMilliseconds(1),
            absoluteDeadlineOffset: TimeSpan.FromMilliseconds(50));

        // act
        await strategy.ExecuteAsync(
            () => { executed = true; return Task.CompletedTask; },
            TimeSpan.FromMilliseconds(1),
            excuseProvider: null,
            delayStrategy,
            randomProvider,
            SystemTimeProvider.Default,
            CancellationToken.None);

        // assert — the analysis was declared sufficient by executive mandate
        executed.Should().BeTrue("the absolute deadline forces the task to run regardless of unanswered questions");
    }

    [Fact]
    public async Task AnalysisParalysis_Cycles_Represent_Analysis_Rounds()
    {
        // arrange
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);

        var strategy = new AnalysisParalysisStrategy(
            analysisDuration: TimeSpan.FromMilliseconds(1),
            absoluteDeadlineOffset: TimeSpan.FromMilliseconds(60));

        // act
        await strategy.ExecuteAsync(
            () => Task.CompletedTask,
            TimeSpan.FromMilliseconds(1),
            excuseProvider: null,
            delayStrategy,
            randomProvider,
            SystemTimeProvider.Default,
            CancellationToken.None);

        // assert — multiple analysis cycles occurred before execution was permitted
        strategy.LastResult.Cycles.Should().BeGreaterThan(0, "at least one analysis cycle must precede execution");
        strategy.LastResult.Executed.Should().BeTrue("the task eventually runs when analysis is declared done");
    }

    [Fact]
    public async Task AnalysisParalysis_Respects_Cancellation()
    {
        // arrange — analysis is cancelled before it can discover any open questions
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);

        var strategy = new AnalysisParalysisStrategy(
            analysisDuration: TimeSpan.FromMilliseconds(1),
            absoluteDeadlineOffset: TimeSpan.FromSeconds(10));

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

        // assert — no analysis, no execution
        executed.Should().BeFalse("cancellation prevents the analysis from starting and the task from running");
    }

    [Fact]
    public async Task AnalysisParalysis_CanBe_Registered_In_Scheduler()
    {
        // arrange — AnalysisParalysis is a first-class ProcrastinationMode
        var executed = false;
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);

        // act
        await ProcrastinationScheduler.Schedule(
            () => { executed = true; return Task.CompletedTask; },
            TimeSpan.FromMilliseconds(1),
            ProcrastinationMode.AnalysisParalysis,
            excuseProvider: null,
            delayStrategy: delayStrategy,
            randomProvider: randomProvider);

        // assert
        executed.Should().BeTrue("ProcrastinationMode.AnalysisParalysis is wired to AnalysisParalysisStrategy");
    }
}
