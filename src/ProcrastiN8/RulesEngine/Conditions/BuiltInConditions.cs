namespace ProcrastiN8.RulesEngine.Conditions;

/// <summary>
/// A condition that checks if the task contains any of the specified tags.
/// </summary>
/// <remarks>
/// Supports patterns like: "when (task.tags contains 'important')"
/// </remarks>
public sealed class TagContainsCondition : IRuleCondition
{
    private readonly IReadOnlySet<string> _tags;
    private readonly bool _matchAll;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagContainsCondition"/> class.
    /// </summary>
    /// <param name="tags">Tags to check for.</param>
    /// <param name="matchAll">If true, all tags must match; if false, any tag matches.</param>
    public TagContainsCondition(IEnumerable<string> tags, bool matchAll = false)
    {
        _tags = new HashSet<string>(tags ?? throw new ArgumentNullException(nameof(tags)), StringComparer.OrdinalIgnoreCase);
        _matchAll = matchAll;
    }

    /// <inheritdoc />
    public bool Evaluate(RuleEvaluationContext context)
    {
        if (_matchAll)
        {
            return _tags.All(t => context.Task.Tags.Contains(t));
        }

        return _tags.Any(t => context.Task.Tags.Contains(t));
    }

    /// <inheritdoc />
    public string Explain(RuleEvaluationContext context)
    {
        var taskTags = string.Join(", ", context.Task.Tags);
        var requiredTags = string.Join(", ", _tags);
        var mode = _matchAll ? "ALL" : "ANY";
        var matched = Evaluate(context);

        return $"Tag condition ({mode} of [{requiredTags}]) was {(matched ? "SATISFIED" : "NOT SATISFIED")}. " +
               $"Task has tags: [{taskTags}]. " +
               $"This condition requires {(_matchAll ? "all specified tags to be present" : "at least one specified tag to be present")}.";
    }
}

/// <summary>
/// A condition that evaluates task priority against a threshold.
/// </summary>
/// <remarks>
/// Supports patterns like: "when (task.priority &lt; 50)"
/// </remarks>
public sealed class PriorityCondition : IRuleCondition
{
    private readonly int _threshold;
    private readonly ComparisonOperator _operator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PriorityCondition"/> class.
    /// </summary>
    /// <param name="threshold">The priority threshold to compare against.</param>
    /// <param name="op">The comparison operator.</param>
    public PriorityCondition(int threshold, ComparisonOperator op = ComparisonOperator.LessThan)
    {
        _threshold = threshold;
        _operator = op;
    }

    /// <inheritdoc />
    public bool Evaluate(RuleEvaluationContext context)
    {
        return _operator switch
        {
            ComparisonOperator.LessThan => context.Task.Priority < _threshold,
            ComparisonOperator.LessThanOrEqual => context.Task.Priority <= _threshold,
            ComparisonOperator.Equal => context.Task.Priority == _threshold,
            ComparisonOperator.GreaterThanOrEqual => context.Task.Priority >= _threshold,
            ComparisonOperator.GreaterThan => context.Task.Priority > _threshold,
            ComparisonOperator.NotEqual => context.Task.Priority != _threshold,
            _ => false
        };
    }

    /// <inheritdoc />
    public string Explain(RuleEvaluationContext context)
    {
        var matched = Evaluate(context);
        return $"Priority condition (task.priority {GetOperatorSymbol()} {_threshold}) was {(matched ? "SATISFIED" : "NOT SATISFIED")}. " +
               $"Task priority is {context.Task.Priority}. " +
               $"Lower priority values indicate higher importance in this system, " +
               $"which may paradoxically justify more elaborate procrastination.";
    }

    private string GetOperatorSymbol() => _operator switch
    {
        ComparisonOperator.LessThan => "<",
        ComparisonOperator.LessThanOrEqual => "<=",
        ComparisonOperator.Equal => "==",
        ComparisonOperator.GreaterThanOrEqual => ">=",
        ComparisonOperator.GreaterThan => ">",
        ComparisonOperator.NotEqual => "!=",
        _ => "?"
    };
}

/// <summary>
/// A condition that checks if the task is approaching its deadline.
/// </summary>
/// <remarks>
/// Supports patterns like: "when (task.deadline within 24 hours)"
/// </remarks>
public sealed class DeadlineProximityCondition : IRuleCondition
{
    private readonly TimeSpan _threshold;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeadlineProximityCondition"/> class.
    /// </summary>
    /// <param name="threshold">The time threshold before deadline.</param>
    public DeadlineProximityCondition(TimeSpan threshold)
    {
        _threshold = threshold;
    }

    /// <inheritdoc />
    public bool Evaluate(RuleEvaluationContext context)
    {
        if (!context.Task.Deadline.HasValue)
        {
            return false;
        }

        var timeUntilDeadline = context.Task.Deadline.Value - context.EvaluatedAt;
        return timeUntilDeadline <= _threshold && timeUntilDeadline > TimeSpan.Zero;
    }

    /// <inheritdoc />
    public string Explain(RuleEvaluationContext context)
    {
        if (!context.Task.Deadline.HasValue)
        {
            return "Deadline proximity condition was NOT SATISFIED because no deadline is set. " +
                   "Tasks without deadlines exist in a state of eternal possibility, " +
                   "and thus cannot be urgently procrastinated.";
        }

        var timeUntilDeadline = context.Task.Deadline.Value - context.EvaluatedAt;
        var matched = Evaluate(context);

        return $"Deadline proximity condition (within {_threshold.TotalHours:F1} hours) was {(matched ? "SATISFIED" : "NOT SATISFIED")}. " +
               $"Time until deadline: {timeUntilDeadline.TotalHours:F1} hours. " +
               $"Threshold: {_threshold.TotalHours:F1} hours. " +
               (matched
                   ? "Panic mode engaged. Maximum procrastination justified by impending doom."
                   : "Insufficient urgency to warrant immediate attention.");
    }
}

/// <summary>
/// A condition that checks estimated effort against a threshold.
/// </summary>
public sealed class EffortCondition : IRuleCondition
{
    private readonly double _threshold;
    private readonly ComparisonOperator _operator;

    /// <summary>
    /// Initializes a new instance of the <see cref="EffortCondition"/> class.
    /// </summary>
    /// <param name="threshold">The effort threshold.</param>
    /// <param name="op">The comparison operator.</param>
    public EffortCondition(double threshold, ComparisonOperator op = ComparisonOperator.GreaterThan)
    {
        _threshold = threshold;
        _operator = op;
    }

    /// <inheritdoc />
    public bool Evaluate(RuleEvaluationContext context)
    {
        return _operator switch
        {
            ComparisonOperator.LessThan => context.Task.EstimatedEffort < _threshold,
            ComparisonOperator.LessThanOrEqual => context.Task.EstimatedEffort <= _threshold,
            ComparisonOperator.Equal => Math.Abs(context.Task.EstimatedEffort - _threshold) < 0.001,
            ComparisonOperator.GreaterThanOrEqual => context.Task.EstimatedEffort >= _threshold,
            ComparisonOperator.GreaterThan => context.Task.EstimatedEffort > _threshold,
            ComparisonOperator.NotEqual => Math.Abs(context.Task.EstimatedEffort - _threshold) >= 0.001,
            _ => false
        };
    }

    /// <inheritdoc />
    public string Explain(RuleEvaluationContext context)
    {
        var matched = Evaluate(context);
        return $"Effort condition (task.effort {GetOperatorSymbol()} {_threshold}) was {(matched ? "SATISFIED" : "NOT SATISFIED")}. " +
               $"Task estimated effort: {context.Task.EstimatedEffort:F2}. " +
               "Effort estimates in ProcrastiN8 are deliberately vague to maximize interpretive flexibility.";
    }

    private string GetOperatorSymbol() => _operator switch
    {
        ComparisonOperator.LessThan => "<",
        ComparisonOperator.LessThanOrEqual => "<=",
        ComparisonOperator.Equal => "==",
        ComparisonOperator.GreaterThanOrEqual => ">=",
        ComparisonOperator.GreaterThan => ">",
        ComparisonOperator.NotEqual => "!=",
        _ => "?"
    };
}

/// <summary>
/// A condition that always evaluates to true.
/// </summary>
/// <remarks>
/// Useful as a catch-all or default rule condition.
/// </remarks>
public sealed class AlwaysTrueCondition : IRuleCondition
{
    /// <inheritdoc />
    public bool Evaluate(RuleEvaluationContext context) => true;

    /// <inheritdoc />
    public string Explain(RuleEvaluationContext context) =>
        "This condition always evaluates to TRUE. " +
        "It represents the inevitable nature of procrastination—universal and inescapable.";
}

/// <summary>
/// A condition that checks a day of week.
/// </summary>
public sealed class DayOfWeekCondition : IRuleCondition
{
    private readonly IReadOnlySet<DayOfWeek> _days;

    /// <summary>
    /// Initializes a new instance of the <see cref="DayOfWeekCondition"/> class.
    /// </summary>
    /// <param name="days">Days of week to match.</param>
    public DayOfWeekCondition(IEnumerable<DayOfWeek> days)
    {
        _days = new HashSet<DayOfWeek>(days ?? throw new ArgumentNullException(nameof(days)));
    }

    /// <inheritdoc />
    public bool Evaluate(RuleEvaluationContext context)
    {
        return _days.Contains(context.EvaluatedAt.DayOfWeek);
    }

    /// <inheritdoc />
    public string Explain(RuleEvaluationContext context)
    {
        var matched = Evaluate(context);
        var dayNames = string.Join(", ", _days);
        return $"Day of week condition ({dayNames}) was {(matched ? "SATISFIED" : "NOT SATISFIED")}. " +
               $"Current day: {context.EvaluatedAt.DayOfWeek}. " +
               "Certain days are statistically more conducive to productive procrastination.";
    }
}

/// <summary>
/// Comparison operators for numeric conditions.
/// </summary>
public enum ComparisonOperator
{
    /// <summary>Less than.</summary>
    LessThan,
    /// <summary>Less than or equal.</summary>
    LessThanOrEqual,
    /// <summary>Equal.</summary>
    Equal,
    /// <summary>Greater than or equal.</summary>
    GreaterThanOrEqual,
    /// <summary>Greater than.</summary>
    GreaterThan,
    /// <summary>Not equal.</summary>
    NotEqual
}
