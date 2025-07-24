namespace ProcrastiN8.JustBecause.CollapseBehaviors;

public static class CollapseBehaviorFactory
{
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
            _ => new SilentFailureCollapseBehavior<T>()
        };
    }
}