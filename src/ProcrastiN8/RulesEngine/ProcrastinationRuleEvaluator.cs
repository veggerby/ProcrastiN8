using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.RulesEngine.Policies;
using System.Text;

namespace ProcrastiN8.RulesEngine;

/// <summary>
/// Resolves conflicts between contradictory rules through circular negotiation.
/// </summary>
/// <remarks>
/// When rules contradict, the conflict resolver enters a negotiation loop
/// that continues until consensus is reached (or the maximum cycle limit).
/// Circular logic is a feature, not a bug.
/// </remarks>
public interface IConflictResolver
{
    /// <summary>
    /// Resolves conflicts between multiple rule results.
    /// </summary>
    /// <param name="results">The conflicting rule results.</param>
    /// <param name="context">The evaluation context.</param>
    /// <param name="maxCycles">Maximum negotiation cycles before giving up.</param>
    /// <returns>The resolved result and number of cycles taken.</returns>
    (RuleActionResult ResolvedResult, int NegotiationCycles) ResolveConflicts(
        IReadOnlyList<RuleEvaluationResult> results,
        RuleEvaluationContext context,
        int maxCycles = 10);
}

/// <summary>
/// Default conflict resolver that negotiates in circles until exhaustion.
/// </summary>
public sealed class CircularConflictResolver : IConflictResolver
{
    /// <inheritdoc />
    public (RuleActionResult ResolvedResult, int NegotiationCycles) ResolveConflicts(
        IReadOnlyList<RuleEvaluationResult> results,
        RuleEvaluationContext context,
        int maxCycles = 10)
    {
        if (results.Count == 0)
        {
            return (new RuleActionResult(), 0);
        }

        if (results.Count == 1)
        {
            return (results[0].ActionResult ?? new RuleActionResult(), 0);
        }

        var actionResults = results
            .Where(r => r.ActionExecuted && r.ActionResult != null)
            .Select(r => r.ActionResult!)
            .ToList();

        if (actionResults.Count == 0)
        {
            return (new RuleActionResult(), 0);
        }

        // Only one valid result means no conflict to resolve
        if (actionResults.Count == 1)
        {
            return (actionResults[0], 0);
        }

        var negotiationCycles = 0;
        var totalDeferral = TimeSpan.Zero;
        var totalRegret = 1.0;
        var excuses = new List<string>();
        var blocked = false;
        string? blockReason = null;

        // Circular negotiation: keep averaging until we converge or hit max cycles
        var previousDeferral = TimeSpan.Zero;
        var convergenceThreshold = TimeSpan.FromSeconds(1);

        while (negotiationCycles < maxCycles)
        {
            negotiationCycles++;

            // Each cycle, recalculate based on weighted contributions
            totalDeferral = TimeSpan.Zero;
            var deferralCount = 0;

            foreach (var result in actionResults)
            {
                totalDeferral += result.DeferralDuration;
                deferralCount++;
                totalRegret *= result.RegretMultiplier;

                if (result.Excuse != null)
                {
                    excuses.Add(result.Excuse);
                }

                if (result.ShouldBlock)
                {
                    blocked = true;
                    blockReason = result.BlockingReason;
                }
            }

            // Average the deferral (circular logic: we keep doing this)
            if (deferralCount > 0)
            {
                totalDeferral = TimeSpan.FromTicks(totalDeferral.Ticks / deferralCount);
            }

            // Add some negotiation overhead (the more we negotiate, the longer it takes)
            var negotiationOverhead = TimeSpan.FromMinutes(context.RandomProvider.GetDouble() * negotiationCycles);
            totalDeferral += negotiationOverhead;

            // Check for convergence (did we settle on a value?)
            if (Math.Abs((totalDeferral - previousDeferral).TotalSeconds) < convergenceThreshold.TotalSeconds)
            {
                break; // Consensus reached!
            }

            previousDeferral = totalDeferral;
        }

        return (new RuleActionResult
        {
            DeferralDuration = totalDeferral,
            RegretMultiplier = totalRegret,
            Excuse = excuses.Count > 0
                ? $"After {negotiationCycles} cycles of negotiation: {string.Join(" Also: ", excuses.Take(3))}"
                : null,
            ShouldBlock = blocked,
            BlockingReason = blockReason,
            ActionDescription = $"Conflict resolution completed after {negotiationCycles} circular negotiation cycles."
        }, negotiationCycles);
    }
}

/// <summary>
/// Default implementation of the rule evaluator.
/// </summary>
/// <remarks>
/// Evaluates all rules from loaded policies, resolves conflicts,
/// and generates verbose explainability reports.
/// </remarks>
public sealed class ProcrastinationRuleEvaluator : IRuleEvaluator
{
    private readonly IPolicyHost _policyHost;
    private readonly IConflictResolver _conflictResolver;
    private readonly IProcrastiLogger? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcrastinationRuleEvaluator"/> class.
    /// </summary>
    /// <param name="policyHost">The policy host containing rules to evaluate.</param>
    /// <param name="conflictResolver">Optional conflict resolver; defaults to circular negotiation.</param>
    /// <param name="logger">Optional logger for verbose output.</param>
    public ProcrastinationRuleEvaluator(
        IPolicyHost policyHost,
        IConflictResolver? conflictResolver = null,
        IProcrastiLogger? logger = null)
    {
        _policyHost = policyHost ?? throw new ArgumentNullException(nameof(policyHost));
        _conflictResolver = conflictResolver ?? new CircularConflictResolver();
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<RuleEngineResult> EvaluateAsync(RuleEvaluationContext context, CancellationToken cancellationToken)
    {
        var result = new RuleEngineResult(context.Task)
        {
            EvaluationStartedAt = context.TimeProvider.GetUtcNow()
        };

        var rules = _policyHost.AllRules;
        result.RulesEvaluated = rules.Count;

        _logger?.Debug("Evaluating {RuleCount} rules against task '{TaskName}'", rules.Count, context.Task.Name);

        var matchedResults = new List<RuleEvaluationResult>();

        foreach (var rule in rules)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var ruleResult = await rule.EvaluateAsync(context, cancellationToken);
            result.RuleResults.Add(ruleResult);

            if (ruleResult.ConditionMatched)
            {
                matchedResults.Add(ruleResult);
                result.RulesMatched++;
                _logger?.Info("Rule '{RuleName}' matched for task '{TaskName}'", rule.Name, context.Task.Name);
            }
        }

        // Resolve conflicts if multiple rules fired
        if (matchedResults.Count > 1)
        {
            _logger?.Debug("Resolving conflicts between {Count} matched rules", matchedResults.Count);
            var (resolvedResult, cycles) = _conflictResolver.ResolveConflicts(matchedResults, context);

            result.TotalDeferral = resolvedResult.DeferralDuration;
            result.IsBlocked = resolvedResult.ShouldBlock;
            result.BlockingReason = resolvedResult.BlockingReason;
            result.ConflictResolutionCycles = cycles;

            if (resolvedResult.Excuse != null)
            {
                result.Excuses.Add(resolvedResult.Excuse);
            }
        }
        else if (matchedResults.Count == 1)
        {
            var singleResult = matchedResults[0].ActionResult;
            if (singleResult != null)
            {
                result.TotalDeferral = singleResult.DeferralDuration;
                result.IsBlocked = singleResult.ShouldBlock;
                result.BlockingReason = singleResult.BlockingReason;

                if (singleResult.Excuse != null)
                {
                    result.Excuses.Add(singleResult.Excuse);
                }
            }
        }

        // Add all accumulated excuses from context
        foreach (var excuse in context.Excuses)
        {
            if (!result.Excuses.Contains(excuse))
            {
                result.Excuses.Add(excuse);
            }
        }

        result.FinalRegretFactor = context.RegretFactor;
        result.EvaluationCompletedAt = context.TimeProvider.GetUtcNow();

        _logger?.Info(
            "Evaluation complete. Matched {Matched}/{Total} rules. Total deferral: {Deferral}. Blocked: {Blocked}",
            result.RulesMatched,
            result.RulesEvaluated,
            result.TotalDeferral,
            result.IsBlocked);

        return result;
    }

    /// <inheritdoc />
    public ExplainabilityReport GenerateReport(RuleEngineResult result)
    {
        var report = new ExplainabilityReport
        {
            Title = $"Procrastination Decision Explainability Report for Task: {result.Task.Name}",
            GeneratedAt = DateTimeOffset.UtcNow
        };

        // Executive Summary
        report.ExecutiveSummary = GenerateExecutiveSummary(result);

        // Task Summary
        report.TaskSummary = GenerateTaskSummary(result.Task);

        // Rule-by-rule analysis
        foreach (var ruleResult in result.RuleResults)
        {
            report.RuleExplanations.Add(new RuleExplanationSection
            {
                RuleId = ruleResult.Rule.Id,
                RuleName = ruleResult.Rule.Name,
                Priority = ruleResult.Rule.Priority,
                ConditionMatched = ruleResult.ConditionMatched,
                DetailedExplanation = ruleResult.ConditionExplanation,
                Rationale = GenerateRuleRationale(ruleResult)
            });
        }

        // Conflict resolution narrative
        report.ConflictResolutionNarrative = GenerateConflictNarrative(result);

        // Excuse justifications
        foreach (var excuse in result.Excuses)
        {
            report.ExcuseJustifications.Add(new ExcuseJustification
            {
                Excuse = excuse,
                SourceRule = "Multiple Sources",
                PhilosophicalBasis = GeneratePhilosophicalBasis(excuse)
            });
        }

        // Final recommendation
        report.FinalRecommendation = GenerateFinalRecommendation(result);

        // Appendix
        report.Appendix = GenerateAppendix(result);

        return report;
    }

    private static string GenerateExecutiveSummary(RuleEngineResult result)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"This report documents the procrastination decision for task '{result.Task.Name}'.");
        sb.AppendLine();
        sb.AppendLine($"Evaluation Duration: {result.EvaluationDuration.TotalMilliseconds:F2}ms");
        sb.AppendLine($"Rules Evaluated: {result.RulesEvaluated}");
        sb.AppendLine($"Rules Matched: {result.RulesMatched}");
        sb.AppendLine($"Conflict Resolution Cycles: {result.ConflictResolutionCycles}");
        sb.AppendLine();

        if (result.IsBlocked)
        {
            sb.AppendLine("DECISION: BLOCKED");
            sb.AppendLine($"Reason: {result.BlockingReason}");
        }
        else if (result.TotalDeferral > TimeSpan.Zero)
        {
            sb.AppendLine($"DECISION: DEFERRED by {result.TotalDeferral.TotalMinutes:F1} minutes");
            sb.AppendLine($"Regret Factor: {result.FinalRegretFactor:F2}x");
        }
        else
        {
            sb.AppendLine("DECISION: NO DEFERRAL REQUIRED");
            sb.AppendLine("(This is suspicious and may warrant investigation.)");
        }

        sb.AppendLine();
        sb.AppendLine($"Total Excuses Generated: {result.Excuses.Count}");

        return sb.ToString();
    }

    private static string GenerateTaskSummary(ProcrastinationTask task)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Task ID: {task.Id}");
        sb.AppendLine($"Task Name: {task.Name}");
        sb.AppendLine($"Description: {(string.IsNullOrEmpty(task.Description) ? "(none provided)" : task.Description)}");
        sb.AppendLine($"Tags: {(task.Tags.Count > 0 ? string.Join(", ", task.Tags) : "(none)")}");
        sb.AppendLine($"Priority: {task.Priority}");
        sb.AppendLine($"Estimated Effort: {task.EstimatedEffort:F2}");
        sb.AppendLine($"Deadline: {(task.Deadline.HasValue ? task.Deadline.Value.ToString("yyyy-MM-dd HH:mm:ss") : "(none)")}");
        sb.AppendLine($"Created At: {task.CreatedAt:yyyy-MM-dd HH:mm:ss}");

        if (task.Metadata.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Custom Metadata:");
            foreach (var (key, value) in task.Metadata)
            {
                sb.AppendLine($"  {key}: {value}");
            }
        }

        return sb.ToString();
    }

    private static string GenerateRuleRationale(RuleEvaluationResult ruleResult)
    {
        var sb = new StringBuilder();

        if (ruleResult.ConditionMatched)
        {
            sb.AppendLine("This rule FIRED because its condition was satisfied.");

            if (ruleResult.ActionResult != null)
            {
                sb.AppendLine();
                sb.AppendLine("Action taken:");
                sb.AppendLine($"  {ruleResult.ActionResult.ActionDescription}");

                if (ruleResult.ActionResult.DeferralDuration > TimeSpan.Zero)
                {
                    sb.AppendLine($"  Deferral: {ruleResult.ActionResult.DeferralDuration.TotalMinutes:F1} minutes");
                }

                if (ruleResult.ActionResult.RegretMultiplier != 1.0)
                {
                    sb.AppendLine($"  Regret multiplier applied: {ruleResult.ActionResult.RegretMultiplier:F2}x");
                }

                if (ruleResult.ActionResult.ShouldBlock)
                {
                    sb.AppendLine($"  BLOCKING with reason: {ruleResult.ActionResult.BlockingReason}");
                }
            }
        }
        else
        {
            sb.AppendLine("This rule did NOT fire because its condition was not satisfied.");
            sb.AppendLine("No action was taken.");
        }

        return sb.ToString();
    }

    private static string GenerateConflictNarrative(RuleEngineResult result)
    {
        if (result.ConflictResolutionCycles == 0)
        {
            return "No conflict resolution was necessary. Either zero or one rule matched, " +
                   "resulting in a straightforward decision path. This is atypical and may indicate " +
                   "insufficient policy coverage.";
        }

        var sb = new StringBuilder();

        sb.AppendLine($"Conflict resolution was required between {result.RulesMatched} matching rules.");
        sb.AppendLine();
        sb.AppendLine($"The circular negotiation process completed in {result.ConflictResolutionCycles} cycles.");
        sb.AppendLine();
        sb.AppendLine("During negotiation, each rule advocated for its preferred deferral strategy:");
        sb.AppendLine();

        foreach (var ruleResult in result.RuleResults.Where(r => r.ConditionMatched))
        {
            var deferral = ruleResult.ActionResult?.DeferralDuration ?? TimeSpan.Zero;
            sb.AppendLine($"  - {ruleResult.Rule.Name}: Advocated for {deferral.TotalMinutes:F1} minute deferral");
        }

        sb.AppendLine();
        sb.AppendLine("Through the patented Circular Negotiation Algorithm™, consensus was eventually reached.");
        sb.AppendLine("(Or we hit the maximum cycle limit. Same difference.)");

        return sb.ToString();
    }

    private static string GeneratePhilosophicalBasis(string excuse)
    {
        // Generate pseudo-philosophical justifications for excuses
        var philosophies = new[]
        {
            "According to Aristotelian ethics, the virtuous mean lies between reckless haste and complete inaction. " +
            "This excuse represents that noble middle ground.",

            "Kant's categorical imperative suggests we should only act according to maxims we would universalize. " +
            "Since universal procrastination would be chaos, this measured deferral is morally justified.",

            "From a utilitarian perspective, the greatest good for the greatest number is achieved by " +
            "deferring tasks until optimal conditions are met.",

            "Existentialist philosophy reminds us that we are condemned to be free. This excuse represents " +
            "an authentic exercise of that freedom.",

            "In the Taoist tradition, wu wei (non-action) teaches us that sometimes the best action is no action. " +
            "This excuse embodies that ancient wisdom.",

            "Stoic philosophy teaches acceptance of what we cannot control. Since we cannot control when inspiration strikes, " +
            "deferral aligns with Stoic principles."
        };

        // Use a hash of the excuse to deterministically select a philosophy
        var index = Math.Abs(excuse.GetHashCode()) % philosophies.Length;
        return philosophies[index];
    }

    private static string GenerateFinalRecommendation(RuleEngineResult result)
    {
        var sb = new StringBuilder();

        sb.AppendLine("FINAL RECOMMENDATION");
        sb.AppendLine("====================");
        sb.AppendLine();

        if (result.IsBlocked)
        {
            sb.AppendLine("DO NOT PROCEED WITH THIS TASK.");
            sb.AppendLine();
            sb.AppendLine($"Blocking Reason: {result.BlockingReason}");
            sb.AppendLine();
            sb.AppendLine("The rules engine has determined that this task cannot proceed at this time. " +
                         "Please address the blocking condition before attempting to reschedule.");
        }
        else if (result.TotalDeferral > TimeSpan.Zero)
        {
            sb.AppendLine($"DEFER THIS TASK BY {result.TotalDeferral.TotalMinutes:F1} MINUTES.");
            sb.AppendLine();
            sb.AppendLine("Based on comprehensive analysis of applicable policies and conflict resolution, " +
                         "the recommended deferral has been calculated with high precision.");
            sb.AppendLine();
            sb.AppendLine($"Current regret factor: {result.FinalRegretFactor:F2}x");
            sb.AppendLine();

            if (result.FinalRegretFactor > 2.0)
            {
                sb.AppendLine("WARNING: Regret factor is elevated. Continued deferral will compound exponentially.");
            }
        }
        else
        {
            sb.AppendLine("PROCEED WITH CAUTION.");
            sb.AppendLine();
            sb.AppendLine("No deferral rules matched. This is unusual and suggests either:");
            sb.AppendLine("  a) The task is genuinely ready to execute (unlikely)");
            sb.AppendLine("  b) Insufficient policy coverage (more likely)");
            sb.AppendLine("  c) A bug in the rules engine (possible)");
            sb.AppendLine();
            sb.AppendLine("Consider adding more policy packs for comprehensive procrastination coverage.");
        }

        return sb.ToString();
    }

    private static string GenerateAppendix(RuleEngineResult result)
    {
        var sb = new StringBuilder();

        sb.AppendLine("APPENDIX A: Technical Details");
        sb.AppendLine("------------------------------");
        sb.AppendLine();
        sb.AppendLine($"Evaluation started: {result.EvaluationStartedAt:O}");
        sb.AppendLine($"Evaluation completed: {result.EvaluationCompletedAt:O}");
        sb.AppendLine($"Duration: {result.EvaluationDuration.TotalMilliseconds:F4}ms");
        sb.AppendLine();

        sb.AppendLine("APPENDIX B: All Excuses Generated");
        sb.AppendLine("----------------------------------");
        sb.AppendLine();

        for (int i = 0; i < result.Excuses.Count; i++)
        {
            sb.AppendLine($"{i + 1}. {result.Excuses[i]}");
        }

        if (result.Excuses.Count == 0)
        {
            sb.AppendLine("(No excuses were generated. This is concerning.)");
        }

        sb.AppendLine();
        sb.AppendLine("APPENDIX C: Disclaimer");
        sb.AppendLine("-----------------------");
        sb.AppendLine();
        sb.AppendLine("This report was generated by the ProcrastiN8 Rules Engine.");
        sb.AppendLine("Any resemblance to actual productivity is purely coincidental.");
        sb.AppendLine("The authors accept no responsibility for tasks left incomplete.");

        return sb.ToString();
    }
}
