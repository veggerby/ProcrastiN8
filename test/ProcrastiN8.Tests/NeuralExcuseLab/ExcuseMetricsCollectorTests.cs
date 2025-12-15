using ProcrastiN8.NeuralExcuseLab;

namespace ProcrastiN8.Tests.NeuralExcuseLab;

public class ExcuseMetricsCollectorTests
{
    [Fact]
    public void RecordExcuse_Should_StoreExcuseMetrics()
    {
        // arrange
        var collector = new ExcuseMetricsCollector();

        // act
        collector.RecordExcuse("Test excuse", "TestModel", 85.5, 42.3);
        var metrics = collector.GetAggregatedMetrics();

        // assert
        metrics["total_excuses"].Should().Be(1);
    }

    [Fact]
    public void GetAggregatedMetrics_Should_CalculateAverages()
    {
        // arrange
        var collector = new ExcuseMetricsCollector();
        collector.RecordExcuse("Excuse 1", "Model1", 80.0, 40.0);
        collector.RecordExcuse("Excuse 2", "Model1", 90.0, 50.0);

        // act
        var metrics = collector.GetAggregatedMetrics();

        // assert
        metrics["avg_quality_score"].Should().Be(85.0);
        metrics["avg_shame_index"].Should().Be(45.0);
    }

    [Fact]
    public void GetAggregatedMetrics_OnEmptyCollector_Should_ReturnZeros()
    {
        // arrange
        var collector = new ExcuseMetricsCollector();

        // act
        var metrics = collector.GetAggregatedMetrics();

        // assert
        metrics["total_excuses"].Should().Be(0);
        metrics["avg_quality_score"].Should().Be(0.0);
        metrics["avg_shame_index"].Should().Be(0.0);
    }

    [Fact]
    public void GetMetricsByModel_Should_FilterByModel()
    {
        // arrange
        var collector = new ExcuseMetricsCollector();
        collector.RecordExcuse("Excuse 1", "Model1", 80.0, 40.0);
        collector.RecordExcuse("Excuse 2", "Model2", 90.0, 50.0);
        collector.RecordExcuse("Excuse 3", "Model1", 85.0, 45.0);

        // act
        var metrics = collector.GetMetricsByModel("Model1");

        // assert
        metrics["excuse_count"].Should().Be(2);
        metrics["avg_quality_score"].Should().Be(82.5, "average of 80 and 85");
    }

    [Fact]
    public void GetAggregatedMetrics_Should_IncludeMinMaxValues()
    {
        // arrange
        var collector = new ExcuseMetricsCollector();
        collector.RecordExcuse("Excuse 1", "Model1", 60.0, 30.0);
        collector.RecordExcuse("Excuse 2", "Model1", 90.0, 80.0);

        // act
        var metrics = collector.GetAggregatedMetrics();

        // assert
        metrics["min_quality_score"].Should().Be(60.0);
        metrics["max_quality_score"].Should().Be(90.0);
        metrics["min_shame_index"].Should().Be(30.0);
        metrics["max_shame_index"].Should().Be(80.0);
    }
}
