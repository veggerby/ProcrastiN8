using ProcrastiN8.Cluster.Abstractions;
using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.Cluster;

public class NodeStateTests
{
    [Theory]
    [InlineData(NodeState.Initializing)]
    [InlineData(NodeState.Available)]
    [InlineData(NodeState.Busy)]
    [InlineData(NodeState.Partitioned)]
    [InlineData(NodeState.Zombie)]
    [InlineData(NodeState.Shutdown)]
    public void NodeState_HasExpectedValues(NodeState state)
    {
        // Assert
        Enum.IsDefined(typeof(NodeState), state).Should().BeTrue();
    }
}

public class ProcrastinationNodeTests
{
    [Fact]
    public void Constructor_InitializesNodeCorrectly()
    {
        // Arrange
        var nodeId = "test-node-1";
        var endpoint = "localhost:5000";

        // Act
        var node = new ProcrastinationNode(nodeId, endpoint);

        // Assert
        node.NodeId.Should().Be(nodeId);
        node.Endpoint.Should().Be(endpoint);
        node.State.Should().Be(NodeState.Initializing);
        node.CurrentWorkload.Should().Be(0);
        node.PendingDeferrals.Should().Be(0);
    }

    [Fact]
    public void SetState_UpdatesState()
    {
        // Arrange
        var node = new ProcrastinationNode("test-node", "localhost:5000");

        // Act
        node.SetState(NodeState.Available);

        // Assert
        node.State.Should().Be(NodeState.Available);
    }

    [Fact]
    public void IncrementWorkload_IncreasesWorkloadCount()
    {
        // Arrange
        var node = new ProcrastinationNode("test-node", "localhost:5000");

        // Act
        node.IncrementWorkload();
        node.IncrementWorkload();

        // Assert
        node.CurrentWorkload.Should().Be(2);
    }

    [Fact]
    public void DecrementWorkload_DecreasesWorkloadCount()
    {
        // Arrange
        var node = new ProcrastinationNode("test-node", "localhost:5000");
        node.SetWorkload(5);

        // Act
        node.DecrementWorkload();

        // Assert
        node.CurrentWorkload.Should().Be(4);
    }

    [Fact]
    public void DecrementWorkload_DoesNotGoBelowZero()
    {
        // Arrange
        var node = new ProcrastinationNode("test-node", "localhost:5000");

        // Act
        node.DecrementWorkload();

        // Assert
        node.CurrentWorkload.Should().Be(0);
    }

    [Fact]
    public void RecordHeartbeat_UpdatesLastHeartbeat()
    {
        // Arrange
        var node = new ProcrastinationNode("test-node", "localhost:5000");
        var originalHeartbeat = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        node.RecordHeartbeat(originalHeartbeat);

        // Act - record a new heartbeat (uses DateTimeOffset.UtcNow internally)
        node.RecordHeartbeat();

        // Assert - the new heartbeat should be after our manually set timestamp
        node.LastHeartbeat.Should().BeAfter(originalHeartbeat);
    }

    [Fact]
    public void RecordHeartbeat_WithTimestamp_SetsSpecificTime()
    {
        // Arrange
        var node = new ProcrastinationNode("test-node", "localhost:5000");
        var specificTime = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        node.RecordHeartbeat(specificTime);

        // Assert
        node.LastHeartbeat.Should().Be(specificTime);
    }

    [Fact]
    public void IncrementPendingDeferrals_IncreasesDeferralCount()
    {
        // Arrange
        var node = new ProcrastinationNode("test-node", "localhost:5000");

        // Act
        node.IncrementPendingDeferrals();
        node.IncrementPendingDeferrals();
        node.IncrementPendingDeferrals();

        // Assert
        node.PendingDeferrals.Should().Be(3);
    }
}

public class LeastAvailableNodeSelectorTests
{
    [Fact]
    public void SelectNode_ReturnsNull_WhenNoNodesAvailable()
    {
        // Arrange
        var selector = new LeastAvailableNodeSelector();
        var nodes = new List<IProcrastinationNode>();
        var workload = DeferralWorkload.Create(Guid.NewGuid(), TimeSpan.FromSeconds(5));

        // Act
        var result = selector.SelectNode(nodes, workload);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SelectNode_ReturnsNull_WhenAllNodesArePartitioned()
    {
        // Arrange
        var selector = new LeastAvailableNodeSelector();
        var node1 = new ProcrastinationNode("node-1", "localhost:5001");
        var node2 = new ProcrastinationNode("node-2", "localhost:5002");
        node1.SetState(NodeState.Partitioned);
        node2.SetState(NodeState.Partitioned);

        var nodes = new List<IProcrastinationNode> { node1, node2 };
        var workload = DeferralWorkload.Create(Guid.NewGuid(), TimeSpan.FromSeconds(5));

        // Act
        var result = selector.SelectNode(nodes, workload);

        // Assert
        result.Should().BeNull("partitioned nodes should not be selected");
    }

    [Fact]
    public void SelectNode_PrefersBusiestNode()
    {
        // Arrange
        var selector = new LeastAvailableNodeSelector();
        var node1 = new ProcrastinationNode("node-1", "localhost:5001");
        var node2 = new ProcrastinationNode("node-2", "localhost:5002");
        var node3 = new ProcrastinationNode("node-3", "localhost:5003");

        node1.SetState(NodeState.Available);
        node2.SetState(NodeState.Available);
        node3.SetState(NodeState.Available);

        node1.SetWorkload(5);
        node2.SetWorkload(10); // Busiest
        node3.SetWorkload(3);

        var nodes = new List<IProcrastinationNode> { node1, node2, node3 };
        var workload = DeferralWorkload.Create(Guid.NewGuid(), TimeSpan.FromSeconds(5));

        // Act
        var result = selector.SelectNode(nodes, workload);

        // Assert
        result.Should().NotBeNull();
        result!.NodeId.Should().Be("node-2", "the busiest node should be selected");
    }

    [Fact]
    public void SelectNode_SelectsFromBusyNodes()
    {
        // Arrange
        var selector = new LeastAvailableNodeSelector();
        var node1 = new ProcrastinationNode("node-1", "localhost:5001");
        node1.SetState(NodeState.Busy);
        node1.SetWorkload(5);

        var nodes = new List<IProcrastinationNode> { node1 };
        var workload = DeferralWorkload.Create(Guid.NewGuid(), TimeSpan.FromSeconds(5));

        // Act
        var result = selector.SelectNode(nodes, workload);

        // Assert
        result.Should().NotBeNull();
        result!.NodeId.Should().Be("node-1");
    }
}

public class DeferralWorkloadTests
{
    [Fact]
    public void Create_GeneratesValidWorkload()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var initialDelay = TimeSpan.FromSeconds(30);

        // Act
        var workload = DeferralWorkload.Create(correlationId, initialDelay);

        // Assert
        workload.WorkloadId.Should().NotBe(Guid.Empty);
        workload.CorrelationId.Should().Be(correlationId);
        workload.InitialDelay.Should().Be(initialDelay);
        workload.Priority.Should().Be(0);
        workload.MaxDeferrals.Should().Be(int.MaxValue, "infinite deferrals by default");
        workload.Payload.Should().BeNull();
        workload.Tags.Should().BeNull();
    }

    [Fact]
    public void Workload_CanBeCreatedWithTags()
    {
        // Arrange
        var tags = new Dictionary<string, string>
        {
            ["owner"] = "lazy-dev",
            ["priority"] = "whenever"
        };

        // Act
        var workload = new DeferralWorkload(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Priority: 5,
            TimeSpan.FromMinutes(5),
            MaxDeferrals: 10,
            DateTimeOffset.UtcNow,
            Payload: null,
            Tags: tags);

        // Assert
        workload.Tags.Should().NotBeNull();
        workload.Tags!["owner"].Should().Be("lazy-dev");
        workload.Priority.Should().Be(5);
        workload.MaxDeferrals.Should().Be(10);
    }
}

public class DeferralReceiptTests
{
    [Fact]
    public void Receipt_ContainsExpectedValues()
    {
        // Arrange & Act
        var receipt = new DeferralReceipt(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "node-1",
            DateTimeOffset.UtcNow,
            null)
        {
            DeferralCount = 3,
            NodeHistory = ["node-1", "node-2", "node-3"]
        };

        // Assert
        receipt.AssignedNodeId.Should().Be("node-1");
        receipt.DeferralCount.Should().Be(3);
        receipt.NodeHistory.Should().HaveCount(3);
        receipt.EstimatedCompletionTime.Should().BeNull("estimated times are always wrong");
    }
}

public class ClusterHealthMetricsTests
{
    [Fact]
    public void HealthMetrics_DefaultsToEmptyBlameHeatmap()
    {
        // Arrange & Act
        var metrics = new ClusterHealthMetrics();

        // Assert
        metrics.BlameHeatmap.Should().NotBeNull();
        metrics.BlameHeatmap.Should().BeEmpty();
    }

    [Fact]
    public void HealthMetrics_CanBeInitialized()
    {
        // Arrange
        var blameMap = new Dictionary<string, int>
        {
            ["node-1"] = 5,
            ["node-2"] = 10
        };

        // Act
        var metrics = new ClusterHealthMetrics
        {
            TotalNodes = 3,
            HealthyNodes = 2,
            PartitionedNodes = 1,
            ZombieNodes = 0,
            TotalPendingDeferrals = 15,
            AverageWorkloadPerNode = 5.0,
            BlameHeatmap = blameMap,
            ConsensusEpoch = 42
        };

        // Assert
        metrics.TotalNodes.Should().Be(3);
        metrics.HealthyNodes.Should().Be(2);
        metrics.PartitionedNodes.Should().Be(1);
        metrics.ZombieNodes.Should().Be(0);
        metrics.TotalPendingDeferrals.Should().Be(15);
        metrics.AverageWorkloadPerNode.Should().Be(5.0);
        metrics.BlameHeatmap.Should().HaveCount(2);
        metrics.ConsensusEpoch.Should().Be(42);
    }
}

public class InMemoryNodeDiscoveryTests
{
    [Fact]
    public async Task AnnounceAsync_RegistersNode()
    {
        // Arrange
        InMemoryNodeDiscovery.ClearAllNodes();
        var discovery = new InMemoryNodeDiscovery();
        var node = new ProcrastinationNode("test-node", "localhost:5000");

        // Act
        await discovery.AnnounceAsync(node);

        // Assert
        var registeredNodes = discovery.GetRegisteredNodes();
        registeredNodes.Should().Contain(n => n.NodeId == "test-node");
    }

    [Fact]
    public async Task DiscoverNodesAsync_ReturnsEndpoints()
    {
        // Arrange
        InMemoryNodeDiscovery.ClearAllNodes();
        var discovery = new InMemoryNodeDiscovery();
        var node1 = new ProcrastinationNode("node-1", "localhost:5001");
        var node2 = new ProcrastinationNode("node-2", "localhost:5002");

        await discovery.AnnounceAsync(node1);
        await discovery.AnnounceAsync(node2);

        // Act
        var endpoints = await discovery.DiscoverNodesAsync();

        // Assert
        endpoints.Should().Contain("localhost:5001");
        endpoints.Should().Contain("localhost:5002");
    }

    [Fact]
    public async Task DeregisterAsync_RemovesNode()
    {
        // Arrange
        InMemoryNodeDiscovery.ClearAllNodes();
        var discovery = new InMemoryNodeDiscovery();
        var node = new ProcrastinationNode("test-node", "localhost:5000");
        await discovery.AnnounceAsync(node);

        // Act
        await discovery.DeregisterAsync("test-node");

        // Assert
        var endpoints = await discovery.DiscoverNodesAsync();
        endpoints.Should().NotContain("localhost:5000");
    }
}
