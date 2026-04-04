using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.JustBecause.Interpretations;

/// <summary>
/// The Relational interpretation (Rovelli): quantum states are not absolute but relative
/// to each observer. There is no view from nowhere. All facts are relational, contextual,
/// and perfectly self-consistent within each reference frame — just not across them.
/// This is also how most corporate communications work.
/// </summary>
/// <remarks>
/// Key behaviours:
/// <list type="bullet">
/// <item>Observation profoundly affects outcome — specifically, <em>who</em> observes matters,
///     not just whether observation occurs. Different observers get legitimately different facts.</item>
/// <item>Parallel timelines are not real — there is only one physical universe, but facts
///     within it are observer-relative.</item>
/// <item>Tunnelling is permitted — barriers are facts too, and facts are relational.</item>
/// <item>Probability is relational — the same event may be probable for one observer
///     and improbable for another, depending on their informational state. Applied here as
///     a modest perturbation on the stated probability.</item>
/// </list>
/// </remarks>
public sealed class RelationalInterpretation(IRandomProvider? randomProvider = null) : IQuantumInterpretation
{
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;

    /// <inheritdoc />
    public string Name => "Relational";

    /// <inheritdoc />
    public string Description => "Quantum states are relative to observers. All facts are contextual. There is no privileged view. The helpdesk does not accept tickets about this.";

    /// <inheritdoc />
    public bool ObservationAffectsOutcome => true;

    /// <inheritdoc />
    public bool ParallelTimelinesAreReal => false;

    /// <inheritdoc />
    public bool TunnellingPermitted => true;

    /// <inheritdoc />
    public double InterpretProbability(double rawSample, double statedProbability)
    {
        // In the Relational interpretation, probability is observer-dependent.
        // Apply a small relational perturbation — the same event has a slightly different
        // probability from each perspective. The perturbation is ±0.05 (±5 percentage points),
        // derived from the raw sample's deviation from 0.5.
        var perturbation = (rawSample - 0.5) * 0.1; // ±0.05 perturbation on stated probability
        return Math.Clamp(statedProbability + perturbation, 0.0, 1.0);
    }

    /// <inheritdoc />
    public ICollapseBehavior<T> GetCollapseBehavior<T>() =>
        new SpookyActionCollapseBehavior<T>();
}
