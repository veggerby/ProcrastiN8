using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.Services;
using ProcrastiN8.Unproductivity;

namespace ProcrastiN8.Tests.Unproductivity;

public class MeetingSimulatorTests
{
    [Fact]
    public async Task RunMeetingAsync_Completes_And_Logs_Start_And_Close()
    {
        // arrange — the meeting must at least start and produce closing remarks
        var logger = Substitute.For<IProcrastiLogger>();
        var randomProvider = Substitute.For<IRandomProvider>();
        var delayProvider = Substitute.For<IDelayProvider>();

        randomProvider.GetDouble().Returns(0.5);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);
        randomProvider.GetRandom(Arg.Any<int>(), Arg.Any<int>()).Returns(3);

        // act — duration 5 for a short meeting (5ms tick)
        await MeetingSimulator.RunMeetingAsync(
            durationMinutes: 5,
            logger: logger,
            randomProvider: randomProvider,
            delayProvider: delayProvider);

        // assert — the meeting opened and produced action items, as meetings do
        logger.Received().Info(Arg.Is<string>(s => s.Contains("Meeting started")), Arg.Any<object[]>());
        logger.Received().Info(Arg.Is<string>(s => s.Contains("Follow-up meeting scheduled")), Arg.Any<object[]>());
    }

    [Fact]
    public async Task RunMeetingAsync_GeneratesActionItems()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var randomProvider = Substitute.For<IRandomProvider>();
        var delayProvider = Substitute.For<IDelayProvider>();

        randomProvider.GetDouble().Returns(0.5);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);
        randomProvider.GetRandom(Arg.Any<int>(), Arg.Any<int>()).Returns(2);

        // act
        await MeetingSimulator.RunMeetingAsync(
            durationMinutes: 3,
            logger: logger,
            randomProvider: randomProvider,
            delayProvider: delayProvider);

        // assert — action items were generated (and will not be completed)
        logger.Received().Info(Arg.Is<string>(s => s.Contains("Action Item")), Arg.Any<object[]>());
    }

    [Fact]
    public async Task RunMeetingAsync_WithCancellation_ExitsGracefully()
    {
        // arrange — participant leaves early; this is not encouraged but is technically supported
        using var cts = new CancellationTokenSource();
        var logger = Substitute.For<IProcrastiLogger>();
        var randomProvider = Substitute.For<IRandomProvider>();

        randomProvider.GetDouble().Returns(0.5);
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);
        randomProvider.GetRandom(Arg.Any<int>(), Arg.Any<int>()).Returns(3);

        var delayProvider = Substitute.For<IDelayProvider>();
        delayProvider
            .When(d => d.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()))
            .Do(_ => cts.Cancel());

        // act
        await MeetingSimulator.RunMeetingAsync(
            durationMinutes: 10,
            logger: logger,
            randomProvider: randomProvider,
            delayProvider: delayProvider,
            cancellationToken: cts.Token);

        // assert — the early departure is logged
        logger.Received().Info(
            Arg.Is<string>(s => s.Contains("left the meeting early")),
            Arg.Any<object[]>());
    }
}
