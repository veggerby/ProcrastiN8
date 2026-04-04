using ProcrastiN8.JustBecause.CollapseBehaviors;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.JustBecause.Interpretations;

/// <summary>
/// The Transactional interpretation (Cramer): quantum events are agreements between
/// an "offer wave" from the source and a "confirmation wave" from the absorber.
/// The future reaches back to confirm the past. The result is agreed before it happens.
/// This is also how retrospectives work.
/// </summary>
/// <remarks>
/// Key behaviours:
/// <list type="bullet">
/// <item>Observation affects outcome — transactions require both offer and confirmation.</item>
/// <item>Parallel timelines are not real — there is one timeline, but it is negotiated
///     across its entire extent simultaneously.</item>
/// <item>Tunnelling is permitted via advanced waves from the future confirming the penetration retroactively.</item>
/// <item>Probability reflects the amplitude of the handshake between offer and confirmation waves.
///     Applied here as a slight amplification of high probabilities and suppression of low ones,
///     simulating the reinforcement of successful transactions.</item>
/// </list>
/// </remarks>
public sealed class TransactionalInterpretation(ITimeProvider? timeProvider = null, IRandomProvider? randomProvider = null) : IQuantumInterpretation
{
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;
    private readonly ITimeProvider _timeProvider = timeProvider ?? SystemTimeProvider.Default;

    /// <inheritdoc />
    public string Name => "Transactional";

    /// <inheritdoc />
    public string Description => "Quantum events are transactions between offer and confirmation waves across time. The future has already confirmed what is about to happen. Retrocausality is a feature.";

    /// <inheritdoc />
    public bool ObservationAffectsOutcome => true;

    /// <inheritdoc />
    public bool ParallelTimelinesAreReal => false;

    /// <inheritdoc />
    public bool TunnellingPermitted => true;

    /// <inheritdoc />
    public double InterpretProbability(double rawSample, double statedProbability)
    {
        // In the Transactional interpretation, probability is reinforced by the handshake amplitude.
        // High probabilities are amplified toward certainty; low probabilities decay toward zero.
        // Applied: p' = p^(2/3) for p > 0.5, p' = p^(3/2) for p <= 0.5
        if (statedProbability > 0.5)
        {
            return Math.Clamp(Math.Pow(statedProbability, 2.0 / 3.0), 0.0, 1.0);
        }

        return Math.Clamp(Math.Pow(statedProbability, 3.0 / 2.0), 0.0, 1.0);
    }

    /// <inheritdoc />
    public ICollapseBehavior<T> GetCollapseBehavior<T>() =>
        new ReverseEntropyCollapseBehavior<T>(_timeProvider, _randomProvider);
}
