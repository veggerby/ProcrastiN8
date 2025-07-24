using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.Tests.JustBecause.CollapseBehaviors;

public class ForkingCollapseBehaviorTests
{
    [Fact]
    public async Task CollapseAsync_Should_Observe_And_Fork()
    {
        // Arrange
        var chosen = new QuantumPromise<string>(() => Task.FromResult("forked"), TimeSpan.FromSeconds(1));
        await Task.Delay(1100); // ensure not too early
        var entangled = new List<QuantumPromise<string>> { chosen };
        var behavior = new ForkingCollapseBehavior<string>();

        // Act
        var result = await behavior.CollapseAsync(entangled, CancellationToken.None);

        // Assert
        result.Should().Be("forked", "the Many-Worlds interpretation guarantees at least one result");
        // This test does not verify registry logic, only the behavior's contract.
        // This test exists in all possible worlds, but only one is audited.
    }
}