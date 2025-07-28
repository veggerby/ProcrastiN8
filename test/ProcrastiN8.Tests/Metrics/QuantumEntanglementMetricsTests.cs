using ProcrastiN8.Metrics;

namespace ProcrastiN8.Tests.Metrics;

public class QuantumEntanglementMetricsTests
{
    [Fact]
    public void Metrics_AreAccessible()
    {
        // arrange
        // (no setup needed)

        // act
        // (no action needed)

        // assert
        QuantumEntanglementMetrics.Entanglements.Should().NotBeNull();
        QuantumEntanglementMetrics.Collapses.Should().NotBeNull();
        QuantumEntanglementMetrics.RippleAttempts.Should().NotBeNull();
        QuantumEntanglementMetrics.RippleFailures.Should().NotBeNull();
        QuantumEntanglementMetrics.CollapseLatency.Should().NotBeNull();
        QuantumEntanglementMetrics.Forks.Should().NotBeNull();
        QuantumEntanglementMetrics.ForkFailures.Should().NotBeNull();
    }
}