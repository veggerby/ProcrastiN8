namespace ProcrastiN8.Cluster.Abstractions;

/// <summary>
/// Represents a distributed procrastination cluster capable of coordinating deferrals
/// across multiple nodes for enterprise-grade inefficiency.
/// </summary>
public interface IProcrastinationCluster
{
    /// <summary>
    /// Gets the local node in this cluster.
    /// </summary>
    IProcrastinationNode LocalNode { get; }

    /// <summary>
    /// Gets all known nodes in the cluster, including zombies and partitioned nodes.
    /// </summary>
    IReadOnlyCollection<IProcrastinationNode> Nodes { get; }

    /// <summary>
    /// Gets the node selector used for load balancing (prefers least available).
    /// </summary>
    INodeSelector NodeSelector { get; }

    /// <summary>
    /// Joins the cluster and begins participating in deferral coordination.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the join operation.</returns>
    Task JoinAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Leaves the cluster gracefully, migrating pending deferrals to other nodes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the leave operation.</returns>
    Task LeaveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Submits a deferral workload to the cluster for distributed procrastination.
    /// </summary>
    /// <param name="workload">The deferral workload to submit.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the submission operation.</returns>
    Task<DeferralReceipt> SubmitDeferralAsync(DeferralWorkload workload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current cluster health metrics.
    /// </summary>
    ClusterHealthMetrics GetHealthMetrics();

    /// <summary>
    /// Event raised when cluster membership changes.
    /// </summary>
    event EventHandler<ClusterMembershipChangedEventArgs>? MembershipChanged;
}
