namespace ProcrastiN8.RulesEngine;

/// <summary>
/// Represents an action that should be taken when a rule's condition is satisfied.
/// </summary>
/// <remarks>
/// Actions define "How" to defer, e.g., defer by fixed amount, exponential regret, or bureaucratic delay.
/// </remarks>
public interface IRuleAction
{
    /// <summary>
    /// Executes the action against the provided context.
    /// </summary>
    /// <param name="context">The evaluation context containing task metadata and mutable state.</param>
    /// <param name="cancellationToken">Cancellation token for cooperative cancellation.</param>
    /// <returns>The result of executing this action.</returns>
    Task<RuleActionResult> ExecuteAsync(RuleEvaluationContext context, CancellationToken cancellationToken);

    /// <summary>
    /// Returns a human-readable explanation of what this action will do.
    /// </summary>
    /// <returns>A verbose explanation suitable for an Explainability Report.</returns>
    string Describe();
}
