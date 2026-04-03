using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class CommitteeReviewStrategyTests
{
    [Fact]
    public async Task CommitteeReview_ExecutesTask_AfterDeadline()
    {
        // arrange — the committee will never agree, so we rely on the absolute deadline
        var executed = false;
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);

        var strategy = new CommitteeReviewStrategy(
            meetingDuration: TimeSpan.FromMilliseconds(1),
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

        // assert — the committee eventually gives up and the task runs
        executed.Should().BeTrue("the committee's deadline expires and the task executes regardless of their objections");
    }

    [Fact]
    public async Task CommitteeReview_Cycles_Are_Counted()
    {
        // arrange
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);

        var strategy = new CommitteeReviewStrategy(
            meetingDuration: TimeSpan.FromMilliseconds(1),
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

        // assert — at least one committee session took place before the task was run
        strategy.LastResult.Cycles.Should().BeGreaterThan(0, "the committee must convene at least once before giving up");
        strategy.LastResult.Executed.Should().BeTrue("the task eventually runs when quorum expires");
    }

    [Fact]
    public async Task CommitteeReview_Respects_Cancellation()
    {
        // arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel(); // pre-cancelled — nobody is even attending this meeting

        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);

        var strategy = new CommitteeReviewStrategy(
            meetingDuration: TimeSpan.FromMilliseconds(1),
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

        // assert — the meeting was cancelled before anyone joined
        executed.Should().BeFalse("the committee never convened; the task was not run");
    }

    [Fact]
    public async Task CommitteeReview_CanBe_Registered_In_Scheduler()
    {
        // arrange — CommitteeReview is now a first-class ProcrastinationMode
        var executed = false;
        var delayStrategy = Substitute.For<IDelayStrategy>();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);

        // act
        await ProcrastinationScheduler.Schedule(
            () => { executed = true; return Task.CompletedTask; },
            TimeSpan.FromMilliseconds(1),
            ProcrastinationMode.CommitteeReview,
            excuseProvider: null,
            delayStrategy: delayStrategy,
            randomProvider: randomProvider);

        // assert — the scheduler routes to CommitteeReviewStrategy successfully
        executed.Should().BeTrue("ProcrastinationMode.CommitteeReview is wired to CommitteeReviewStrategy");
    }
}
