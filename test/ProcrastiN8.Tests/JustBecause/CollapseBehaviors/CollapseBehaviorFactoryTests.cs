using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.Tests.JustBecause.CollapseBehaviors;

public class CollapseBehaviorFactoryTests
{
    [Fact]
    public void Create_Should_Return_Correct_Behavior_For_Each_ComplianceLevel()
    {
        // arrange
        // (no setup needed)

        // act & assert
        CollapseBehaviorFactory.Create<object>(QuantumComplianceLevel.None)
            .Should().BeOfType<SilentFailureCollapseBehavior<object>>("None should yield silent failure");

        CollapseBehaviorFactory.Create<object>(QuantumComplianceLevel.Entanglish)
            .Should().BeOfType<RandomUnfairCollapseBehavior<object>>("Entanglish should yield random unfair");

        CollapseBehaviorFactory.Create<object>(QuantumComplianceLevel.Copenhagen)
            .Should().BeOfType<CopenhagenCollapseBehavior<object>>("Copenhagen should yield copenhagen");

        CollapseBehaviorFactory.Create<object>(QuantumComplianceLevel.ManyWorlds)
            .Should().BeOfType<ForkingCollapseBehavior<object>>("ManyWorlds should yield forking");

        CollapseBehaviorFactory.Create<object>(QuantumComplianceLevel.BellInequalityPlus)
            .Should().BeOfType<SpookyActionCollapseBehavior<object>>("BellInequalityPlus should yield spooky action");

        CollapseBehaviorFactory.Create<object>(QuantumComplianceLevel.EnterpriseQuantum)
            .Should().BeOfType<EnterpriseQuantumCollapseBehavior<object>>("EnterpriseQuantum should yield enterprise quantum");
    }
}
// This test is as exhaustive as a compliance audit, but with more quantum uncertainty.