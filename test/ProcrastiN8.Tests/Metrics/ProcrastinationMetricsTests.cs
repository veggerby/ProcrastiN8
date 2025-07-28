using ProcrastiN8.Metrics;

namespace ProcrastiN8.Tests.Metrics;

public class ProcrastinationMetricsTests
{
    [Fact]
    public void Metrics_AreAccessible()
    {
        // arrange
        // (no setup needed)

        // act
        // (no action needed)

        // assert
        Assert.NotNull(ProcrastinationMetrics.TotalTimeProcrastinated);
        Assert.NotNull(ProcrastinationMetrics.ExcusesGenerated);
        Assert.NotNull(ProcrastinationMetrics.DelaysTotal);
        Assert.NotNull(ProcrastinationMetrics.SnoozeDurations);
        Assert.NotNull(ProcrastinationMetrics.CommentaryTotal);
        Assert.NotNull(ProcrastinationMetrics.RetryAttempts);
        Assert.NotNull(ProcrastinationMetrics.TasksCompleted);
        Assert.NotNull(ProcrastinationMetrics.TasksNeverDone);
    }
}