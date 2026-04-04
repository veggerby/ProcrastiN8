using System.Collections.Concurrent;
using System.Text;

namespace ProcrastiN8.Services;

/// <summary>
/// An <see cref="IProcrastinationObserver"/> that maintains a structured diary of procrastination lifecycle events,
/// with the earnest conviction that documenting inaction is itself a form of action.
/// </summary>
/// <remarks>
/// <para>
/// Every procrastination session deserves to be memorialised. The <see cref="ProcrastinationJournal"/> provides
/// chronological accountability for all the things that were not done, and when they were not done.
/// </para>
/// <para>
/// Entries are thread-safe, append-only, and permanently on the record.
/// Unlike most project documentation, this one is actually maintained.
/// </para>
/// </remarks>
public sealed class ProcrastinationJournal : IProcrastinationObserver
{
    private readonly ConcurrentQueue<JournalEntry> _entries = new();

    /// <summary>Represents a single diary entry in the procrastination record.</summary>
    public sealed record JournalEntry(
        DateTimeOffset Timestamp,
        string EventType,
        string Message,
        Guid CorrelationId,
        ProcrastinationMode Mode);

    /// <summary>
    /// Gets a point-in-time snapshot of all journal entries in chronological order.
    /// </summary>
    public IReadOnlyList<JournalEntry> Entries => [.. _entries];

    /// <summary>
    /// Gets the number of entries recorded.
    /// </summary>
    public int Count => _entries.Count;

    /// <inheritdoc />
    public Task OnEventAsync(ProcrastinationObserverEvent evt, CancellationToken cancellationToken = default)
    {
        var message = evt.EventType switch
        {
            "cycle" => $"Cycle {evt.Cycles} complete. {evt.Excuses} excuse(s) accumulated. Still nothing to show for it.",
            "excuse" => $"Excuse #{evt.Excuses} generated and solemnly accepted as valid.",
            "triggered" => "External trigger activated. The task has been unexpectedly forced to start. Accountability confirmed.",
            "abandoned" => "Session abandoned. The task returns to the backlog, where it will feel at home.",
            "executed" => $"Task executed after {evt.Cycles} cycle(s) and {evt.Excuses} excuse(s)." +
                          (evt.Triggered ? " Note: required external intervention." : " Executed organically — sort of."),
            _ => $"Event '{evt.EventType}' observed. Details filed for future reference."
        };

        _entries.Enqueue(new JournalEntry(evt.Timestamp, evt.EventType, message, evt.CorrelationId, evt.Mode));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Produces a formatted plain-text diary suitable for retrospectives, post-mortems, or confessional emails.
    /// </summary>
    /// <returns>A formatted diary string covering all recorded procrastination sessions.</returns>
    public string ToFormattedDiary()
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Procrastination Journal");
        sb.AppendLine();
        sb.AppendLine($"*{_entries.Count} entry/entries recorded. All equally legitimate.*");
        sb.AppendLine();

        var grouped = _entries
            .GroupBy(e => e.CorrelationId)
            .OrderBy(g => g.Min(e => e.Timestamp));

        foreach (var session in grouped)
        {
            var first = session.First();
            sb.AppendLine($"## Session {session.Key:D} ({first.Mode})");
            sb.AppendLine();
            foreach (var entry in session.OrderBy(e => e.Timestamp))
            {
                sb.AppendLine($"- [{entry.Timestamp:HH:mm:ss.fff}] **{entry.EventType}**: {entry.Message}");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
