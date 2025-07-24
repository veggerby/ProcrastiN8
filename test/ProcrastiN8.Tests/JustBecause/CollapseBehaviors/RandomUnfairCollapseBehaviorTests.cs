using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.Tests.JustBecause.CollapseBehaviors;

public class RandomUnfairCollapseBehaviorTests
{
    [Fact]
    public async Task CollapseAsync_Should_Observe_One_And_Ripple_Others()
    {
        // Arrange
        var chosen = new QuantumPromise<string>(() => Task.FromResult("observed"), TimeSpan.FromSeconds(1));
        var other = new QuantumPromise<string>(() => Task.FromResult("rippled"), TimeSpan.FromSeconds(1));
        await Task.Delay(1100); // ensure not too early
        var entangled = new List<QuantumPromise<string>> { chosen, other };
        var behavior = new RandomUnfairCollapseBehavior<string>();

        // Act
        var result = await behavior.CollapseAsync(entangled, CancellationToken.None);

        // Assert
        result.Should().Be("observed", "the chosen promise should be observed, others may ripple");
        // This test does not verify registry logic, only the behavior's contract.
        // The ripple is as unfair as the test itself, and as serious as quantum business logic.
    }
}