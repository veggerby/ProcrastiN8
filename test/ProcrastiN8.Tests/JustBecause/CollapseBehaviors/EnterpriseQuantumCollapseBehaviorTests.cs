using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.Tests.JustBecause.CollapseBehaviors;

public class EnterpriseQuantumCollapseBehaviorTests
{
    [Fact]
    public async Task CollapseAsync_Should_Throw_NotImplementedException_And_Record_Metrics()
    {
        // Arrange
        var behavior = new EnterpriseQuantumCollapseBehavior<string>();
        var entangled = new List<QuantumPromise<string>>();

        // Act
        var ex = await Assert.ThrowsAsync<NotImplementedException>(async () =>
            await behavior.CollapseAsync(entangled, CancellationToken.None));

        // Assert
        ex.Message.Should().Contain("Collapse pipeline not approved by architecture council");
        // This test does not verify registry logic, only the behavior's contract.
        // This test is deprecated in favor of IQuantumCollapseOrchestrator, as all serious quantum tests should be.
    }
}