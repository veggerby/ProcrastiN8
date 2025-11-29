using System.Text;

namespace ProcrastiN8.RulesEngine;

/// <summary>
/// A comprehensive, verbose report explaining why a task was deferred.
/// </summary>
/// <remarks>
/// The explainability report is designed to be absurdly detailed,
/// providing a 12-page justification for procrastination decisions.
/// Perfect for compliance audits and blame deflection.
/// </remarks>
public sealed class ExplainabilityReport
{
    /// <summary>
    /// Gets or sets the report title.
    /// </summary>
    public string Title { get; set; } = "Procrastination Decision Explainability Report";

    /// <summary>
    /// Gets or sets the executive summary.
    /// </summary>
    public string ExecutiveSummary { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the task summary section.
    /// </summary>
    public string TaskSummary { get; set; } = string.Empty;

    /// <summary>
    /// Gets the rule evaluation details for each rule.
    /// </summary>
    public IList<RuleExplanationSection> RuleExplanations { get; } = new List<RuleExplanationSection>();

    /// <summary>
    /// Gets or sets the conflict resolution narrative.
    /// </summary>
    public string ConflictResolutionNarrative { get; set; } = string.Empty;

    /// <summary>
    /// Gets the excuses generated with their philosophical justifications.
    /// </summary>
    public IList<ExcuseJustification> ExcuseJustifications { get; } = new List<ExcuseJustification>();

    /// <summary>
    /// Gets or sets the final recommendation section.
    /// </summary>
    public string FinalRecommendation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the appendix containing additional details.
    /// </summary>
    public string Appendix { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the report was generated.
    /// </summary>
    public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the estimated page count (always aims for 12+ pages).
    /// </summary>
    public int EstimatedPageCount => Math.Max(12, CalculatePageCount());

    private int CalculatePageCount()
    {
        var totalChars = ExecutiveSummary.Length
            + TaskSummary.Length
            + RuleExplanations.Sum(r => r.DetailedExplanation.Length + r.Rationale.Length)
            + ConflictResolutionNarrative.Length
            + ExcuseJustifications.Sum(e => e.PhilosophicalBasis.Length)
            + FinalRecommendation.Length
            + Appendix.Length;

        // Assume approximately 2000 characters per page (generous margins for maximum padding)
        return (totalChars / 2000) + 1;
    }

    /// <summary>
    /// Generates the full report as a formatted string.
    /// </summary>
    /// <returns>The complete explainability report.</returns>
    public string ToFullReport()
    {
        var sb = new StringBuilder();

        sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
        sb.AppendLine($"  {Title}");
        sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
        sb.AppendLine();
        sb.AppendLine($"Generated: {GeneratedAt:yyyy-MM-dd HH:mm:ss UTC}");
        sb.AppendLine($"Estimated Pages: {EstimatedPageCount}");
        sb.AppendLine();

        sb.AppendLine("┌─────────────────────────────────────────────────────────────────────────┐");
        sb.AppendLine("│  EXECUTIVE SUMMARY                                                      │");
        sb.AppendLine("└─────────────────────────────────────────────────────────────────────────┘");
        sb.AppendLine();
        sb.AppendLine(ExecutiveSummary);
        sb.AppendLine();

        sb.AppendLine("┌─────────────────────────────────────────────────────────────────────────┐");
        sb.AppendLine("│  TASK SUMMARY                                                           │");
        sb.AppendLine("└─────────────────────────────────────────────────────────────────────────┘");
        sb.AppendLine();
        sb.AppendLine(TaskSummary);
        sb.AppendLine();

        sb.AppendLine("┌─────────────────────────────────────────────────────────────────────────┐");
        sb.AppendLine("│  RULE-BY-RULE ANALYSIS                                                  │");
        sb.AppendLine("└─────────────────────────────────────────────────────────────────────────┘");
        sb.AppendLine();

        foreach (var rule in RuleExplanations)
        {
            sb.AppendLine($"  ▶ Rule: {rule.RuleName} (ID: {rule.RuleId})");
            sb.AppendLine($"    Priority: {rule.Priority}");
            sb.AppendLine($"    Condition Matched: {(rule.ConditionMatched ? "YES" : "NO")}");
            sb.AppendLine($"    Detailed Explanation:");
            sb.AppendLine($"      {rule.DetailedExplanation}");
            sb.AppendLine($"    Rationale:");
            sb.AppendLine($"      {rule.Rationale}");
            sb.AppendLine();
        }

        sb.AppendLine("┌─────────────────────────────────────────────────────────────────────────┐");
        sb.AppendLine("│  CONFLICT RESOLUTION NARRATIVE                                          │");
        sb.AppendLine("└─────────────────────────────────────────────────────────────────────────┘");
        sb.AppendLine();
        sb.AppendLine(ConflictResolutionNarrative);
        sb.AppendLine();

        sb.AppendLine("┌─────────────────────────────────────────────────────────────────────────┐");
        sb.AppendLine("│  EXCUSE JUSTIFICATIONS                                                  │");
        sb.AppendLine("└─────────────────────────────────────────────────────────────────────────┘");
        sb.AppendLine();

        foreach (var excuse in ExcuseJustifications)
        {
            sb.AppendLine($"  ✦ Excuse: \"{excuse.Excuse}\"");
            sb.AppendLine($"    Source Rule: {excuse.SourceRule}");
            sb.AppendLine($"    Philosophical Basis:");
            sb.AppendLine($"      {excuse.PhilosophicalBasis}");
            sb.AppendLine();
        }

        sb.AppendLine("┌─────────────────────────────────────────────────────────────────────────┐");
        sb.AppendLine("│  FINAL RECOMMENDATION                                                   │");
        sb.AppendLine("└─────────────────────────────────────────────────────────────────────────┘");
        sb.AppendLine();
        sb.AppendLine(FinalRecommendation);
        sb.AppendLine();

        sb.AppendLine("┌─────────────────────────────────────────────────────────────────────────┐");
        sb.AppendLine("│  APPENDIX                                                               │");
        sb.AppendLine("└─────────────────────────────────────────────────────────────────────────┘");
        sb.AppendLine();
        sb.AppendLine(Appendix);
        sb.AppendLine();

        sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
        sb.AppendLine("  END OF REPORT");
        sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");

        return sb.ToString();
    }
}

/// <summary>
/// Represents a detailed explanation section for a single rule.
/// </summary>
public sealed class RuleExplanationSection
{
    /// <summary>
    /// Gets or sets the rule identifier.
    /// </summary>
    public string RuleId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rule name.
    /// </summary>
    public string RuleName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rule priority.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets whether the condition matched.
    /// </summary>
    public bool ConditionMatched { get; set; }

    /// <summary>
    /// Gets or sets the detailed explanation of the evaluation.
    /// </summary>
    public string DetailedExplanation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rationale for the rule's behavior.
    /// </summary>
    public string Rationale { get; set; } = string.Empty;
}

/// <summary>
/// Represents a philosophical justification for an excuse.
/// </summary>
public sealed class ExcuseJustification
{
    /// <summary>
    /// Gets or sets the excuse text.
    /// </summary>
    public string Excuse { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source rule that generated this excuse.
    /// </summary>
    public string SourceRule { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the philosophical basis for this excuse.
    /// </summary>
    public string PhilosophicalBasis { get; set; } = string.Empty;
}
