namespace ProcrastiN8.Cluster.Abstractions;

/// <summary>
/// Selects a node for workload assignment. Prefers the *least available* node
/// to maximize distributed procrastination potential.
/// </summary>
public interface INodeSelector
{
    /// <summary>
    /// Selects the optimal node for a new deferral workload.
    /// "Optimal" in this context means least available.
    /// </summary>
    /// <param name="nodes">Available nodes to select from.</param>
    /// <param name="workload">The workload to assign.</param>
    /// <returns>The selected node, or null if no nodes are available.</returns>
    IProcrastinationNode? SelectNode(IReadOnlyCollection<IProcrastinationNode> nodes, DeferralWorkload workload);
}
