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
        var entangled = new List<IQuantumPromise<string>>();

        // Act
        var result = await behavior.CollapseAsync(entangled, CancellationToken.None);

        // Assert
        result.Should().BeNull("if no one is watching, nothing collapses");
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
        // Schrödinger's audit: only observed collapses count, and only in serious tests.
    }

    [Fact]
    public async Task CollapseAsync_Should_Collapse_All_Entangled_To_Same_Value()
    {
        // Arrange
        var observer = Substitute.For<IObserverContext>();
        observer.IsObserved(Arg.Any<string>()).Returns(true);
        var chosen = Substitute.For<IQuantumPromise<string>>();
        chosen.ObserveAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult("collapse-value"));

        var copenhagen1 = Substitute.For<ICopenhagenCollapsible<string>>();
        var copenhagen2 = Substitute.For<ICopenhagenCollapsible<string>>();
        // Non-collapsible promise
        var nonCopenhagen = Substitute.For<IQuantumPromise<string>>();
        nonCopenhagen.ObserveAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult("other"));

        // The entangled set must be IQuantumPromise<string>, but copenhagen1/2 must also be castable to ICopenhagenCollapsible<string>
        var entangled = new List<IQuantumPromise<string>>
        {
            chosen,
            copenhagen1,
            copenhagen2,
            nonCopenhagen
        };
        var behavior = new CopenhagenCollapseBehavior<string>(observer);

        // Patch the random selection to always pick the first (chosen) promise
        // This is necessary because the behavior picks randomly
        // We'll use a test double for Random if the implementation allows, otherwise we check all possible outcomes
        // For now, we will run the test multiple times to increase coverage
        var observed = false;
        for (int i = 0; i < 10; i++)
        {
            // Act
            var result = await behavior.CollapseAsync(entangled, CancellationToken.None);

            // Assert
            if (result == "collapse-value")
            {
                observed = true;
                await copenhagen1.Received().CollapseToValueAsync("collapse-value", Arg.Any<CancellationToken>());
                await copenhagen2.Received().CollapseToValueAsync("collapse-value", Arg.Any<CancellationToken>());
                await nonCopenhagen.Received().ObserveAsync(Arg.Any<CancellationToken>());
                break;
            }
        }
        observed.Should().BeTrue("the chosen value should dictate the fate of all entangled promises at least once");
        // All entangled promises must yield to the Copenhagen consensus, except those that refuse to comply.
    }
}