using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;
using ProcrastiN8.Tests.Common;

namespace ProcrastiN8.Tests.JustBecause.CollapseBehaviors;

public class ForkingCollapseBehaviorTests
{
    [Fact]
    public async Task CollapseAsync_With_Empty_Set_Should_Throw()
    {
        // Arrange
        var behavior = new ForkingCollapseBehavior<string>();

        // Act
        Func<Task> act = async () => await behavior.CollapseAsync(Array.Empty<IQuantumPromise<string>>(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task CollapseAsync_Should_Observe_And_Fork()
    {
        // Arrange
        var chosen = new PredictableQuantumPromise<string>("forked");
        var entangled = new List<IQuantumPromise<string>> { chosen };
        var behavior = new ForkingCollapseBehavior<string>();

        // Act
        var result = await behavior.CollapseAsync(entangled, CancellationToken.None);

        // Assert
        result.Should().Be("forked", "the Many-Worlds interpretation guarantees at least one result");
        // This test does not verify registry logic, only the behavior's contract.
        // This test exists in all possible worlds, but only one is audited.
    }
}
