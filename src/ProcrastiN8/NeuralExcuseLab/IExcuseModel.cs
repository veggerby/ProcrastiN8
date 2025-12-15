namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// Represents an AI model capable of generating excuses with advanced neural sophistication.
/// </summary>
/// <remarks>
/// This abstraction enables pluggable LLM providers for excuse generation,
/// supporting everything from cutting-edge GPT models to fortune cookies.
/// </remarks>
public interface IExcuseModel
{
    /// <summary>
    /// Gets the name of this model provider.
    /// </summary>
    string ModelName { get; }

    /// <summary>
    /// Generates an excuse using this model's advanced neural architecture.
    /// </summary>
    /// <param name="prompt">The prompt to guide excuse generation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the generated excuse.</returns>
    Task<string> GenerateExcuseAsync(string prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets metadata about this model's capabilities and configuration.
    /// </summary>
    /// <returns>A dictionary of model metadata.</returns>
    IDictionary<string, object> GetMetadata();
}
