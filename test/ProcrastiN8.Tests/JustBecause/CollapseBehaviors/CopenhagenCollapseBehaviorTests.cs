using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.Tests.JustBecause.CollapseBehaviors;

public class CopenhagenCollapseBehaviorTests
{
    [Fact]
    public async Task CollapseAsync_Should_Return_Default_If_Not_Observed()
    {
        // Arrange
        var observer = Substitute.For<IObserverContext>();
        observer.IsObserved(Arg.Any<string>()).Returns(false);
        var behavior = new CopenhagenCollapseBehavior<string>(observer);
        var entangled = new List<QuantumPromise<string>>();

        // Act
        var result = await behavior.CollapseAsync(entangled, CancellationToken.None);

        // Assert
        result.Should().BeNull("if no one is watching, nothing collapses");
        // This test does not verify registry logic, only the behavior's contract.
        // The universe remains undecided until observed, as per quantum audit standards.
    }

    [Fact]
    public async Task CollapseAsync_Should_Observe_If_Observed()
    {
        // Arrange
        var observer = Substitute.For<IObserverContext>();
        observer.IsObserved(Arg.Any<string>()).Returns(true);
        var chosen = Substitute.For<IQuantumPromise<string>>();
        chosen.ObserveAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult("observed"));
        var entangled = new List<IQuantumPromise<string>> { chosen };
        var behavior = new CopenhagenCollapseBehavior<string>(observer);

        // Act
        var result = await behavior.CollapseAsync(entangled, CancellationToken.None);

        // Assert
        result.Should().Be("observed", "observation is the only way to collapse");
        await chosen.Received().ObserveAsync(Arg.Any<CancellationToken>());
        // This test does not verify registry logic, only the behavior's contract.
        // Schrödinger's audit: only observed collapses count, and only in serious tests.
    }
}