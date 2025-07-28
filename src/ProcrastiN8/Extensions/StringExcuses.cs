namespace ProcrastiN8.Extensions;

/// <summary>
/// Provides extension methods for <see cref="string"/> to generate plausible excuses.
/// </summary>
public static class StringExcuses
{
    /// <summary>
    /// Returns a plausible excuse for the given string context.
    /// </summary>
    /// <param name="context">The context for which an excuse is needed.</param>
    /// <returns>A plausible excuse string.</returns>
    public static string ToExcuse(this string context)
    {
        return $"Unable to process '{context}' due to unforeseen circumstances.";
    }
}
