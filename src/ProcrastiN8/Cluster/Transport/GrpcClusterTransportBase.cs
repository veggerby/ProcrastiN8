namespace ProcrastiN8.Cluster.Transport;

/// <summary>
/// Placeholder for gRPC-based cluster transport.
/// This abstract class defines the contract for gRPC implementations without
/// introducing any runtime dependencies to the core library.
/// </summary>
/// <remarks>
/// To use gRPC transport, implement a concrete class in a separate assembly
/// with the actual gRPC dependencies. This keeps the core ProcrastiN8 library
/// dependency-free while allowing enterprise-grade RPC when needed.
/// </remarks>
public abstract class GrpcClusterTransportBase : IClusterTransport
{
    /// <inheritdoc />
    public abstract string TransportName { get; }

    /// <inheritdoc />
    public abstract bool IsConnected { get; }

    /// <inheritdoc />
    public abstract event EventHandler<ClusterMessageReceivedEventArgs>? MessageReceived;

    /// <inheritdoc />
    public abstract event EventHandler<TransportConnectionChangedEventArgs>? ConnectionStateChanged;

    /// <inheritdoc />
    public abstract Task ConnectAsync(string endpoint, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task DisconnectAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task SendAsync(IClusterMessage message, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task BroadcastAsync(IClusterMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the gRPC channel options to use for this transport.
    /// Implementations should override this to provide custom channel configuration.
    /// </summary>
    /// <returns>A dictionary of channel options.</returns>
    protected virtual IDictionary<string, object> GetChannelOptions()
    {
        return new Dictionary<string, object>
        {
            ["grpc.keepalive_time_ms"] = 30000,
            ["grpc.keepalive_timeout_ms"] = 10000,
            ["grpc.max_receive_message_length"] = 4 * 1024 * 1024 // 4MB
        };
    }

    /// <summary>
    /// Serializes a cluster message for gRPC transmission.
    /// Implementations should override this to provide custom serialization.
    /// </summary>
    /// <param name="message">The message to serialize.</param>
    /// <returns>The serialized message bytes.</returns>
    protected virtual byte[] SerializeMessage(IClusterMessage message)
    {
        // Default implementation uses simple concatenation
        // Real implementations should use protobuf or similar
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write(message.MessageId.ToByteArray());
        writer.Write(message.CorrelationId.ToByteArray());
        writer.Write((int)message.MessageType);
        writer.Write(message.SourceNodeId);
        writer.Write(message.TargetNodeId ?? string.Empty);
        writer.Write(message.Timestamp.ToUnixTimeMilliseconds());
        writer.Write(message.Payload?.Length ?? 0);
        if (message.Payload != null)
        {
            writer.Write(message.Payload);
        }
        writer.Write(message.Epoch);

        return ms.ToArray();
    }

    /// <summary>
    /// Deserializes a cluster message from gRPC transmission.
    /// Implementations should override this to provide custom deserialization.
    /// </summary>
    /// <param name="data">The serialized message bytes.</param>
    /// <returns>The deserialized message.</returns>
    protected virtual IClusterMessage DeserializeMessage(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);

        var messageId = new Guid(reader.ReadBytes(16));
        var correlationId = new Guid(reader.ReadBytes(16));
        var messageType = (ClusterMessageType)reader.ReadInt32();
        var sourceNodeId = reader.ReadString();
        var targetNodeId = reader.ReadString();
        if (string.IsNullOrEmpty(targetNodeId))
        {
            targetNodeId = null;
        }
        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(reader.ReadInt64());
        var payloadLength = reader.ReadInt32();
        byte[]? payload = null;
        if (payloadLength > 0)
        {
            payload = reader.ReadBytes(payloadLength);
        }
        var epoch = reader.ReadInt64();

        return new ClusterMessage(
            messageId,
            correlationId,
            messageType,
            sourceNodeId,
            targetNodeId,
            timestamp,
            payload,
            epoch);
    }
}
