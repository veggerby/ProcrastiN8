using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.JustBecause.Interpretations;

/// <summary>
/// QBism (Quantum Bayesianism): quantum mechanics is a tool for agents to update personal beliefs,
/// not a description of objective reality. Probabilities are subjective degrees of belief.
/// The wavefunction is an agent's handbook, not a physical object.
/// This interpretation is held with great personal conviction, which is the point.
/// </summary>
/// <remarks>
/// Key behaviours:
/// <list type="bullet">
/// <item>Observation affects outcome — but only from the agent's perspective; others may legitimately disagree.</item>
/// <item>Parallel timelines are not real — there is one reality, but each agent has their own probability handbook.</item>
/// <item>Tunnelling is permitted if the agent believes it is. Belief is load-bearing.</item>
/// <item>Probability is entirely subjective. Stated probability is the agent's personal Bayesian degree
///     of belief and is respected as such — but boosted slightly by self-confidence, which is also a variable.</item>
/// </list>
/// </remarks>
public sealed class QBistInterpretation(IRandomProvider? randomProvider = null) : IQuantumInterpretation
{
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;

    // How much the agent's stated probability is boosted by personal conviction (0–0.15 range)
    private const double ConfidenceBoost = 0.1;

    /// <inheritdoc />
    public string Name => "QBism";

    /// <inheritdoc />
    public string Description => "Quantum probabilities are personal Bayesian beliefs. The wavefunction is your handbook, not a physical object. Your confidence in success is a valid input.";

    /// <inheritdoc />
    public bool ObservationAffectsOutcome => true;

    /// <inheritdoc />
    public bool ParallelTimelinesAreReal => false;

    /// <inheritdoc />
    public bool TunnellingPermitted => true;

    /// <inheritdoc />
    public double InterpretProbability(double rawSample, double statedProbability)
    {
        // In QBism, probability is a personal belief. The agent's stated probability is
        // boosted by a fixed confidence term — because believing you will succeed
        // is, in this interpretation, a legitimate epistemic input.
        return Math.Clamp(statedProbability + ConfidenceBoost, 0.0, 1.0);
    }

    /// <inheritdoc />
    public ICollapseBehavior<T> GetCollapseBehavior<T>() =>
        new EnterpriseQuantumCollapseBehavior<T>();
}
