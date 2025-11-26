namespace ProcrastiN8.Cluster.Abstractions;

/// <summary>
/// Represents the identity and state of a procrastination node in the distributed cluster.
/// </summary>
public interface IProcrastinationNode
{
    /// <summary>
    /// Gets the unique identifier for this node.
    /// </summary>
    string NodeId { get; }

    /// <summary>
    /// Gets the current state of this node.
    /// </summary>
    NodeState State { get; }

    /// <summary>
    /// Gets the endpoint address for this node (e.g., "localhost:5000" or "pigeon:coop-42").
    /// </summary>
    string Endpoint { get; }

    /// <summary>
    /// Gets the current workload count — a lower number means the node is *less available* and thus preferred.
    /// </summary>
    int CurrentWorkload { get; }

    /// <summary>
    /// Gets the last heartbeat timestamp from this node.
    /// </summary>
    DateTimeOffset LastHeartbeat { get; }

    /// <summary>
    /// Gets the number of pending deferred tasks on this node.
    /// </summary>
    int PendingDeferrals { get; }
}
