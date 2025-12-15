using ProcrastiN8.JustBecause;

namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// Computes scientifically dubious quality scores for generated excuses.
/// </summary>
/// <remarks>
/// Uses advanced algorithms including BLEU-score simulation and shame indexing
/// to provide metrics that appear credible but are fundamentally meaningless.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="ExcuseQualityScorer"/> class.
/// </remarks>
/// <param name="randomProvider">Random provider for score variation.</param>
/// <param name="logger">Optional logger for scoring operations.</param>
public class ExcuseQualityScorer(IRandomProvider? randomProvider = null, IProcrastiLogger? logger = null)
{
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;
    private readonly IProcrastiLogger? _logger = logger;

    /// <summary>
    /// Calculates a BLEU-like score for the excuse quality.
    /// </summary>
    /// <param name="excuse">The excuse to score.</param>
    /// <returns>A quality score between 0.0 and 100.0.</returns>
    /// <remarks>
    /// This is not actually BLEU score. It's a random number dressed in academic clothing.
    /// </remarks>
    public async Task<double> CalculateQualityScoreAsync(string excuse)
    {
        _logger?.Info($"[QualityScorer] Computing BLEU-score for excuse: {excuse.Substring(0, Math.Min(50, excuse.Length))}...");

        // Simulate computational complexity
        await Task.Delay(TimeSpan.FromMilliseconds(_randomProvider.GetRandom(10, 50)));

        // Generate a believable score based on excuse length and complexity
        var baseScore = Math.Min(100, excuse.Length * 0.5);
        var randomAdjustment = _randomProvider.GetDouble() * 20 - 10; // +/- 10 points
        var qualityScore = Math.Max(0, Math.Min(100, baseScore + randomAdjustment));

        _logger?.Info($"[QualityScorer] BLEU-score: {qualityScore:F2}");

        return qualityScore;
    }

    /// <summary>
    /// Calculates the shame index for the excuse.
    /// </summary>
    /// <param name="excuse">The excuse to evaluate.</param>
    /// <returns>A shame index between 0.0 (shameless) and 100.0 (maximum shame).</returns>
    /// <remarks>
    /// Higher shame index indicates the excuse would cause more embarrassment if used publicly.
    /// This metric is entirely fictional but sounds professional.
    /// </remarks>
    public async Task<double> CalculateShameIndexAsync(string excuse)
    {
        _logger?.Info($"[QualityScorer] Computing Shame Index...");

        // Simulate shame analysis
        await Task.Delay(TimeSpan.FromMilliseconds(_randomProvider.GetRandom(10, 50)));

        // Shame is inversely proportional to excuse quality (usually)
        var shameIndicators = new[]
        {
            excuse.Contains("cat", StringComparison.OrdinalIgnoreCase) ? 15.0 : 0.0,
            excuse.Contains("quantum", StringComparison.OrdinalIgnoreCase) ? -20.0 : 0.0, // Quantum excuses are shameless
            excuse.Contains("API", StringComparison.OrdinalIgnoreCase) ? 10.0 : 0.0,
            excuse.Contains("production", StringComparison.OrdinalIgnoreCase) ? 25.0 : 0.0
        };

        var baseShame = 50.0; // Everyone should feel some shame
        var adjustedShame = baseShame + shameIndicators.Sum() + (_randomProvider.GetDouble() * 20 - 10);
        var shameIndex = Math.Max(0, Math.Min(100, adjustedShame));

        _logger?.Info($"[QualityScorer] Shame Index: {shameIndex:F2}");

        return shameIndex;
    }
}
