namespace ProcrastiN8.Cluster.Transport;

/// <summary>
/// Default implementation of <see cref="IClusterMessage"/> for cluster communication.
/// </summary>
public sealed record ClusterMessage(
    Guid MessageId,
    Guid CorrelationId,
    ClusterMessageType MessageType,
    string SourceNodeId,
    string? TargetNodeId,
    DateTimeOffset Timestamp,
    byte[]? Payload,
    long Epoch) : IClusterMessage
{
    /// <summary>
    /// Creates a new cluster message with auto-generated ID and current timestamp.
    /// </summary>
    /// <param name="correlationId">Correlation ID for tracing.</param>
    /// <param name="messageType">Type of message.</param>
    /// <param name="sourceNodeId">Source node ID.</param>
    /// <param name="targetNodeId">Target node ID (null for broadcast).</param>
    /// <param name="payload">Message payload.</param>
    /// <param name="epoch">Current consensus epoch.</param>
    /// <returns>A new cluster message.</returns>
    public static ClusterMessage Create(
        Guid correlationId,
        ClusterMessageType messageType,
        string sourceNodeId,
        string? targetNodeId = null,
        byte[]? payload = null,
        long epoch = 0)
    {
        return new ClusterMessage(
            MessageId: Guid.NewGuid(),
            CorrelationId: correlationId,
            MessageType: messageType,
            SourceNodeId: sourceNodeId,
            TargetNodeId: targetNodeId,
            Timestamp: DateTimeOffset.UtcNow,
            Payload: payload,
            Epoch: epoch);
    }

    /// <summary>
    /// Creates a heartbeat message.
    /// </summary>
    /// <param name="sourceNodeId">Source node ID.</param>
    /// <param name="epoch">Current consensus epoch.</param>
    /// <returns>A heartbeat message.</returns>
    public static ClusterMessage Heartbeat(string sourceNodeId, long epoch = 0)
    {
        return Create(
            correlationId: Guid.NewGuid(),
            messageType: ClusterMessageType.Heartbeat,
            sourceNodeId: sourceNodeId,
            epoch: epoch);
    }
}
