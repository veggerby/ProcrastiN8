namespace ProcrastiN8.RulesEngine.Policies;

/// <summary>
/// Manages the loading, unloading, and rollback of policy packs at runtime.
/// </summary>
/// <remarks>
/// The policy host is responsible for maintaining the active set of rules
/// and coordinating conflict resolution between overlapping policies.
/// </remarks>
public interface IPolicyHost
{
    /// <summary>
    /// Gets all currently loaded policy packs.
    /// </summary>
    IReadOnlyList<IPolicyPack> LoadedPolicies { get; }

    /// <summary>
    /// Gets all rules from all loaded policies.
    /// </summary>
    IReadOnlyList<IRule> AllRules { get; }

    /// <summary>
    /// Loads a policy pack into the host.
    /// </summary>
    /// <param name="policyPack">The policy pack to load.</param>
    void LoadPolicy(IPolicyPack policyPack);

    /// <summary>
    /// Unloads a policy pack from the host.
    /// </summary>
    /// <param name="policyId">The ID of the policy pack to unload.</param>
    /// <returns>True if the policy was unloaded; false if it was not loaded.</returns>
    bool UnloadPolicy(string policyId);

    /// <summary>
    /// Rolls back to a previous policy state.
    /// </summary>
    /// <param name="snapshotId">The snapshot to roll back to.</param>
    /// <returns>True if rollback succeeded; false otherwise.</returns>
    bool RollbackTo(string snapshotId);

    /// <summary>
    /// Creates a snapshot of the current policy state.
    /// </summary>
    /// <returns>A unique identifier for the snapshot.</returns>
    string CreateSnapshot();

    /// <summary>
    /// Gets the history of policy changes.
    /// </summary>
    IReadOnlyList<PolicyChangeRecord> GetHistory();

    /// <summary>
    /// Occurs when a policy is loaded.
    /// </summary>
    event EventHandler<PolicyLoadedEventArgs>? PolicyLoaded;

    /// <summary>
    /// Occurs when a policy is unloaded.
    /// </summary>
    event EventHandler<PolicyUnloadedEventArgs>? PolicyUnloaded;
}

/// <summary>
/// Records a change to the policy configuration.
/// </summary>
/// <remarks>
/// Id and Timestamp should be set explicitly for testability.
/// </remarks>
public sealed class PolicyChangeRecord
{
    /// <summary>
    /// Gets or sets the unique identifier for this change.
    /// </summary>
    /// <remarks>
    /// Should be set explicitly for testability. Defaults to a new GUID if not set.
    /// </remarks>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the type of change.
    /// </summary>
    public PolicyChangeType ChangeType { get; set; }

    /// <summary>
    /// Gets or sets the policy ID affected.
    /// </summary>
    public string PolicyId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the change occurred.
    /// </summary>
    /// <remarks>
    /// Should be set explicitly using an ITimeProvider for testability.
    /// </remarks>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the snapshot ID before this change.
    /// </summary>
    public string? PreviousSnapshotId { get; set; }

    /// <summary>
    /// Gets or sets the snapshot ID after this change.
    /// </summary>
    public string? NewSnapshotId { get; set; }
}

/// <summary>
/// Types of policy changes.
/// </summary>
public enum PolicyChangeType
{
    /// <summary>A policy was loaded.</summary>
    Loaded,
    /// <summary>A policy was unloaded.</summary>
    Unloaded,
    /// <summary>A rollback occurred.</summary>
    Rollback,
    /// <summary>A snapshot was created.</summary>
    SnapshotCreated
}

/// <summary>
/// Event arguments for when a policy is loaded.
/// </summary>
public sealed class PolicyLoadedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PolicyLoadedEventArgs"/> class.
    /// </summary>
    /// <param name="policyPack">The loaded policy pack.</param>
    public PolicyLoadedEventArgs(IPolicyPack policyPack)
    {
        PolicyPack = policyPack ?? throw new ArgumentNullException(nameof(policyPack));
    }

    /// <summary>
    /// Gets the loaded policy pack.
    /// </summary>
    public IPolicyPack PolicyPack { get; }
}

/// <summary>
/// Event arguments for when a policy is unloaded.
/// </summary>
public sealed class PolicyUnloadedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PolicyUnloadedEventArgs"/> class.
    /// </summary>
    /// <param name="policyId">The ID of the unloaded policy.</param>
    public PolicyUnloadedEventArgs(string policyId)
    {
        PolicyId = policyId ?? throw new ArgumentNullException(nameof(policyId));
    }

    /// <summary>
    /// Gets the ID of the unloaded policy.
    /// </summary>
    public string PolicyId { get; }
}
