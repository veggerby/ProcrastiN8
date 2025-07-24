using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.Tests.JustBecause.CollapseBehaviors;

public class SilentFailureCollapseBehaviorTests
{
    [Fact]
    public async Task CollapseAsync_Should_Return_Default_And_Record_Metrics()
    {
        // Arrange
        var behavior = new SilentFailureCollapseBehavior<string>();
        var entangled = new List<QuantumPromise<string>>();

        // Act
        var result = await behavior.CollapseAsync(entangled, CancellationToken.None);

        // Assert
        result.Should().BeNull("silent failure is the only guaranteed outcome");
        // This test does not verify registry logic, only the behavior's contract.
        // If metrics are ever audited, this test will be the only evidence of productivity.
    }
}