namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// Provides caching capabilities for generated excuses to optimize performance and reduce redundant AI calls.
/// </summary>
/// <remarks>
/// Caching excuses is critical for maintaining the illusion of efficiency while actually stalling further.
/// </remarks>
public interface IExcuseCache
{
    /// <summary>
    /// Attempts to retrieve a cached excuse for the given prompt.
    /// </summary>
    /// <param name="prompt">The prompt key.</param>
    /// <param name="excuse">The cached excuse if found.</param>
    /// <returns><c>true</c> if a cached excuse was found; otherwise, <c>false</c>.</returns>
    bool TryGet(string prompt, out string? excuse);

    /// <summary>
    /// Stores an excuse in the cache.
    /// </summary>
    /// <param name="prompt">The prompt key.</param>
    /// <param name="excuse">The excuse to cache.</param>
    void Set(string prompt, string excuse);

    /// <summary>
    /// Clears all cached excuses.
    /// </summary>
    void Clear();

    /// <summary>
    /// Gets cache hit statistics for reporting purposes.
    /// </summary>
    /// <returns>A dictionary containing cache statistics.</returns>
    IDictionary<string, object> GetStatistics();
}
