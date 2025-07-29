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

        // Ensure predictable behavior for the test
        mockRandomProvider.GetDouble().Returns(0.05, 0.2, 0.05, 0.19, 0.3, 0.8); // Adjusted sequence of values

        string? result = null;
        Exception? exception = null;

        // Act
        try
        {
            result = await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(true), delayStrategy: mockDelayStrategy, randomProvider: mockRandomProvider);
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Assert
        if (exception is not null)
        {
            exception.Should().BeOfType<SuperpositionCollapseException>("sometimes, quantum indecision is fatal");
        }
        else
        {
            result.Should().NotBeNullOrWhiteSpace("the undecider must eventually decide, even if it's nonsense");
        }
    }

    [Fact]
    public async Task ObserveDecisionAsync_ThrowsOnCollapseTooEarly_Sometimes()
    {
        // Arrange
        var actualTriggered = 0;
        var mockDelayStrategy = Substitute.For<IDelayStrategy>();
        var mockRandomProvider = Substitute.For<IRandomProvider>();
        mockRandomProvider.GetDouble().Returns(0.2, 1, 1, 1, 0.05, 0.05); // Ensure specific sequence of values

        // Act
        for (var i = 0; i < 3; i++)
        {
            try
            {
                await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(false), delayStrategy: mockDelayStrategy, randomProvider: mockRandomProvider);
            }
            catch (CollapseTooEarlyException)
            {
                actualTriggered++;
            }
            catch
            {
            }
        }

        // Assert
        actualTriggered.Should().Be(2, "eventually, the quantum undecider must throw CollapseTooEarlyException (by design)");
        mockRandomProvider.Received(2).GetDouble(); // Ensure the mock is being used
    }

    [Fact]
    public async Task DelayUntilInevitabilityAsync_ReturnsOutcomeOrThrows()
    {
        // Arrange
        // (no setup needed)

        // Act
        string? result = null;
        Exception? exception = null;
        try { result = await QuantumUndecider.DelayUntilInevitabilityAsync(TimeSpan.FromMilliseconds(100)); }
        catch (Exception ex) { exception = ex; }

        // Assert
        if (exception is not null)
        {
            exception.Should().BeOfType<SuperpositionCollapseException>("sometimes, the universe just gives up");
        }
        else
        {
            result.Should().NotBeNullOrWhiteSpace("the undecider must eventually output something, even if it's nonsense");
        }
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