using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.JustBecause;

public class ManyWorldsSchedulerTests
{
    [Fact]
    public async Task ScheduleAsync_SingleUniverse_ReturnsResult()
    {
        // arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.0);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);

        var delayProvider = Substitute.For<IDelayProvider>();
        delayProvider.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // act
        var result = await ManyWorldsScheduler.ScheduleAsync(
            () => Task.FromResult(99),
            universeCount: 1,
            randomProvider: randomProvider,
            delayProvider: delayProvider);

        // assert
        result.Should().Be(99, "even a single universe can deliver the expected result");
    }

    [Fact]
    public async Task ScheduleAsync_MultipleUniverses_ReturnsFirstSuccess()
    {
        // arrange — three universes, all successful; first arrival wins
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.0);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0); // zero jitter so all start simultaneously

        var delayProvider = Substitute.For<IDelayProvider>();
        delayProvider.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // act
        var result = await ManyWorldsScheduler.ScheduleAsync(
            () => Task.FromResult(7),
            universeCount: 3,
            randomProvider: randomProvider,
            delayProvider: delayProvider);

        // assert
        result.Should().Be(7, "all universes agree; the one that arrived first collapsed into the prime timeline");
    }

    [Fact]
    public async Task ScheduleAsync_UniverseIndexed_DifferentBehaviourPerUniverse()
    {
        // arrange — universe 0 fails, universe 1 succeeds, universe 2 is too late
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);
        randomProvider.GetDouble().Returns(0.0);

        var delayProvider = Substitute.For<IDelayProvider>();
        delayProvider.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // act — universe 0 throws; universe 1 returns the answer
        var result = await ManyWorldsScheduler.ScheduleAsync<int>(
            i => i == 0
                ? Task.FromException<int>(new Exception("Universe 0 collapsed badly"))
                : Task.FromResult(i * 10),
            universeCount: 3,
            randomProvider: randomProvider,
            delayProvider: delayProvider);

        // assert — a surviving universe delivered the result
        result.Should().BeGreaterThan(0, "at least one universe survived to deliver a result");
    }

    [Fact]
    public async Task ScheduleAsync_AllUniversesFail_ThrowsAggregateException()
    {
        // arrange — no universe is capable of success (a relatable situation)
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);
        randomProvider.GetDouble().Returns(0.0);

        var delayProvider = Substitute.For<IDelayProvider>();
        delayProvider.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // act
        Func<Task> act = () => ManyWorldsScheduler.ScheduleAsync<int>(
            _ => Task.FromException<int>(new Exception("This universe is not cooperating")),
            universeCount: 2,
            randomProvider: randomProvider,
            delayProvider: delayProvider);

        // assert
        await act.Should().ThrowAsync<AggregateException>("all universes failed — the task was not meant to be done in any timeline");
    }

    [Fact]
    public async Task ScheduleAsync_ZeroUniverses_ThrowsArgumentOutOfRangeException()
    {
        // arrange + act
        Func<Task> act = () => ManyWorldsScheduler.ScheduleAsync(
            () => Task.FromResult(0),
            universeCount: 0);

        // assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>("zero universes is a philosophical position, not a scheduling strategy");
    }
}
