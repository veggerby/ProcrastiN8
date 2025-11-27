using System.Collections.Concurrent;
using ProcrastiN8.Cluster.Consensus;
using ProcrastiN8.Cluster.Diagnostics;
using ProcrastiN8.Cluster.Transport;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Cluster.Abstractions;

/// <summary>
/// Default implementation of <see cref="IProcrastinationCluster"/> providing distributed procrastination capabilities.
/// </summary>
public sealed class ProcrastinationCluster : IProcrastinationCluster, IDisposable
{
    private readonly ConcurrentDictionary<string, IProcrastinationNode> _nodes = new();
    private readonly IClusterTransport _transport;
    private readonly INodeDiscovery _discovery;
    private readonly IConsensusProtocol _consensus;
    private readonly BlameHeatmapTracker _blameTracker;
    private readonly IProcrastiLogger _logger;
    private readonly ITimeProvider _timeProvider;
    private readonly ProcrastinationNode _localNode;
    private readonly DateTimeOffset _startTime;

    private bool _isJoined;
    private bool _disposed;

    /// <summary>
    /// Initializes a new procrastination cluster.
    /// </summary>
    /// <param name="nodeId">The local node's ID.</param>
    /// <param name="endpoint">The local node's endpoint.</param>
    /// <param name="transport">The cluster transport.</param>
    /// <param name="discovery">The node discovery service.</param>
    /// <param name="consensus">The consensus protocol.</param>
    /// <param name="nodeSelector">The node selector for load balancing.</param>
    /// <param name="logger">Logger for cluster events.</param>
    /// <param name="timeProvider">Time provider.</param>
    public ProcrastinationCluster(
        string nodeId,
        string endpoint,
        IClusterTransport? transport = null,
        INodeDiscovery? discovery = null,
        IConsensusProtocol? consensus = null,
        INodeSelector? nodeSelector = null,
        IProcrastiLogger? logger = null,
        ITimeProvider? timeProvider = null)
    {
        _logger = logger ?? DefaultLogger.Instance;
        _timeProvider = timeProvider ?? SystemTimeProvider.Default;
        _transport = transport ?? new InMemoryClusterTransport();
        _discovery = discovery ?? new InMemoryNodeDiscovery(_logger);
        _consensus = consensus ?? new GlobalMovingTargetClock(nodeId, logger: _logger, timeProvider: _timeProvider);
        NodeSelector = nodeSelector ?? new LeastAvailableNodeSelector();
        _blameTracker = new BlameHeatmapTracker();
        _startTime = _timeProvider.GetUtcNow();

        _localNode = new ProcrastinationNode(nodeId, endpoint);
        _nodes[nodeId] = _localNode;

        // Wire up transport events
        _transport.MessageReceived += OnMessageReceived;
    }

    /// <inheritdoc />
    public IProcrastinationNode LocalNode => _localNode;

    /// <inheritdoc />
    public IReadOnlyCollection<IProcrastinationNode> Nodes => _nodes.Values.ToList();

    /// <inheritdoc />
    public INodeSelector NodeSelector { get; }

    /// <inheritdoc />
    public event EventHandler<ClusterMembershipChangedEventArgs>? MembershipChanged;

    /// <inheritdoc />
    public async Task JoinAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_isJoined)
        {
            _logger.Warn("Node {NodeId} is already joined to the cluster", _localNode.NodeId);
            return;
        }

        _logger.Info("Node {NodeId} is joining the cluster at endpoint {Endpoint}",
            _localNode.NodeId, _localNode.Endpoint);

        // Connect transport
        await _transport.ConnectAsync(_localNode.Endpoint, cancellationToken);

        // Announce to discovery service
        await _discovery.AnnounceAsync(_localNode, cancellationToken);

        // Discover existing nodes
        var existingEndpoints = await _discovery.DiscoverNodesAsync(cancellationToken);
        _logger.Debug("Discovered {Count} existing nodes", existingEndpoints.Count);

        // Update local node state
        _localNode.SetState(NodeState.Available);
        _isJoined = true;

        // Record diagnostics
        ClusterDiagnostics.RecordNodeJoin(_localNode.NodeId);

        // Raise membership changed event
        MembershipChanged?.Invoke(this, new ClusterMembershipChangedEventArgs(
            MembershipChangeType.NodeJoined,
            _localNode,
            _timeProvider.GetUtcNow(),
            "Node successfully joined the cluster"));

        _logger.Info("Node {NodeId} successfully joined the cluster", _localNode.NodeId);
    }

    /// <inheritdoc />
    public async Task LeaveAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_isJoined)
        {
            _logger.Warn("Node {NodeId} is not joined to the cluster", _localNode.NodeId);
            return;
        }

        _logger.Info("Node {NodeId} is leaving the cluster", _localNode.NodeId);

        // Update state
        _localNode.SetState(NodeState.Shutdown);
        _isJoined = false;

        // Deregister from discovery
        await _discovery.DeregisterAsync(_localNode.NodeId, cancellationToken);

        // Disconnect transport
        await _transport.DisconnectAsync(cancellationToken);

        // Record diagnostics
        ClusterDiagnostics.RecordNodeLeave(_localNode.NodeId);

        // Raise membership changed event
        MembershipChanged?.Invoke(this, new ClusterMembershipChangedEventArgs(
            MembershipChangeType.NodeLeft,
            _localNode,
            _timeProvider.GetUtcNow(),
            "Node gracefully left the cluster"));

        _logger.Info("Node {NodeId} successfully left the cluster", _localNode.NodeId);
    }

    /// <inheritdoc />
    public async Task<DeferralReceipt> SubmitDeferralAsync(DeferralWorkload workload, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_isJoined)
        {
            throw new InvalidOperationException("Node must be joined to the cluster before submitting deferrals.");
        }

        using var activity = ClusterDiagnostics.StartDeferralSubmission(workload.CorrelationId, workload.WorkloadId);

        // Select the target node (prefer least available)
        var targetNode = NodeSelector.SelectNode(Nodes, workload);
        if (targetNode == null)
        {
            _logger.Warn("No available nodes to accept deferral {WorkloadId}", workload.WorkloadId);
            throw new InvalidOperationException("No available nodes in the cluster.");
        }

        _logger.Info("Submitting deferral {WorkloadId} to node {NodeId}",
            workload.WorkloadId, targetNode.NodeId);

        // Update workload tracking
        if (targetNode is ProcrastinationNode pn)
        {
            pn.IncrementWorkload();
            pn.IncrementPendingDeferrals();
        }

        // Record blame
        _blameTracker.RecordDeferral(targetNode.NodeId, "Accepted deferral");

        // Record diagnostics
        ClusterDiagnostics.RecordDeferralSubmitted(targetNode.NodeId);

        // Create message and send
        var message = ClusterMessage.Create(
            workload.CorrelationId,
            ClusterMessageType.DeferralSubmission,
            _localNode.NodeId,
            targetNode.NodeId,
            workload.Payload,
            _consensus.CurrentEpoch);

        await _transport.SendAsync(message, cancellationToken);
        ClusterDiagnostics.RecordMessageSent(ClusterMessageType.DeferralSubmission.ToString());

        // Create and return receipt
        var receipt = new DeferralReceipt(
            workload.WorkloadId,
            workload.CorrelationId,
            targetNode.NodeId,
            _timeProvider.GetUtcNow(),
            null) // Estimated completion time is always wrong, so we don't even try
        {
            DeferralCount = 0,
            NodeHistory = [_localNode.NodeId, targetNode.NodeId]
        };

        _logger.Info("Deferral {WorkloadId} submitted to node {NodeId}. Receipt created.",
            workload.WorkloadId, targetNode.NodeId);

        return receipt;
    }

    /// <inheritdoc />
    public ClusterHealthMetrics GetHealthMetrics()
    {
        var nodes = Nodes.ToList();
        var healthyNodes = nodes.Count(n => n.State == NodeState.Available || n.State == NodeState.Busy);
        var partitionedNodes = nodes.Count(n => n.State == NodeState.Partitioned);
        var zombieNodes = nodes.Count(n => n.State == NodeState.Zombie);
        var totalPendingDeferrals = nodes.Sum(n => n.PendingDeferrals);
        var avgWorkload = nodes.Count > 0 ? nodes.Average(n => n.CurrentWorkload) : 0;

        return new ClusterHealthMetrics
        {
            TotalNodes = nodes.Count,
            HealthyNodes = healthyNodes,
            PartitionedNodes = partitionedNodes,
            ZombieNodes = zombieNodes,
            TotalPendingDeferrals = totalPendingDeferrals,
            AverageWorkloadPerNode = avgWorkload,
            ClusterUptime = _timeProvider.GetUtcNow() - _startTime,
            LastTopologyChange = _timeProvider.GetUtcNow(), // Simplified
            BlameHeatmap = _blameTracker.GetHeatmap(),
            ConsensusEpoch = _consensus.CurrentEpoch
        };
    }

    /// <summary>
    /// Adds a remote node to the cluster.
    /// </summary>
    /// <param name="node">The node to add.</param>
    public void AddNode(IProcrastinationNode node)
    {
        if (_nodes.TryAdd(node.NodeId, node))
        {
            _logger.Info("Node {NodeId} added to cluster", node.NodeId);

            MembershipChanged?.Invoke(this, new ClusterMembershipChangedEventArgs(
                MembershipChangeType.NodeJoined,
                node,
                _timeProvider.GetUtcNow(),
                "Remote node discovered"));
        }
    }

    /// <summary>
    /// Removes a node from the cluster.
    /// </summary>
    /// <param name="nodeId">The node ID to remove.</param>
    public void RemoveNode(string nodeId)
    {
        if (_nodes.TryRemove(nodeId, out var node))
        {
            _logger.Info("Node {NodeId} removed from cluster", nodeId);

            MembershipChanged?.Invoke(this, new ClusterMembershipChangedEventArgs(
                MembershipChangeType.NodeLeft,
                node,
                _timeProvider.GetUtcNow(),
                "Node removed from cluster"));
        }
    }

    /// <summary>
    /// Marks a node as zombie.
    /// </summary>
    /// <param name="nodeId">The node ID to zombify.</param>
    public void ZombifyNode(string nodeId)
    {
        if (_nodes.TryGetValue(nodeId, out var node) && node is ProcrastinationNode pn)
        {
            pn.SetState(NodeState.Zombie);
            ClusterDiagnostics.RecordNodeZombified(nodeId);
            _blameTracker.RecordTimeout(nodeId);

            _logger.Warn("Node {NodeId} has been zombified", nodeId);

            MembershipChanged?.Invoke(this, new ClusterMembershipChangedEventArgs(
                MembershipChangeType.NodeZombified,
                node,
                _timeProvider.GetUtcNow(),
                "Node stopped responding but may still be procrastinating"));
        }
    }

    /// <summary>
    /// Gets the blame heatmap tracker.
    /// </summary>
    public BlameHeatmapTracker BlameTracker => _blameTracker;

    private void OnMessageReceived(object? sender, ClusterMessageReceivedEventArgs e)
    {
        ClusterDiagnostics.RecordMessageReceived(e.Message.MessageType.ToString());

        _logger.Debug("Received message {MessageType} from {SourceNodeId}",
            e.Message.MessageType, e.Message.SourceNodeId);

        // Handle different message types
        switch (e.Message.MessageType)
        {
            case ClusterMessageType.Heartbeat:
                HandleHeartbeat(e.Message);
                break;
            case ClusterMessageType.DeferralSubmission:
                HandleDeferralSubmission(e.Message);
                break;
            // Add more handlers as needed
        }
    }

    private void HandleHeartbeat(IClusterMessage message)
    {
        if (_nodes.TryGetValue(message.SourceNodeId, out var node) && node is ProcrastinationNode pn)
        {
            pn.RecordHeartbeat(message.Timestamp);
            _logger.Debug("Heartbeat received from node {NodeId}", message.SourceNodeId);
        }
    }

    private void HandleDeferralSubmission(IClusterMessage message)
    {
        using var activity = ClusterDiagnostics.StartDeferralProcessing(
            message.CorrelationId,
            message.MessageId,
            _localNode.NodeId);

        _localNode.IncrementWorkload();
        _localNode.IncrementPendingDeferrals();

        _logger.Info("Received deferral submission from {SourceNodeId}", message.SourceNodeId);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _transport.MessageReceived -= OnMessageReceived;
        _disposed = true;
    }
}
