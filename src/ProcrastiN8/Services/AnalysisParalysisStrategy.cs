using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

/// <summary>
/// A procrastination strategy that perpetually discovers one more critical open question
/// requiring further analysis before work can responsibly begin.
/// </summary>
/// <remarks>
/// Every cycle yields a new and equally urgent prerequisite research area. The task executes
/// only when the safety cap or absolute deadline is reached, at which point the analysis
/// is declared "sufficient" by executive mandate.
/// </remarks>
public class AnalysisParalysisStrategy : ProcrastinationStrategyBase
{
    private static readonly string[] OpenQuestions =
    [
        "What are the edge cases for the edge cases?",
        "Have we fully mapped the second-order blast radius?",
        "Is this reversible? (Answer: unclear — needs analysis.)",
        "We haven't benchmarked the benchmark harness yet.",
        "What does the industry do? (We need a 40-page landscape review.)",
        "This decision tree has too many leaves. More pruning required.",
        "We need a proof of concept before we can scope the proof of concept.",
        "Has anyone considered the multi-region implications?",
        "The requirements are ambiguous. We should clarify the ambiguity.",
        "What happens if we do nothing? (We need to model that scenario too.)",
        "Are we measuring the right thing? (We should measure that.)",
        "The data is insufficient. We need more data to know what data we need.",
    ];

    private static readonly string[] AnalysisDeliverables =
    [
        "a SWOT analysis of the SWOT analysis",
        "a stakeholder map of the stakeholders",
        "a risk register for the risk register",
        "a gap analysis of the current gap analysis",
        "a competitive landscape for the competitive landscape",
        "a cost-benefit analysis of the cost-benefit analysis",
        "a dependency diagram for the dependency diagram",
        "a phase 2 plan for deciding whether to have a phase 2",
    ];

    /// <summary>Synthetic per-cycle delay used to simulate deep contemplation (deterministic for tests).</summary>
    public static readonly TimeSpan DefaultAnalysisDuration = TimeSpan.FromMilliseconds(15);
    /// <summary>Hard deadline from strategy start after which the analysis is declared complete by decree.</summary>
    public static readonly TimeSpan DefaultAbsoluteDeadlineOffset = TimeSpan.FromSeconds(2);

    private readonly TimeSpan _analysisDuration;
    private readonly TimeSpan _absoluteDeadlineOffset;
    private readonly IProcrastiLogger? _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="AnalysisParalysisStrategy"/>.
    /// </summary>
    /// <param name="analysisDuration">Duration of each analysis cycle. Defaults to 15ms.</param>
    /// <param name="absoluteDeadlineOffset">
    /// Hard cap from strategy start after which the analysis is considered done regardless of pending questions.
    /// </param>
    /// <param name="logger">Optional logger for open question commentary. If not provided, logs are suppressed.</param>
    public AnalysisParalysisStrategy(TimeSpan? analysisDuration = null, TimeSpan? absoluteDeadlineOffset = null, IProcrastiLogger? logger = null)
    {
        _analysisDuration = analysisDuration ?? DefaultAnalysisDuration;
        _absoluteDeadlineOffset = absoluteDeadlineOffset ?? DefaultAbsoluteDeadlineOffset;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteCoreAsync(
        Func<Task> task,
        TimeSpan initialDelay,
        IExcuseProvider? excuseProvider,
        IDelayStrategy delayStrategy,
        IRandomProvider randomProvider,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var absoluteDeadline = StartUtc + _absoluteDeadlineOffset;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (await CheckForExternalOverrideAsync(task)) { return; }

            var question = OpenQuestions[randomProvider.GetRandom(OpenQuestions.Length)];
            var deliverable = AnalysisDeliverables[randomProvider.GetRandom(AnalysisDeliverables.Length)];
            _logger?.Info("[AnalysisParalysis] Open question: {Question} Requires: {Deliverable}", question, deliverable);

            await InvokeExcuseAsync(excuseProvider);
            IncrementCycle();
            await delayStrategy.DelayAsync(_analysisDuration, _analysisDuration, cancellationToken: cancellationToken);
            await NotifyCycleAsync(ControlContext, cancellationToken);

            if (SafetyCapReached() || timeProvider.GetUtcNow() >= absoluteDeadline)
            {
                // Analysis is hereby declared sufficient. Proceed, reluctantly.
                break;
            }
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return; // Analysis cancelled. The open questions will remain open indefinitely.
        }

        await task();
        MarkExecuted();
    }
}
