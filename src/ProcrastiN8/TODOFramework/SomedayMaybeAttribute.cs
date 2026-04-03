namespace ProcrastiN8.TODOFramework;

/// <summary>
/// Marks a code element as aspirationally deferred.
/// This will absolutely happen someday. Maybe. The year is purely indicative.
/// </summary>
/// <remarks>
/// Unlike <see cref="TodoAttribute"/>, which is vaguely optimistic, <see cref="SomedayMaybeAttribute"/>
/// is more realistic about the temporal commitment involved. "Someday" is a valid sprint goal.
/// </remarks>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public sealed class SomedayMaybeAttribute(string? aspiration = null, int estimatedYear = 0) : Attribute
{
    /// <summary>
    /// A description of what will eventually happen, in the most optimistic of timelines.
    /// </summary>
    public string? Aspiration { get; } = aspiration;

    /// <summary>
    /// The approximate year in which this might realistically be addressed.
    /// Returns <c>0</c> when no year has been committed to — which is always the correct answer.
    /// </summary>
    public int EstimatedYear { get; } = estimatedYear;

    /// <summary>
    /// Gets whether an estimated year was provided. <c>false</c> indicates the timeline is "open."
    /// </summary>
    public bool HasEstimatedYear => EstimatedYear > 0;
}
