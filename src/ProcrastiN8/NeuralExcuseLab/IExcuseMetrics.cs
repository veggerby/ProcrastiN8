namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// Collects and reports metrics on excuse generation quality and performance.
/// </summary>
/// <remarks>
/// Essential for maintaining the appearance of scientific rigor in excuse evaluation.
/// </remarks>
public interface IExcuseMetrics
{
    /// <summary>
    /// Records a generated excuse along with its quality metrics.
    /// </summary>
    /// <param name="excuse">The generated excuse.</param>
    /// <param name="modelName">The model that generated it.</param>
    /// <param name="qualityScore">The computed quality score.</param>
    /// <param name="shameIndex">The computed shame index.</param>
    void RecordExcuse(string excuse, string modelName, double qualityScore, double shameIndex);

    /// <summary>
    /// Gets aggregated metrics for all recorded excuses.
    /// </summary>
    /// <returns>A dictionary containing metric summaries.</returns>
    IDictionary<string, object> GetAggregatedMetrics();

    /// <summary>
    /// Gets metrics filtered by model name.
    /// </summary>
    /// <param name="modelName">The model name to filter by.</param>
    /// <returns>A dictionary containing model-specific metrics.</returns>
    IDictionary<string, object> GetMetricsByModel(string modelName);
}
