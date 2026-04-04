using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class ProcrastinationJournalTests
{
    [Fact]
    public async Task Journal_RecordsEvents_FromObserver()
    {
        // arrange
        var journal = new ProcrastinationJournal();
        var correlationId = Guid.NewGuid();

        var evt = new ProcrastinationObserverEvent(
            correlationId,
            ProcrastinationMode.MovingTarget,
            "cycle",
            Cycles: 1,
            Excuses: 0,
            Triggered: false,
            Abandoned: false,
            Timestamp: DateTimeOffset.UtcNow);

        // act
        await ((IProcrastinationObserver)journal).OnEventAsync(evt);

        // assert
        journal.Count.Should().Be(1, "each event generates one journal entry");
        journal.Entries[0].EventType.Should().Be("cycle");
        journal.Entries[0].CorrelationId.Should().Be(correlationId);
        journal.Entries[0].Mode.Should().Be(ProcrastinationMode.MovingTarget);
    }

    [Fact]
    public async Task Journal_RecordsMultiple_EventTypes()
    {
        // arrange
        var journal = new ProcrastinationJournal();
        var correlationId = Guid.NewGuid();

        var eventTypes = new[] { "cycle", "excuse", "triggered", "abandoned", "executed" };

        // act — emit all known event types
        foreach (var eventType in eventTypes)
        {
            await ((IProcrastinationObserver)journal).OnEventAsync(new ProcrastinationObserverEvent(
                correlationId,
                ProcrastinationMode.CommitteeReview,
                eventType,
                Cycles: 1,
                Excuses: 1,
                Triggered: eventType == "triggered",
                Abandoned: eventType == "abandoned",
                Timestamp: DateTimeOffset.UtcNow));
        }

        // assert — all five event types are in the diary
        journal.Count.Should().Be(5, "each event type generates a journal entry");
        journal.Entries.Select(e => e.EventType).Should().BeEquivalentTo(eventTypes);
    }

    [Fact]
    public async Task ToFormattedDiary_ContainsAllEntries()
    {
        // arrange
        var journal = new ProcrastinationJournal();
        var correlationId = Guid.NewGuid();

        await ((IProcrastinationObserver)journal).OnEventAsync(new ProcrastinationObserverEvent(
            correlationId,
            ProcrastinationMode.AnalysisParalysis,
            "cycle",
            Cycles: 3,
            Excuses: 2,
            Triggered: false,
            Abandoned: false,
            Timestamp: DateTimeOffset.UtcNow));

        // act
        var diary = journal.ToFormattedDiary();

        // assert — the diary contains a header and the session content
        diary.Should().Contain("# Procrastination Journal", "the diary has a professional title");
        diary.Should().Contain("AnalysisParalysis", "the mode is documented for forensic review");
        diary.Should().Contain("cycle", "the event type appears in the entry");
    }

    [Fact]
    public void EmptyJournal_ToFormattedDiary_IsCoherent()
    {
        // arrange
        var journal = new ProcrastinationJournal();

        // act
        var diary = journal.ToFormattedDiary();

        // assert — an empty journal still produces a valid document
        diary.Should().Contain("0 entry/entries recorded", "zero is still a valid number of procrastination sessions");
    }

    [Fact]
    public async Task Journal_IsThreadSafe_UnderConcurrentEvents()
    {
        // arrange
        var journal = new ProcrastinationJournal();
        const int eventCount = 50;

        // act — fire events from many tasks concurrently
        var tasks = Enumerable.Range(0, eventCount).Select(i =>
            ((IProcrastinationObserver)journal).OnEventAsync(new ProcrastinationObserverEvent(
                Guid.NewGuid(),
                ProcrastinationMode.MovingTarget,
                "cycle",
                Cycles: i,
                Excuses: 0,
                Triggered: false,
                Abandoned: false,
                Timestamp: DateTimeOffset.UtcNow)));

        await Task.WhenAll(tasks);

        // assert — no entries lost to race conditions
        journal.Count.Should().Be(eventCount, "thread-safe appends preserve all concurrent entries");
    }
}
