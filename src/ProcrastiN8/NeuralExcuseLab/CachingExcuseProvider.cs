using ProcrastiN8.Common;

namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// An <see cref="IExcuseProvider"/> decorator that adds caching capabilities.
/// </summary>
/// <remarks>
/// Wraps any excuse provider with transparent caching to reduce redundant AI calls.
/// Critical for maintaining the appearance of optimization.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="CachingExcuseProvider"/> class.
/// </remarks>
/// <param name="innerProvider">The underlying excuse provider to cache.</param>
/// <param name="cache">The cache implementation to use.</param>
/// <param name="logger">Optional logger for cache operations.</param>
public class CachingExcuseProvider(IExcuseProvider innerProvider, IExcuseCache cache, IProcrastiLogger? logger = null) : IExcuseProvider
{
    private readonly IExcuseProvider _innerProvider = innerProvider ?? throw new ArgumentNullException(nameof(innerProvider));
    private readonly IExcuseCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly IProcrastiLogger? _logger = logger;
    private const string DefaultPrompt = "default_excuse_prompt";

    /// <inheritdoc />
    public async Task<string> GetExcuseAsync()
    {
        // Use a constant key since the base IExcuseProvider doesn't accept prompts
        if (_cache.TryGet(DefaultPrompt, out var cachedExcuse) && cachedExcuse != null)
        {
            _logger?.Info($"[CachingExcuseProvider] Cache hit for default prompt");
            return cachedExcuse;
        }

        _logger?.Info($"[CachingExcuseProvider] Cache miss, generating new excuse");
        var excuse = await _innerProvider.GetExcuseAsync();
        
        _cache.Set(DefaultPrompt, excuse);
        
        return excuse;
    }

    /// <summary>
    /// Gets the cache statistics for monitoring purposes.
    /// </summary>
    /// <returns>A dictionary containing cache statistics.</returns>
    public IDictionary<string, object> GetCacheStatistics()
    {
        return _cache.GetStatistics();
    }
}
