using ProcrastiN8.Common;

namespace ProcrastiN8.RulesEngine.Actions;

/// <summary>
/// Defers a task by a fixed duration.
/// </summary>
/// <remarks>
/// Simple and predictable. Perfect for bureaucratic delays.
/// </remarks>
public sealed class FixedDeferralAction : IRuleAction
{
    private readonly TimeSpan _duration;
    private readonly string? _excuse;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedDeferralAction"/> class.
    /// </summary>
    /// <param name="duration">The fixed deferral duration.</param>
    /// <param name="excuse">Optional excuse to provide.</param>
    public FixedDeferralAction(TimeSpan duration, string? excuse = null)
    {
        _duration = duration;
        _excuse = excuse;
    }

    /// <inheritdoc />
    public Task<RuleActionResult> ExecuteAsync(RuleEvaluationContext context, CancellationToken cancellationToken)
    {
        return Task.FromResult(new RuleActionResult
        {
            DeferralDuration = _duration,
            Excuse = _excuse ?? ExcuseGenerator.GetRandomExcuse(context.RandomProvider),
            ActionDescription = $"Applied fixed deferral of {_duration.TotalMinutes:F1} minutes."
        });
    }

    /// <inheritdoc />
    public string Describe() => $"Defer by {_duration.TotalMinutes:F1} minutes (fixed).";
}

/// <summary>
/// Defers a task by an exponentially increasing amount based on "regret factor."
/// </summary>
/// <remarks>
/// The more you defer, the more you need to defer. It's the procrastination snowball effect.
/// Pattern: "defer by exponential regret"
/// </remarks>
public sealed class ExponentialRegretAction : IRuleAction
{
    private readonly TimeSpan _baseDuration;
    private readonly double _regretMultiplier;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialRegretAction"/> class.
    /// </summary>
    /// <param name="baseDuration">Base deferral duration before regret is applied.</param>
    /// <param name="regretMultiplier">How much the regret factor grows with each deferral.</param>
    public ExponentialRegretAction(TimeSpan baseDuration, double regretMultiplier = 1.5)
    {
        _baseDuration = baseDuration;
        _regretMultiplier = regretMultiplier;
    }

    /// <inheritdoc />
    public Task<RuleActionResult> ExecuteAsync(RuleEvaluationContext context, CancellationToken cancellationToken)
    {
        var effectiveDuration = TimeSpan.FromTicks((long)(_baseDuration.Ticks * context.RegretFactor));

        return Task.FromResult(new RuleActionResult
        {
            DeferralDuration = effectiveDuration,
            RegretMultiplier = _regretMultiplier,
            Excuse = $"Exponential regret demands {effectiveDuration.TotalMinutes:F1} more minutes. Regret factor now at {context.RegretFactor * _regretMultiplier:F2}x.",
            ActionDescription = $"Applied exponential regret deferral. Base: {_baseDuration.TotalMinutes:F1}m, " +
                              $"Effective: {effectiveDuration.TotalMinutes:F1}m, Next regret multiplier: {_regretMultiplier:F2}x."
        });
    }

    /// <inheritdoc />
    public string Describe() => $"Defer by exponential regret (base: {_baseDuration.TotalMinutes:F1}m, multiplier: {_regretMultiplier:F2}x).";
}

/// <summary>
/// Generates a random deferral within a range.
/// </summary>
/// <remarks>
/// Adds uncertainty to procrastination. You'll get to it... eventually... probably.
/// </remarks>
public sealed class RandomDeferralAction : IRuleAction
{
    private readonly TimeSpan _minimum;
    private readonly TimeSpan _maximum;

    /// <summary>
    /// Initializes a new instance of the <see cref="RandomDeferralAction"/> class.
    /// </summary>
    /// <param name="minimum">Minimum deferral duration.</param>
    /// <param name="maximum">Maximum deferral duration.</param>
    public RandomDeferralAction(TimeSpan minimum, TimeSpan maximum)
    {
        _minimum = minimum;
        _maximum = maximum;
    }

    /// <inheritdoc />
    public Task<RuleActionResult> ExecuteAsync(RuleEvaluationContext context, CancellationToken cancellationToken)
    {
        var range = _maximum - _minimum;
        var randomFactor = context.RandomProvider.GetDouble();
        var duration = _minimum + TimeSpan.FromTicks((long)(range.Ticks * randomFactor));

        return Task.FromResult(new RuleActionResult
        {
            DeferralDuration = duration,
            Excuse = ExcuseGenerator.GetRandomExcuse(context.RandomProvider),
            ActionDescription = $"Applied random deferral of {duration.TotalMinutes:F1} minutes (range: {_minimum.TotalMinutes:F1}-{_maximum.TotalMinutes:F1}m)."
        });
    }

    /// <inheritdoc />
    public string Describe() => $"Defer by random amount ({_minimum.TotalMinutes:F1}-{_maximum.TotalMinutes:F1} minutes).";
}

/// <summary>
/// Blocks the task entirely with a bureaucratic reason.
/// </summary>
/// <remarks>
/// Sometimes the best procrastination is refusing to acknowledge the task exists at all.
/// </remarks>
public sealed class BlockTaskAction : IRuleAction
{
    private readonly string _reason;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlockTaskAction"/> class.
    /// </summary>
    /// <param name="reason">The bureaucratic reason for blocking.</param>
    public BlockTaskAction(string reason)
    {
        _reason = reason ?? throw new ArgumentNullException(nameof(reason));
    }

    /// <inheritdoc />
    public Task<RuleActionResult> ExecuteAsync(RuleEvaluationContext context, CancellationToken cancellationToken)
    {
        return Task.FromResult(new RuleActionResult
        {
            ShouldBlock = true,
            BlockingReason = _reason,
            Excuse = _reason,
            ActionDescription = $"Task blocked: {_reason}"
        });
    }

    /// <inheritdoc />
    public string Describe() => $"Block task with reason: {_reason}";
}

/// <summary>
/// Defers based on how close the deadline is (panic-scaling).
/// </summary>
/// <remarks>
/// The closer the deadline, the more we defer. It's counterintuitive but very human.
/// </remarks>
public sealed class PanicScalingDeferralAction : IRuleAction
{
    private readonly TimeSpan _baseDuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="PanicScalingDeferralAction"/> class.
    /// </summary>
    /// <param name="baseDuration">Base deferral duration.</param>
    public PanicScalingDeferralAction(TimeSpan baseDuration)
    {
        _baseDuration = baseDuration;
    }

    /// <inheritdoc />
    public Task<RuleActionResult> ExecuteAsync(RuleEvaluationContext context, CancellationToken cancellationToken)
    {
        double panicFactor = 1.0;

        if (context.Task.Deadline.HasValue)
        {
            var timeUntilDeadline = context.Task.Deadline.Value - context.EvaluatedAt;
            if (timeUntilDeadline.TotalHours > 0 && timeUntilDeadline.TotalHours < 24)
            {
                // Panic increases as deadline approaches
                panicFactor = Math.Max(1.0, 24.0 / timeUntilDeadline.TotalHours);
            }
        }

        var effectiveDuration = TimeSpan.FromTicks((long)(_baseDuration.Ticks * panicFactor));

        return Task.FromResult(new RuleActionResult
        {
            DeferralDuration = effectiveDuration,
            Excuse = $"Panic level at {panicFactor:F1}x. Need {effectiveDuration.TotalMinutes:F1} more minutes to mentally prepare.",
            ActionDescription = $"Applied panic-scaling deferral. Panic factor: {panicFactor:F1}x, Duration: {effectiveDuration.TotalMinutes:F1}m."
        });
    }

    /// <inheritdoc />
    public string Describe() => $"Defer by panic-scaling (base: {_baseDuration.TotalMinutes:F1}m, scales with deadline proximity).";
}

/// <summary>
/// Adds a custom excuse without any deferral.
/// </summary>
/// <remarks>
/// Sometimes you just need an excuse, not more time.
/// </remarks>
public sealed class ExcuseOnlyAction : IRuleAction
{
    private readonly Func<RuleEvaluationContext, string> _excuseGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExcuseOnlyAction"/> class.
    /// </summary>
    /// <param name="excuse">The excuse to generate.</param>
    public ExcuseOnlyAction(string excuse)
    {
        _excuseGenerator = _ => excuse ?? throw new ArgumentNullException(nameof(excuse));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExcuseOnlyAction"/> class.
    /// </summary>
    /// <param name="excuseGenerator">A function to generate contextual excuses.</param>
    public ExcuseOnlyAction(Func<RuleEvaluationContext, string> excuseGenerator)
    {
        _excuseGenerator = excuseGenerator ?? throw new ArgumentNullException(nameof(excuseGenerator));
    }

    /// <inheritdoc />
    public Task<RuleActionResult> ExecuteAsync(RuleEvaluationContext context, CancellationToken cancellationToken)
    {
        var excuse = _excuseGenerator(context);

        return Task.FromResult(new RuleActionResult
        {
            Excuse = excuse,
            ActionDescription = $"Generated excuse: {excuse}"
        });
    }

    /// <inheritdoc />
    public string Describe() => "Generate excuse only (no deferral).";
}
