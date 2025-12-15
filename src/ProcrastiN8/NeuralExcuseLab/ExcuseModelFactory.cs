using ProcrastiN8.JustBecause;

namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// Default implementation of <see cref="IExcuseModelFactory"/> supporting multiple model providers.
/// </summary>
/// <remarks>
/// Factory pattern implementation for creating excuse models with provider-specific configuration.
/// Supports OpenAI, local models, and fortune cookie wisdom.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="ExcuseModelFactory"/> class.
/// </remarks>
/// <param name="httpClient">HTTP client for cloud-based providers.</param>
/// <param name="randomProvider">Random provider for local and fortune cookie models.</param>
/// <param name="logger">Optional logger for factory operations.</param>
public class ExcuseModelFactory(HttpClient? httpClient = null, IRandomProvider? randomProvider = null, IProcrastiLogger? logger = null) : IExcuseModelFactory
{
    private readonly HttpClient _httpClient = httpClient ?? new HttpClient();
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;
    private readonly IProcrastiLogger? _logger = logger;

    private static readonly string[] SupportedProviders = ["openai", "local", "fortune"];

    /// <inheritdoc />
    public IExcuseModel CreateModel(string providerName, IDictionary<string, object>? configuration = null)
    {
        configuration ??= new Dictionary<string, object>();

        var normalizedProvider = providerName.ToLowerInvariant();

        return normalizedProvider switch
        {
            "openai" => CreateOpenAIModel(configuration),
            "local" => CreateLocalModel(configuration),
            "fortune" => CreateFortuneCookieModel(configuration),
            _ => throw new ArgumentException($"Unknown provider: {providerName}. Supported providers: {string.Join(", ", SupportedProviders)}", nameof(providerName))
        };
    }

    /// <inheritdoc />
    public IEnumerable<string> GetRegisteredProviders()
    {
        return SupportedProviders;
    }

    private IExcuseModel CreateOpenAIModel(IDictionary<string, object> configuration)
    {
        var apiKey = configuration.TryGetValue("api_key", out var key) 
            ? key.ToString() ?? throw new ArgumentException("OpenAI API key is required", nameof(configuration))
            : throw new ArgumentException("OpenAI API key is required", nameof(configuration));

        return new OpenAIExcuseModel(apiKey, _httpClient, _logger);
    }

    private IExcuseModel CreateLocalModel(IDictionary<string, object> configuration)
    {
        var modelPath = configuration.TryGetValue("model_path", out var path) 
            ? path.ToString() 
            : "models/excuse-llama-7b.gguf";

        return new LocalExcuseModel(modelPath!, _randomProvider, _logger);
    }

    private IExcuseModel CreateFortuneCookieModel(IDictionary<string, object> configuration)
    {
        return new FortuneCookieExcuseModel(_randomProvider, _logger);
    }
}
