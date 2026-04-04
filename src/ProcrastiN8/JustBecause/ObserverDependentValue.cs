using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// A value that returns a different result depending on who is observing it.
/// This is consistent with the Copenhagen interpretation, in which the act of observation
/// fundamentally alters the observed system — and also with most enterprise software.
/// </summary>
/// <remarks>
/// <para>
/// Each caller receives a value tailored to their identity. This is not a bug.
/// It is a feature of reality, properly modelled.
/// </para>
/// <para>
/// Values are registered by caller member name. When the same member name calls from multiple
/// call sites, it receives the same registered value each time — because the universe is
/// at least internally consistent, even if it is not honest.
/// </para>
/// </remarks>
/// <typeparam name="T">The type of the value that exists differently for each observer.</typeparam>
public sealed class ObserverDependentValue<T>
{
    private readonly Dictionary<string, T> _observerValues = new(StringComparer.Ordinal);
    private readonly T _defaultValue;
    private readonly IQuantumInterpretation _interpretation;
    private readonly IProcrastiLogger? _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ObserverDependentValue{T}"/> with a default value
    /// returned to observers who have not been specifically targeted.
    /// </summary>
    /// <param name="defaultValue">The value returned to unregistered, anonymous, or insufficiently important callers.</param>
    /// <param name="interpretation">
    /// The quantum mechanics interpretation governing observation behaviour.
    /// Defaults to <see cref="QuantumInterpretations.Copenhagen"/>.
    /// Under <see cref="QuantumInterpretations.ManyWorlds"/>, observation does not affect outcome —
    /// all callers receive the default value regardless of registration.
    /// Under <see cref="QuantumInterpretations.PilotWave"/>, outcome is predetermined and cannot be
    /// altered by observer-specific registration.
    /// </param>
    /// <param name="logger">Optional logger for observation event commentary.</param>
    public ObserverDependentValue(T defaultValue, IQuantumInterpretation? interpretation = null, IProcrastiLogger? logger = null)
    {
        _defaultValue = defaultValue;
        _interpretation = interpretation ?? QuantumInterpretations.Copenhagen;
        _logger = logger;
    }

    /// <summary>
    /// Registers a specific value to be returned when the named observer calls <see cref="Observe"/>.
    /// </summary>
    /// <param name="observerName">The caller member name to target. Case-sensitive.</param>
    /// <param name="value">The value that observer will receive. They will not know others receive different values.</param>
    /// <returns>This instance, for fluent registration.</returns>
    public ObserverDependentValue<T> For(string observerName, T value)
    {
        _observerValues[observerName] = value;
        return this;
    }

    /// <summary>
    /// Observes the value. The result depends on who is calling.
    /// </summary>
    /// <param name="callerMemberName">Automatically populated with the caller's method name. Do not override this.</param>
    /// <returns>
    /// A value tailored to the caller's identity. Unregistered callers receive the default value,
    /// which is still technically a value.
    /// </returns>
    public T Observe([CallerMemberName] string? callerMemberName = null)
    {
        var observerKey = callerMemberName ?? "<unknown>";

        if (!_interpretation.ObservationAffectsOutcome)
        {
            _logger?.Debug(
                "[ObserverDependentValue] Interpretation '{Interpretation}' holds that observation does not affect outcome. Returning default for all observers.",
                _interpretation.Name);
            return _defaultValue;
        }

        if (_observerValues.TryGetValue(observerKey, out var specificValue))
        {
            _logger?.Debug("[ObserverDependentValue] Observer '{Observer}' receives their specific truth ({Interpretation}).", observerKey, _interpretation.Name);
            return specificValue;
        }

        _logger?.Debug("[ObserverDependentValue] Unknown observer '{Observer}' receives the default truth. All truths are valid ({Interpretation}).", observerKey, _interpretation.Name);
        return _defaultValue;
    }

    /// <summary>Gets the quantum interpretation governing this instance's observation behaviour.</summary>
    public IQuantumInterpretation Interpretation => _interpretation;

    /// <summary>
    /// Returns the number of registered observer-specific values.
    /// </summary>
    public int RegisteredObserverCount => _observerValues.Count;

    /// <summary>
    /// Returns the names of all registered observers.
    /// </summary>
    public IReadOnlyCollection<string> RegisteredObservers => _observerValues.Keys;
}
