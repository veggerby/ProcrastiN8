using ProcrastiN8.Common;

namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// An <see cref="IExcuseModel"/> implementation that wraps the existing OpenAI excuse provider.
/// </summary>
/// <remarks>
/// Bridges the legacy <see cref="OpenAIExcuseProvider"/> into the new neural excuse lab architecture.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="OpenAIExcuseModel"/> class.
/// </remarks>
/// <param name="apiKey">The OpenAI API key.</param>
/// <param name="httpClient">The HTTP client for API requests.</param>
/// <param name="logger">Optional logger for diagnostic output.</param>
public class OpenAIExcuseModel(string apiKey, HttpClient httpClient, IProcrastiLogger? logger = null) : IExcuseModel
{
    private readonly OpenAIExcuseProvider _provider = new(apiKey, httpClient);
    private readonly IProcrastiLogger? _logger = logger;

    /// <inheritdoc />
    public string ModelName => "OpenAI-GPT4";

    /// <inheritdoc />
    public async Task<string> GenerateExcuseAsync(string prompt, CancellationToken cancellationToken = default)
    {
        _logger?.Info($"[{ModelName}] Generating excuse with prompt: {prompt}");
        
        var excuse = await _provider.GetExcuseAsync();
        
        _logger?.Info($"[{ModelName}] Generated excuse: {excuse}");
        
        return excuse;
    }

    /// <inheritdoc />
    public IDictionary<string, object> GetMetadata()
    {
        return new Dictionary<string, object>
        {
            { "provider", "OpenAI" },
            { "model", "gpt-4" },
            { "type", "cloud" },
            { "cost_per_call", 0.03 },
            { "requires_api_key", true }
        };
    }
}
