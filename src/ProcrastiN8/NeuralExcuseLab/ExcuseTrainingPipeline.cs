using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// Simulates a sophisticated ML training pipeline for excuse generation.
/// </summary>
/// <remarks>
/// Performs no actual training but generates extremely verbose logs
/// to maintain the illusion of scientific progress.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="ExcuseTrainingPipeline"/> class.
/// </remarks>
/// <param name="randomProvider">Random provider for training metrics.</param>
/// <param name="delayProvider">Delay provider for simulating training time.</param>
/// <param name="logger">Logger for training output.</param>
public class ExcuseTrainingPipeline(
    IRandomProvider? randomProvider = null,
    IDelayProvider? delayProvider = null,
    IProcrastiLogger? logger = null)
{
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;
    private readonly IDelayProvider _delayProvider = delayProvider ?? new TaskDelayProvider();
    private readonly IProcrastiLogger _logger = logger ?? new DefaultLogger();

    /// <summary>
    /// Executes the fake training pipeline with elaborate logging.
    /// </summary>
    /// <param name="datasetPath">The fictional path to training data.</param>
    /// <param name="epochs">Number of fake training epochs.</param>
    /// <param name="cancellationToken">Cancellation token for stopping the charade.</param>
    /// <returns>A task representing the fake training operation.</returns>
    public async Task TrainAsync(string datasetPath, int epochs = 10, CancellationToken cancellationToken = default)
    {
        _logger.Info($"╔═══════════════════════════════════════════════════════╗");
        _logger.Info($"║   Neural Excuse Fine-Tuning Pipeline v2.0            ║");
        _logger.Info($"╚═══════════════════════════════════════════════════════╝");
        _logger.Info($"");
        _logger.Info($"[Pipeline] Initializing training session...");
        _logger.Info($"[Pipeline] Dataset: {datasetPath}");
        _logger.Info($"[Pipeline] Epochs: {epochs}");
        _logger.Info($"");

        await _delayProvider.DelayAsync(TimeSpan.FromMilliseconds(_randomProvider.GetRandom(100, 300)), cancellationToken);

        _logger.Info($"[DataLoader] Loading backlog comments from {datasetPath}...");
        await _delayProvider.DelayAsync(TimeSpan.FromMilliseconds(_randomProvider.GetRandom(200, 500)), cancellationToken);
        
        var recordCount = _randomProvider.GetRandom(1000, 10000);
        _logger.Info($"[DataLoader] Loaded {recordCount:N0} training samples");
        _logger.Info($"[DataLoader] Tokenizing excuses...");
        await _delayProvider.DelayAsync(TimeSpan.FromMilliseconds(_randomProvider.GetRandom(300, 700)), cancellationToken);
        _logger.Info($"[DataLoader] Vocabulary size: {_randomProvider.GetRandom(5000, 15000):N0} tokens");
        _logger.Info($"");

        for (var epoch = 1; epoch <= epochs; epoch++)
        {
            _logger.Info($"═══ Epoch {epoch}/{epochs} ═══");
            
            var batchCount = _randomProvider.GetRandom(10, 50);
            for (var batch = 1; batch <= batchCount; batch++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.Warn($"[Pipeline] Training cancelled at epoch {epoch}, batch {batch}");
                    return;
                }

                var loss = 10.0 / (epoch * 0.5 + 1) * (1 + _randomProvider.GetDouble() * 0.2);
                var accuracy = Math.Min(0.95, 0.5 + (epoch * 0.05) + _randomProvider.GetDouble() * 0.05);
                
                _logger.Info($"  Batch {batch}/{batchCount} - Loss: {loss:F4} - Accuracy: {accuracy:F4}");
                await _delayProvider.DelayAsync(TimeSpan.FromMilliseconds(_randomProvider.GetRandom(50, 150)), cancellationToken);
            }

            var epochLoss = 10.0 / (epoch * 0.5 + 1);
            var epochAccuracy = Math.Min(0.95, 0.5 + (epoch * 0.05));
            
            _logger.Info($"  → Epoch Loss: {epochLoss:F4} | Accuracy: {epochAccuracy:F4}");
            _logger.Info($"");
        }

        _logger.Info($"[Pipeline] Training complete!");
        _logger.Info($"[Pipeline] Saving model weights to disk (not really)...");
        await _delayProvider.DelayAsync(TimeSpan.FromMilliseconds(_randomProvider.GetRandom(200, 500)), cancellationToken);
        _logger.Info($"[Pipeline] Model checkpoint saved: /models/excuse-fine-tuned-{DateTime.UtcNow:yyyyMMdd-HHmmss}.pt");
        _logger.Info($"");
        _logger.Info($"✓ Training pipeline completed successfully");
        _logger.Info($"  (Note: No actual training occurred. All metrics are fictional.)");
    }
}
