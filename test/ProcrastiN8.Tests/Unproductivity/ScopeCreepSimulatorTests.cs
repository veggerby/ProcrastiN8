using ProcrastiN8.JustBecause;
using ProcrastiN8.Unproductivity;

namespace ProcrastiN8.Tests.Unproductivity;

public class ScopeCreepSimulatorTests
{
    [Fact]
    public void Constructor_AddsInitialRequirement()
    {
        // arrange + act
        var sim = new ScopeCreepSimulator(initialRequirement: "Make it work");

        // assert
        sim.RequirementCount.Should().Be(1, "the initial requirement is already in scope before any creep occurs");
        sim.CurrentScope.Should().Contain("Make it work");
    }

    [Fact]
    public void AddRequirement_ExpandsScope()
    {
        // arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0); // always picks the first expander

        var sim = new ScopeCreepSimulator(randomProvider: randomProvider);
        var initialCount = sim.RequirementCount;

        // act
        var added = sim.AddRequirement();

        // assert
        sim.RequirementCount.Should().Be(initialCount + 1, "scope only ever grows — it never shrinks");
        sim.CurrentScope.Should().Contain(added, "the added requirement appears in the current scope");
    }

    [Fact]
    public void AddRequirements_AddsMultiple_AtOnce()
    {
        // arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetRandom(Arg.Any<int>()).Returns(1);

        var sim = new ScopeCreepSimulator(randomProvider: randomProvider);

        // act — simulate a stakeholder alignment session
        var added = sim.AddRequirements(3);

        // assert — three new requirements added in one meeting, estimate unchanged
        added.Should().HaveCount(3, "the batch method adds exactly the requested number of requirements");
        sim.RequirementCount.Should().Be(4, "initial (1) + added (3) = 4");
    }

    [Fact]
    public void GetScopeSummary_ContainsRequirementCount()
    {
        // arrange
        var sim = new ScopeCreepSimulator(initialRequirement: "A simple request");

        // act
        var summary = sim.GetScopeSummary();

        // assert — the summary is suitable for stakeholder presentation
        summary.Should().Contain("1 requirement", "the summary reports the current scope size");
        summary.Should().Contain("A simple request", "the initial requirement appears in the summary");
        summary.Should().Contain("Q4", "delivery is always Q4, regardless of when this is read");
    }

    [Fact]
    public void CurrentScope_IsSnapshot_NotLiveReference()
    {
        // arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetRandom(Arg.Any<int>()).Returns(0);
        var sim = new ScopeCreepSimulator(randomProvider: randomProvider);

        // act — take snapshot, then expand
        var snapshot = sim.CurrentScope;
        sim.AddRequirement();

        // assert — snapshot is not affected by subsequent expansion (scope creep is continuous but snapshots are not)
        snapshot.Should().HaveCount(1, "the snapshot captures scope at the moment it was taken");
        sim.CurrentScope.Should().HaveCount(2, "the live scope has grown since the snapshot");
    }
}
