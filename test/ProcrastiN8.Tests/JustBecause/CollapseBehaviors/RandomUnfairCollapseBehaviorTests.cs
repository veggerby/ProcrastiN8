using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;
using ProcrastiN8.Tests.Common;

namespace ProcrastiN8.Tests.JustBecause.CollapseBehaviors;

public class RandomUnfairCollapseBehaviorTests
{
    [Fact]
    public async Task CollapseAsync_With_Empty_Set_Should_Throw()
    {
        // Arrange
        var behavior = new RandomUnfairCollapseBehavior<string>();

        // Act
        Func<Task> act = async () => await behavior.CollapseAsync(Array.Empty<IQuantumPromise<string>>(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task CollapseAsync_Should_Observe_One_And_Ripple_Others()
    {
        // Arrange
        var chosen = new PredictableQuantumPromise<string>("observed");
        var other = new PredictableQuantumPromise<string>("rippled");
        var entangled = new List<IQuantumPromise<string>> { chosen, other };
        var behavior = new RandomUnfairCollapseBehavior<string>();

        // Act
        var result = await behavior.CollapseAsync(entangled, CancellationToken.None);

        // Assert
        result.Should().Be("observed", "the chosen promise should be observed, others may ripple");
        // This test does not verify registry logic, only the behavior's contract.
        // The ripple is as unfair as the test itself, and as serious as quantum business logic.
    }
}
