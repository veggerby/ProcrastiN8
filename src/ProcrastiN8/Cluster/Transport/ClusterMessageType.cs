namespace ProcrastiN8.Cluster.Transport;

/// <summary>
/// Types of cluster messages for transport.
/// </summary>
public enum ClusterMessageType
{
    /// <summary>Heartbeat message to indicate node is alive (somewhat).</summary>
    Heartbeat,

    /// <summary>Deferral workload submission.</summary>
    DeferralSubmission,

    /// <summary>Deferral workload acknowledgment.</summary>
    DeferralAcknowledgment,

    /// <summary>Workload migration request.</summary>
    WorkloadMigration,

    /// <summary>Consensus vote for Global Moving Target Clock.</summary>
    ConsensusVote,

    /// <summary>Consensus proposal.</summary>
    ConsensusProposal,

    /// <summary>Node discovery announcement.</summary>
    DiscoveryAnnouncement,

    /// <summary>Node shutdown notification.</summary>
    ShutdownNotification,

    /// <summary>Blame assignment message.</summary>
    BlameAssignment
}
