namespace ProcrastiN8.Cluster.Abstractions;

/// <summary>
/// Represents the lifecycle state of a procrastination node within the cluster.
/// States are intentionally pessimistic — availability is merely a theoretical maximum.
/// </summary>
public enum NodeState
{
    /// <summary>Node is starting up and is not yet accepting workloads.</summary>
    Initializing,

    /// <summary>Node is technically available but prefer to send work elsewhere.</summary>
    Available,

    /// <summary>Node is busy pretending to work on something else.</summary>
    Busy,

    /// <summary>Node has been partitioned from the cluster — may or may not still be procrastinating.</summary>
    Partitioned,

    /// <summary>Node is a zombie — unresponsive but still procrastinating on previously assigned tasks.</summary>
    Zombie,

    /// <summary>Node has gracefully shut down and handed off its workload to another procrastinator.</summary>
    Shutdown
}
