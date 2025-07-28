namespace ProcrastiN8.JustBecause;

/// <summary>
/// Validates that a feature is not, and never will be, needed.
/// </summary>
/// <remarks>
/// The <see cref="YAGNIValidator"/> is a critical component for ensuring that unnecessary work is never performed.
/// </remarks>
public static class YAGNIValidator
{
    /// <summary>
    /// Throws a <see cref="NotSupportedException"/> if the feature is even remotely considered.
    /// </summary>
    /// <param name="feature">The feature to validate as unnecessary.</param>
    public static void Validate(string feature)
    {
        throw new NotSupportedException($"Feature '{feature}' is not needed and will not be implemented. YAGNI compliance enforced.");
    }
}
