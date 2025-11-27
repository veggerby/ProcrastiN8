namespace ProcrastiN8.Cluster.Transport;

/// <summary>
/// Represents a message sent between nodes in the procrastination cluster.
/// </summary>
public interface IClusterMessage
{
    /// <summary>
    /// Gets the unique message identifier.
    /// </summary>
    Guid MessageId { get; }

    /// <summary>
    /// Gets the correlation ID for distributed tracing.
    /// </summary>
    Guid CorrelationId { get; }

    /// <summary>
    /// Gets the type of this message.
    /// </summary>
    ClusterMessageType MessageType { get; }

    /// <summary>
    /// Gets the source node ID.
    /// </summary>
    string SourceNodeId { get; }

    /// <summary>
    /// Gets the target node ID (null for broadcast).
    /// </summary>
    string? TargetNodeId { get; }

    /// <summary>
    /// Gets the message timestamp.
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the message payload.
    /// </summary>
    byte[]? Payload { get; }

    /// <summary>
    /// Gets the current consensus epoch.
    /// </summary>
    long Epoch { get; }
}
