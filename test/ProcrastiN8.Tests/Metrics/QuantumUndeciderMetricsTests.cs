using ProcrastiN8.Metrics;

namespace ProcrastiN8.Tests.Metrics;

public class QuantumUndeciderMetricsTests
{
    [Fact]
    public void Metrics_AreAccessible()
    {
        // arrange
        // (no setup needed)

        // act
        // (no action needed)

        // assert
        QuantumUndeciderMetrics.Observations.Should().NotBeNull();
        QuantumUndeciderMetrics.Failures.Should().NotBeNull();
        QuantumUndeciderMetrics.Outcomes.Should().NotBeNull();
        QuantumUndeciderMetrics.DecisionLatency.Should().NotBeNull();
    }
}
