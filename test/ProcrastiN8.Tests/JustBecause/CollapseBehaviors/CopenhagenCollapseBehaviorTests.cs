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
        var randomProvider = Substitute.For<ProcrastiN8.JustBecause.IRandomProvider>();
        randomProvider.GetDouble().Returns(0); // Always pick the first
        var behavior = new CopenhagenCollapseBehavior<string>(observer, randomProvider);
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
        var randomProvider = Substitute.For<ProcrastiN8.JustBecause.IRandomProvider>();
        randomProvider.GetDouble().Returns(0); // Always pick the first
        var behavior = new CopenhagenCollapseBehavior<string>(observer, randomProvider);

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
        var nonCopenhagen = Substitute.For<IQuantumPromise<string>>();
        nonCopenhagen.ObserveAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult("other"));

        // To ensure deterministic selection, only include 'chosen' as the first element
        var entangled = new List<IQuantumPromise<string>>
        {
            chosen,
            copenhagen1,
            copenhagen2,
            nonCopenhagen
        };
        var randomProvider = Substitute.For<ProcrastiN8.JustBecause.IRandomProvider>();
        randomProvider.GetDouble().Returns(0); // Always pick the first
        var behavior = new CopenhagenCollapseBehavior<string>(observer, randomProvider);

        // Act
        var result = await behavior.CollapseAsync(entangled, CancellationToken.None);

        // Assert
        result.Should().Be("collapse-value", "the chosen value should dictate the fate of all entangled promises");
        await copenhagen1.Received().CollapseToValueAsync("collapse-value", Arg.Any<CancellationToken>());
        await copenhagen2.Received().CollapseToValueAsync("collapse-value", Arg.Any<CancellationToken>());
        await nonCopenhagen.Received().ObserveAsync(Arg.Any<CancellationToken>());
        // All entangled promises must yield to the Copenhagen consensus, except those that refuse to comply.
    }
}