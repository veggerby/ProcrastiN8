namespace ProcrastiN8.RulesEngine;

/// <summary>
/// Represents a task that can be evaluated by the rules engine.
/// </summary>
/// <remarks>
/// Tasks are the subjects of procrastination rules. They carry metadata like tags,
/// priority, and estimated effort that conditions can evaluate against.
/// Note: Id and CreatedAt should be set explicitly for testability.
/// </remarks>
public sealed class ProcrastinationTask
{
    /// <summary>
    /// Gets or sets the unique identifier for this task.
    /// </summary>
    /// <remarks>
    /// Should be set explicitly for testability. Defaults to a new GUID if not set.
    /// </remarks>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the human-readable name of this task.
    /// </summary>
    public string Name { get; set; } = "Unnamed Task";

    /// <summary>
    /// Gets or sets the description of this task.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tags associated with this task.
    /// </summary>
    public ISet<string> Tags { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets the priority level (lower = more important).
    /// </summary>
    public int Priority { get; set; } = 100;

    /// <summary>
    /// Gets or sets the estimated effort in arbitrary units (for maximum vagueness).
    /// </summary>
    public double EstimatedEffort { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets the deadline for this task, if any.
    /// </summary>
    public DateTimeOffset? Deadline { get; set; }

    /// <summary>
    /// Gets or sets when this task was created.
    /// </summary>
    /// <remarks>
    /// Should be set explicitly for testability. Defaults to UtcNow if not set.
    /// </remarks>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets custom metadata for this task.
    /// </summary>
    public IDictionary<string, object?> Metadata { get; set; } = new Dictionary<string, object?>();
}
