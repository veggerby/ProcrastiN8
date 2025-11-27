using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Cluster.Consensus;

/// <summary>
/// The Global Moving Target Clock — a consensus protocol where the agreed-upon time
/// keeps shifting, ensuring no task ever runs when expected.
/// </summary>
/// <remarks>
/// This protocol is RAFT-ish in the sense that it involves votes and epochs,
/// but the target time drifts continuously based on cluster entropy.
/// Perfect for coordinated procrastination at scale.
/// </remarks>
public sealed class GlobalMovingTargetClock : IConsensusProtocol
{
    private readonly IRandomProvider _randomProvider;
    private readonly ITimeProvider _timeProvider;
    private readonly IProcrastiLogger _logger;
    private readonly object _lock = new();
    private readonly double _driftFactor;
    private readonly TimeSpan _minDrift;
    private readonly TimeSpan _maxDrift;

    private long _currentEpoch;
    private ConsensusState _state = ConsensusState.Idle;
    private string? _leaderNodeId;
    private string _localNodeId;
    private DateTimeOffset _lastTargetTime;
    private int _voteCount;
    private int _requiredVotes;

    /// <summary>
    /// Initializes a new Global Moving Target Clock.
    /// </summary>
    /// <param name="localNodeId">The local node's ID.</param>
    /// <param name="randomProvider">Random provider for drift calculations.</param>
    /// <param name="timeProvider">Time provider for clock operations.</param>
    /// <param name="logger">Logger for consensus events.</param>
    /// <param name="driftFactor">How much the target time drifts per epoch (0.0 to 1.0). Default is 0.15.</param>
    /// <param name="minDrift">Minimum drift amount. Default is 10ms.</param>
    /// <param name="maxDrift">Maximum drift amount. Default is 5 seconds.</param>
    public GlobalMovingTargetClock(
        string localNodeId,
        IRandomProvider? randomProvider = null,
        ITimeProvider? timeProvider = null,
        IProcrastiLogger? logger = null,
        double driftFactor = 0.15,
        TimeSpan? minDrift = null,
        TimeSpan? maxDrift = null)
    {
        _localNodeId = localNodeId;
        _randomProvider = randomProvider ?? RandomProvider.Default;
        _timeProvider = timeProvider ?? SystemTimeProvider.Default;
        _logger = logger ?? DefaultLogger.Instance;
        _driftFactor = Math.Clamp(driftFactor, 0.0, 1.0);
        _minDrift = minDrift ?? TimeSpan.FromMilliseconds(10);
        _maxDrift = maxDrift ?? TimeSpan.FromSeconds(5);
        _lastTargetTime = _timeProvider.GetUtcNow();
    }

    /// <inheritdoc />
    public long CurrentEpoch
    {
        get { lock (_lock) { return _currentEpoch; } }
    }

    /// <inheritdoc />
    public ConsensusState State
    {
        get { lock (_lock) { return _state; } }
    }

    /// <inheritdoc />
    public bool IsLeader
    {
        get { lock (_lock) { return _leaderNodeId == _localNodeId; } }
    }

    /// <inheritdoc />
    public string? LeaderNodeId
    {
        get { lock (_lock) { return _leaderNodeId; } }
    }

    /// <inheritdoc />
    public event EventHandler<EpochChangedEventArgs>? EpochChanged;

    /// <inheritdoc />
    public event EventHandler<LeaderElectedEventArgs>? LeaderElected;

    /// <inheritdoc />
    public Task<bool> ProposeTargetTimeAsync(DateTimeOffset proposedTime, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            if (_state != ConsensusState.Idle && _state != ConsensusState.Committed)
            {
                _logger.Warn("Cannot propose new target time: consensus round already in progress (state: {State})", _state);
                return Task.FromResult(false);
            }

            // Apply drift to the proposed time — it can never be exact
            var drift = CalculateDrift();
            var driftedTime = proposedTime.Add(drift);

            _state = ConsensusState.Proposing;
            _voteCount = 1; // Self-vote
            _lastTargetTime = driftedTime;

            _logger.Info("Proposing target time {ProposedTime} (drifted to {DriftedTime}) for epoch {Epoch}",
                proposedTime, driftedTime, _currentEpoch + 1);
        }

        // In a real implementation, this would broadcast to other nodes
        // For now, we auto-commit since we're the only node we trust
        return Task.FromResult(CommitProposal());
    }

    /// <inheritdoc />
    public Task VoteAsync(long proposalEpoch, bool accept, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            if (proposalEpoch != _currentEpoch + 1)
            {
                _logger.Warn("Ignoring vote for epoch {ProposalEpoch}: expected {ExpectedEpoch}",
                    proposalEpoch, _currentEpoch + 1);
                return Task.CompletedTask;
            }

            if (accept)
            {
                _voteCount++;
                _logger.Debug("Received positive vote. Total votes: {VoteCount}/{RequiredVotes}",
                    _voteCount, _requiredVotes);

                if (_voteCount >= _requiredVotes)
                {
                    CommitProposal();
                }
            }
            else
            {
                _logger.Debug("Received negative vote for epoch {Epoch}", proposalEpoch);
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<long> AdvanceEpochAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        long newEpoch;
        lock (_lock)
        {
            var oldEpoch = _currentEpoch;
            _currentEpoch++;
            newEpoch = _currentEpoch;
            _state = ConsensusState.Idle;

            // Apply additional drift on each epoch advance
            var drift = CalculateDrift();
            _lastTargetTime = _lastTargetTime.Add(drift);

            _logger.Info("Epoch advanced from {OldEpoch} to {NewEpoch}. Target time shifted by {Drift}ms",
                oldEpoch, newEpoch, drift.TotalMilliseconds);

            EpochChanged?.Invoke(this, new EpochChangedEventArgs(oldEpoch, newEpoch, "Epoch manually advanced"));
        }

        return Task.FromResult(newEpoch);
    }

    /// <summary>
    /// Gets the current target time (which keeps moving).
    /// </summary>
    /// <returns>The current target time.</returns>
    public DateTimeOffset GetCurrentTargetTime()
    {
        lock (_lock)
        {
            // Apply real-time drift based on elapsed time since last access
            var drift = CalculateDrift();
            _lastTargetTime = _lastTargetTime.Add(drift);
            return _lastTargetTime;
        }
    }

    /// <summary>
    /// Sets the required votes for consensus.
    /// </summary>
    /// <param name="clusterSize">The total cluster size.</param>
    public void SetClusterSize(int clusterSize)
    {
        lock (_lock)
        {
            _requiredVotes = (clusterSize / 2) + 1; // Simple majority
            _logger.Debug("Cluster size set to {ClusterSize}, requiring {RequiredVotes} votes for consensus",
                clusterSize, _requiredVotes);
        }
    }

    /// <summary>
    /// Attempts to become the leader.
    /// </summary>
    /// <returns>True if this node is now the leader.</returns>
    public bool TryBecomeLeader()
    {
        lock (_lock)
        {
            if (_leaderNodeId == null)
            {
                var previousLeader = _leaderNodeId;
                _leaderNodeId = _localNodeId;
                _logger.Info("Node {NodeId} has become the leader for epoch {Epoch}", _localNodeId, _currentEpoch);
                LeaderElected?.Invoke(this, new LeaderElectedEventArgs(previousLeader, _localNodeId, _currentEpoch));
                return true;
            }

            return _leaderNodeId == _localNodeId;
        }
    }

    private TimeSpan CalculateDrift()
    {
        // Drift is random but bounded
        var driftMs = _minDrift.TotalMilliseconds +
            (_randomProvider.GetDouble() * (_maxDrift.TotalMilliseconds - _minDrift.TotalMilliseconds) * _driftFactor);

        // Direction is also random
        if (_randomProvider.GetDouble() < 0.5)
        {
            driftMs = -driftMs;
        }

        return TimeSpan.FromMilliseconds(driftMs);
    }

    private bool CommitProposal()
    {
        var oldEpoch = _currentEpoch;
        _currentEpoch++;
        _state = ConsensusState.Committed;

        _logger.Info("Consensus reached for epoch {Epoch}. Target time: {TargetTime}",
            _currentEpoch, _lastTargetTime);

        EpochChanged?.Invoke(this, new EpochChangedEventArgs(oldEpoch, _currentEpoch, "Proposal committed"));

        return true;
    }
}
