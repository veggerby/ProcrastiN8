using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.JustBecause.Interpretations;

/// <summary>
/// The Copenhagen interpretation: observation collapses the wavefunction. The act of measurement
/// determines the outcome. Before measurement, nothing is real — or at least, nothing can be discussed.
/// This is the interpretation taught in textbooks, which is itself suspicious.
/// </summary>
/// <remarks>
/// Key behaviours:
/// <list type="bullet">
/// <item>Observation affects the outcome — looking changes what is seen.</item>
/// <item>Parallel timelines are not considered real — only the observed result exists.</item>
/// <item>Tunnelling is marginal — possible in principle, improbable in practice.</item>
/// <item>Probability is objective and governed by the Born rule.</item>
/// </list>
/// </remarks>
public sealed class CopenhagenInterpretation(IRandomProvider? randomProvider = null) : IQuantumInterpretation
{
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;

    /// <inheritdoc />
    public string Name => "Copenhagen";

    /// <inheritdoc />
    public string Description => "Observation collapses the wavefunction. Reality is created by measurement. Very popular at conferences.";

    /// <inheritdoc />
    public bool ObservationAffectsOutcome => true;

    /// <inheritdoc />
    public bool ParallelTimelinesAreReal => false;

    /// <inheritdoc />
    public bool TunnellingPermitted => true;

    /// <inheritdoc />
    public double InterpretProbability(double rawSample, double statedProbability)
    {
        // Standard Born rule: stated probability is taken at face value.
        return statedProbability;
    }

    /// <inheritdoc />
    public ICollapseBehavior<T> GetCollapseBehavior<T>() =>
        new CopenhagenCollapseBehavior<T>(randomProvider: _randomProvider);
}
