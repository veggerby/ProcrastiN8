using ProcrastiN8.Cluster.Transport;
using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.Cluster;

public class ClusterMessageTypeTests
{
    [Theory]
    [InlineData(ClusterMessageType.Heartbeat)]
    [InlineData(ClusterMessageType.DeferralSubmission)]
    [InlineData(ClusterMessageType.DeferralAcknowledgment)]
    [InlineData(ClusterMessageType.WorkloadMigration)]
    [InlineData(ClusterMessageType.ConsensusVote)]
    [InlineData(ClusterMessageType.ConsensusProposal)]
    [InlineData(ClusterMessageType.DiscoveryAnnouncement)]
    [InlineData(ClusterMessageType.ShutdownNotification)]
    [InlineData(ClusterMessageType.BlameAssignment)]
    public void ClusterMessageType_HasExpectedValues(ClusterMessageType type)
    {
        // Assert
        Enum.IsDefined(typeof(ClusterMessageType), type).Should().BeTrue();
    }
}

public class ClusterMessageTests
{
    [Fact]
    public void Create_GeneratesValidMessage()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var sourceNodeId = "source-node";

        // Act
        var message = ClusterMessage.Create(
            correlationId,
            ClusterMessageType.Heartbeat,
            sourceNodeId);

        // Assert
        message.MessageId.Should().NotBe(Guid.Empty);
        message.CorrelationId.Should().Be(correlationId);
        message.MessageType.Should().Be(ClusterMessageType.Heartbeat);
        message.SourceNodeId.Should().Be(sourceNodeId);
        message.TargetNodeId.Should().BeNull();
        message.Payload.Should().BeNull();
        message.Epoch.Should().Be(0);
    }

    [Fact]
    public void Create_WithTargetNode_SetsTarget()
    {
        // Arrange
        var correlationId = Guid.NewGuid();

        // Act
        var message = ClusterMessage.Create(
            correlationId,
            ClusterMessageType.DeferralSubmission,
            "source-node",
            "target-node");

        // Assert
        message.TargetNodeId.Should().Be("target-node");
    }

    [Fact]
    public void Heartbeat_CreatesHeartbeatMessage()
    {
        // Arrange
        var sourceNodeId = "heartbeat-source";

        // Act
        var message = ClusterMessage.Heartbeat(sourceNodeId, epoch: 42);

        // Assert
        message.MessageType.Should().Be(ClusterMessageType.Heartbeat);
        message.SourceNodeId.Should().Be(sourceNodeId);
        message.Epoch.Should().Be(42);
        message.TargetNodeId.Should().BeNull();
    }

    [Fact]
    public void Create_WithPayload_SetsPayload()
    {
        // Arrange
        var payload = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        var message = ClusterMessage.Create(
            Guid.NewGuid(),
            ClusterMessageType.DeferralSubmission,
            "source",
            "target",
            payload,
            epoch: 10);

        // Assert
        message.Payload.Should().BeEquivalentTo(payload);
        message.Epoch.Should().Be(10);
    }
}

public class InMemoryClusterTransportTests
{
    [Fact]
    public async Task ConnectAsync_SetsConnectedState()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        var transport = new InMemoryClusterTransport();

        // Act
        await transport.ConnectAsync("test-endpoint");

        // Assert
        transport.IsConnected.Should().BeTrue();
        transport.TransportName.Should().Be("InMemory");
    }

    [Fact]
    public async Task DisconnectAsync_SetsDisconnectedState()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        var transport = new InMemoryClusterTransport();
        await transport.ConnectAsync("test-endpoint");

        // Act
        await transport.DisconnectAsync();

        // Assert
        transport.IsConnected.Should().BeFalse();
    }

    [Fact]
    public async Task SendAsync_ThrowsWhenNotConnected()
    {
        // Arrange
        var transport = new InMemoryClusterTransport();
        var message = ClusterMessage.Heartbeat("source");

        // Act
        var act = () => transport.SendAsync(message);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task BroadcastAsync_ThrowsWhenNotConnected()
    {
        // Arrange
        var transport = new InMemoryClusterTransport();
        var message = ClusterMessage.Heartbeat("source");

        // Act
        var act = () => transport.BroadcastAsync(message);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task SendAsync_DeliversMessageToTarget()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        var transport1 = new InMemoryClusterTransport();
        var transport2 = new InMemoryClusterTransport();
        await transport1.ConnectAsync("node-1");
        await transport2.ConnectAsync("node-2");

        IClusterMessage? receivedMessage = null;
        transport2.MessageReceived += (_, e) => receivedMessage = e.Message;

        var message = ClusterMessage.Create(
            Guid.NewGuid(),
            ClusterMessageType.Heartbeat,
            "node-1",
            "node-2");

        // Act
        await transport1.SendAsync(message);

        // Assert
        receivedMessage.Should().NotBeNull();
        receivedMessage!.SourceNodeId.Should().Be("node-1");
    }

    [Fact]
    public async Task BroadcastAsync_DeliversToAllExceptSender()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        var transport1 = new InMemoryClusterTransport();
        var transport2 = new InMemoryClusterTransport();
        var transport3 = new InMemoryClusterTransport();
        await transport1.ConnectAsync("node-1");
        await transport2.ConnectAsync("node-2");
        await transport3.ConnectAsync("node-3");

        var receivedCount = 0;
        transport2.MessageReceived += (_, _) => receivedCount++;
        transport3.MessageReceived += (_, _) => receivedCount++;

        var message = ClusterMessage.Heartbeat("node-1");

        // Act
        await transport1.BroadcastAsync(message);

        // Assert
        receivedCount.Should().Be(2, "both other nodes should receive the broadcast");
    }

    [Fact]
    public async Task ConnectionStateChanged_RaisedOnConnect()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        var transport = new InMemoryClusterTransport();
        var eventRaised = false;
        transport.ConnectionStateChanged += (_, e) =>
        {
            if (e.IsConnected)
            {
                eventRaised = true;
            }
        };

        // Act
        await transport.ConnectAsync("test-endpoint");

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public async Task ConnectionStateChanged_RaisedOnDisconnect()
    {
        // Arrange
        InMemoryClusterTransport.ClearAllEndpoints();
        var transport = new InMemoryClusterTransport();
        await transport.ConnectAsync("test-endpoint");

        var disconnectEventRaised = false;
        transport.ConnectionStateChanged += (_, e) =>
        {
            if (!e.IsConnected)
            {
                disconnectEventRaised = true;
            }
        };

        // Act
        await transport.DisconnectAsync();

        // Assert
        disconnectEventRaised.Should().BeTrue();
    }
}

public class CarrierPigeonTransportTests
{
    [Fact]
    public async Task ConnectAsync_SetsConnectedState()
    {
        // Arrange
        var transport = new CarrierPigeonTransport();

        // Act
        await transport.ConnectAsync("coop-1");

        // Assert
        transport.IsConnected.Should().BeTrue();
        transport.TransportName.Should().Be("CarrierPigeon");
    }

    [Fact]
    public async Task DisconnectAsync_SetsDisconnectedState()
    {
        // Arrange
        var transport = new CarrierPigeonTransport();
        await transport.ConnectAsync("coop-1");

        // Act
        await transport.DisconnectAsync();

        // Assert
        transport.IsConnected.Should().BeFalse();
    }

    [Fact]
    public async Task SendAsync_ThrowsWhenNotConnected()
    {
        // Arrange
        var transport = new CarrierPigeonTransport();
        var message = ClusterMessage.Heartbeat("source");

        // Act
        var act = () => transport.SendAsync(message);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Pigeon coop*");
    }

    [Fact]
    public async Task SendAsync_DeliversMessageWithDelay()
    {
        // Arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5); // Consistent random values

        var transport = new CarrierPigeonTransport(
            randomProvider,
            baseFlightTime: TimeSpan.FromMilliseconds(10),
            messageDropRate: 0.0); // No drops for this test

        await transport.ConnectAsync("coop-1");

        IClusterMessage? receivedMessage = null;
        transport.MessageReceived += (_, e) => receivedMessage = e.Message;

        var message = ClusterMessage.Heartbeat("coop-1");

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await transport.SendAsync(message);
        stopwatch.Stop();

        // Assert
        receivedMessage.Should().NotBeNull();
        stopwatch.ElapsedMilliseconds.Should().BeGreaterThanOrEqualTo(10);
    }

    [Fact]
    public async Task SendAsync_DropsMessage_WhenRandomBelowDropRate()
    {
        // Arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.05); // Below 0.1 drop rate

        var transport = new CarrierPigeonTransport(
            randomProvider,
            baseFlightTime: TimeSpan.FromMilliseconds(1),
            messageDropRate: 0.1);

        await transport.ConnectAsync("coop-1");

        IClusterMessage? receivedMessage = null;
        transport.MessageReceived += (_, e) => receivedMessage = e.Message;

        var message = ClusterMessage.Heartbeat("coop-1");

        // Act
        await transport.SendAsync(message);

        // Assert
        receivedMessage.Should().BeNull("message should be dropped by pigeon");
    }

    [Fact]
    public void GetRandomExcuse_ReturnsNonEmptyString()
    {
        // Arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        var transport = new CarrierPigeonTransport(randomProvider);

        // Act
        var excuse = transport.GetRandomExcuse();

        // Assert
        excuse.Should().NotBeNullOrEmpty();
    }
}
