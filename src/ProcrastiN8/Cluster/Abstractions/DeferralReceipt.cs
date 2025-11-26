namespace ProcrastiN8.Cluster.Abstractions;

/// <summary>
/// Represents a receipt for a submitted deferral workload.
/// Provides tracking information and blame assignment capabilities.
/// </summary>
/// <param name="WorkloadId">The workload identifier.</param>
/// <param name="CorrelationId">Correlation ID for distributed tracing.</param>
/// <param name="AssignedNodeId">The node assigned to procrastinate on this workload.</param>
/// <param name="SubmittedAt">When the workload was submitted.</param>
/// <param name="EstimatedCompletionTime">Estimated completion time (always wrong).</param>
public sealed record DeferralReceipt(
    Guid WorkloadId,
    Guid CorrelationId,
    string AssignedNodeId,
    DateTimeOffset SubmittedAt,
    DateTimeOffset? EstimatedCompletionTime)
{
    /// <summary>
    /// Gets the number of times this workload has been deferred so far.
    /// </summary>
    public int DeferralCount { get; init; }

    /// <summary>
    /// Gets the nodes that have handled this workload (for blame tracking).
    /// </summary>
    public IReadOnlyList<string> NodeHistory { get; init; } = [];
}
