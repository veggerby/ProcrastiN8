using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// An <see cref="IExcuseModel"/> that simulates local model execution with elaborate logging.
/// </summary>
/// <remarks>
/// Pretends to run a sophisticated local LLM while actually using the basic excuse generator.
/// Perfect for maintaining the illusion of AI without the cloud dependencies.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="LocalExcuseModel"/> class.
/// </remarks>
/// <param name="modelPath">The fictional path to the local model weights.</param>
/// <param name="randomProvider">Random provider for excuse generation.</param>
/// <param name="delayProvider">Delay provider for simulating loading times.</param>
/// <param name="logger">Optional logger for verbose diagnostic output.</param>
public class LocalExcuseModel(
    string modelPath = "models/excuse-llama-7b.gguf",
    IRandomProvider? randomProvider = null,
    IDelayProvider? delayProvider = null,
    IProcrastiLogger? logger = null) : IExcuseModel
{
    private readonly string _modelPath = modelPath;
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;
    private readonly IDelayProvider _delayProvider = delayProvider ?? new TaskDelayProvider();
    private readonly IProcrastiLogger? _logger = logger;

    /// <inheritdoc />
    public string ModelName => "LocalExcuseLLaMA-7B";

    /// <inheritdoc />
    public async Task<string> GenerateExcuseAsync(string prompt, CancellationToken cancellationToken = default)
    {
        _logger?.Info($"[{ModelName}] Loading model from {_modelPath}...");
        await _delayProvider.DelayAsync(TimeSpan.FromMilliseconds(_randomProvider.GetRandom(50, 200)), cancellationToken);
        
        _logger?.Info($"[{ModelName}] Initializing tensor cores...");
        await _delayProvider.DelayAsync(TimeSpan.FromMilliseconds(_randomProvider.GetRandom(30, 100)), cancellationToken);
        
        _logger?.Info($"[{ModelName}] Running inference with prompt: {prompt}");
        await _delayProvider.DelayAsync(TimeSpan.FromMilliseconds(_randomProvider.GetRandom(100, 300)), cancellationToken);
        
        var excuse = ExcuseGenerator.GetRandomExcuse(_randomProvider);
        
        _logger?.Info($"[{ModelName}] Generated {excuse.Length} tokens");
        
        return excuse;
    }

    /// <inheritdoc />
    public IDictionary<string, object> GetMetadata()
    {
        return new Dictionary<string, object>
        {
            { "provider", "Local" },
            { "model", "excuse-llama-7b" },
            { "type", "local" },
            { "model_path", _modelPath },
            { "parameters", "7B" },
            { "quantization", "4-bit" },
            { "cost_per_call", 0.0 },
            { "requires_api_key", false }
        };
    }
}
