namespace ProcrastiN8.RulesEngine;

/// <summary>
/// Represents the aggregated result of evaluating all rules in the rules engine.
/// </summary>
/// <remarks>
/// Contains the final deferral recommendation, all excuses, and the complete evaluation trail.
/// Timestamps should be set explicitly using an ITimeProvider for testability.
/// </remarks>
public sealed class RuleEngineResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RuleEngineResult"/> class.
    /// </summary>
    /// <param name="task">The task that was evaluated.</param>
    public RuleEngineResult(ProcrastinationTask task)
    {
        Task = task ?? throw new ArgumentNullException(nameof(task));
    }

    /// <summary>
    /// Gets the task that was evaluated.
    /// </summary>
    public ProcrastinationTask Task { get; }

    /// <summary>
    /// Gets or sets the total recommended deferral duration.
    /// </summary>
    public TimeSpan TotalDeferral { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Gets or sets a value indicating whether the task should be blocked entirely.
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Gets or sets the blocking reason if applicable.
    /// </summary>
    public string? BlockingReason { get; set; }

    /// <summary>
    /// Gets the list of all excuses generated.
    /// </summary>
    public IList<string> Excuses { get; } = new List<string>();

    /// <summary>
    /// Gets the evaluation results for each rule that was evaluated.
    /// </summary>
    public IList<RuleEvaluationResult> RuleResults { get; } = new List<RuleEvaluationResult>();

    /// <summary>
    /// Gets or sets the number of rules that matched.
    /// </summary>
    public int RulesMatched { get; set; }

    /// <summary>
    /// Gets or sets the number of rules that were evaluated.
    /// </summary>
    public int RulesEvaluated { get; set; }

    /// <summary>
    /// Gets or sets the number of conflict resolutions that occurred.
    /// </summary>
    public int ConflictResolutionCycles { get; set; }

    /// <summary>
    /// Gets or sets the final regret factor after all rules have been applied.
    /// </summary>
    public double FinalRegretFactor { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets the timestamp when evaluation began.
    /// </summary>
    /// <remarks>
    /// Should be set explicitly using an ITimeProvider for testability.
    /// </remarks>
    public DateTimeOffset EvaluationStartedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when evaluation completed.
    /// </summary>
    /// <remarks>
    /// Should be set explicitly using an ITimeProvider for testability.
    /// </remarks>
    public DateTimeOffset EvaluationCompletedAt { get; set; }

    /// <summary>
    /// Gets the evaluation duration.
    /// </summary>
    public TimeSpan EvaluationDuration => EvaluationCompletedAt - EvaluationStartedAt;
}
