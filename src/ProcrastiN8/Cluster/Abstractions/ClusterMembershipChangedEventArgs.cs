namespace ProcrastiN8.Cluster.Abstractions;

/// <summary>
/// Event arguments for cluster membership changes.
/// </summary>
public sealed class ClusterMembershipChangedEventArgs(
    MembershipChangeType changeType,
    IProcrastinationNode affectedNode,
    DateTimeOffset timestamp,
    string? reason = null) : EventArgs
{
    /// <summary>
    /// Gets the type of membership change.
    /// </summary>
    public MembershipChangeType ChangeType { get; } = changeType;

    /// <summary>
    /// Gets the node that was affected.
    /// </summary>
    public IProcrastinationNode AffectedNode { get; } = affectedNode;

    /// <summary>
    /// Gets the timestamp of the change.
    /// </summary>
    public DateTimeOffset Timestamp { get; } = timestamp;

    /// <summary>
    /// Gets the reason for the change (excuse).
    /// </summary>
    public string? Reason { get; } = reason;
}

/// <summary>
/// Types of cluster membership changes.
/// </summary>
public enum MembershipChangeType
{
    /// <summary>A new node joined the cluster.</summary>
    NodeJoined,

    /// <summary>A node left the cluster gracefully.</summary>
    NodeLeft,

    /// <summary>A node became partitioned from the cluster.</summary>
    NodePartitioned,

    /// <summary>A node became a zombie.</summary>
    NodeZombified,

    /// <summary>A node recovered from partition or zombie state.</summary>
    NodeRecovered,

    /// <summary>A node's state changed.</summary>
    NodeStateChanged
}
