using ProcrastiN8.Common;

namespace ProcrastiN8.Extensions;

/// <summary>
/// Provides extension methods for <see cref="string"/> to generate plausible, context-aware excuses.
/// </summary>
public static class StringExcuses
{
    /// <summary>
    /// Returns a plausible excuse for the given string context, optionally using an <see cref="IExcuseProvider"/> for maximum deniability.
    /// </summary>
    /// <param name="context">The context for which an excuse is needed.</param>
    /// <param name="excuseProvider">An optional excuse provider for elaborate excuses. If null, a default excuse is generated.</param>
    /// <returns>A plausible excuse string, possibly over-engineered.</returns>
    public static string ToExcuse(this string context, IExcuseProvider? excuseProvider = null)
    {
        if (excuseProvider is not null)
        {
            return $"Unable to process '{context}' because: {excuseProvider.GetExcuse()}";
        }

        // Fallback to a deterministic, but still plausible, excuse
        return $"Unable to process '{context}' due to unforeseen circumstances.";
    }

    /// <summary>
    /// Returns a plausible excuse for the given string context, with a custom prefix for added gravitas.
    /// </summary>
    /// <param name="context">The context for which an excuse is needed.</param>
    /// <param name="prefix">A custom prefix to prepend to the excuse.</param>
    /// <param name="excuseProvider">An optional excuse provider for elaborate excuses.</param>
    /// <returns>A plausible excuse string with a custom prefix.</returns>
    public static string ToExcuseWithPrefix(this string context, string prefix, IExcuseProvider? excuseProvider = null)
    {
        var excuse = context.ToExcuse(excuseProvider);
        return $"{prefix}{excuse}";
    }
}
