namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// Manages versioned excuse generation prompts with rollout strategies.
/// </summary>
/// <remarks>
/// Provides enterprise-grade prompt governance for systematic excuse generation.
/// Supports versioning, A/B testing, and gradual rollout of new prompt templates.
/// </remarks>
public class ExcusePromptRegistry
{
    private readonly Dictionary<string, PromptVersion> _prompts = new();
    private readonly object _lock = new();
    private string _activeVersion = "v1.0";

    /// <summary>
    /// Represents a versioned prompt template.
    /// </summary>
    public record PromptVersion(
        string Version,
        string Template,
        string Tone,
        DateTime CreatedAt,
        bool IsActive);

    /// <summary>
    /// Initializes a new instance of the <see cref="ExcusePromptRegistry"/> class with default prompts.
    /// </summary>
    public ExcusePromptRegistry()
    {
        // Register default prompts
        RegisterPrompt("v1.0", "Generate a creative excuse for procrastination.", "casual", isActive: true);
        RegisterPrompt("v1.1", "Generate a highly technical excuse involving quantum mechanics.", "technical", isActive: false);
        RegisterPrompt("v2.0", "Generate an excuse that demonstrates deep philosophical introspection.", "philosophical", isActive: false);
    }

    /// <summary>
    /// Registers a new prompt version in the registry.
    /// </summary>
    /// <param name="version">The version identifier.</param>
    /// <param name="template">The prompt template.</param>
    /// <param name="tone">The tone of generated excuses.</param>
    /// <param name="isActive">Whether this version is currently active.</param>
    public void RegisterPrompt(string version, string template, string tone, bool isActive = false)
    {
        lock (_lock)
        {
            var prompt = new PromptVersion(version, template, tone, DateTime.UtcNow, isActive);
            _prompts[version] = prompt;

            if (isActive)
            {
                _activeVersion = version;
            }
        }
    }

    /// <summary>
    /// Gets the currently active prompt.
    /// </summary>
    /// <returns>The active prompt version.</returns>
    public PromptVersion GetActivePrompt()
    {
        lock (_lock)
        {
            return _prompts[_activeVersion];
        }
    }

    /// <summary>
    /// Gets a specific prompt version.
    /// </summary>
    /// <param name="version">The version identifier.</param>
    /// <returns>The requested prompt version.</returns>
    public PromptVersion GetPrompt(string version)
    {
        lock (_lock)
        {
            if (!_prompts.TryGetValue(version, out var prompt))
            {
                throw new ArgumentException($"Prompt version '{version}' not found", nameof(version));
            }

            return prompt;
        }
    }

    /// <summary>
    /// Sets the active prompt version.
    /// </summary>
    /// <param name="version">The version to activate.</param>
    public void SetActiveVersion(string version)
    {
        lock (_lock)
        {
            if (!_prompts.ContainsKey(version))
            {
                throw new ArgumentException($"Prompt version '{version}' not found", nameof(version));
            }

            _activeVersion = version;
        }
    }

    /// <summary>
    /// Gets all registered prompt versions.
    /// </summary>
    /// <returns>A collection of all prompt versions.</returns>
    public IEnumerable<PromptVersion> GetAllPrompts()
    {
        lock (_lock)
        {
            return _prompts.Values.ToList();
        }
    }
}
