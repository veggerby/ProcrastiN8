using System;
using System.Threading;
using System.Threading.Tasks;

using ProcrastiN8.JustBecause;

using Xunit;

namespace ProcrastiN8.Tests.JustBecause
{
    public class QuantumUndeciderTests
    {
        [Fact]
        public async Task ObserveDecisionAsync_ReturnsDefinitiveOrPartial()
        {
            // Arrange
            // (no setup needed)

            // Act
            var result = await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(true));

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ObserveDecisionAsync_ThrowsOnCollapseTooEarly()
        {
            // Arrange
            // (no setup needed)

            // Act
            // This test is probabilistic, may not always trigger
            var triggered = false;
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(true));
                }
                catch (SuperpositionCollapseException)
                {
                    triggered = true;
                    break;
                }
            }

            // Assert
            triggered.Should().BeTrue();
        }

        [Fact]
        public async Task DelayUntilInevitabilityAsync_ReturnsOutcome()
        {
            // Arrange
            // (no setup needed)

            // Act
            var result = await QuantumUndecider.DelayUntilInevitabilityAsync(TimeSpan.FromMilliseconds(100));

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task DelayUntilInevitabilityAsync_ThrowsOnEntropyDecay()
        {
            // Arrange
            // (no setup needed)

            // Act
            // This test is probabilistic, may not always trigger
            var triggered = false;
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    await QuantumUndecider.DelayUntilInevitabilityAsync(TimeSpan.FromMilliseconds(100));
                }
                catch (SuperpositionCollapseException)
                {
                    triggered = true;
                    break;
                }
            }

            // Assert
            triggered.Should().BeTrue();
        }

        [Fact]
        public async Task ObserveDecisionAsync_TriggersEntangledCallback()
        {
            // Arrange
            string? observed = null;
            QuantumUndecider.OnEntangledDecision += s => observed = s;

            // Act
            await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(true));

            // Assert
            observed.Should().NotBeNullOrWhiteSpace();
            QuantumUndecider.OnEntangledDecision -= s => observed = s;
        }
    }
}