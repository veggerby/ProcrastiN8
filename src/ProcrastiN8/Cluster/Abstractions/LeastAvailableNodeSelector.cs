using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Cluster.Abstractions;

/// <summary>
/// Node selector that prefers the *least available* node for maximum procrastination potential.
/// Load balancing, but in reverse — because why be efficient?
/// </summary>
public sealed class LeastAvailableNodeSelector : INodeSelector
{
    private readonly IRandomProvider _randomProvider;

    /// <summary>
    /// Initializes a new least available node selector.
    /// </summary>
    /// <param name="randomProvider">Random provider for tie-breaking.</param>
    public LeastAvailableNodeSelector(IRandomProvider? randomProvider = null)
    {
        _randomProvider = randomProvider ?? RandomProvider.Default;
    }

    /// <inheritdoc />
    public IProcrastinationNode? SelectNode(IReadOnlyCollection<IProcrastinationNode> nodes, DeferralWorkload workload)
    {
        if (nodes.Count == 0)
        {
            return null;
        }

        // Filter to only available or busy nodes (not partitioned, zombie, or shutdown)
        var eligibleNodes = nodes
            .Where(n => n.State == NodeState.Available || n.State == NodeState.Busy)
            .ToList();

        if (eligibleNodes.Count == 0)
        {
            return null;
        }

        // Sort by "least available" criteria:
        // 1. Highest workload (busiest)
        // 2. Most pending deferrals
        // 3. Oldest last heartbeat (most likely to fail)
        var sortedNodes = eligibleNodes
            .OrderByDescending(n => n.CurrentWorkload)
            .ThenByDescending(n => n.PendingDeferrals)
            .ThenBy(n => n.LastHeartbeat)
            .ToList();

        // If there's a tie at the top, pick randomly among tied nodes
        var maxWorkload = sortedNodes[0].CurrentWorkload;
        var tiedNodes = sortedNodes.Where(n => n.CurrentWorkload == maxWorkload).ToList();

        if (tiedNodes.Count > 1)
        {
            var randomIndex = (int)(_randomProvider.GetDouble() * tiedNodes.Count);
            return tiedNodes[randomIndex];
        }

        return sortedNodes[0];
    }
}
