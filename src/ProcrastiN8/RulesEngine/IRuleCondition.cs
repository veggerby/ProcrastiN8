namespace ProcrastiN8.RulesEngine;

/// <summary>
/// Represents a condition that can be evaluated against a task context to determine if a rule should fire.
/// </summary>
/// <remarks>
/// Conditions form the predicate portion of procrastination rules, answering "When should we defer?"
/// </remarks>
public interface IRuleCondition
{
    /// <summary>
    /// Evaluates whether this condition is satisfied given the current task context.
    /// </summary>
    /// <param name="context">The evaluation context containing task metadata.</param>
    /// <returns>True if the condition is satisfied; otherwise, false.</returns>
    bool Evaluate(RuleEvaluationContext context);

    /// <summary>
    /// Returns a human-readable explanation of why this condition did or did not match.
    /// </summary>
    /// <param name="context">The evaluation context containing task metadata.</param>
    /// <returns>A verbose explanation suitable for an Explainability Report.</returns>
    string Explain(RuleEvaluationContext context);
}
