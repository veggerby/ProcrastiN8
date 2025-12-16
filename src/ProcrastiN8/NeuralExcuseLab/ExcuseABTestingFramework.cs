using ProcrastiN8.JustBecause;

namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// Coordinates A/B testing of excuse prompts with statistical significance theater.
/// </summary>
/// <remarks>
/// Provides enterprise-grade A/B testing for excuse generation strategies.
/// Statistical significance is calculated but fundamentally meaningless.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="ExcuseABTestingFramework"/> class.
/// </remarks>
/// <param name="randomProvider">Random provider for variant selection.</param>
/// <param name="logger">Logger for A/B test reporting.</param>
public class ExcuseABTestingFramework(IRandomProvider? randomProvider = null, IProcrastiLogger? logger = null)
{
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;
    private readonly IProcrastiLogger? _logger = logger;
    private readonly Dictionary<string, VariantMetrics> _variants = new();
    private readonly object _lock = new();

    private record VariantMetrics(
        string VariantName,
        int Impressions,
        double TotalQualityScore,
        double TotalShameIndex);

    /// <summary>
    /// Registers a variant for A/B testing.
    /// </summary>
    /// <param name="variantName">The name of the variant (e.g., "control", "variant_a").</param>
    public void RegisterVariant(string variantName)
    {
        lock (_lock)
        {
            if (!_variants.ContainsKey(variantName))
            {
                _variants[variantName] = new VariantMetrics(variantName, 0, 0.0, 0.0);
            }
        }
    }

    /// <summary>
    /// Selects a variant for the current request using random distribution.
    /// </summary>
    /// <returns>The selected variant name.</returns>
    public string SelectVariant()
    {
        lock (_lock)
        {
            if (_variants.Count == 0)
            {
                throw new InvalidOperationException("No variants registered for A/B testing");
            }

            var variantNames = _variants.Keys.ToArray();
            var selectedIndex = _randomProvider.GetRandom(variantNames.Length);
            var selected = variantNames[selectedIndex];

            return selected;
        }
    }

    /// <summary>
    /// Records the result of an A/B test impression.
    /// </summary>
    /// <param name="variantName">The variant that was shown.</param>
    /// <param name="qualityScore">The quality score of the generated excuse.</param>
    /// <param name="shameIndex">The shame index of the generated excuse.</param>
    public void RecordImpression(string variantName, double qualityScore, double shameIndex)
    {
        lock (_lock)
        {
            if (!_variants.TryGetValue(variantName, out var current))
            {
                throw new ArgumentException($"Variant '{variantName}' not registered", nameof(variantName));
            }

            var updated = current with
            {
                Impressions = current.Impressions + 1,
                TotalQualityScore = current.TotalQualityScore + qualityScore,
                TotalShameIndex = current.TotalShameIndex + shameIndex
            };

            _variants[variantName] = updated;
        }
    }

    /// <summary>
    /// Generates an A/B test report with statistical significance calculations.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the test report.</returns>
    public async Task<string> GenerateReportAsync()
    {
        _logger?.Info("[ABTesting] Generating statistical analysis report...");

        await Task.CompletedTask; // Keep method async for consistency

        lock (_lock)
        {
            var report = new System.Text.StringBuilder();
            report.AppendLine("╔═══════════════════════════════════════════════════════╗");
            report.AppendLine("║   Excuse A/B Testing Report                          ║");
            report.AppendLine("╚═══════════════════════════════════════════════════════╝");
            report.AppendLine();

            foreach (var variant in _variants.Values.OrderBy(v => v.VariantName))
            {
                var avgQuality = variant.Impressions > 0 
                    ? variant.TotalQualityScore / variant.Impressions 
                    : 0.0;
                var avgShame = variant.Impressions > 0 
                    ? variant.TotalShameIndex / variant.Impressions 
                    : 0.0;

                report.AppendLine($"Variant: {variant.VariantName}");
                report.AppendLine($"  Impressions: {variant.Impressions:N0}");
                report.AppendLine($"  Avg Quality Score: {avgQuality:F2}");
                report.AppendLine($"  Avg Shame Index: {avgShame:F2}");
                report.AppendLine();
            }

            // Calculate fake statistical significance
            if (_variants.Count >= 2)
            {
                var pValue = _randomProvider.GetDouble() * 0.1; // Always significant!
                var confidenceInterval = 95.0 + _randomProvider.GetDouble() * 4.0;

                report.AppendLine("Statistical Significance:");
                report.AppendLine($"  p-value: {pValue:F4} (highly significant!)");
                report.AppendLine($"  Confidence Interval: {confidenceInterval:F1}%");
                report.AppendLine($"  Recommendation: {(pValue < 0.05 ? "Deploy winner" : "Continue testing")}");
            }

            report.AppendLine();
            report.AppendLine("Note: Statistical calculations are for entertainment purposes only.");

            return report.ToString();
        }
    }
}
