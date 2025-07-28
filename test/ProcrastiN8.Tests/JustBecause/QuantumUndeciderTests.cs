using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.JustBecause;

public class QuantumUndeciderTests
{
    [Fact]
    public async Task ObserveDecisionAsync_ReturnsDefinitiveOrPartialOrThrows()
    {
        // Arrange
        var mockRandomProvider = Substitute.For<IRandomProvider>();
        QuantumUndecider.SetRandomProvider(mockRandomProvider);

        // Ensure predictable behavior for the test
        mockRandomProvider.NextDouble().Returns(0.05, 0.2, 0.05, 0.19, 0.3, 0.8); // Adjusted sequence of values
        mockRandomProvider.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(2); // Index for "It depends."

        string? result = null;
        Exception? exception = null;

        // Act
        try
        {
            result = await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(true));
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
        var triggered = false;
        var mockRandomProvider = Substitute.For<IRandomProvider>();
        QuantumUndecider.SetRandomProvider(mockRandomProvider);
        mockRandomProvider.NextDouble().Returns(0.2, 0.05, 0.05); // Ensure specific sequence of values

        // Act
        for (int i = 0; i < 30; i++)
        {
            try
            {
                await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(false));
            }
            catch (CollapseTooEarlyException)
            {
                triggered = true;
                break;
            }
            catch { /* ignore other exceptions for this test */ }
        }

        // Assert
        triggered.Should().BeTrue("eventually, the quantum undecider must throw CollapseTooEarlyException (by design)");
        mockRandomProvider.Received().NextDouble(); // Ensure the mock is being used
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
        QuantumUndecider.SetRandomProvider(mockRandomProvider);

        // Ensure entropy collapse condition is triggered
        mockRandomProvider.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(100); // Arbitrary delay
        mockRandomProvider.NextDouble().Returns(0.1); // Trigger entropy collapse

        var logger = Substitute.For<IProcrastiLogger>();

        // Act
        Func<Task> act = async () => await QuantumUndecider.DelayUntilInevitabilityAsync(
            TimeSpan.FromSeconds(1), logger);

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
            QuantumUndecider.SetRandomProvider(mockRandomProvider);

            // Ensure callback is triggered by aligning with the "It depends." decision state
            mockRandomProvider.NextDouble().Returns(0.2); // Ensure partial decision condition is met
            mockRandomProvider.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(2); // Index for "It depends."

            // Act
            await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(true));

            // Assert
            observed.Should().Be("It depends.", "the entangled callback should always be triggered with the correct state");
        }
        finally
        {
            QuantumUndecider.OnEntangledDecision -= Handler;
        }
    }
}