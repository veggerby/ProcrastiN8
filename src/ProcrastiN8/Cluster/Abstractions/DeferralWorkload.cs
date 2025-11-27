namespace ProcrastiN8.Cluster.Abstractions;

/// <summary>
/// Represents a unit of procrastination work to be distributed across the cluster.
/// </summary>
/// <param name="WorkloadId">Unique identifier for this workload.</param>
/// <param name="CorrelationId">Correlation ID for distributed tracing.</param>
/// <param name="Priority">Priority level (lower = more important = will be deferred longer).</param>
/// <param name="InitialDelay">Initial delay before execution attempts begin.</param>
/// <param name="MaxDeferrals">Maximum number of times this workload can be deferred.</param>
/// <param name="CreatedAt">Timestamp when the workload was created.</param>
/// <param name="Payload">Serialized task payload.</param>
/// <param name="Tags">Optional metadata tags for blame tracking.</param>
public sealed record DeferralWorkload(
    Guid WorkloadId,
    Guid CorrelationId,
    int Priority,
    TimeSpan InitialDelay,
    int MaxDeferrals,
    DateTimeOffset CreatedAt,
    byte[]? Payload,
    IReadOnlyDictionary<string, string>? Tags = null)
{
    /// <summary>
    /// Creates a new deferral workload with default values.
    /// </summary>
    /// <param name="correlationId">Correlation ID for tracing.</param>
    /// <param name="initialDelay">Initial delay before execution.</param>
    /// <returns>A new deferral workload.</returns>
    public static DeferralWorkload Create(Guid correlationId, TimeSpan initialDelay)
    {
        return new DeferralWorkload(
            WorkloadId: Guid.NewGuid(),
            CorrelationId: correlationId,
            Priority: 0,
            InitialDelay: initialDelay,
            MaxDeferrals: int.MaxValue, // Infinite deferrals by default
            CreatedAt: DateTimeOffset.UtcNow,
            Payload: null,
            Tags: null);
    }
}
