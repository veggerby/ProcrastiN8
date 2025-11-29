namespace ProcrastiN8.RulesEngine.Conditions;

/// <summary>
/// Combines multiple conditions with AND logic.
/// </summary>
public sealed class AndCondition : IRuleCondition
{
    private readonly IReadOnlyList<IRuleCondition> _conditions;

    /// <summary>
    /// Initializes a new instance of the <see cref="AndCondition"/> class.
    /// </summary>
    /// <param name="conditions">The conditions to combine.</param>
    public AndCondition(params IRuleCondition[] conditions)
    {
        _conditions = conditions?.ToList() ?? throw new ArgumentNullException(nameof(conditions));
    }

    /// <inheritdoc />
    public bool Evaluate(RuleEvaluationContext context)
    {
        return _conditions.All(c => c.Evaluate(context));
    }

    /// <inheritdoc />
    public string Explain(RuleEvaluationContext context)
    {
        var explanations = _conditions.Select(c => c.Explain(context));
        var matched = Evaluate(context);

        return $"AND condition combining {_conditions.Count} sub-conditions was {(matched ? "SATISFIED" : "NOT SATISFIED")}. " +
               "All sub-conditions must be true for this composite condition to match. " +
               $"Sub-condition explanations: {string.Join(" | ", explanations)}";
    }
}

/// <summary>
/// Combines multiple conditions with OR logic.
/// </summary>
public sealed class OrCondition : IRuleCondition
{
    private readonly IReadOnlyList<IRuleCondition> _conditions;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrCondition"/> class.
    /// </summary>
    /// <param name="conditions">The conditions to combine.</param>
    public OrCondition(params IRuleCondition[] conditions)
    {
        _conditions = conditions?.ToList() ?? throw new ArgumentNullException(nameof(conditions));
    }

    /// <inheritdoc />
    public bool Evaluate(RuleEvaluationContext context)
    {
        return _conditions.Any(c => c.Evaluate(context));
    }

    /// <inheritdoc />
    public string Explain(RuleEvaluationContext context)
    {
        var explanations = _conditions.Select(c => c.Explain(context));
        var matched = Evaluate(context);

        return $"OR condition combining {_conditions.Count} sub-conditions was {(matched ? "SATISFIED" : "NOT SATISFIED")}. " +
               "Any sub-condition being true causes this composite condition to match. " +
               $"Sub-condition explanations: {string.Join(" | ", explanations)}";
    }
}

/// <summary>
/// Negates a condition.
/// </summary>
public sealed class NotCondition : IRuleCondition
{
    private readonly IRuleCondition _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotCondition"/> class.
    /// </summary>
    /// <param name="inner">The condition to negate.</param>
    public NotCondition(IRuleCondition inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    /// <inheritdoc />
    public bool Evaluate(RuleEvaluationContext context)
    {
        return !_inner.Evaluate(context);
    }

    /// <inheritdoc />
    public string Explain(RuleEvaluationContext context)
    {
        var innerExplanation = _inner.Explain(context);
        var matched = Evaluate(context);

        return $"NOT condition was {(matched ? "SATISFIED" : "NOT SATISFIED")}. " +
               $"Inner condition: {innerExplanation}. " +
               "Negation inverts the truth value of the inner condition, " +
               "allowing us to procrastinate when things are NOT a certain way.";
    }
}
