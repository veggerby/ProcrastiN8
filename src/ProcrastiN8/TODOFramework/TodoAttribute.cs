namespace ProcrastiN8.TODOFramework;

/// <summary>
/// Indicates that the decorated code element is intentionally left incomplete, deferred, or otherwise scheduled for perpetual postponement.
/// </summary>
/// <remarks>
/// This attribute is for documentation, tooling, and existential dread only. It has no runtime effect unless paired with a compatible scheduler.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="TodoAttribute"/> class.
/// </remarks>
/// <param name="reason">The reason for not completing the work.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public sealed class TodoAttribute(string? reason = null) : Attribute
{
    /// <summary>
    /// The reason for the procrastination, if any.
    /// </summary>
    public string? Reason { get; } = reason;
}
