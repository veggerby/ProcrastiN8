using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.JustBecause.Interpretations;

/// <summary>
/// The de Broglie–Bohm Pilot Wave interpretation: particles have definite positions at all times,
/// guided by an invisible pilot wave. The universe is deterministic. There is no randomness —
/// only insufficient information about initial conditions, which is also the root cause of most production bugs.
/// </summary>
/// <remarks>
/// Key behaviours:
/// <list type="bullet">
/// <item>Observation does not change the outcome — the result was determined at initialisation.</item>
/// <item>Parallel timelines are not real — there is exactly one trajectory, guided by the wave.</item>
/// <item>Tunnelling occurs, but via the pilot wave pushing the particle through — not by probability.</item>
/// <item>Probability is epistemic only — reflecting ignorance, not fundamental randomness.
///     Effective probability is therefore always 1.0 or 0.0 — the outcome was fixed; we just don't know which.</item>
/// </list>
/// </remarks>
public sealed class PilotWaveInterpretation : IQuantumInterpretation
{
    /// <inheritdoc />
    public string Name => "Pilot Wave";

    /// <inheritdoc />
    public string Description => "Particles have definite trajectories guided by an invisible wave. Deterministic. Nonlocal. Deeply inconvenient for unit tests.";

    /// <inheritdoc />
    public bool ObservationAffectsOutcome => false;

    /// <inheritdoc />
    public bool ParallelTimelinesAreReal => false;

    /// <inheritdoc />
    public bool TunnellingPermitted => true;

    /// <inheritdoc />
    public double InterpretProbability(double rawSample, double statedProbability)
    {
        // In the Pilot Wave interpretation, all randomness is epistemic.
        // The outcome is fixed; probability merely reflects our ignorance.
        // Round to the nearest classical truth: if the stated probability is ≥ 0.5, assume success.
        return statedProbability >= 0.5 ? 1.0 : 0.0;
    }

    /// <inheritdoc />
    public ICollapseBehavior<T> GetCollapseBehavior<T>() =>
        new RandomUnfairCollapseBehavior<T>();
}
