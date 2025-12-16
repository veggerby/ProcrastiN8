namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// Factory for creating excuse model instances with appropriate configuration.
/// </summary>
/// <remarks>
/// Provides a centralized mechanism for instantiating excuse models,
/// supporting dependency injection and configuration management.
/// </remarks>
public interface IExcuseModelFactory
{
    /// <summary>
    /// Creates an excuse model instance by provider name.
    /// </summary>
    /// <param name="providerName">The name of the provider (e.g., "openai", "local", "fortune").</param>
    /// <param name="configuration">Optional configuration parameters for the model.</param>
    /// <returns>A configured instance of <see cref="IExcuseModel"/>.</returns>
    IExcuseModel CreateModel(string providerName, IDictionary<string, object>? configuration = null);

    /// <summary>
    /// Gets the names of all registered providers.
    /// </summary>
    /// <returns>An enumerable of provider names.</returns>
    IEnumerable<string> GetRegisteredProviders();
}
