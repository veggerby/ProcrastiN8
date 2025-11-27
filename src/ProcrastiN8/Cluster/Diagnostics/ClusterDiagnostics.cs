using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ProcrastiN8.Cluster.Diagnostics;

/// <summary>
/// Diagnostics and metrics surface for the distributed procrastination cluster.
/// Provides OpenTelemetry-compatible instrumentation for blame assignment and observability.
/// </summary>
public static class ClusterDiagnostics
{
    /// <summary>
    /// Activity source name for distributed tracing.
    /// </summary>
    public const string ActivitySourceName = "ProcrastiN8.Cluster";

    /// <summary>
    /// Meter name for cluster metrics.
    /// </summary>
    public const string MeterName = "ProcrastiN8.Cluster";

    private static readonly ActivitySource _activitySource = new(ActivitySourceName);
    private static readonly Meter _meter = new(MeterName, "1.0.0");

    // Counters
    private static readonly Counter<int> _deferralsSubmitted = _meter.CreateCounter<int>("cluster.deferrals.submitted");
    private static readonly Counter<int> _deferralsCompleted = _meter.CreateCounter<int>("cluster.deferrals.completed");
    private static readonly Counter<int> _deferralsMigrated = _meter.CreateCounter<int>("cluster.deferrals.migrated");
    private static readonly Counter<int> _messagesSent = _meter.CreateCounter<int>("cluster.messages.sent");
    private static readonly Counter<int> _messagesReceived = _meter.CreateCounter<int>("cluster.messages.received");
    private static readonly Counter<int> _messagesDropped = _meter.CreateCounter<int>("cluster.messages.dropped");
    private static readonly Counter<int> _heartbeatsSent = _meter.CreateCounter<int>("cluster.heartbeats.sent");
    private static readonly Counter<int> _heartbeatsMissed = _meter.CreateCounter<int>("cluster.heartbeats.missed");
    private static readonly Counter<int> _consensusRounds = _meter.CreateCounter<int>("cluster.consensus.rounds");
    private static readonly Counter<int> _consensusFailures = _meter.CreateCounter<int>("cluster.consensus.failures");
    private static readonly Counter<int> _leaderElections = _meter.CreateCounter<int>("cluster.leader.elections");
    private static readonly Counter<int> _nodeJoins = _meter.CreateCounter<int>("cluster.nodes.joined");
    private static readonly Counter<int> _nodeLeaves = _meter.CreateCounter<int>("cluster.nodes.left");
    private static readonly Counter<int> _nodeZombifications = _meter.CreateCounter<int>("cluster.nodes.zombified");

    /// <summary>
    /// Starts a new activity for deferral submission.
    /// </summary>
    /// <param name="correlationId">Correlation ID for tracing.</param>
    /// <param name="workloadId">Workload ID.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartDeferralSubmission(Guid correlationId, Guid workloadId)
    {
        var activity = _activitySource.StartActivity("DeferralSubmission", ActivityKind.Producer);
        activity?.SetTag("cluster.correlation_id", correlationId.ToString());
        activity?.SetTag("cluster.workload_id", workloadId.ToString());
        return activity;
    }

    /// <summary>
    /// Starts a new activity for deferral processing.
    /// </summary>
    /// <param name="correlationId">Correlation ID for tracing.</param>
    /// <param name="workloadId">Workload ID.</param>
    /// <param name="nodeId">Processing node ID.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartDeferralProcessing(Guid correlationId, Guid workloadId, string nodeId)
    {
        var activity = _activitySource.StartActivity("DeferralProcessing", ActivityKind.Consumer);
        activity?.SetTag("cluster.correlation_id", correlationId.ToString());
        activity?.SetTag("cluster.workload_id", workloadId.ToString());
        activity?.SetTag("cluster.node_id", nodeId);
        return activity;
    }

    /// <summary>
    /// Starts a new activity for consensus operations.
    /// </summary>
    /// <param name="epoch">Current epoch.</param>
    /// <param name="operation">Consensus operation name.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartConsensusOperation(long epoch, string operation)
    {
        var activity = _activitySource.StartActivity($"Consensus.{operation}", ActivityKind.Internal);
        activity?.SetTag("cluster.epoch", epoch);
        activity?.SetTag("cluster.consensus_operation", operation);
        return activity;
    }

    /// <summary>
    /// Records a deferral submission.
    /// </summary>
    /// <param name="nodeId">Submitting node ID.</param>
    public static void RecordDeferralSubmitted(string nodeId)
    {
        _deferralsSubmitted.Add(1, new KeyValuePair<string, object?>("node_id", nodeId));
    }

    /// <summary>
    /// Records a deferral completion.
    /// </summary>
    /// <param name="nodeId">Completing node ID.</param>
    public static void RecordDeferralCompleted(string nodeId)
    {
        _deferralsCompleted.Add(1, new KeyValuePair<string, object?>("node_id", nodeId));
    }

    /// <summary>
    /// Records a deferral migration.
    /// </summary>
    /// <param name="fromNodeId">Source node ID.</param>
    /// <param name="toNodeId">Destination node ID.</param>
    public static void RecordDeferralMigrated(string fromNodeId, string toNodeId)
    {
        _deferralsMigrated.Add(1,
            new KeyValuePair<string, object?>("from_node_id", fromNodeId),
            new KeyValuePair<string, object?>("to_node_id", toNodeId));
    }

    /// <summary>
    /// Records a message sent.
    /// </summary>
    /// <param name="messageType">Type of message.</param>
    public static void RecordMessageSent(string messageType)
    {
        _messagesSent.Add(1, new KeyValuePair<string, object?>("message_type", messageType));
    }

    /// <summary>
    /// Records a message received.
    /// </summary>
    /// <param name="messageType">Type of message.</param>
    public static void RecordMessageReceived(string messageType)
    {
        _messagesReceived.Add(1, new KeyValuePair<string, object?>("message_type", messageType));
    }

    /// <summary>
    /// Records a dropped message.
    /// </summary>
    /// <param name="reason">Reason for dropping.</param>
    public static void RecordMessageDropped(string reason)
    {
        _messagesDropped.Add(1, new KeyValuePair<string, object?>("reason", reason));
    }

    /// <summary>
    /// Records a heartbeat sent.
    /// </summary>
    /// <param name="nodeId">Sending node ID.</param>
    public static void RecordHeartbeatSent(string nodeId)
    {
        _heartbeatsSent.Add(1, new KeyValuePair<string, object?>("node_id", nodeId));
    }

    /// <summary>
    /// Records a missed heartbeat.
    /// </summary>
    /// <param name="nodeId">Node that missed heartbeat.</param>
    public static void RecordHeartbeatMissed(string nodeId)
    {
        _heartbeatsMissed.Add(1, new KeyValuePair<string, object?>("node_id", nodeId));
    }

    /// <summary>
    /// Records a consensus round.
    /// </summary>
    /// <param name="epoch">Epoch of the round.</param>
    /// <param name="successful">Whether consensus was reached.</param>
    public static void RecordConsensusRound(long epoch, bool successful)
    {
        _consensusRounds.Add(1,
            new KeyValuePair<string, object?>("epoch", epoch),
            new KeyValuePair<string, object?>("successful", successful));

        if (!successful)
        {
            _consensusFailures.Add(1, new KeyValuePair<string, object?>("epoch", epoch));
        }
    }

    /// <summary>
    /// Records a leader election.
    /// </summary>
    /// <param name="newLeaderId">New leader's node ID.</param>
    /// <param name="epoch">Election epoch.</param>
    public static void RecordLeaderElection(string newLeaderId, long epoch)
    {
        _leaderElections.Add(1,
            new KeyValuePair<string, object?>("leader_id", newLeaderId),
            new KeyValuePair<string, object?>("epoch", epoch));
    }

    /// <summary>
    /// Records a node join.
    /// </summary>
    /// <param name="nodeId">Joining node ID.</param>
    public static void RecordNodeJoin(string nodeId)
    {
        _nodeJoins.Add(1, new KeyValuePair<string, object?>("node_id", nodeId));
    }

    /// <summary>
    /// Records a node leave.
    /// </summary>
    /// <param name="nodeId">Leaving node ID.</param>
    public static void RecordNodeLeave(string nodeId)
    {
        _nodeLeaves.Add(1, new KeyValuePair<string, object?>("node_id", nodeId));
    }

    /// <summary>
    /// Records a node zombification.
    /// </summary>
    /// <param name="nodeId">Zombified node ID.</param>
    public static void RecordNodeZombified(string nodeId)
    {
        _nodeZombifications.Add(1, new KeyValuePair<string, object?>("node_id", nodeId));
    }
}
