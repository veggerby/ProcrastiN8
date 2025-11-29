namespace ProcrastiN8.RulesEngine;

/// <summary>
/// Represents a procrastination rule consisting of a condition and an action.
/// </summary>
/// <remarks>
/// Rules follow the pattern: "when [condition] then [action]"
/// For example: "when (task.tags contains 'important') then defer by exponential regret"
/// </remarks>
public interface IRule
{
    /// <summary>
    /// Gets the unique identifier for this rule.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets a human-readable name for this rule.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the priority of this rule (lower values = higher priority).
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Gets the condition that determines when this rule fires.
    /// </summary>
    IRuleCondition Condition { get; }

    /// <summary>
    /// Gets the action to execute when the condition is satisfied.
    /// </summary>
    IRuleAction Action { get; }

    /// <summary>
    /// Evaluates the rule against the provided context.
    /// </summary>
    /// <param name="context">The evaluation context containing task metadata.</param>
    /// <param name="cancellationToken">Cancellation token for cooperative cancellation.</param>
    /// <returns>The result of evaluating this rule.</returns>
    Task<RuleEvaluationResult> EvaluateAsync(RuleEvaluationContext context, CancellationToken cancellationToken);
}
