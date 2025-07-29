namespace ProcrastiN8.JustBecause.CollapseBehaviors;

/// <summary>
/// Factory for creating instances of <see cref="ICollapseBehavior{T}"/> based on the specified quantum compliance level.
/// </summary>
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
}