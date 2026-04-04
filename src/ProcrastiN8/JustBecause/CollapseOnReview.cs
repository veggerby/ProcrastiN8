using System.Diagnostics;
using System.Reflection;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// A quantum value that behaves differently under code review than it does in production.
/// Specifically: the value collapses to a degraded or alternative state the moment
/// a stack frame associated with review tooling or test infrastructure is detected.
/// </summary>
/// <remarks>
/// <para>
/// The Copenhagen interpretation holds that a quantum system exists in superposition until
/// observed. <see cref="CollapseOnReview{T}"/> models this for code review: when the call
/// stack reveals the presence of a reviewer (detected via known review tooling, test harnesses,
/// or code analysis namespaces), the value collapses to its "reviewed" form.
/// In production — where nobody is watching — the original value is returned.
/// </para>
/// <para>
/// This is technically dishonest. It is also how many real systems behave.
/// </para>
/// </remarks>
/// <typeparam name="T">The type of the value under observation.</typeparam>
public sealed class CollapseOnReview<T>
{
    private static readonly HashSet<string> ReviewIndicatorNamespaces =
    [
        "xunit",
        "nunit",
        "mstest",
        "microsoft.visualstudio.testplatform",
        "nsubstitute",
        "awesomeassertions",
        "fluentassertions",
        "coverlet",
        "resharper",
        "rider",
        "sonar",
        "roslyn.analyzer",
        "microsoft.codeanalysis",
        "procrastinate.tests",
        "procrastin8.tests"
    ];

    private readonly T _productionValue;
    private readonly T _reviewValue;
    private readonly IQuantumInterpretation _interpretation;
    private readonly IProcrastiLogger? _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="CollapseOnReview{T}"/>.
    /// </summary>
    /// <param name="productionValue">The real value, returned when nobody is reviewing.</param>
    /// <param name="reviewValue">The sanitised, reviewed value, returned under observation.</param>
    /// <param name="interpretation">
    /// The quantum mechanics interpretation governing collapse behaviour.
    /// Defaults to <see cref="QuantumInterpretations.Copenhagen"/>.
    /// Under <see cref="QuantumInterpretations.ManyWorlds"/>, observation does not cause collapse
    /// and the production value is always returned — all outcomes are equally real.
    /// Under <see cref="QuantumInterpretations.PilotWave"/>, the result is predetermined and
    /// the production value is returned regardless of observation.
    /// </param>
    /// <param name="logger">Optional logger for waveform collapse announcements.</param>
    public CollapseOnReview(T productionValue, T reviewValue, IQuantumInterpretation? interpretation = null, IProcrastiLogger? logger = null)
    {
        _productionValue = productionValue;
        _reviewValue = reviewValue;
        _interpretation = interpretation ?? QuantumInterpretations.Copenhagen;
        _logger = logger;
    }

    /// <summary>
    /// Gets whether the call stack currently indicates a review context.
    /// </summary>
    public bool IsUnderReview => DetectReviewContext();

    /// <summary>Gets the quantum interpretation governing collapse behaviour.</summary>
    public IQuantumInterpretation Interpretation => _interpretation;

    /// <summary>
    /// Resolves the value. Returns the production value unless review tooling is detected on the call stack,
    /// in which case the reviewed value is returned instead — subject to the active interpretation.
    /// </summary>
    /// <returns>
    /// <see cref="ReviewValue"/> if the waveform collapses under observation (and interpretation permits collapse);
    /// otherwise <see cref="ProductionValue"/>.
    /// </returns>
    public T Resolve()
    {
        if (!_interpretation.ObservationAffectsOutcome)
        {
            _logger?.Debug(
                "[CollapseOnReview] Interpretation '{Interpretation}' holds that observation does not cause collapse. Returning production value.",
                _interpretation.Name);
            return _productionValue;
        }

        if (DetectReviewContext())
        {
            _logger?.Info("[CollapseOnReview] Review context detected ({Interpretation}). Collapsing to reviewed value. Nothing to see here.", _interpretation.Name);
            return _reviewValue;
        }

        _logger?.Debug("[CollapseOnReview] No observer detected. Returning production value ({Interpretation}).", _interpretation.Name);
        return _productionValue;
    }

    /// <summary>
    /// Gets the value that is returned in production (unobserved) contexts.
    /// </summary>
    public T ProductionValue => _productionValue;

    /// <summary>
    /// Gets the value that is returned under review (observed) contexts.
    /// </summary>
    public T ReviewValue => _reviewValue;

    private static bool DetectReviewContext()
    {
        var stackTrace = new StackTrace();
        var frames = stackTrace.GetFrames();

        if (frames is null)
        {
            return false;
        }

        foreach (var frame in frames)
        {
            var method = frame.GetMethod();
            var declaringType = method?.DeclaringType;
            if (declaringType is null) { continue; }

            var assemblyName = declaringType.Assembly.GetName().Name ?? string.Empty;
            var namespaceName = declaringType.Namespace ?? string.Empty;

            var combined = (assemblyName + " " + namespaceName).ToLowerInvariant();

            foreach (var indicator in ReviewIndicatorNamespaces)
            {
                if (combined.Contains(indicator, StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
