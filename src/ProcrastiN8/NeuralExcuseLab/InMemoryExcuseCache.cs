namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// In-memory implementation of <see cref="IExcuseCache"/> for caching generated excuses.
/// </summary>
/// <remarks>
/// Provides thread-safe caching with hit/miss statistics for performance monitoring.
/// Essential for appearing efficient while actually doing less work.
/// </remarks>
public class InMemoryExcuseCache : IExcuseCache
{
    private readonly Dictionary<string, string> _cache = new();
    private readonly object _lock = new();
    private int _hits = 0;
    private int _misses = 0;

    /// <inheritdoc />
    public bool TryGet(string prompt, out string? excuse)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(prompt, out var cached))
            {
                _hits++;
                excuse = cached;
                return true;
            }

            _misses++;
            excuse = null;
            return false;
        }
    }

    /// <inheritdoc />
    public void Set(string prompt, string excuse)
    {
        lock (_lock)
        {
            _cache[prompt] = excuse;
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        lock (_lock)
        {
            _cache.Clear();
            _hits = 0;
            _misses = 0;
        }
    }

    /// <inheritdoc />
    public IDictionary<string, object> GetStatistics()
    {
        lock (_lock)
        {
            var total = _hits + _misses;
            var hitRate = total > 0 ? (double)_hits / total : 0.0;

            return new Dictionary<string, object>
            {
                { "total_requests", total },
                { "cache_hits", _hits },
                { "cache_misses", _misses },
                { "hit_rate", hitRate },
                { "cached_entries", _cache.Count }
            };
        }
    }
}
