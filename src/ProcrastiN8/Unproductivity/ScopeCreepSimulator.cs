using System.Collections.Concurrent;

using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Unproductivity;

/// <summary>
/// Perpetually expands the scope of a task by appending random new requirements.
/// The task is never complete because the scope is never final.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ScopeCreepSimulator"/> is the honest architectural model for most software projects.
/// It starts with a clear, bounded requirement and methodically ensures that clarity is temporary.
/// </para>
/// <para>
/// New requirements are added with procedural rigor and enterprise-grade naming conventions.
/// None of them reduce the scope. None of them have acceptance criteria.
/// </para>
/// </remarks>
public sealed class ScopeCreepSimulator
{
    private static readonly string[] ScopeExpanders =
    [
        "Add support for dark mode",
        "Also support light mode, but optionally",
        "Internationalisation (just the framework — translations later, probably never)",
        "Offline support",
        "Real-time sync across all devices",
        "Undo/redo for all user actions",
        "Export to PDF",
        "Import from Excel",
        "A public REST API (versioned)",
        "A GraphQL endpoint as well, just in case",
        "Admin dashboard",
        "Audit log for all admin actions",
        "Two-factor authentication",
        "SSO with at least three providers",
        "Accessibility (WCAG AA, retroactively)",
        "Performance optimisation pass",
        "A mobile app (native)",
        "Also a web app",
        "And a desktop app, for power users",
        "Notification system (email, push, SMS, carrier pigeon)",
        "AI-powered recommendations",
        "Configurable AI thresholds",
        "Explainable AI (the AI must justify itself)",
        "Support for legacy browsers",
        "Progressive web app capabilities",
        "Blockchain-based audit trail (compliance mandate)",
        "White-label support",
        "Multi-tenant architecture",
        "Per-tenant configuration overrides",
        "Feature flags for every feature",
        "A/B testing framework for the feature flags",
    ];

    private readonly ConcurrentBag<string> _requirements = [];
    private readonly IRandomProvider _randomProvider;
    private readonly IProcrastiLogger? _logger;

    /// <summary>
    /// Gets a point-in-time snapshot of the current requirement set.
    /// Note: by the time you read this, the scope has already grown.
    /// </summary>
    public IReadOnlyList<string> CurrentScope => [.. _requirements];

    /// <summary>
    /// Initializes a new instance of <see cref="ScopeCreepSimulator"/>.
    /// </summary>
    /// <param name="initialRequirement">The deceptively simple original requirement.</param>
    /// <param name="randomProvider">Injectable random source for requirement selection.</param>
    /// <param name="logger">Optional logger for scope update notifications.</param>
    public ScopeCreepSimulator(
        string initialRequirement = "Make it work",
        IRandomProvider? randomProvider = null,
        IProcrastiLogger? logger = null)
    {
        _randomProvider = randomProvider ?? RandomProvider.Default;
        _logger = logger;
        _requirements.Add(initialRequirement);
        _logger?.Info("[ScopeCreep] Scope initialised: '{Requirement}'. Estimated effort: 2 weeks.", initialRequirement);
    }

    /// <summary>
    /// Appends a randomly selected new requirement to the scope.
    /// This is always described as "just a small addition."
    /// </summary>
    /// <returns>The new requirement that was added.</returns>
    public string AddRequirement()
    {
        var newRequirement = ScopeExpanders[_randomProvider.GetRandom(ScopeExpanders.Length)];
        _requirements.Add(newRequirement);
        _logger?.Info("[ScopeCreep] Scope expanded: '{Requirement}' added. Estimate unchanged.", newRequirement);
        return newRequirement;
    }

    /// <summary>
    /// Adds a specified number of requirements in one stakeholder-pleasing session.
    /// </summary>
    /// <param name="count">Number of new requirements to generate. There is no upper bound. Consider this a suggestion.</param>
    /// <returns>The requirements that were added.</returns>
    public IReadOnlyList<string> AddRequirements(int count)
    {
        var added = new List<string>(count);
        for (var i = 0; i < count; i++)
        {
            added.Add(AddRequirement());
        }

        _logger?.Warn("[ScopeCreep] {Count} requirement(s) added in one session. Delivery date still Q4.", count);
        return added;
    }

    /// <summary>
    /// Returns a formal scope summary suitable for presentation to stakeholders who were not
    /// present when the original requirements were agreed upon.
    /// </summary>
    /// <returns>A formatted scope summary.</returns>
    public string GetScopeSummary()
    {
        var scope = CurrentScope;
        var lines = new System.Text.StringBuilder();
        lines.AppendLine($"## Current Scope ({scope.Count} requirement(s))");
        lines.AppendLine();
        for (var i = 0; i < scope.Count; i++)
        {
            lines.AppendLine($"{i + 1}. {scope[i]}");
        }

        lines.AppendLine();
        lines.AppendLine($"*Estimated delivery: still Q4. Scope subject to further clarification.*");
        return lines.ToString();
    }

    /// <summary>
    /// Returns the number of requirements currently in scope.
    /// </summary>
    public int RequirementCount => _requirements.Count;
}
