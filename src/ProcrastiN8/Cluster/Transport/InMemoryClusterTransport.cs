using System.Collections.Concurrent;

namespace ProcrastiN8.Cluster.Transport;

/// <summary>
/// In-memory transport for testing and local cluster scenarios.
/// Messages are delivered instantly through shared memory — no actual network involved.
/// </summary>
public sealed class InMemoryClusterTransport : IClusterTransport
{
    private static readonly ConcurrentDictionary<string, InMemoryClusterTransport> _endpoints = new();
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

    /// <inheritdoc />
    public Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (_localEndpoint != null)
        {
            _endpoints.TryRemove(_localEndpoint, out _);
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

        if (message.TargetNodeId != null && _endpoints.TryGetValue(message.TargetNodeId, out var targetTransport))
        {
            targetTransport.DeliverMessage(message);
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
    /// Clears all connected endpoints. Useful for test cleanup.
    /// </summary>
    public static void ClearAllEndpoints()
    {
        _endpoints.Clear();
    }
}
