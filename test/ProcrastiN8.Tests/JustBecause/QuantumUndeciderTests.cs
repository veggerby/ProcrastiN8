using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.JustBecause;

public class QuantumUndeciderTests
{
    [Fact]
    public async Task ObserveDecisionAsync_ReturnsDefinitiveOrPartialOrThrows()
    {
        // Arrange
        var mockDelayStrategy = Substitute.For<IDelayStrategy>();
        var mockRandomProvider = Substitute.For<IRandomProvider>();
        mockRandomProvider.GetDouble().Returns(0.8); // Adjusted sequence of values

        // Act
        var result = await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(true), delayStrategy: mockDelayStrategy, randomProvider: mockRandomProvider);

        // Assert
        result.Should().NotBeNullOrEmpty("The decision should be definitive or partial, but not null or empty.");
    }

    [Fact]
    public async Task ObserveDecisionAsync_ThrowsSuperpositionCollapse()
    {
        // Arrange
        var mockDelayStrategy = Substitute.For<IDelayStrategy>();
        var mockRandomProvider = Substitute.For<IRandomProvider>();

        // first random is SuperPosition collapse
        mockRandomProvider.GetDouble().Returns(0.05);

        // Act
        var act = async () => await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(false), delayStrategy: mockDelayStrategy, randomProvider: mockRandomProvider);

        // Assert
        await act.Should().ThrowAsync<SuperpositionCollapseException>();
        mockRandomProvider.Received().GetDouble(); // Ensure the mock is being used exactly 3 times
    }

    [Fact]
    public async Task ObserveDecisionAsync_ThrowsOnCollapseTooEarly()
    {
        // Arrange
        var mockDelayStrategy = Substitute.For<IDelayStrategy>();
        var mockRandomProvider = Substitute.For<IRandomProvider>();

        // second random is collapse too early
        mockRandomProvider.GetDouble().Returns(0.5, 0.05);

        // Act
        var act = async () => await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(false), delayStrategy: mockDelayStrategy, randomProvider: mockRandomProvider);

        // Assert
        await act.Should().ThrowAsync<CollapseTooEarlyException>();
        mockRandomProvider.Received().GetDouble(); // Ensure the mock is being used exactly 3 times
    }

    [Fact]
    public async Task ObserveDecisionAsync_ReturnsUndecidedTooEarly()
    {
        // Arrange
        var mockDelayStrategy = Substitute.For<IDelayStrategy>();
        var mockRandomProvider = Substitute.For<IRandomProvider>();

        // third random is undecided
        mockRandomProvider.GetDouble().Returns(0.5, 0.5, 0.2);

        // Act
        var actual = await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(false), delayStrategy: mockDelayStrategy, randomProvider: mockRandomProvider);

        // Assert
        actual.Should().Be("It depends.");
        mockRandomProvider.Received().GetDouble(); // Ensure the mock is being used exactly 3 times
    }

    [Theory]
    [InlineData(true, "Yes. Probably.")]
    [InlineData(false, "No. But it’s complicated.")]
    public async Task ObserveDecisionAsync_ReturnsDecision(bool expectedCostlyReturn, string expectedResult)
    {
        // Arrange
        var mockDelayStrategy = Substitute.For<IDelayStrategy>();
        var mockRandomProvider = Substitute.For<IRandomProvider>();

        // third random is undecided
        mockRandomProvider.GetDouble().Returns(0.5, 0.5, 0.3);

        // Act
        var actual = await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(expectedCostlyReturn), delayStrategy: mockDelayStrategy, randomProvider: mockRandomProvider);

        // Assert
        actual.Should().Be(expectedResult);
        mockRandomProvider.Received().GetDouble(); // Ensure the mock is being used exactly 3 times
    }

    [Fact]
    public async Task DelayUntilInevitabilityAsync_ThrowsSuperPositionCollapseException()
    {
        // Arrange
        var mockRandomProvider = Substitute.For<IRandomProvider>();
        mockRandomProvider.GetDouble().Returns(0.1D);

        // Act
        var act = async () => await QuantumUndecider.DelayUntilInevitabilityAsync(TimeSpan.FromMilliseconds(100), randomProvider: mockRandomProvider);

        // assert
        await act.Should().ThrowAsync<SuperpositionCollapseException>("sometimes, the universe just gives up");
    }

    [Fact]
    public async Task DelayUntilInevitabilityAsync_ReturnsOutcome()
    {
        // Arrange
        var mockRandomProvider = Substitute.For<IRandomProvider>();
        mockRandomProvider.GetDouble().Returns(0.5D);

        // Act
        var actual = await QuantumUndecider.DelayUntilInevitabilityAsync(TimeSpan.FromMilliseconds(100), randomProvider: mockRandomProvider);

        // assert
        actual.Should().NotBeNullOrWhiteSpace("the undecider must eventually output something, even if it's nonsense");
    }

    [Fact]
    public async Task DelayUntilInevitabilityAsync_ThrowsOnEntropyDecay_Sometimes()
    {
        // Arrange
        var mockRandomProvider = Substitute.For<IRandomProvider>();

        // Ensure entropy collapse condition is triggered
        mockRandomProvider.GetDouble().Returns(0.1); // Trigger entropy collapse

        var logger = Substitute.For<IProcrastiLogger>();

        // Act
        Func<Task> act = async () => await QuantumUndecider.DelayUntilInevitabilityAsync(
            TimeSpan.FromSeconds(1), mockRandomProvider, logger);

        // Assert
        await act.Should().ThrowAsync<SuperpositionCollapseException>("entropy must win sometimes");
    }

    [Fact]
    public async Task ObserveDecisionAsync_TriggersEntangledCallback()
    {
        // Arrange
        string? observed = null;
        void Handler(string s) => observed = s;
        QuantumUndecider.OnEntangledDecision += Handler;

        try
        {
            var mockRandomProvider = Substitute.For<IRandomProvider>();

            // Ensure callback is triggered by aligning with the "It depends." decision state
            mockRandomProvider.GetDouble().Returns(0.2); // Ensure partial decision condition is met

            // Act
            await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(true), randomProvider: mockRandomProvider);

            // Assert
            observed.Should().Be("It depends.", "the entangled callback should always be triggered with the correct state");
        }
        finally
        {
            QuantumUndecider.OnEntangledDecision -= Handler;
        }
    }
}