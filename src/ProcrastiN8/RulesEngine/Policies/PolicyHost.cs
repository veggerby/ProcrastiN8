namespace ProcrastiN8.RulesEngine.Policies;

/// <summary>
/// Default implementation of the policy host.
/// </summary>
/// <remarks>
/// Manages policy packs with support for snapshots and rollback.
/// Because sometimes you procrastinated better in the past.
/// </remarks>
public sealed class PolicyHost : IPolicyHost
{
    private readonly Dictionary<string, IPolicyPack> _policies = new();
    private readonly Dictionary<string, PolicySnapshot> _snapshots = new();
    private readonly List<PolicyChangeRecord> _history = new();
    private readonly object _lock = new();

    /// <inheritdoc />
    public IReadOnlyList<IPolicyPack> LoadedPolicies
    {
        get
        {
            lock (_lock)
            {
                return _policies.Values.ToList().AsReadOnly();
            }
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<IRule> AllRules
    {
        get
        {
            lock (_lock)
            {
                return _policies.Values
                    .SelectMany(p => p.Rules)
                    .OrderBy(r => r.Priority)
                    .ToList()
                    .AsReadOnly();
            }
        }
    }

    /// <inheritdoc />
    public event EventHandler<PolicyLoadedEventArgs>? PolicyLoaded;

    /// <inheritdoc />
    public event EventHandler<PolicyUnloadedEventArgs>? PolicyUnloaded;

    /// <inheritdoc />
    public void LoadPolicy(IPolicyPack policyPack)
    {
        ArgumentNullException.ThrowIfNull(policyPack);

        lock (_lock)
        {
            var previousSnapshotId = CreateSnapshotInternal();

            _policies[policyPack.Id] = policyPack;

            var newSnapshotId = CreateSnapshotInternal();

            _history.Add(new PolicyChangeRecord
            {
                ChangeType = PolicyChangeType.Loaded,
                PolicyId = policyPack.Id,
                PreviousSnapshotId = previousSnapshotId,
                NewSnapshotId = newSnapshotId
            });
        }

        PolicyLoaded?.Invoke(this, new PolicyLoadedEventArgs(policyPack));
    }

    /// <inheritdoc />
    public bool UnloadPolicy(string policyId)
    {
        ArgumentNullException.ThrowIfNull(policyId);

        bool removed;
        lock (_lock)
        {
            if (!_policies.ContainsKey(policyId))
            {
                return false;
            }

            var previousSnapshotId = CreateSnapshotInternal();

            removed = _policies.Remove(policyId);

            if (removed)
            {
                var newSnapshotId = CreateSnapshotInternal();

                _history.Add(new PolicyChangeRecord
                {
                    ChangeType = PolicyChangeType.Unloaded,
                    PolicyId = policyId,
                    PreviousSnapshotId = previousSnapshotId,
                    NewSnapshotId = newSnapshotId
                });
            }
        }

        if (removed)
        {
            PolicyUnloaded?.Invoke(this, new PolicyUnloadedEventArgs(policyId));
        }

        return removed;
    }

    /// <inheritdoc />
    public bool RollbackTo(string snapshotId)
    {
        ArgumentNullException.ThrowIfNull(snapshotId);

        lock (_lock)
        {
            if (!_snapshots.TryGetValue(snapshotId, out var snapshot))
            {
                return false;
            }

            var previousSnapshotId = CreateSnapshotInternal();

            _policies.Clear();
            foreach (var policy in snapshot.Policies)
            {
                _policies[policy.Id] = policy;
            }

            _history.Add(new PolicyChangeRecord
            {
                ChangeType = PolicyChangeType.Rollback,
                PolicyId = snapshotId,
                PreviousSnapshotId = previousSnapshotId,
                NewSnapshotId = snapshotId
            });

            return true;
        }
    }

    /// <inheritdoc />
    public string CreateSnapshot()
    {
        lock (_lock)
        {
            var snapshotId = CreateSnapshotInternal();

            _history.Add(new PolicyChangeRecord
            {
                ChangeType = PolicyChangeType.SnapshotCreated,
                PolicyId = string.Empty,
                NewSnapshotId = snapshotId
            });

            return snapshotId;
        }
    }

    private string CreateSnapshotInternal()
    {
        var snapshotId = Guid.NewGuid().ToString();
        var snapshot = new PolicySnapshot
        {
            Id = snapshotId,
            Policies = _policies.Values.ToList(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        _snapshots[snapshotId] = snapshot;
        return snapshotId;
    }

    /// <inheritdoc />
    public IReadOnlyList<PolicyChangeRecord> GetHistory()
    {
        lock (_lock)
        {
            return _history.ToList().AsReadOnly();
        }
    }

    private sealed class PolicySnapshot
    {
        public string Id { get; set; } = string.Empty;
        public IReadOnlyList<IPolicyPack> Policies { get; set; } = Array.Empty<IPolicyPack>();
        public DateTimeOffset CreatedAt { get; set; }
    }
}
