using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// Defines the behavioural contract for a quantum mechanics interpretation.
/// Each interpretation governs how quantum phenomena — observation, superposition, tunnelling,
/// probability, and collapse — behave across all ProcrastiN8 quantum primitives.
/// </summary>
/// <remarks>
/// <para>
/// There are at least a dozen competing interpretations of quantum mechanics, none of which
/// are experimentally distinguishable and all of which are held with considerable passion.
/// This interface models exactly as much of that disagreement as is useful for procrastination.
/// </para>
/// <para>
/// To obtain a standard interpretation, use <see cref="QuantumInterpretations"/>.
/// To invent your own, implement this interface and carry the philosophical consequences.
/// </para>
/// </remarks>
public interface IQuantumInterpretation
{
    /// <summary>Gets the canonical name of this interpretation.</summary>
    string Name { get; }

    /// <summary>Gets a brief, earnest description suitable for a conference badge.</summary>
    string Description { get; }

    /// <summary>
    /// Whether observation by a caller changes the observed outcome.
    /// <c>true</c> in Copenhagen; <c>false</c> in Many-Worlds, where all outcomes exist regardless.
    /// </summary>
    bool ObservationAffectsOutcome { get; }

    /// <summary>
    /// Whether parallel execution timelines are considered equally real.
    /// <c>true</c> in Many-Worlds; <c>false</c> in Copenhagen (only one outcome survives).
    /// </summary>
    bool ParallelTimelinesAreReal { get; }

    /// <summary>
    /// Whether quantum tunnelling through exception barriers is permitted.
    /// <c>true</c> in interpretations that accept barrier penetration; <c>false</c> where classical authority is respected.
    /// </summary>
    bool TunnellingPermitted { get; }

    /// <summary>
    /// Adjusts a raw probability sample according to this interpretation's Born-rule variant.
    /// Receives a value in [0,1) and returns an adjusted value in [0,1).
    /// Implementations may treat probability as objective, subjective, Bayesian, or wholly imaginary.
    /// </summary>
    /// <param name="rawSample">The raw probability sample from an <see cref="IRandomProvider"/>.</param>
    /// <param name="statedProbability">The probability as declared by the caller.</param>
    /// <returns>The effective probability under this interpretation, in [0,1].</returns>
    double InterpretProbability(double rawSample, double statedProbability);

    /// <summary>
    /// Returns the <see cref="ICollapseBehavior{T}"/> that governs collapse of entangled
    /// <see cref="QuantumPromise{T}"/> instances under this interpretation.
    /// </summary>
    ICollapseBehavior<T> GetCollapseBehavior<T>();
}
