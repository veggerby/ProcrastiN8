using System.Collections.Concurrent;

namespace ProcrastiN8.Cluster.Transport;

/// <summary>
/// In-memory transport for testing and local cluster scenarios.
/// Messages are delivered instantly through shared memory — no actual network involved.
/// </summary>
/// <remarks>
/// For point-to-point messaging, TargetNodeId should match the endpoint string used
/// when connecting (e.g., use the same value for both nodeId and endpoint in tests).
/// In production scenarios with separate node IDs and endpoints, use the endpoint
/// value in TargetNodeId or register a node-to-endpoint mapping.
/// </remarks>
public sealed class InMemoryClusterTransport : IClusterTransport
{
    private static readonly ConcurrentDictionary<string, InMemoryClusterTransport> _endpoints = new();
    private static readonly ConcurrentDictionary<string, string> _nodeIdToEndpoint = new();
    private readonly ConcurrentQueue<IClusterMessage> _pendingMessages = new();
    private string? _localEndpoint;
    private bool _isConnected;

    /// <inheritdoc />
    public string TransportName => "InMemory";

    /// <inheritdoc />
    public bool IsConnected => _isConnected;

    /// <inheritdoc />
    public event EventHandler<ClusterMessageReceivedEventArgs>? MessageReceived;

    /// <inheritdoc />
    public event EventHandler<TransportConnectionChangedEventArgs>? ConnectionStateChanged;

    /// <inheritdoc />
    public Task ConnectAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _localEndpoint = endpoint;
        _endpoints[endpoint] = this;
        _isConnected = true;

        ConnectionStateChanged?.Invoke(this, new TransportConnectionChangedEventArgs(true, "Connected to in-memory transport"));

        return Task.CompletedTask;
    }

    /// <summary>
    /// Connects with both an endpoint and a node ID for proper routing.
    /// </summary>
    /// <param name="endpoint">The endpoint address.</param>
    /// <param name="nodeId">The node identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task ConnectAsync(string endpoint, string nodeId, CancellationToken cancellationToken = default)
    {
        _nodeIdToEndpoint[nodeId] = endpoint;
        return ConnectAsync(endpoint, cancellationToken);
    }

    /// <inheritdoc />
    public Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (_localEndpoint != null)
        {
            _endpoints.TryRemove(_localEndpoint, out _);

            // Remove any node ID mappings to this endpoint
            foreach (var kvp in _nodeIdToEndpoint)
            {
                if (kvp.Value == _localEndpoint)
                {
                    _nodeIdToEndpoint.TryRemove(kvp.Key, out _);
                }
            }
        }

        _isConnected = false;
        ConnectionStateChanged?.Invoke(this, new TransportConnectionChangedEventArgs(false, "Disconnected from in-memory transport"));

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SendAsync(IClusterMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_isConnected)
        {
            throw new InvalidOperationException("Transport is not connected.");
        }

        if (message.TargetNodeId != null)
        {
            // First try to look up the node ID in the mapping
            var targetEndpoint = _nodeIdToEndpoint.TryGetValue(message.TargetNodeId, out var endpoint)
                ? endpoint
                : message.TargetNodeId; // Fall back to using TargetNodeId as endpoint

            if (_endpoints.TryGetValue(targetEndpoint, out var targetTransport))
            {
                targetTransport.DeliverMessage(message);
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task BroadcastAsync(IClusterMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_isConnected)
        {
            throw new InvalidOperationException("Transport is not connected.");
        }

        foreach (var kvp in _endpoints)
        {
            if (kvp.Key != _localEndpoint)
            {
                kvp.Value.DeliverMessage(message);
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Delivers a message to this transport instance.
    /// </summary>
    /// <param name="message">The message to deliver.</param>
    internal void DeliverMessage(IClusterMessage message)
    {
        _pendingMessages.Enqueue(message);
        MessageReceived?.Invoke(this, new ClusterMessageReceivedEventArgs(message));
    }

    /// <summary>
    /// Gets the number of connected endpoints in the in-memory transport.
    /// Useful for testing cluster membership.
    /// </summary>
    public static int ConnectedEndpointCount => _endpoints.Count;

    /// <summary>
    /// Clears all connected endpoints and node mappings. Useful for test cleanup.
    /// </summary>
    public static void ClearAllEndpoints()
    {
        _endpoints.Clear();
        _nodeIdToEndpoint.Clear();
    }
}
