namespace ProcrastiN8.Cluster.Abstractions;

/// <summary>
/// Represents cluster health metrics for observability and blame assignment.
/// </summary>
public sealed record ClusterHealthMetrics
{
    /// <summary>
    /// Gets the total number of nodes in the cluster.
    /// </summary>
    public int TotalNodes { get; init; }

    /// <summary>
    /// Gets the number of healthy (available) nodes.
    /// </summary>
    public int HealthyNodes { get; init; }

    /// <summary>
    /// Gets the number of partitioned nodes.
    /// </summary>
    public int PartitionedNodes { get; init; }

    /// <summary>
    /// Gets the number of zombie nodes.
    /// </summary>
    public int ZombieNodes { get; init; }

    /// <summary>
    /// Gets the total pending deferrals across the cluster.
    /// </summary>
    public int TotalPendingDeferrals { get; init; }

    /// <summary>
    /// Gets the average workload per node.
    /// </summary>
    public double AverageWorkloadPerNode { get; init; }

    /// <summary>
    /// Gets the cluster uptime.
    /// </summary>
    public TimeSpan ClusterUptime { get; init; }

    /// <summary>
    /// Gets the last time the cluster topology changed.
    /// </summary>
    public DateTimeOffset LastTopologyChange { get; init; }

    /// <summary>
    /// Gets the blame heatmap — which nodes have deferred the most work.
    /// </summary>
    public IReadOnlyDictionary<string, int> BlameHeatmap { get; init; } = new Dictionary<string, int>();

    /// <summary>
    /// Gets the current consensus epoch (for Global Moving Target Clock).
    /// </summary>
    public long ConsensusEpoch { get; init; }
}
