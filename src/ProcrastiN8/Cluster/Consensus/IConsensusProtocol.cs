namespace ProcrastiN8.Cluster.Consensus;

/// <summary>
/// Abstraction for consensus protocols in the distributed procrastination cluster.
/// </summary>
public interface IConsensusProtocol
{
    /// <summary>
    /// Gets the current consensus epoch.
    /// </summary>
    long CurrentEpoch { get; }

    /// <summary>
    /// Gets the current consensus state.
    /// </summary>
    ConsensusState State { get; }

    /// <summary>
    /// Gets whether this node is currently the leader.
    /// </summary>
    bool IsLeader { get; }

    /// <summary>
    /// Gets the current leader's node ID (null if no leader).
    /// </summary>
    string? LeaderNodeId { get; }

    /// <summary>
    /// Proposes a new deferral target time to the cluster.
    /// </summary>
    /// <param name="proposedTime">The proposed deferral target time.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if consensus was reached, false otherwise.</returns>
    Task<bool> ProposeTargetTimeAsync(DateTimeOffset proposedTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Votes on a proposed deferral target time.
    /// </summary>
    /// <param name="proposalEpoch">The epoch of the proposal.</param>
    /// <param name="accept">Whether to accept or reject the proposal.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the vote operation.</returns>
    Task VoteAsync(long proposalEpoch, bool accept, CancellationToken cancellationToken = default);

    /// <summary>
    /// Advances the consensus epoch (typically after a successful deferral).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The new epoch value.</returns>
    Task<long> AdvanceEpochAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Event raised when the epoch changes.
    /// </summary>
    event EventHandler<EpochChangedEventArgs>? EpochChanged;

    /// <summary>
    /// Event raised when a new leader is elected.
    /// </summary>
    event EventHandler<LeaderElectedEventArgs>? LeaderElected;
}

/// <summary>
/// Event arguments for epoch changes.
/// </summary>
public sealed class EpochChangedEventArgs(long oldEpoch, long newEpoch, string reason) : EventArgs
{
    /// <summary>Gets the previous epoch.</summary>
    public long OldEpoch { get; } = oldEpoch;

    /// <summary>Gets the new epoch.</summary>
    public long NewEpoch { get; } = newEpoch;

    /// <summary>Gets the reason for the epoch change.</summary>
    public string Reason { get; } = reason;
}

/// <summary>
/// Event arguments for leader election.
/// </summary>
public sealed class LeaderElectedEventArgs(string? previousLeaderId, string newLeaderId, long epoch) : EventArgs
{
    /// <summary>Gets the previous leader's node ID (null if none).</summary>
    public string? PreviousLeaderId { get; } = previousLeaderId;

    /// <summary>Gets the new leader's node ID.</summary>
    public string NewLeaderId { get; } = newLeaderId;

    /// <summary>Gets the epoch in which the election occurred.</summary>
    public long Epoch { get; } = epoch;
}
