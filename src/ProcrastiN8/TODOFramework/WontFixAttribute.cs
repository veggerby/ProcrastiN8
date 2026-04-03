namespace ProcrastiN8.TODOFramework;

/// <summary>
/// Documents a known issue that has been heroically reclassified as a feature.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="WontFixAttribute"/> is the responsible way to close tickets you have no intention of addressing.
/// It signals intentionality, communicates a deliberate product decision, and — most importantly — gets the bug
/// count down without any actual work.
/// </para>
/// <para>
/// "Working as intended" is the default reason, because it is always technically accurate.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public sealed class WontFixAttribute(string reason = "Working as intended.") : Attribute
{
    /// <summary>
    /// The official reason this issue will not be fixed.
    /// Defaults to "Working as intended." — a classic.
    /// </summary>
    public string Reason { get; } = reason;
}
