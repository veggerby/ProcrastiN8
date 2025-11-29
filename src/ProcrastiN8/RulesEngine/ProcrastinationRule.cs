namespace ProcrastiN8.RulesEngine;

/// <summary>
/// Base implementation of a procrastination rule.
/// </summary>
/// <remarks>
/// Provides common functionality for evaluating conditions and executing actions.
/// </remarks>
public class ProcrastinationRule : IRule
{
    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public int Priority { get; }

    /// <inheritdoc />
    public IRuleCondition Condition { get; }

    /// <inheritdoc />
    public IRuleAction Action { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcrastinationRule"/> class.
    /// </summary>
    /// <param name="id">Unique identifier for this rule.</param>
    /// <param name="name">Human-readable name for this rule.</param>
    /// <param name="condition">The condition that triggers this rule.</param>
    /// <param name="action">The action to execute when the condition is met.</param>
    /// <param name="priority">Priority level (lower = higher priority).</param>
    public ProcrastinationRule(
        string id,
        string name,
        IRuleCondition condition,
        IRuleAction action,
        int priority = 100)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        Action = action ?? throw new ArgumentNullException(nameof(action));
        Priority = priority;
    }

    /// <inheritdoc />
    public async Task<RuleEvaluationResult> EvaluateAsync(RuleEvaluationContext context, CancellationToken cancellationToken)
    {
        var result = new RuleEvaluationResult(this)
        {
            EvaluatedAt = context.TimeProvider.GetUtcNow()
        };

        result.ConditionMatched = Condition.Evaluate(context);
        result.ConditionExplanation = Condition.Explain(context);

        if (result.ConditionMatched)
        {
            result.ActionExecuted = true;
            result.ActionResult = await Action.ExecuteAsync(context, cancellationToken);

            // Update context with action results
            context.AccumulatedDeferral += result.ActionResult.DeferralDuration;
            context.RegretFactor *= result.ActionResult.RegretMultiplier;

            if (result.ActionResult.Excuse != null)
            {
                context.Excuses.Add(result.ActionResult.Excuse);
            }

            if (result.ActionResult.ShouldBlock)
            {
                context.IsBlocked = true;
                context.BlockingReason = result.ActionResult.BlockingReason;
            }
        }

        context.EvaluationTrail.Add(result);
        return result;
    }
}
