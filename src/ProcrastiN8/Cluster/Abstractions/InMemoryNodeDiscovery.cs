using System.Collections.Concurrent;

namespace ProcrastiN8.Cluster.Abstractions;

/// <summary>
/// In-memory node discovery implementation for testing and local cluster scenarios.
/// </summary>
public sealed class InMemoryNodeDiscovery : INodeDiscovery
{
    private static readonly ConcurrentDictionary<string, IProcrastinationNode> _registeredNodes = new();
    private readonly IProcrastiLogger _logger;

    /// <summary>
    /// Initializes a new in-memory node discovery service.
    /// </summary>
    /// <param name="logger">Logger for discovery events.</param>
    public InMemoryNodeDiscovery(IProcrastiLogger? logger = null)
    {
        _logger = logger ?? DefaultLogger.Instance;
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<string>> DiscoverNodesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var endpoints = _registeredNodes.Values
            .Select(n => n.Endpoint)
            .ToList();

        _logger.Debug("Discovered {Count} nodes in the cluster", endpoints.Count);

        return Task.FromResult<IReadOnlyCollection<string>>(endpoints);
    }

    /// <inheritdoc />
    public Task AnnounceAsync(IProcrastinationNode node, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _registeredNodes[node.NodeId] = node;
        _logger.Info("Node {NodeId} announced at endpoint {Endpoint}", node.NodeId, node.Endpoint);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeregisterAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_registeredNodes.TryRemove(nodeId, out var node))
        {
            _logger.Info("Node {NodeId} deregistered from endpoint {Endpoint}", nodeId, node.Endpoint);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets all registered nodes (for testing purposes).
    /// </summary>
    /// <returns>Collection of registered nodes.</returns>
    public IReadOnlyCollection<IProcrastinationNode> GetRegisteredNodes()
    {
        return _registeredNodes.Values.ToList();
    }

    /// <summary>
    /// Clears all registered nodes (for testing purposes).
    /// </summary>
    public static void ClearAllNodes()
    {
        _registeredNodes.Clear();
    }
}
