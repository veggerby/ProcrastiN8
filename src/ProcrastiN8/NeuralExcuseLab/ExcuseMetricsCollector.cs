namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// In-memory implementation of <see cref="IExcuseMetrics"/> for tracking excuse quality.
/// </summary>
/// <remarks>
/// Collects scientifically questionable metrics on excuse generation performance.
/// Thread-safe for enterprise-grade metric collection.
/// </remarks>
public class ExcuseMetricsCollector : IExcuseMetrics
{
    private readonly List<ExcuseMetricEntry> _entries = new();
    private readonly object _lock = new();

    private record ExcuseMetricEntry(string Excuse, string ModelName, double QualityScore, double ShameIndex, DateTime Timestamp);

    /// <inheritdoc />
    public void RecordExcuse(string excuse, string modelName, double qualityScore, double shameIndex)
    {
        lock (_lock)
        {
            _entries.Add(new ExcuseMetricEntry(excuse, modelName, qualityScore, shameIndex, DateTime.UtcNow));
        }
    }

    /// <inheritdoc />
    public IDictionary<string, object> GetAggregatedMetrics()
    {
        lock (_lock)
        {
            if (_entries.Count == 0)
            {
                return new Dictionary<string, object>
                {
                    { "total_excuses", 0 },
                    { "avg_quality_score", 0.0 },
                    { "avg_shame_index", 0.0 }
                };
            }

            var avgQuality = _entries.Average(e => e.QualityScore);
            var avgShame = _entries.Average(e => e.ShameIndex);
            var modelCounts = _entries.GroupBy(e => e.ModelName)
                .ToDictionary(g => g.Key, g => g.Count());

            return new Dictionary<string, object>
            {
                { "total_excuses", _entries.Count },
                { "avg_quality_score", avgQuality },
                { "avg_shame_index", avgShame },
                { "min_quality_score", _entries.Min(e => e.QualityScore) },
                { "max_quality_score", _entries.Max(e => e.QualityScore) },
                { "min_shame_index", _entries.Min(e => e.ShameIndex) },
                { "max_shame_index", _entries.Max(e => e.ShameIndex) },
                { "models_used", modelCounts }
            };
        }
    }

    /// <inheritdoc />
    public IDictionary<string, object> GetMetricsByModel(string modelName)
    {
        lock (_lock)
        {
            var modelEntries = _entries.Where(e => e.ModelName == modelName).ToList();

            if (modelEntries.Count == 0)
            {
                return new Dictionary<string, object>
                {
                    { "model", modelName },
                    { "excuse_count", 0 },
                    { "avg_quality_score", 0.0 },
                    { "avg_shame_index", 0.0 }
                };
            }

            return new Dictionary<string, object>
            {
                { "model", modelName },
                { "excuse_count", modelEntries.Count },
                { "avg_quality_score", modelEntries.Average(e => e.QualityScore) },
                { "avg_shame_index", modelEntries.Average(e => e.ShameIndex) },
                { "min_quality_score", modelEntries.Min(e => e.QualityScore) },
                { "max_quality_score", modelEntries.Max(e => e.QualityScore) }
            };
        }
    }
}
