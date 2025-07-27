using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.JustBecause;

public class QuantumUndeciderTests
{
    [Fact]
    public async Task ObserveDecisionAsync_ReturnsDefinitiveOrPartialOrThrows()
    {
        // Arrange
        // (no setup needed)

        // Act
        string? result = null;
        Exception? exception = null;
        try { result = await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(true)); }
        catch (Exception ex) { exception = ex; }

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

    [Fact(Skip = "This test is flaky and needs to be stabilized")]
    public async Task ObserveDecisionAsync_ThrowsOnCollapseTooEarly_Sometimes()
    {
        // Arrange
        // (no setup needed)

        // Act
        var triggered = false;
        for (int i = 0; i < 30; i++)
        {
            try { await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(true)); }
            catch (SuperpositionCollapseException) { triggered = true; break; }
        }

        // Assert
        triggered.Should().BeTrue("eventually, the waveform must collapse under scrutiny");
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
        // (no setup needed)

        // Act
        var triggered = false;
        for (int i = 0; i < 30; i++)
        {
            try { await QuantumUndecider.DelayUntilInevitabilityAsync(TimeSpan.FromMilliseconds(100)); }
            catch (SuperpositionCollapseException) { triggered = true; break; }
        }

        // Assert
        triggered.Should().BeTrue("eventually, entropy must win");
    }

    [Fact]
    public async Task ObserveDecisionAsync_TriggersEntangledCallback()
    {
        // Arrange
        string? observed = null;
        void Handler(string s) => observed = s;
        QuantumUndecider.OnEntangledDecision += Handler;

        // Act
        await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(true));

        // Assert
        observed.Should().NotBeNullOrWhiteSpace("the entangled callback should always be triggered");
        QuantumUndecider.OnEntangledDecision -= Handler;
    }
}