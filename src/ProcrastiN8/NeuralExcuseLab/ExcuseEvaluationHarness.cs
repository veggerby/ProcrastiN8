using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// Coordinates the evaluation of excuse models using quality scoring and metrics collection.
/// </summary>
/// <remarks>
/// Provides a unified interface for evaluating excuse models with BLEU-score simulation
/// and shame index classification. All evaluation is scientifically dubious.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="ExcuseEvaluationHarness"/> class.
/// </remarks>
/// <param name="scorer">The quality scorer to use.</param>
/// <param name="metrics">The metrics collector to use.</param>
/// <param name="randomProvider">Random provider for evaluation.</param>
/// <param name="delayProvider">Delay provider for simulating evaluation time.</param>
/// <param name="logger">Logger for evaluation operations.</param>
public class ExcuseEvaluationHarness(
    ExcuseQualityScorer? scorer = null,
    IExcuseMetrics? metrics = null,
    IRandomProvider? randomProvider = null,
    IDelayProvider? delayProvider = null,
    IProcrastiLogger? logger = null)
{
    private readonly ExcuseQualityScorer _scorer = scorer ?? new ExcuseQualityScorer(randomProvider, delayProvider, logger);
    private readonly IExcuseMetrics _metrics = metrics ?? new ExcuseMetricsCollector();
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;
    private readonly IProcrastiLogger? _logger = logger;

    /// <summary>
    /// Evaluates a generated excuse and records metrics.
    /// </summary>
    /// <param name="excuse">The excuse to evaluate.</param>
    /// <param name="modelName">The model that generated the excuse.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains evaluation results.</returns>
    public async Task<EvaluationResult> EvaluateExcuseAsync(string excuse, string modelName)
    {
        _logger?.Info($"[EvaluationHarness] Evaluating excuse from {modelName}...");

        var qualityScore = await _scorer.CalculateQualityScoreAsync(excuse);
        var shameIndex = await _scorer.CalculateShameIndexAsync(excuse);

        _metrics.RecordExcuse(excuse, modelName, qualityScore, shameIndex);

        _logger?.Info($"[EvaluationHarness] Evaluation complete - Quality: {qualityScore:F2}, Shame: {shameIndex:F2}");

        return new EvaluationResult(excuse, modelName, qualityScore, shameIndex);
    }

    /// <summary>
    /// Runs a benchmark comparing multiple excuse models.
    /// </summary>
    /// <param name="models">The models to benchmark.</param>
    /// <param name="iterationsPerModel">Number of excuses to generate per model.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains benchmark results.</returns>
    public async Task<BenchmarkResult> RunBenchmarkAsync(
        IEnumerable<IExcuseModel> models,
        int iterationsPerModel = 10,
        CancellationToken cancellationToken = default)
    {
        _logger?.Info($"[EvaluationHarness] Starting benchmark with {models.Count()} models, {iterationsPerModel} iterations each");

        var results = new List<EvaluationResult>();
        var modelList = models.ToList();

        foreach (var model in modelList)
        {
            _logger?.Info($"[EvaluationHarness] Benchmarking {model.ModelName}...");

            for (var i = 0; i < iterationsPerModel; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var excuse = await model.GenerateExcuseAsync("Generate an excuse", cancellationToken);
                var result = await EvaluateExcuseAsync(excuse, model.ModelName);
                results.Add(result);
            }
        }

        _logger?.Info($"[EvaluationHarness] Benchmark complete - {results.Count} excuses evaluated");

        return new BenchmarkResult(results, _metrics.GetAggregatedMetrics());
    }

    /// <summary>
    /// Gets the metrics collector for direct access.
    /// </summary>
    public IExcuseMetrics Metrics => _metrics;
}

/// <summary>
/// Represents the result of evaluating a single excuse.
/// </summary>
public record EvaluationResult(
    string Excuse,
    string ModelName,
    double QualityScore,
    double ShameIndex);

/// <summary>
/// Represents the results of a benchmark run.
/// </summary>
public record BenchmarkResult(
    IReadOnlyList<EvaluationResult> Results,
    IDictionary<string, object> AggregatedMetrics);
