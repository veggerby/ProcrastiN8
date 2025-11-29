using ProcrastiN8.RulesEngine.Actions;
using ProcrastiN8.RulesEngine.Conditions;

namespace ProcrastiN8.RulesEngine.Policies;

/// <summary>
/// A policy pack modeled after ISO 9001 quality management, but for procrastination.
/// </summary>
/// <remarks>
/// ISO-9001-Procrastination ensures your deferral processes are documented, auditable,
/// and consistently applied across all tasks. Non-conformities in procrastination
/// are tracked and addressed through corrective action procedures.
/// </remarks>
public sealed class Iso9001ProcrastinationPolicyPack : PolicyPackBase
{
    /// <inheritdoc />
    public override string Id => "ISO-9001-Procrastination";

    /// <inheritdoc />
    public override string Name => "ISO 9001: Quality Procrastination Management";

    /// <inheritdoc />
    public override string Description =>
        "A comprehensive procrastination management system based on ISO 9001 principles. " +
        "Ensures all deferral activities are documented, measured, and continuously improved upon.";

    /// <inheritdoc />
    public override Version Version => new(1, 0, 0);

    /// <summary>
    /// Initializes a new instance of the <see cref="Iso9001ProcrastinationPolicyPack"/> class.
    /// </summary>
    public Iso9001ProcrastinationPolicyPack()
    {
        Metadata["Standard"] = "ISO 9001:2015 (Procrastination Adaptation)";
        Metadata["CertificationBody"] = "International Standards for Organized Negligence";

        // Rule 1: Important tasks require documented justification before deferral
        AddRule(new ProcrastinationRule(
            "ISO-9001-PROC-001",
            "Document Control for Important Task Deferral",
            new TagContainsCondition(new[] { "important", "critical", "urgent" }),
            new FixedDeferralAction(
                TimeSpan.FromMinutes(30),
                "Deferral requires Form 27B/6: Request for Task Postponement. Please allow 30 minutes for processing."),
            priority: 10
        ));

        // Rule 2: High-effort tasks trigger quality review process
        AddRule(new ProcrastinationRule(
            "ISO-9001-PROC-002",
            "Quality Review Process for High-Effort Tasks",
            new EffortCondition(5.0, ComparisonOperator.GreaterThanOrEqual),
            new ExponentialRegretAction(TimeSpan.FromMinutes(15), 1.3),
            priority: 20
        ));

        // Rule 3: All tasks require baseline documentation delay
        AddRule(new ProcrastinationRule(
            "ISO-9001-PROC-003",
            "Baseline Documentation Compliance",
            new AlwaysTrueCondition(),
            new FixedDeferralAction(
                TimeSpan.FromMinutes(5),
                "Mandatory 5-minute documentation window as per Section 7.5.3.1 of the Procrastination Manual."),
            priority: 100
        ));
    }
}

/// <summary>
/// A policy pack for agile teams who pretend to be agile.
/// </summary>
/// <remarks>
/// Agile-But-Not-Really implements common enterprise "agile" anti-patterns as formalized
/// procrastination rules. Sprint planning takes all week, standups become marathons,
/// and every task needs a JIRA ticket before it can be deferred.
/// </remarks>
public sealed class AgileButNotReallyPolicyPack : PolicyPackBase
{
    /// <inheritdoc />
    public override string Id => "Agile-But-Not-Really";

    /// <inheritdoc />
    public override string Name => "Agile But Not Really: Enterprise Procrastination Framework";

    /// <inheritdoc />
    public override string Description =>
        "For organizations that adopted agile methodology but kept all their old processes. " +
        "Features sprint-based deferral, ceremony overhead, and mandatory retrospective excuses.";

    /// <inheritdoc />
    public override Version Version => new(2, 0, 0);

    /// <summary>
    /// Initializes a new instance of the <see cref="AgileButNotReallyPolicyPack"/> class.
    /// </summary>
    public AgileButNotReallyPolicyPack()
    {
        Metadata["Framework"] = "SAFe-ish (Scaled Agile Framework-ish)";
        Metadata["SprintLength"] = "2 weeks (but actually 3)";

        // Rule 1: Tasks without story points must wait for grooming
        AddRule(new ProcrastinationRule(
            "AGILE-001",
            "Ungroomed Backlog Item",
            new NotCondition(new TagContainsCondition(new[] { "groomed", "estimated", "pointed" })),
            new BlockTaskAction("Cannot proceed: This item has not been through backlog grooming. " +
                              "Please wait for the next grooming session (scheduled for never)."),
            priority: 5
        ));

        // Rule 2: Monday tasks defer to standup discussion
        AddRule(new ProcrastinationRule(
            "AGILE-002",
            "Monday Standup Syndrome",
            new DayOfWeekCondition(new[] { DayOfWeek.Monday }),
            new FixedDeferralAction(
                TimeSpan.FromMinutes(45),
                "Deferring to post-standup. The 15-minute standup will only take 45 minutes today."),
            priority: 15
        ));

        // Rule 3: Friday tasks defer to next sprint
        AddRule(new ProcrastinationRule(
            "AGILE-003",
            "Friday Sprint Boundary",
            new DayOfWeekCondition(new[] { DayOfWeek.Friday }),
            new FixedDeferralAction(
                TimeSpan.FromHours(72),
                "This would destabilize the sprint. Moving to next sprint planning."),
            priority: 10
        ));

        // Rule 4: Critical tasks require synchronous meeting
        AddRule(new ProcrastinationRule(
            "AGILE-004",
            "Synchronous Alignment Required",
            new TagContainsCondition(new[] { "critical", "blocker" }),
            new RandomDeferralAction(
                TimeSpan.FromMinutes(30),
                TimeSpan.FromMinutes(120)),
            priority: 20
        ));
    }
}

/// <summary>
/// A policy pack for emotional data protection and feelings compliance.
/// </summary>
/// <remarks>
/// GDPR-For-Feelings ensures that all task deferrals respect the emotional well-being
/// of the procrastinator. Tasks that might cause stress require consent before proceeding,
/// and uncomfortable feelings have the right to be forgotten.
/// </remarks>
public sealed class GdprForFeelingsPolicyPack : PolicyPackBase
{
    /// <inheritdoc />
    public override string Id => "GDPR-For-Feelings";

    /// <inheritdoc />
    public override string Name => "GDPR for Feelings: Emotional Data Protection Compliance";

    /// <inheritdoc />
    public override string Description =>
        "A comprehensive framework for protecting emotional well-being during task execution. " +
        "Implements consent-based deferral, the right to emotional erasure, and stress impact assessments.";

    /// <inheritdoc />
    public override Version Version => new(1, 2, 0);

    /// <summary>
    /// Initializes a new instance of the <see cref="GdprForFeelingsPolicyPack"/> class.
    /// </summary>
    public GdprForFeelingsPolicyPack()
    {
        Metadata["Regulation"] = "General Data Protection Regulation for Feelings (GDPR-F)";
        Metadata["DataProtectionOfficer"] = "Your Inner Critic (on vacation)";

        // Rule 1: Stressful tasks require consent
        AddRule(new ProcrastinationRule(
            "GDPR-F-001",
            "Stress Consent Requirement",
            new TagContainsCondition(new[] { "stressful", "anxiety-inducing", "overwhelming" }),
            new FixedDeferralAction(
                TimeSpan.FromMinutes(60),
                "This task processes emotional data. Under GDPR-F Article 6, explicit consent is required. " +
                "Please wait 60 minutes for the consent form to load."),
            priority: 5
        ));

        // Rule 2: High-priority tasks trigger impact assessment
        AddRule(new ProcrastinationRule(
            "GDPR-F-002",
            "Emotional Impact Assessment",
            new PriorityCondition(30, ComparisonOperator.LessThan),
            new ExponentialRegretAction(TimeSpan.FromMinutes(20), 1.4),
            priority: 15
        ));

        // Rule 3: Tasks approaching deadline invoke right to be forgotten
        AddRule(new ProcrastinationRule(
            "GDPR-F-003",
            "Right to Emotional Erasure",
            new DeadlineProximityCondition(TimeSpan.FromHours(24)),
            new ExcuseOnlyAction(ctx =>
                $"Under GDPR-F Article 17, you have the right to have uncomfortable feelings about " +
                $"'{ctx.Task.Name}' erased. Deferral is a form of emotional data protection."),
            priority: 25
        ));

        // Rule 4: All tasks require feelings privacy notice
        AddRule(new ProcrastinationRule(
            "GDPR-F-004",
            "Feelings Privacy Notice",
            new AlwaysTrueCondition(),
            new FixedDeferralAction(
                TimeSpan.FromMinutes(2),
                "This task may affect your emotional state. " +
                "By proceeding, you consent to experiencing feelings. 2-minute cool-off period initiated."),
            priority: 100
        ));
    }
}
