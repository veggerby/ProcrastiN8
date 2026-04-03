using System.Diagnostics;

using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.Metrics;
using ProcrastiN8.Services;

namespace ProcrastiN8.Unproductivity;

/// <summary>
/// Simulates a stakeholder meeting in which time is consumed, action items are generated,
/// and no decisions are made. Output guaranteed to be indistinguishable from the real thing.
/// </summary>
public static class MeetingSimulator
{
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.Unproductivity.MeetingSimulator");

    private static readonly string[] AgendaItems =
    [
        "Review last sprint's action items (none were completed)",
        "Discuss blockers that nobody mentioned in standups",
        "Re-explain the requirements to those who attended the last re-explanation",
        "Push the demo to next week (again)",
        "Schedule a follow-up meeting to discuss scheduling",
        "Brainstorm synonyms for 'done'",
        "Debate the definition of 'done'",
        "Request an async update that could have been an email",
        "Align on the alignment strategy",
        "Revisit the parking lot from the last three meetings",
        "Circle back on the circling back",
        "Present the roadmap that superseded the previous roadmap",
    ];

    private static readonly string[] ActionItems =
    [
        "Everyone: Review the deck by EOD (no deadline specified)",
        "TBD: Own the follow-up (owner TBD)",
        "Someone: Update Confluence with what we just decided (decision TBD)",
        "All: Block calendars for the next meeting",
        "Open: Revisit in next sprint planning",
        "Team: Async alignment before Thursday (or whenever)",
        "Champion TBD: Draft a one-pager on what a one-pager should contain",
        "Anyone: Socialize the concept before the next socialization meeting",
    ];

    private static readonly string[] ClosingRemarks =
    [
        "Great meeting, everyone. Lots of progress.",
        "Good conversation. Let's take this offline.",
        "Thanks for the time, team. See you in the follow-up.",
        "Let's keep the momentum going. Whatever that means.",
        "Super productive session. The action items will be sent shortly (they will not).",
        "We've really moved the needle today. Please do not ask which direction.",
    ];

    /// <summary>
    /// Runs a simulated meeting of the specified duration. Consumes time. Generates action items.
    /// Produces no deliverables.
    /// </summary>
    /// <param name="durationMinutes">
    /// The nominal duration of the meeting in minutes. Each minute is represented by a proportional delay.
    /// Defaults to 30 (because all meetings default to 30 minutes and run over).
    /// </param>
    /// <param name="logger">Optional logger for meeting updates and procedural commentary.</param>
    /// <param name="randomProvider">Optional random provider for agenda and action item selection.</param>
    /// <param name="commentaryService">Optional commentary service for between-agenda observations.</param>
    /// <param name="delayProvider">Optional delay provider. Defaults to <see cref="TaskDelayProvider"/>.</param>
    /// <param name="cancellationToken">Token to leave the meeting early. Frowned upon but supported.</param>
    public static async Task RunMeetingAsync(
        int durationMinutes = 30,
        IProcrastiLogger? logger = null,
        IRandomProvider? randomProvider = null,
        ICommentaryService? commentaryService = null,
        IDelayProvider? delayProvider = null,
        CancellationToken cancellationToken = default)
    {
        logger ??= new DefaultLogger();
        randomProvider ??= RandomProvider.Default;
        commentaryService ??= new CommentaryService(randomProvider);
        delayProvider ??= new TaskDelayProvider();

        // Each "minute" of meeting = 1ms tick for deterministic tests
        var tickDuration = TimeSpan.FromMilliseconds(Math.Max(1, durationMinutes));

        using var activity = ActivitySource.StartActivity("ProcrastiN8.MeetingSimulator.RunMeeting", ActivityKind.Internal);
        activity?.SetTag("meeting.durationMinutes", durationMinutes);

        logger.Info("[MeetingSimulator] Meeting started. Estimated duration: {Duration} minute(s).", durationMinutes);
        logger.Info("[MeetingSimulator] Reminder: this could have been an email.");

        var agendaCount = randomProvider.GetRandom(3, Math.Min(AgendaItems.Length + 1, Math.Max(4, durationMinutes)));

        try
        {
            for (var i = 0; i < agendaCount; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    logger.Info("[MeetingSimulator] Participant left the meeting early. Meeting continues without them.");
                    break;
                }

                var agenda = AgendaItems[randomProvider.GetRandom(AgendaItems.Length)];
                logger.Info("[MeetingSimulator] Agenda item {Item}/{Total}: {Agenda}", i + 1, agendaCount, agenda);
                commentaryService.LogRandomRemark();

                await delayProvider.DelayAsync(tickDuration, cancellationToken);

                ProcrastinationMetrics.TotalTimeProcrastinated.Add(
                    (long)tickDuration.TotalSeconds,
                    KeyValuePair.Create<string, object?>("component", "MeetingSimulator"));
            }

            // Generate action items
            var actionCount = randomProvider.GetRandom(2, 5);
            logger.Info("[MeetingSimulator] Meeting concluded. Generating {Count} action item(s) that will not be completed.", actionCount);

            for (var i = 0; i < actionCount; i++)
            {
                var action = ActionItems[randomProvider.GetRandom(ActionItems.Length)];
                logger.Info("[MeetingSimulator] Action Item #{Item}: {Action}", i + 1, action);

                ProcrastinationMetrics.ExcusesGenerated.Add(1,
                    KeyValuePair.Create<string, object?>("category", "meeting-action-item"));
            }

            var closing = ClosingRemarks[randomProvider.GetRandom(ClosingRemarks.Length)];
            logger.Info("[MeetingSimulator] {Closing}", closing);
            logger.Info("[MeetingSimulator] Follow-up meeting scheduled. Nothing was decided.");

            ProcrastinationMetrics.TasksNeverDone.Add(1);
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (OperationCanceledException)
        {
            logger.Info("[MeetingSimulator] Meeting terminated early. Notes will be sent out. They will not.");
            ProcrastinationMetrics.TasksNeverDone.Add(1);
            activity?.SetStatus(ActivityStatusCode.Ok, "Meeting left early");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "[MeetingSimulator] Something went wrong during the meeting. A retrospective will be scheduled.");
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}
