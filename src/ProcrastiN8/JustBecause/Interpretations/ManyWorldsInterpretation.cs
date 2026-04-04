using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.JustBecause.Interpretations;

/// <summary>
/// The Many-Worlds interpretation: every quantum event branches reality into parallel universes.
/// All outcomes happen. All timelines are equally real. The branch you inhabit is a matter
/// of perspective, not privilege.
/// </summary>
/// <remarks>
/// Key behaviours:
/// <list type="bullet">
/// <item>Observation does not collapse anything — all outcomes continue to exist simultaneously.</item>
/// <item>Parallel timelines are fully real — cancelling them is philosophically incorrect.</item>
/// <item>Tunnelling is universally permitted — in some universe, the barrier was never there.</item>
/// <item>Probability reflects the fraction of branches that succeed.</item>
/// </list>
/// </remarks>
public sealed class ManyWorldsInterpretation(IRandomProvider? randomProvider = null) : IQuantumInterpretation
{
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;

    /// <inheritdoc />
    public string Name => "Many-Worlds";

    /// <inheritdoc />
    public string Description => "Every quantum event branches reality. All outcomes are real. You simply inhabit one branch. The others are not your problem.";

    /// <inheritdoc />
    public bool ObservationAffectsOutcome => false;

    /// <inheritdoc />
    public bool ParallelTimelinesAreReal => true;

    /// <inheritdoc />
    public bool TunnellingPermitted => true;

    /// <inheritdoc />
    public double InterpretProbability(double rawSample, double statedProbability)
    {
        // Probability reflects the branching ratio — stated probability is the measure
        // of branches where success occurs. No adjustment needed.
        return statedProbability;
    }

    /// <inheritdoc />
    public ICollapseBehavior<T> GetCollapseBehavior<T>() =>
        new ForkingCollapseBehavior<T>(_randomProvider);
}
