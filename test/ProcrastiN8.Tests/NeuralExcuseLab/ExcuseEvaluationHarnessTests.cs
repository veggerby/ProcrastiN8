using ProcrastiN8.JustBecause;
using ProcrastiN8.NeuralExcuseLab;

namespace ProcrastiN8.Tests.NeuralExcuseLab;

public class ExcuseEvaluationHarnessTests
{
    [Fact]
    public async Task EvaluateExcuseAsync_Should_ReturnEvaluationResult()
    {
        // arrange
        var harness = new ExcuseEvaluationHarness();

        // act
        var result = await harness.EvaluateExcuseAsync("Test excuse", "TestModel");

        // assert
        result.Excuse.Should().Be("Test excuse");
        result.ModelName.Should().Be("TestModel");
        result.QualityScore.Should().BeGreaterThan(0.0, "excuses must have some quality, even if dubious");
        result.ShameIndex.Should().BeGreaterThan(0.0, "all excuses carry shame");
    }

    [Fact]
    public async Task EvaluateExcuseAsync_Should_RecordMetrics()
    {
        // arrange
        var metrics = new ExcuseMetricsCollector();
        var harness = new ExcuseEvaluationHarness(metrics: metrics);

        // act
        await harness.EvaluateExcuseAsync("Test excuse", "TestModel");
        var aggregated = harness.Metrics.GetAggregatedMetrics();

        // assert
        aggregated["total_excuses"].Should().Be(1);
    }

    [Fact]
    public async Task RunBenchmarkAsync_Should_EvaluateMultipleModels()
    {
        // arrange
        var model1 = new LocalExcuseModel();
        var model2 = new FortuneCookieExcuseModel();
        var harness = new ExcuseEvaluationHarness();

        // act
        var benchmark = await harness.RunBenchmarkAsync([model1, model2], iterationsPerModel: 2);

        // assert
        benchmark.Results.Should().HaveCount(4, "2 models * 2 iterations each");
        benchmark.AggregatedMetrics["total_excuses"].Should().Be(4);
    }

    [Fact]
    public async Task RunBenchmarkAsync_WithCancellation_Should_StopEarly()
    {
        // arrange
        var model = new LocalExcuseModel();
        var harness = new ExcuseEvaluationHarness();

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // act
        var benchmark = await harness.RunBenchmarkAsync([model], iterationsPerModel: 10, cts.Token);

        // assert
        benchmark.Results.Should().HaveCount(0, "benchmark should be cancelled immediately");
    }

    [Fact]
    public async Task RunBenchmarkAsync_Should_IncludeAggregatedMetrics()
    {
        // arrange
        var model = new LocalExcuseModel();
        var harness = new ExcuseEvaluationHarness();

        // act
        var benchmark = await harness.RunBenchmarkAsync([model], iterationsPerModel: 3);

        // assert
        benchmark.AggregatedMetrics.Should().ContainKey("total_excuses");
        benchmark.AggregatedMetrics.Should().ContainKey("avg_quality_score");
        benchmark.AggregatedMetrics.Should().ContainKey("avg_shame_index");
    }
}
