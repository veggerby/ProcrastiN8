using ProcrastiN8.Metrics;
using Xunit;

namespace ProcrastiN8.Tests.Metrics;

public class QuantumPromiseMetricsTests
{
    [Fact]
    public void Metrics_AreAccessible()
    {
        // arrange
        // (no setup needed)

        // act
        // (no action needed)

        // assert
        QuantumPromiseMetrics.Observations.Should().NotBeNull();
        QuantumPromiseMetrics.Collapses.Should().NotBeNull();
        QuantumPromiseMetrics.Failures.Should().NotBeNull();
        QuantumPromiseMetrics.CollapseDuration.Should().NotBeNull();
    }
}
