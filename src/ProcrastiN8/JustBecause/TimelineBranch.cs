using ProcrastiN8.LazyTasks;
using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// Represents a timeline branch where quantum promises can exist and be observed in alternate realities.
/// </summary>
/// <remarks>
/// Timeline branches enable promises to be completed in parallel universes without affecting the main timeline.
/// This is particularly useful for procrastination, as work can be perpetually "done in another timeline"
/// while remaining incomplete in the current reality. Timeline divergence is measured and recorded for
/// diagnostic purposes and existential dread.
/// </remarks>
public sealed class TimelineBranch
{
    private readonly string _branchId;
    private readonly DateTimeOffset _divergencePoint;
    private readonly ITimeProvider _timeProvider;
    private readonly Dictionary<object, object> _branchState;
    private readonly object _lock = new();
    private int _observationCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimelineBranch"/> class.
    /// </summary>
    /// <param name="branchId">A unique identifier for this timeline branch.</param>
    /// <param name="divergencePoint">The point in time when this branch diverged from the main timeline.</param>
    /// <param name="timeProvider">Optional time provider for this branch. If null, uses system time.</param>
    public TimelineBranch(string branchId, DateTimeOffset divergencePoint, ITimeProvider? timeProvider = null)
    {
        _branchId = branchId ?? throw new ArgumentNullException(nameof(branchId));
        _divergencePoint = divergencePoint;
        _timeProvider = timeProvider ?? SystemTimeProvider.Default;
        _branchState = new Dictionary<object, object>();
        _observationCount = 0;

        TemporalMetrics.TimelineDivergences.Add(1, 
            new KeyValuePair<string, object?>("branch_id", _branchId));
    }

    /// <summary>
    /// Gets the unique identifier for this timeline branch.
    /// </summary>
    public string BranchId => _branchId;

    /// <summary>
    /// Gets the point in time when this branch diverged from the main timeline.
    /// </summary>
    public DateTimeOffset DivergencePoint => _divergencePoint;

    /// <summary>
    /// Gets the number of observations that have occurred in this timeline branch.
    /// </summary>
    public int ObservationCount => _observationCount;

    /// <summary>
    /// Calculates the divergence index: how far this branch has diverged from the main timeline.
    /// </summary>
    /// <returns>A divergence index value, where 0 means no divergence and higher values indicate greater separation.</returns>
    public double GetDivergenceIndex()
    {
        var elapsed = _timeProvider.GetUtcNow() - _divergencePoint;
        var baseIndex = elapsed.TotalHours;
        
        // Divergence increases with observation count (observer effect amplifies differences)
        var observationMultiplier = Math.Log10(_observationCount + 1);
        
        return baseIndex * (1.0 + observationMultiplier);
    }

    /// <summary>
    /// Stores state specific to this timeline branch.
    /// </summary>
    /// <param name="key">The key identifying the state.</param>
    /// <param name="value">The value to store.</param>
    public void SetBranchState(object key, object value)
    {
        lock (_lock)
        {
            _branchState[key] = value;
        }
    }

    /// <summary>
    /// Retrieves state specific to this timeline branch.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key identifying the state.</param>
    /// <returns>The stored value, or default if not found.</returns>
    public TValue? GetBranchState<TValue>(object key)
    {
        lock (_lock)
        {
            if (_branchState.TryGetValue(key, out var value))
            {
                return (TValue?)value;
            }
            return default;
        }
    }

    /// <summary>
    /// Records an observation in this timeline branch.
    /// </summary>
    internal void RecordObservation()
    {
        lock (_lock)
        {
            _observationCount++;
        }
    }

    /// <summary>
    /// Determines whether this branch has diverged beyond a paradoxical threshold.
    /// </summary>
    /// <param name="paradoxThreshold">The divergence index threshold for paradox detection.</param>
    /// <returns><c>true</c> if the branch is paradoxical; otherwise, <c>false</c>.</returns>
    public bool IsParadoxical(double paradoxThreshold = 1000.0)
    {
        return GetDivergenceIndex() > paradoxThreshold;
    }
}

/// <summary>
/// Extensions for integrating timeline branching with quantum promises.
/// </summary>
public static class TimelineBranchExtensions
{
    private static readonly Dictionary<string, TimelineBranch> _branches = new();
    private static readonly object _branchLock = new();

    /// <summary>
    /// Creates or retrieves a timeline branch.
    /// </summary>
    /// <param name="branchId">The unique identifier for the branch.</param>
    /// <param name="timeProvider">Optional time provider for the branch.</param>
    /// <returns>A timeline branch instance.</returns>
    public static TimelineBranch GetOrCreateBranch(string branchId, ITimeProvider? timeProvider = null)
    {
        lock (_branchLock)
        {
            if (!_branches.TryGetValue(branchId, out var branch))
            {
                var divergencePoint = (timeProvider ?? SystemTimeProvider.Default).GetUtcNow();
                branch = new TimelineBranch(branchId, divergencePoint, timeProvider);
                _branches[branchId] = branch;
            }
            return branch;
        }
    }

    /// <summary>
    /// Observes a quantum promise in an alternate timeline branch.
    /// </summary>
    /// <typeparam name="T">The type of the promise value.</typeparam>
    /// <param name="promise">The quantum promise to observe.</param>
    /// <param name="branchId">The timeline branch in which to observe the promise.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the observation in the alternate timeline.</returns>
    /// <remarks>
    /// The promise is observed in the specified timeline branch, allowing it to collapse in an
    /// alternate reality without affecting the main timeline. The result exists only in that branch.
    /// This is ideal for claiming work is "done" while it remains incomplete in the current timeline.
    /// </remarks>
    public static async Task<T> ObserveInBranchAsync<T>(
        this IQuantumPromise<T> promise,
        string branchId,
        CancellationToken cancellationToken = default)
    {
        var branch = GetOrCreateBranch(branchId);
        branch.RecordObservation();

        // Update global divergence metrics
        TemporalMetrics.UpdateDivergenceIndex(branch.GetDivergenceIndex());

        // Observe the promise (in this alternate reality, it collapses normally)
        var result = await promise.ObserveAsync(cancellationToken);

        // Store the result in branch-specific state (null results are allowed in quantum mechanics)
        branch.SetBranchState(promise, result!);

        return result;
    }

    /// <summary>
    /// Checks if a promise has been observed in a specific timeline branch.
    /// </summary>
    /// <typeparam name="T">The type of the promise value.</typeparam>
    /// <param name="promise">The quantum promise.</param>
    /// <param name="branchId">The timeline branch to check.</param>
    /// <returns><c>true</c> if the promise was observed in that branch; otherwise, <c>false</c>.</returns>
    public static bool IsObservedInBranch<T>(this IQuantumPromise<T> promise, string branchId)
    {
        lock (_branchLock)
        {
            if (_branches.TryGetValue(branchId, out var branch))
            {
                return branch.GetBranchState<T>(promise) != null;
            }
            return false;
        }
    }
}
