using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.RulesEngine;

/// <summary>
/// Provides context for rule evaluation including task details and mutable state.
/// </summary>
/// <remarks>
/// The context is passed to conditions and actions during rule evaluation,
/// allowing them to read task metadata and modify deferral state.
/// </remarks>
public sealed class RuleEvaluationContext
{
    /// <summary>
    /// Gets the task being evaluated.
    /// </summary>
    public ProcrastinationTask Task { get; }

    /// <summary>
    /// Gets the time provider for deterministic temporal operations.
    /// </summary>
    public ITimeProvider TimeProvider { get; }

    /// <summary>
    /// Gets the random provider for stochastic decisions.
    /// </summary>
    public IRandomProvider RandomProvider { get; }

    /// <summary>
    /// Gets or sets the accumulated deferral time from rule actions.
    /// </summary>
    public TimeSpan AccumulatedDeferral { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Gets or sets the accumulated regret factor (exponentially increasing delay multiplier).
    /// </summary>
    public double RegretFactor { get; set; } = 1.0;

    /// <summary>
    /// Gets the excuses generated during evaluation.
    /// </summary>
    public IList<string> Excuses { get; } = new List<string>();

    /// <summary>
    /// Gets the evaluation timestamp.
    /// </summary>
    public DateTimeOffset EvaluatedAt { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the task should be completely blocked.
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Gets or sets the blocking reason if the task is blocked.
    /// </summary>
    public string? BlockingReason { get; set; }

    /// <summary>
    /// Gets the evaluation trail recording which rules fired.
    /// </summary>
    public IList<RuleEvaluationResult> EvaluationTrail { get; } = new List<RuleEvaluationResult>();

    /// <summary>
    /// Gets custom state that can be shared between rules.
    /// </summary>
    public IDictionary<string, object?> SharedState { get; } = new Dictionary<string, object?>();

    /// <summary>
    /// Initializes a new instance of the <see cref="RuleEvaluationContext"/> class.
    /// </summary>
    /// <param name="task">The task being evaluated.</param>
    /// <param name="timeProvider">Optional time provider; defaults to system time.</param>
    /// <param name="randomProvider">Optional random provider; defaults to system random.</param>
    public RuleEvaluationContext(
        ProcrastinationTask task,
        ITimeProvider? timeProvider = null,
        IRandomProvider? randomProvider = null)
    {
        Task = task ?? throw new ArgumentNullException(nameof(task));
        TimeProvider = timeProvider ?? SystemTimeProvider.Default;
        RandomProvider = randomProvider ?? JustBecause.RandomProvider.Default;
        EvaluatedAt = TimeProvider.GetUtcNow();
    }
}
