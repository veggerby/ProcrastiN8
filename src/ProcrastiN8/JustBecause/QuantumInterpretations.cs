using ProcrastiN8.JustBecause.Interpretations;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// A registry of well-known quantum mechanics interpretations, each implemented with appropriate
/// earnestness and an awareness that none of them are experimentally distinguishable.
/// </summary>
/// <remarks>
/// <para>
/// Use <see cref="Copenhagen"/> for standard quantum behaviour: observers affect outcomes,
/// parallel universes are discarded, and probability is exactly what it says on the label.
/// </para>
/// <para>
/// Use <see cref="ManyWorlds"/> if you believe all outcomes are equally real and declining to
/// cancel background tasks is a philosophical commitment rather than a resource leak.
/// </para>
/// <para>
/// Use <see cref="PilotWave"/> if you believe everything is deterministic and randomness
/// is simply inadequate bookkeeping.
/// </para>
/// <para>
/// Use <see cref="Relational"/> if you believe that truth is observer-dependent and would
/// like your software to document this formally.
/// </para>
/// <para>
/// Use <see cref="QBist"/> if probability is a personal belief system and you would like
/// a modest confidence boost.
/// </para>
/// <para>
/// Use <see cref="Transactional"/> if the future has already confirmed the outcome and
/// you are only going through the motions for procedural reasons.
/// </para>
/// </remarks>
public static class QuantumInterpretations
{
    /// <summary>
    /// The Copenhagen interpretation. Observation collapses the wavefunction. This is the default.
    /// Any quantum primitive that does not receive an explicit interpretation will behave as a Copenhagener.
    /// </summary>
    public static IQuantumInterpretation Copenhagen { get; } = new CopenhagenInterpretation();

    /// <summary>
    /// The Many-Worlds interpretation. All outcomes are real. Parallel timelines are not cancelled — they are equally valid.
    /// </summary>
    public static IQuantumInterpretation ManyWorlds { get; } = new ManyWorldsInterpretation();

    /// <summary>
    /// The Pilot Wave (de Broglie–Bohm) interpretation. Everything is deterministic.
    /// Randomness is insufficient bookkeeping. Probability collapses to 0 or 1.
    /// </summary>
    public static IQuantumInterpretation PilotWave { get; } = new PilotWaveInterpretation();

    /// <summary>
    /// The Relational interpretation (Rovelli). Facts are observer-relative.
    /// Different callers may legitimately obtain different truths from the same quantum primitive.
    /// </summary>
    public static IQuantumInterpretation Relational { get; } = new RelationalInterpretation();

    /// <summary>
    /// QBism (Quantum Bayesianism). Probabilities are personal degrees of belief.
    /// Stated probability is boosted by a modest confidence term.
    /// </summary>
    public static IQuantumInterpretation QBist { get; } = new QBistInterpretation();

    /// <summary>
    /// The Transactional interpretation. The future confirms the past via advanced waves.
    /// High-probability outcomes are amplified; low-probability outcomes decay.
    /// </summary>
    public static IQuantumInterpretation Transactional { get; } = new TransactionalInterpretation();

    /// <summary>
    /// Returns all registered well-known interpretations.
    /// </summary>
    public static IReadOnlyList<IQuantumInterpretation> All { get; } =
    [
        Copenhagen, ManyWorlds, PilotWave, Relational, QBist, Transactional
    ];

    /// <summary>
    /// Looks up an interpretation by name (case-insensitive).
    /// Returns <see cref="Copenhagen"/> when the name is not recognised,
    /// because defaulting to the most popular interpretation is itself a very Copenhagen move.
    /// </summary>
    /// <param name="name">The name of the desired interpretation.</param>
    /// <returns>The matching interpretation, or <see cref="Copenhagen"/> if none matches.</returns>
    public static IQuantumInterpretation ByName(string name) =>
        All.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        ?? Copenhagen;
}
