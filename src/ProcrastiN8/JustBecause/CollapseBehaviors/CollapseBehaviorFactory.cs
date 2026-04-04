using ProcrastiN8.JustBecause;

namespace ProcrastiN8.JustBecause.CollapseBehaviors;

/// <summary>
/// Factory for creating instances of <see cref="ICollapseBehavior{T}"/> based on the specified quantum compliance level
/// or a fully abstracted <see cref="IQuantumInterpretation"/>.
/// </summary>
/// <remarks>
/// <para>
/// Two entry points are provided:
/// <list type="bullet">
/// <item>
///     <see cref="Create{T}(QuantumComplianceLevel)"/> — selects a behavior by the legacy compliance level enum.
///     Use this when you know exactly which collapse strategy you want and have no philosophical objection to enums.
/// </item>
/// <item>
///     <see cref="Create{T}(IQuantumInterpretation)"/> — delegates to the interpretation's own
///     <see cref="IQuantumInterpretation.GetCollapseBehavior{T}"/> method.
///     Use this when you are letting a quantum interpretation govern the collapse semantics — which is the
///     recommended approach for all code written after the Copenhagen interpretation was popularised.
/// </item>
/// </list>
/// </para>
/// </remarks>
public static class CollapseBehaviorFactory
{
    /// <summary>
    /// Creates an instance of <see cref="ICollapseBehavior{T}"/> corresponding to the given quantum compliance level.
    /// </summary>
    /// <typeparam name="T">The type of the quantum value.</typeparam>
    /// <param name="level">The quantum compliance level that determines the behavior.</param>
    /// <returns>An instance of <see cref="ICollapseBehavior{T}"/>.</returns>
    public static ICollapseBehavior<T> Create<T>(QuantumComplianceLevel level)
    {
        return level switch
        {
            QuantumComplianceLevel.None => new SilentFailureCollapseBehavior<T>(),
            QuantumComplianceLevel.Entanglish => new RandomUnfairCollapseBehavior<T>(),
            QuantumComplianceLevel.Copenhagen => new CopenhagenCollapseBehavior<T>(),
            QuantumComplianceLevel.ManyWorlds => new ForkingCollapseBehavior<T>(),
            QuantumComplianceLevel.BellInequalityPlus => new SpookyActionCollapseBehavior<T>(),
            QuantumComplianceLevel.EnterpriseQuantum => new EnterpriseQuantumCollapseBehavior<T>(),
            QuantumComplianceLevel.ReverseEntropy => new ReverseEntropyCollapseBehavior<T>(),
            QuantumComplianceLevel.HeisenLogging => new HeisenLoggingCollapseBehavior<T>(),
            QuantumComplianceLevel.StringTheoryCollapse => new StringTheoryCollapseBehavior<T>(),
            _ => new SilentFailureCollapseBehavior<T>()
        };
    }

    /// <summary>
    /// Creates an instance of <see cref="ICollapseBehavior{T}"/> governed by the specified quantum interpretation.
    /// Delegates to <see cref="IQuantumInterpretation.GetCollapseBehavior{T}"/>, ensuring that the collapse
    /// semantics are consistent with the interpretation's worldview — whatever that worldview happens to be.
    /// </summary>
    /// <typeparam name="T">The type of the quantum value.</typeparam>
    /// <param name="interpretation">
    /// The quantum interpretation to query. Must not be null, because null is not a valid interpretation of quantum mechanics.
    /// </param>
    /// <returns>
    /// An <see cref="ICollapseBehavior{T}"/> consistent with the specified interpretation.
    /// The Copenhagen interpretation returns <see cref="CopenhagenCollapseBehavior{T}"/>;
    /// Many-Worlds returns <see cref="ForkingCollapseBehavior{T}"/>; and so on.
    /// The caller should not be surprised by the result, but often will be.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="interpretation"/> is null.
    /// Failing to provide an interpretation is itself a philosophical position, but not a supported one.
    /// </exception>
    public static ICollapseBehavior<T> Create<T>(IQuantumInterpretation interpretation)
    {
        ArgumentNullException.ThrowIfNull(interpretation);
        return interpretation.GetCollapseBehavior<T>();
    }
}