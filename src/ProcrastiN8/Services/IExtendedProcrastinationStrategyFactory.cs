namespace ProcrastiN8.Services;

/// <summary>
/// Extended factory supporting string-based custom strategy keys in addition to enum modes.
/// </summary>
public interface IExtendedProcrastinationStrategyFactory : IProcrastinationStrategyFactory
{
    /// <summary>Creates a strategy by custom key.</summary>
    IProcrastinationStrategy Create(string key);

    /// <summary>Registers or replaces a strategy factory delegate for a custom key.</summary>
    void Register(string key, Func<IProcrastinationStrategy> factory);
}

/// <summary>In-memory implementation of <see cref="IExtendedProcrastinationStrategyFactory"/>.</summary>
public sealed class CompositeProcrastinationStrategyFactory : IExtendedProcrastinationStrategyFactory
{
    private readonly Dictionary<string, Func<IProcrastinationStrategy>> _registry = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _sync = new();

    public CompositeProcrastinationStrategyFactory()
    {
        // Seed with defaults mapping enum names.
        Register(ProcrastinationMode.MovingTarget.ToString(), () => new MovingTargetStrategy());
        Register(ProcrastinationMode.InfiniteEstimation.ToString(), () => new InfiniteEstimationStrategy());
        Register(ProcrastinationMode.WeekendFallback.ToString(), () => new WeekendFallbackStrategy());
    }

    public IProcrastinationStrategy Create(ProcrastinationMode mode) => Create(mode.ToString());
    public IProcrastinationStrategy Create(string key)
    {
        lock (_sync)
        {
            if (!_registry.TryGetValue(key, out var factory))
            {
                throw new KeyNotFoundException($"No procrastination strategy registered for key '{key}'.");
            }
            return factory();
        }
    }

    public void Register(string key, Func<IProcrastinationStrategy> factory)
    {
        if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentException("Key required", nameof(key)); }
        lock (_sync)
        {
            _registry[key] = factory ?? throw new ArgumentNullException(nameof(factory));
        }
    }
}