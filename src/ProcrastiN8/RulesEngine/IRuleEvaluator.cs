namespace ProcrastiN8.RulesEngine;

/// <summary>
/// Evaluates a set of rules against a task context to determine deferral behavior.
/// </summary>
/// <remarks>
/// The evaluator is responsible for applying all matching rules, resolving conflicts,
/// and generating comprehensive explainability reports.
/// </remarks>
public interface IRuleEvaluator
{
    /// <summary>
    /// Evaluates all applicable rules against the provided context.
    /// </summary>
    /// <param name="context">The evaluation context containing task metadata.</param>
    /// <param name="cancellationToken">Cancellation token for cooperative cancellation.</param>
    /// <returns>The aggregated result of all rule evaluations.</returns>
    Task<RuleEngineResult> EvaluateAsync(RuleEvaluationContext context, CancellationToken cancellationToken);

    /// <summary>
    /// Generates an explainability report detailing why decisions were made.
    /// </summary>
    /// <param name="result">The result of a previous evaluation.</param>
    /// <returns>A verbose, multi-page explanation of the procrastination decision.</returns>
    ExplainabilityReport GenerateReport(RuleEngineResult result);
}
