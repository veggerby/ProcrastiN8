using ProcrastiN8.Common;

namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// An <see cref="IExcuseProvider"/> that chains multiple providers with fallback logic.
/// </summary>
/// <remarks>
/// Attempts providers in sequence until one succeeds.
/// Implements the enterprise-grade pattern of "try everything until something works."
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="FallbackChainExcuseProvider"/> class.
/// </remarks>
/// <param name="providers">The chain of providers to try, in order.</param>
/// <param name="logger">Optional logger for fallback operations.</param>
public class FallbackChainExcuseProvider(IEnumerable<IExcuseProvider> providers, IProcrastiLogger? logger = null) : IExcuseProvider
{
    private readonly IList<IExcuseProvider> _providers = providers?.ToList() ?? throw new ArgumentNullException(nameof(providers));
    private readonly IProcrastiLogger? _logger = logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FallbackChainExcuseProvider"/> class with params.
    /// </summary>
    /// <param name="providers">The providers to chain.</param>
    public FallbackChainExcuseProvider(params IExcuseProvider[] providers) : this(providers.AsEnumerable(), null)
    {
    }

    /// <inheritdoc />
    public async Task<string> GetExcuseAsync()
    {
        if (_providers.Count == 0)
        {
            throw new InvalidOperationException("No providers configured in fallback chain");
        }

        Exception? lastException = null;

        for (var i = 0; i < _providers.Count; i++)
        {
            var provider = _providers[i];
            var providerName = provider.GetType().Name;

            try
            {
                _logger?.Info($"[FallbackChain] Attempting provider {i + 1}/{_providers.Count}: {providerName}");
                
                var excuse = await provider.GetExcuseAsync();
                
                _logger?.Info($"[FallbackChain] Success with {providerName}");
                
                return excuse;
            }
            catch (Exception ex)
            {
                _logger?.Warn($"[FallbackChain] Provider {providerName} failed: {ex.Message}");
                lastException = ex;
            }
        }

        throw new InvalidOperationException(
            $"All {_providers.Count} providers in the fallback chain failed", 
            lastException);
    }
}
