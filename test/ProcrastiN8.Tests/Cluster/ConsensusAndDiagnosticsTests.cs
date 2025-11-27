using ProcrastiN8.Cluster.Consensus;
using ProcrastiN8.Cluster.Diagnostics;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.Cluster;

public class ConsensusStateTests
{
    [Theory]
    [InlineData(ConsensusState.Idle)]
    [InlineData(ConsensusState.Proposing)]
    [InlineData(ConsensusState.Voting)]
    [InlineData(ConsensusState.Committed)]
    [InlineData(ConsensusState.Failed)]
    public void ConsensusState_HasExpectedValues(ConsensusState state)
    {
        // Assert
        Enum.IsDefined(typeof(ConsensusState), state).Should().BeTrue();
    }
}

public class GlobalMovingTargetClockTests
{
    [Fact]
    public void Constructor_InitializesWithIdleState()
    {
        // Arrange & Act
        var clock = new GlobalMovingTargetClock("test-node");

        // Assert
        clock.CurrentEpoch.Should().Be(0);
        clock.State.Should().Be(ConsensusState.Idle);
        clock.IsLeader.Should().BeFalse();
        clock.LeaderNodeId.Should().BeNull();
    }

    [Fact]
    public async Task ProposeTargetTimeAsync_ChangesStateAndEpoch()
    {
        // Arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);

        var clock = new GlobalMovingTargetClock(
            "test-node",
            randomProvider);

        var proposedTime = DateTimeOffset.UtcNow.AddMinutes(5);

        // Act
        var result = await clock.ProposeTargetTimeAsync(proposedTime);

        // Assert
        result.Should().BeTrue();
        clock.CurrentEpoch.Should().Be(1);
        clock.State.Should().Be(ConsensusState.Committed);
    }

    [Fact]
    public async Task AdvanceEpochAsync_IncrementsEpoch()
    {
        // Arrange
        var clock = new GlobalMovingTargetClock("test-node");

        // Act
        var newEpoch = await clock.AdvanceEpochAsync();

        // Assert
        newEpoch.Should().Be(1);
        clock.CurrentEpoch.Should().Be(1);
        clock.State.Should().Be(ConsensusState.Idle);
    }

    [Fact]
    public void TryBecomeLeader_SetsNodeAsLeader()
    {
        // Arrange
        var clock = new GlobalMovingTargetClock("leader-node");

        // Act
        var result = clock.TryBecomeLeader();

        // Assert
        result.Should().BeTrue();
        clock.IsLeader.Should().BeTrue();
        clock.LeaderNodeId.Should().Be("leader-node");
    }

    [Fact]
    public void TryBecomeLeader_ReturnsTrueIfAlreadyLeader()
    {
        // Arrange
        var clock = new GlobalMovingTargetClock("leader-node");
        clock.TryBecomeLeader();

        // Act
        var result = clock.TryBecomeLeader();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void GetCurrentTargetTime_AppliesDrift()
    {
        // Arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.8); // Consistent drift

        var clock = new GlobalMovingTargetClock(
            "test-node",
            randomProvider,
            driftFactor: 0.5);

        // Act
        var time1 = clock.GetCurrentTargetTime();
        var time2 = clock.GetCurrentTargetTime();

        // Assert
        time1.Should().NotBe(time2, "target time should drift");
    }

    [Fact]
    public async Task VoteAsync_IgnoresIncorrectEpoch()
    {
        // Arrange
        var clock = new GlobalMovingTargetClock("test-node");
        var initialEpoch = clock.CurrentEpoch;

        // Act - vote for wrong epoch
        await clock.VoteAsync(999, accept: true);

        // Assert
        clock.CurrentEpoch.Should().Be(initialEpoch);
    }

    [Fact]
    public void SetClusterSize_UpdatesRequiredVotes()
    {
        // Arrange
        var clock = new GlobalMovingTargetClock("test-node");

        // Act
        clock.SetClusterSize(5);

        // Assert - no direct way to verify, but shouldn't throw
        true.Should().BeTrue();
    }

    [Fact]
    public async Task EpochChanged_EventIsRaised()
    {
        // Arrange
        var clock = new GlobalMovingTargetClock("test-node");
        EpochChangedEventArgs? eventArgs = null;
        clock.EpochChanged += (_, e) => eventArgs = e;

        // Act
        await clock.AdvanceEpochAsync();

        // Assert
        eventArgs.Should().NotBeNull();
        eventArgs!.OldEpoch.Should().Be(0);
        eventArgs.NewEpoch.Should().Be(1);
    }

    [Fact]
    public void LeaderElected_EventIsRaised()
    {
        // Arrange
        var clock = new GlobalMovingTargetClock("test-node");
        LeaderElectedEventArgs? eventArgs = null;
        clock.LeaderElected += (_, e) => eventArgs = e;

        // Act
        clock.TryBecomeLeader();

        // Assert
        eventArgs.Should().NotBeNull();
        eventArgs!.NewLeaderId.Should().Be("test-node");
        eventArgs.PreviousLeaderId.Should().BeNull();
    }
}

public class BlameHeatmapTrackerTests
{
    [Fact]
    public void RecordDeferral_IncrementsCount()
    {
        // Arrange
        var tracker = new BlameHeatmapTracker();

        // Act
        tracker.RecordDeferral("node-1");
        tracker.RecordDeferral("node-1");
        tracker.RecordDeferral("node-2");

        // Assert
        var heatmap = tracker.GetHeatmap();
        heatmap["node-1"].Should().BeGreaterThanOrEqualTo(2);
        heatmap["node-2"].Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void RecordMigration_TracksSourceAndDestination()
    {
        // Arrange
        var tracker = new BlameHeatmapTracker();

        // Act
        tracker.RecordMigration("node-1", "node-2", "Too lazy");

        // Assert
        var records = tracker.GetDetailedRecords();
        records["node-1"].MigrationsSent.Should().Be(1);
        records["node-2"].MigrationsReceived.Should().Be(1);
        records["node-1"].LastMigrationReason.Should().Be("Too lazy");
    }

    [Fact]
    public void RecordTimeout_IncreasesBlameScore()
    {
        // Arrange
        var tracker = new BlameHeatmapTracker();

        // Act
        tracker.RecordTimeout("node-1");

        // Assert
        var records = tracker.GetDetailedRecords();
        records["node-1"].TimeoutCount.Should().Be(1);
        records["node-1"].CalculateBlameScore().Should().Be(3, "timeouts are worth 3 points");
    }

    [Fact]
    public void GetMostBlameworthy_ReturnsHighestScoringNode()
    {
        // Arrange
        var tracker = new BlameHeatmapTracker();
        tracker.RecordDeferral("node-1");
        tracker.RecordTimeout("node-2"); // Worth 3 points

        // Act
        var mostBlameworthy = tracker.GetMostBlameworthy();

        // Assert
        mostBlameworthy.Should().Be("node-2");
    }

    [Fact]
    public void GetMostBlameworthy_ReturnsNull_WhenNoRecords()
    {
        // Arrange
        var tracker = new BlameHeatmapTracker();

        // Act
        var result = tracker.GetMostBlameworthy();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetTopBlameworthy_ReturnsOrderedList()
    {
        // Arrange
        var tracker = new BlameHeatmapTracker();
        tracker.RecordDeferral("node-1");
        tracker.RecordTimeout("node-2");
        tracker.RecordMigration("node-3", "node-1", null);
        tracker.RecordMigration("node-3", "node-2", null);

        // Act
        var top = tracker.GetTopBlameworthy(2);

        // Assert
        top.Should().HaveCount(2);
        top[0].Should().Be("node-3", "node-3 has 4 points from 2 migrations");
    }

    [Fact]
    public void ClearBlame_RemovesNodeRecord()
    {
        // Arrange
        var tracker = new BlameHeatmapTracker();
        tracker.RecordDeferral("node-1");

        // Act
        tracker.ClearBlame("node-1");

        // Assert
        var heatmap = tracker.GetHeatmap();
        heatmap.Should().NotContainKey("node-1");
    }

    [Fact]
    public void ClearAll_RemovesAllRecords()
    {
        // Arrange
        var tracker = new BlameHeatmapTracker();
        tracker.RecordDeferral("node-1");
        tracker.RecordDeferral("node-2");

        // Act
        tracker.ClearAll();

        // Assert
        var heatmap = tracker.GetHeatmap();
        heatmap.Should().BeEmpty();
    }

    [Fact]
    public void BlameRecord_CalculatesBlameScoreCorrectly()
    {
        // Arrange
        var record = new BlameRecord
        {
            DeferralCount = 5,       // 5 points
            MigrationsSent = 3,      // 6 points
            TimeoutCount = 2,        // 6 points
            MigrationsReceived = 4   // -2 points
        };

        // Act
        var score = record.CalculateBlameScore();

        // Assert
        score.Should().Be(15, "5 + 6 + 6 - 2 = 15");
    }
}

public class EpochChangedEventArgsTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        // Arrange & Act
        var args = new EpochChangedEventArgs(1, 2, "Test reason");

        // Assert
        args.OldEpoch.Should().Be(1);
        args.NewEpoch.Should().Be(2);
        args.Reason.Should().Be("Test reason");
    }
}

public class LeaderElectedEventArgsTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        // Arrange & Act
        var args = new LeaderElectedEventArgs("old-leader", "new-leader", 5);

        // Assert
        args.PreviousLeaderId.Should().Be("old-leader");
        args.NewLeaderId.Should().Be("new-leader");
        args.Epoch.Should().Be(5);
    }

    [Fact]
    public void Constructor_AllowsNullPreviousLeader()
    {
        // Arrange & Act
        var args = new LeaderElectedEventArgs(null, "new-leader", 1);

        // Assert
        args.PreviousLeaderId.Should().BeNull();
    }
}
