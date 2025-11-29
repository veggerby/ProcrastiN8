namespace ProcrastiN8.RulesEngine.Policies;

/// <summary>
/// Represents a versioned collection of procrastination rules with a shared theme.
/// </summary>
/// <remarks>
/// Policy packs are like compliance frameworks, but for avoiding work.
/// Examples: ISO-9001-Procrastination, Agile-But-Not-Really, GDPR-For-Feelings.
/// </remarks>
public interface IPolicyPack
{
    /// <summary>
    /// Gets the unique identifier for this policy pack.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the human-readable name of this policy pack.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of what this policy pack does.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the version of this policy pack.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Gets the rules defined in this policy pack.
    /// </summary>
    IReadOnlyList<IRule> Rules { get; }

    /// <summary>
    /// Gets the metadata associated with this policy pack.
    /// </summary>
    IDictionary<string, object?> Metadata { get; }
}
