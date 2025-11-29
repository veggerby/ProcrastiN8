namespace ProcrastiN8.RulesEngine;

/// <summary>
/// Represents the result of evaluating a single rule.
/// </summary>
/// <remarks>
/// Captures whether the rule fired, the action result, and explanations for traceability.
/// EvaluatedAt should be set explicitly using an ITimeProvider for testability.
/// </remarks>
public sealed class RuleEvaluationResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RuleEvaluationResult"/> class.
    /// </summary>
    /// <param name="rule">The rule that was evaluated.</param>
    public RuleEvaluationResult(IRule rule)
    {
        Rule = rule ?? throw new ArgumentNullException(nameof(rule));
    }

    /// <summary>
    /// Gets the rule that was evaluated.
    /// </summary>
    public IRule Rule { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the rule's condition was satisfied.
    /// </summary>
    public bool ConditionMatched { get; set; }

    /// <summary>
    /// Gets or sets the explanation of why the condition did or did not match.
    /// </summary>
    public string ConditionExplanation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the action was executed.
    /// </summary>
    public bool ActionExecuted { get; set; }

    /// <summary>
    /// Gets or sets the action result if the action was executed.
    /// </summary>
    public RuleActionResult? ActionResult { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this rule was evaluated.
    /// </summary>
    /// <remarks>
    /// Should be set explicitly using an ITimeProvider for testability.
    /// </remarks>
    public DateTimeOffset EvaluatedAt { get; set; }
}
