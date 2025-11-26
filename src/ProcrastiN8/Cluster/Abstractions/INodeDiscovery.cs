namespace ProcrastiN8.Cluster.Abstractions;

/// <summary>
/// Provides node discovery services for the procrastination cluster.
/// Discovers nodes in a manner that maximizes delay while maintaining plausible deniability.
/// </summary>
public interface INodeDiscovery
{
    /// <summary>
    /// Discovers available nodes in the cluster.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of discovered node endpoints.</returns>
    Task<IReadOnlyCollection<string>> DiscoverNodesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Announces this node's presence to the cluster.
    /// </summary>
    /// <param name="node">The node to announce.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the announcement operation.</returns>
    Task AnnounceAsync(IProcrastinationNode node, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deregisters this node from the discovery service.
    /// </summary>
    /// <param name="nodeId">The node ID to deregister.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the deregistration operation.</returns>
    Task DeregisterAsync(string nodeId, CancellationToken cancellationToken = default);
}
