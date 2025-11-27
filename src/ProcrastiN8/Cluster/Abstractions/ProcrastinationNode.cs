namespace ProcrastiN8.Cluster.Abstractions;

/// <summary>
/// Default implementation of <see cref="IProcrastinationNode"/> representing a node in the cluster.
/// </summary>
public sealed class ProcrastinationNode : IProcrastinationNode
{
    private NodeState _state;
    private int _currentWorkload;
    private int _pendingDeferrals;
    private DateTimeOffset _lastHeartbeat;
    private readonly object _lock = new();

    /// <summary>
    /// Initializes a new procrastination node.
    /// </summary>
    /// <param name="nodeId">The unique node identifier.</param>
    /// <param name="endpoint">The node's endpoint address.</param>
    public ProcrastinationNode(string nodeId, string endpoint)
    {
        NodeId = nodeId;
        Endpoint = endpoint;
        _state = NodeState.Initializing;
        _lastHeartbeat = DateTimeOffset.UtcNow;
    }

    /// <inheritdoc />
    public string NodeId { get; }

    /// <inheritdoc />
    public string Endpoint { get; }

    /// <inheritdoc />
    public NodeState State
    {
        get { lock (_lock) { return _state; } }
    }

    /// <inheritdoc />
    public int CurrentWorkload
    {
        get { lock (_lock) { return _currentWorkload; } }
    }

    /// <inheritdoc />
    public DateTimeOffset LastHeartbeat
    {
        get { lock (_lock) { return _lastHeartbeat; } }
    }

    /// <inheritdoc />
    public int PendingDeferrals
    {
        get { lock (_lock) { return _pendingDeferrals; } }
    }

    /// <summary>
    /// Updates the node's state.
    /// </summary>
    /// <param name="newState">The new state.</param>
    public void SetState(NodeState newState)
    {
        lock (_lock)
        {
            _state = newState;
        }
    }

    /// <summary>
    /// Records a heartbeat for this node.
    /// </summary>
    public void RecordHeartbeat()
    {
        lock (_lock)
        {
            _lastHeartbeat = DateTimeOffset.UtcNow;
        }
    }

    /// <summary>
    /// Records a heartbeat with a specific timestamp.
    /// </summary>
    /// <param name="timestamp">The heartbeat timestamp.</param>
    public void RecordHeartbeat(DateTimeOffset timestamp)
    {
        lock (_lock)
        {
            _lastHeartbeat = timestamp;
        }
    }

    /// <summary>
    /// Increments the workload count.
    /// </summary>
    public void IncrementWorkload()
    {
        lock (_lock)
        {
            _currentWorkload++;
        }
    }

    /// <summary>
    /// Decrements the workload count.
    /// </summary>
    public void DecrementWorkload()
    {
        lock (_lock)
        {
            if (_currentWorkload > 0)
            {
                _currentWorkload--;
            }
        }
    }

    /// <summary>
    /// Sets the workload count directly.
    /// </summary>
    /// <param name="workload">The new workload count.</param>
    public void SetWorkload(int workload)
    {
        lock (_lock)
        {
            _currentWorkload = Math.Max(0, workload);
        }
    }

    /// <summary>
    /// Increments the pending deferrals count.
    /// </summary>
    public void IncrementPendingDeferrals()
    {
        lock (_lock)
        {
            _pendingDeferrals++;
        }
    }

    /// <summary>
    /// Decrements the pending deferrals count.
    /// </summary>
    public void DecrementPendingDeferrals()
    {
        lock (_lock)
        {
            if (_pendingDeferrals > 0)
            {
                _pendingDeferrals--;
            }
        }
    }

    /// <summary>
    /// Sets the pending deferrals count directly.
    /// </summary>
    /// <param name="deferrals">The new deferrals count.</param>
    public void SetPendingDeferrals(int deferrals)
    {
        lock (_lock)
        {
            _pendingDeferrals = Math.Max(0, deferrals);
        }
    }
}
