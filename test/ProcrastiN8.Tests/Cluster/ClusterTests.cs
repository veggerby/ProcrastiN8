using ProcrastiN8.Cluster.Abstractions;
using ProcrastiN8.Cluster.Transport;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.Cluster;

public class ProcrastinationClusterTests
{
    [Fact]
    public void Constructor_InitializesClusterCorrectly()
    {
        // Arrange & Act
        using var cluster = new ProcrastinationCluster(
            "test-node",
            "localhost:5000");

        // Assert
        cluster.LocalNode.Should().NotBeNull();
        cluster.LocalNode.NodeId.Should().Be("test-node");
        cluster.LocalNode.Endpoint.Should().Be("localhost:5000");
        cluster.Nodes.Should().HaveCount(1);
        cluster.NodeSelector.Should().NotBeNull();
    }

    [Fact]
    public async Task JoinAsync_ChangesNodeToAvailable()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        InMemoryNodeDiscovery.ClearAllNodes();

        using var cluster = new ProcrastinationCluster(
            "test-node",
            "localhost:5000");

        // Act
        await cluster.JoinAsync();

        // Assert
        cluster.LocalNode.State.Should().Be(NodeState.Available);
    }

    [Fact]
    public async Task JoinAsync_RaisesMembershipChangedEvent()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        InMemoryNodeDiscovery.ClearAllNodes();

        using var cluster = new ProcrastinationCluster(
            "test-node",
            "localhost:5000");

        ClusterMembershipChangedEventArgs? eventArgs = null;
        cluster.MembershipChanged += (_, e) => eventArgs = e;

        // Act
        await cluster.JoinAsync();

        // Assert
        eventArgs.Should().NotBeNull();
        eventArgs!.ChangeType.Should().Be(MembershipChangeType.NodeJoined);
        eventArgs.AffectedNode.NodeId.Should().Be("test-node");
    }

    [Fact]
    public async Task LeaveAsync_ChangesNodeToShutdown()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        InMemoryNodeDiscovery.ClearAllNodes();

        using var cluster = new ProcrastinationCluster(
            "test-node",
            "localhost:5000");

        await cluster.JoinAsync();

        // Act
        await cluster.LeaveAsync();

        // Assert
        cluster.LocalNode.State.Should().Be(NodeState.Shutdown);
    }

    [Fact]
    public async Task LeaveAsync_RaisesMembershipChangedEvent()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        InMemoryNodeDiscovery.ClearAllNodes();

        using var cluster = new ProcrastinationCluster(
            "test-node",
            "localhost:5000");

        await cluster.JoinAsync();

        ClusterMembershipChangedEventArgs? eventArgs = null;
        cluster.MembershipChanged += (_, e) => eventArgs = e;

        // Act
        await cluster.LeaveAsync();

        // Assert
        eventArgs.Should().NotBeNull();
        eventArgs!.ChangeType.Should().Be(MembershipChangeType.NodeLeft);
    }

    [Fact]
    public async Task SubmitDeferralAsync_ThrowsWhenNotJoined()
    {
        // Arrange
        using var cluster = new ProcrastinationCluster(
            "test-node",
            "localhost:5000");

        var workload = DeferralWorkload.Create(Guid.NewGuid(), TimeSpan.FromSeconds(5));

        // Act
        var act = () => cluster.SubmitDeferralAsync(workload);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*must be joined*");
    }

    [Fact]
    public async Task SubmitDeferralAsync_ReturnsReceipt()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        InMemoryNodeDiscovery.ClearAllNodes();

        using var cluster = new ProcrastinationCluster(
            "test-node",
            "localhost:5000");

        await cluster.JoinAsync();

        var workload = DeferralWorkload.Create(Guid.NewGuid(), TimeSpan.FromSeconds(5));

        // Act
        var receipt = await cluster.SubmitDeferralAsync(workload);

        // Assert
        receipt.Should().NotBeNull();
        receipt.WorkloadId.Should().Be(workload.WorkloadId);
        receipt.CorrelationId.Should().Be(workload.CorrelationId);
        receipt.NodeHistory.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetHealthMetrics_ReturnsValidMetrics()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        InMemoryNodeDiscovery.ClearAllNodes();

        using var cluster = new ProcrastinationCluster(
            "test-node",
            "localhost:5000");

        await cluster.JoinAsync();

        // Act
        var metrics = cluster.GetHealthMetrics();

        // Assert
        metrics.TotalNodes.Should().Be(1);
        metrics.HealthyNodes.Should().Be(1);
        metrics.BlameHeatmap.Should().NotBeNull();
    }

    [Fact]
    public void AddNode_AddsRemoteNode()
    {
        // Arrange
        using var cluster = new ProcrastinationCluster(
            "local-node",
            "localhost:5000");

        var remoteNode = new ProcrastinationNode("remote-node", "localhost:5001");

        // Act
        cluster.AddNode(remoteNode);

        // Assert
        cluster.Nodes.Should().HaveCount(2);
        cluster.Nodes.Should().Contain(n => n.NodeId == "remote-node");
    }

    [Fact]
    public void RemoveNode_RemovesRemoteNode()
    {
        // Arrange
        using var cluster = new ProcrastinationCluster(
            "local-node",
            "localhost:5000");

        var remoteNode = new ProcrastinationNode("remote-node", "localhost:5001");
        cluster.AddNode(remoteNode);

        // Act
        cluster.RemoveNode("remote-node");

        // Assert
        cluster.Nodes.Should().HaveCount(1);
        cluster.Nodes.Should().NotContain(n => n.NodeId == "remote-node");
    }

    [Fact]
    public void ZombifyNode_ChangesNodeStateToZombie()
    {
        // Arrange
        using var cluster = new ProcrastinationCluster(
            "local-node",
            "localhost:5000");

        var remoteNode = new ProcrastinationNode("remote-node", "localhost:5001");
        cluster.AddNode(remoteNode);

        // Act
        cluster.ZombifyNode("remote-node");

        // Assert
        var node = cluster.Nodes.First(n => n.NodeId == "remote-node");
        node.State.Should().Be(NodeState.Zombie);
    }

    [Fact]
    public void ZombifyNode_RaisesMembershipChangedEvent()
    {
        // Arrange
        using var cluster = new ProcrastinationCluster(
            "local-node",
            "localhost:5000");

        var remoteNode = new ProcrastinationNode("remote-node", "localhost:5001");
        cluster.AddNode(remoteNode);

        ClusterMembershipChangedEventArgs? eventArgs = null;
        cluster.MembershipChanged += (_, e) => eventArgs = e;

        // Act
        cluster.ZombifyNode("remote-node");

        // Assert
        eventArgs.Should().NotBeNull();
        eventArgs!.ChangeType.Should().Be(MembershipChangeType.NodeZombified);
    }

    [Fact]
    public void BlameTracker_IsAccessible()
    {
        // Arrange
        using var cluster = new ProcrastinationCluster(
            "test-node",
            "localhost:5000");

        // Act
        var tracker = cluster.BlameTracker;

        // Assert
        tracker.Should().NotBeNull();
    }
}

public class ClusterScenarioTests
{
    [Fact]
    public async Task Scenario_NodeJoinsAndLeaves()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        InMemoryNodeDiscovery.ClearAllNodes();

        using var cluster = new ProcrastinationCluster(
            "ephemeral-node",
            "localhost:5000");

        // Act & Assert - Join
        await cluster.JoinAsync();
        cluster.LocalNode.State.Should().Be(NodeState.Available);

        // Act & Assert - Leave
        await cluster.LeaveAsync();
        cluster.LocalNode.State.Should().Be(NodeState.Shutdown);
    }

    [Fact]
    public async Task Scenario_MultipleNodesInCluster()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        InMemoryNodeDiscovery.ClearAllNodes();

        using var cluster1 = new ProcrastinationCluster("node-1", "localhost:5001");
        using var cluster2 = new ProcrastinationCluster("node-2", "localhost:5002");

        // Act
        await cluster1.JoinAsync();
        cluster1.AddNode(new ProcrastinationNode("node-2", "localhost:5002"));

        await cluster2.JoinAsync();
        cluster2.AddNode(new ProcrastinationNode("node-1", "localhost:5001"));

        // Assert
        cluster1.Nodes.Should().HaveCount(2);
        cluster2.Nodes.Should().HaveCount(2);
    }

    [Fact]
    public async Task Scenario_NodeBecomesPartitioned()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        InMemoryNodeDiscovery.ClearAllNodes();

        using var cluster = new ProcrastinationCluster("main-node", "localhost:5000");

        var partitionedNode = new ProcrastinationNode("partition-node", "localhost:5001");
        partitionedNode.SetState(NodeState.Available);
        cluster.AddNode(partitionedNode);

        await cluster.JoinAsync();

        // Act - simulate partition
        partitionedNode.SetState(NodeState.Partitioned);

        // Assert
        var metrics = cluster.GetHealthMetrics();
        metrics.PartitionedNodes.Should().Be(1);
        metrics.HealthyNodes.Should().Be(1, "only the local node is healthy");
    }

    [Fact]
    public async Task Scenario_ZombieNodeStillProcrastinates()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        InMemoryNodeDiscovery.ClearAllNodes();

        using var cluster = new ProcrastinationCluster("main-node", "localhost:5000");

        var zombieNode = new ProcrastinationNode("zombie-node", "localhost:5001");
        zombieNode.SetState(NodeState.Available);
        zombieNode.SetPendingDeferrals(5); // Has pending work
        cluster.AddNode(zombieNode);

        await cluster.JoinAsync();

        // Act - node becomes zombie
        cluster.ZombifyNode("zombie-node");

        // Assert
        var metrics = cluster.GetHealthMetrics();
        metrics.ZombieNodes.Should().Be(1);
        metrics.TotalPendingDeferrals.Should().BeGreaterThanOrEqualTo(5, 
            "zombie still has pending deferrals — it's procrastinating even in death");
    }

    [Fact]
    public async Task Scenario_DeferralMigratesToBusiestNode()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        InMemoryNodeDiscovery.ClearAllNodes();

        using var cluster = new ProcrastinationCluster("local-node", "localhost:5000");

        var busyNode = new ProcrastinationNode("busy-node", "localhost:5001");
        busyNode.SetState(NodeState.Available);
        busyNode.SetWorkload(100); // Very busy

        var idleNode = new ProcrastinationNode("idle-node", "localhost:5002");
        idleNode.SetState(NodeState.Available);
        idleNode.SetWorkload(0); // Idle

        cluster.AddNode(busyNode);
        cluster.AddNode(idleNode);

        await cluster.JoinAsync();

        // Ensure local node is less busy than busy-node
        ((ProcrastinationNode)cluster.LocalNode).SetWorkload(1);

        var workload = DeferralWorkload.Create(Guid.NewGuid(), TimeSpan.FromSeconds(5));

        // Act
        var receipt = await cluster.SubmitDeferralAsync(workload);

        // Assert
        receipt.AssignedNodeId.Should().Be("busy-node", 
            "least available selector should pick the busiest node");
    }

    [Fact]
    public void Scenario_BlameAccumulatesOverTime()
    {
        // Arrange
        using var cluster = new ProcrastinationCluster("main-node", "localhost:5000");

        // Act - simulate multiple deferrals
        cluster.BlameTracker.RecordDeferral("lazy-node");
        cluster.BlameTracker.RecordDeferral("lazy-node");
        cluster.BlameTracker.RecordMigration("lazy-node", "victim-node");
        cluster.BlameTracker.RecordTimeout("lazy-node");

        // Assert
        var mostBlameworthy = cluster.BlameTracker.GetMostBlameworthy();
        mostBlameworthy.Should().Be("lazy-node");

        var heatmap = cluster.GetHealthMetrics().BlameHeatmap;
        heatmap["lazy-node"].Should().BeGreaterThan(heatmap.GetValueOrDefault("victim-node"));
    }
}

public class ClusterMembershipChangedEventArgsTests
{
    [Fact]
    public void Constructor_InitializesAllProperties()
    {
        // Arrange
        var node = new ProcrastinationNode("test-node", "localhost:5000");
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        var args = new ClusterMembershipChangedEventArgs(
            MembershipChangeType.NodeJoined,
            node,
            timestamp,
            "Node joined for testing");

        // Assert
        args.ChangeType.Should().Be(MembershipChangeType.NodeJoined);
        args.AffectedNode.Should().Be(node);
        args.Timestamp.Should().Be(timestamp);
        args.Reason.Should().Be("Node joined for testing");
    }

    [Theory]
    [InlineData(MembershipChangeType.NodeJoined)]
    [InlineData(MembershipChangeType.NodeLeft)]
    [InlineData(MembershipChangeType.NodePartitioned)]
    [InlineData(MembershipChangeType.NodeZombified)]
    [InlineData(MembershipChangeType.NodeRecovered)]
    [InlineData(MembershipChangeType.NodeStateChanged)]
    public void MembershipChangeType_HasExpectedValues(MembershipChangeType type)
    {
        // Assert
        Enum.IsDefined(typeof(MembershipChangeType), type).Should().BeTrue();
    }
}
