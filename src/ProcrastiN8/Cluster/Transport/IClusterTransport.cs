namespace ProcrastiN8.Cluster.Transport;

/// <summary>
/// Abstraction for cluster communication transport.
/// Implementations may include in-memory, gRPC, or carrier pigeon protocols.
/// </summary>
public interface IClusterTransport
{
    /// <summary>
    /// Gets the name of this transport implementation.
    /// </summary>
    string TransportName { get; }

    /// <summary>
    /// Gets whether this transport is currently connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Connects to the cluster transport layer.
    /// </summary>
    /// <param name="endpoint">The local endpoint to bind to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the connection operation.</returns>
    Task ConnectAsync(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects from the cluster transport layer.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the disconnection operation.</returns>
    Task DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a message to a specific node.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the send operation.</returns>
    Task SendAsync(IClusterMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Broadcasts a message to all nodes in the cluster.
    /// </summary>
    /// <param name="message">The message to broadcast.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the broadcast operation.</returns>
    Task BroadcastAsync(IClusterMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Event raised when a message is received.
    /// </summary>
    event EventHandler<ClusterMessageReceivedEventArgs>? MessageReceived;

    /// <summary>
    /// Event raised when the transport connection state changes.
    /// </summary>
    event EventHandler<TransportConnectionChangedEventArgs>? ConnectionStateChanged;
}

/// <summary>
/// Event arguments for received cluster messages.
/// </summary>
public sealed class ClusterMessageReceivedEventArgs(IClusterMessage message) : EventArgs
{
    /// <summary>
    /// Gets the received message.
    /// </summary>
    public IClusterMessage Message { get; } = message;
}

/// <summary>
/// Event arguments for transport connection state changes.
/// </summary>
public sealed class TransportConnectionChangedEventArgs(bool isConnected, string? reason = null) : EventArgs
{
    /// <summary>
    /// Gets whether the transport is now connected.
    /// </summary>
    public bool IsConnected { get; } = isConnected;

    /// <summary>
    /// Gets the reason for the state change.
    /// </summary>
    public string? Reason { get; } = reason;
}
