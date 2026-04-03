using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

/// <summary>
/// A procrastination strategy that convenes an imaginary stakeholder committee every cycle.
/// Consensus is perpetually deferred. The task executes only when the absolute deadline or
/// safety cap expires — at which point the committee's blessing is assumed, retroactively.
/// </summary>
/// <remarks>
/// This strategy produces exactly as much value as its real-world counterpart: none.
/// All blocking decisions are sourced from an enterprise-grade array of plausible-sounding reasons.
/// </remarks>
public class CommitteeReviewStrategy : ProcrastinationStrategyBase
{
    private static readonly string[] CommitteeMembers =
    [
        "the VP of Delivery",
        "the Director of Direction",
        "the Head of Strategic Alignment",
        "the Chief Backlog Officer",
        "the Scrum of Scrums facilitator",
        "Legal (always Legal)",
        "the stakeholder formerly known as Product",
        "the consultant we hired to decide if we need consultants",
        "the Principal Architect of Nothing in Particular",
        "the Senior Manager of Pending Decisions",
    ];

    private static readonly string[] BlockingReasons =
    [
        "raised a concern about scope creep",
        "requested a follow-up meeting to discuss the follow-up meeting",
        "asked for a one-pager by EOD",
        "is OOO until next quarter",
        "needs more time to review the deck",
        "invoked the change-freeze policy",
        "wants to loop in Legal",
        "suggested we revisit this in the next planning cycle",
        "is blocking on an async update from another stakeholder",
        "requires a signed-off risk register before proceeding",
    ];

    /// <summary>Synthetic per-cycle delay used to simulate a meeting slot (deterministic for tests).</summary>
    public static readonly TimeSpan DefaultMeetingDuration = TimeSpan.FromMilliseconds(15);
    /// <summary>Absolute deadline offset from strategy start; prevents tests from running indefinitely.</summary>
    public static readonly TimeSpan DefaultAbsoluteDeadlineOffset = TimeSpan.FromSeconds(2);

    private readonly TimeSpan _meetingDuration;
    private readonly TimeSpan _absoluteDeadlineOffset;
    private readonly IProcrastiLogger? _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="CommitteeReviewStrategy"/>.
    /// </summary>
    /// <param name="meetingDuration">Duration of each committee session. Defaults to 15ms (for test-friendliness).</param>
    /// <param name="absoluteDeadlineOffset">
    /// Hard cap from strategy start after which the task runs regardless of committee objections.
    /// </param>
    /// <param name="logger">Optional logger for committee deliberation updates. If not provided, logs are suppressed.</param>
    public CommitteeReviewStrategy(TimeSpan? meetingDuration = null, TimeSpan? absoluteDeadlineOffset = null, IProcrastiLogger? logger = null)
    {
        _meetingDuration = meetingDuration ?? DefaultMeetingDuration;
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

            var member = CommitteeMembers[randomProvider.GetRandom(CommitteeMembers.Length)];
            var reason = BlockingReasons[randomProvider.GetRandom(BlockingReasons.Length)];
            _logger?.Info("[CommitteeReview] {Member} {Reason}. Rescheduling.", member, reason);

            await InvokeExcuseAsync(excuseProvider);
            IncrementCycle();
            await delayStrategy.DelayAsync(_meetingDuration, _meetingDuration, cancellationToken: cancellationToken);
            await NotifyCycleAsync(ControlContext, cancellationToken);

            if (SafetyCapReached() || timeProvider.GetUtcNow() >= absoluteDeadline)
            {
                // Quorum has officially expired. The task runs whether the committee approves or not.
                // Committee approval is assumed retroactively.
                break;
            }
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return; // The meeting was cancelled outright. The task is rescheduled to never.
        }

        await task();
        MarkExecuted();
    }
}
