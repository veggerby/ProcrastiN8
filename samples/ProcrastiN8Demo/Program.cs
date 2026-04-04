// ProcrastiN8Demo — A comprehensive, earnest, and entirely unproductive tour of ProcrastiN8.
//
// This program demonstrates every major feature of the ProcrastiN8 library:
// the quantum primitives, the procrastination scheduler, the unproductivity utilities,
// the TODO framework, and the interpretational machinery that makes all of the above
// look scientifically credible to non-physicists.
//
// Estimated time to run: indeterminate.
// Estimated productivity: definitively zero.
// Confidence in shipping this as documentation: surprisingly high.

using System.Reflection;

using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.Services;
using ProcrastiN8.TODOFramework;
using ProcrastiN8.Unproductivity;

var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

static void Section(string title)
{
    Console.WriteLine();
    Console.WriteLine(new string('═', 60));
    Console.WriteLine($"  {title}");
    Console.WriteLine(new string('═', 60));
}

static void Print(string message) => Console.WriteLine($"  {message}");

// ─── Welcome ─────────────────────────────────────────────────────────────────
Console.WriteLine();
Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  PROCRASTIN8 COMPREHENSIVE CAPABILITY DEMONSTRATION       ║");
Console.WriteLine("║  Version: eventually. Delivery: Q4. Quarter: undefined.   ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

// ─── 1. Eventually ───────────────────────────────────────────────────────────
Section("1. Eventually — procrastinating with feeling");
Print("The simplest unit of procrastination. Do a thing. Just... not right now.");

await Eventually.Do(
    async () =>
    {
        await Task.CompletedTask;
        Print("The task has been completed. Eventually.");
    },
    within: TimeSpan.FromSeconds(1),
    cancellationToken: cts.Token);

// ─── 2. FakeProgress ─────────────────────────────────────────────────────────
Section("2. FakeProgress — simulating productivity for a short while");
Print("Nothing useful happens. The progress bar moves. Stakeholders are satisfied.");

using var progressCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(400));
try
{
    await FakeProgress.ShowFakeProgressAsync(
        stepDuration: TimeSpan.FromMilliseconds(50),
        steps: 3,
        logger: null,
        cancellationToken: progressCts.Token);
}
catch (OperationCanceledException)
{
    Print("Progress cancelled. This is fine. The progress was always fake.");
}

// ─── 3. MeetingSimulator ─────────────────────────────────────────────────────
Section("3. MeetingSimulator — consuming time with procedural rigor");
Print("A meeting is convened. Agenda items are discussed. Nothing is decided.");
Print("Action items are generated. Nobody will complete them.");
Print("A follow-up is scheduled.");

using var meetingCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(600));
try
{
    await MeetingSimulator.RunMeetingAsync(
        durationMinutes: 1,
        cancellationToken: meetingCts.Token);
}
catch (OperationCanceledException)
{
    Print("Meeting terminated early due to time constraints.");
    Print("A follow-up will be scheduled to discuss the termination.");
}

// ─── 4. ScopeCreepSimulator ──────────────────────────────────────────────────
Section("4. ScopeCreepSimulator — growing the problem space");
Print("A task is defined. Stakeholders are involved. The scope expands.");

var scope = new ScopeCreepSimulator("Build a button");
scope.AddRequirements(3);
Console.WriteLine();
Console.WriteLine(scope.GetScopeSummary());

// ─── 5. ProcrastinationJournal ───────────────────────────────────────────────
Section("5. ProcrastinationJournal — accountability through documentation");
Print("All procrastination is recorded. This is the honest part.");

var journal = new ProcrastinationJournal();
await ProcrastinationScheduler.Schedule(
    async () => await Task.CompletedTask,
    TimeSpan.FromMilliseconds(100),
    ProcrastinationMode.LastMinute,
    observers: [journal],
    cancellationToken: cts.Token);

Console.WriteLine();
Console.Write(journal.ToFormattedDiary());

// ─── 6. ProcrastinationScheduler — all three new modes ───────────────────────
Section("6. ProcrastinationScheduler — CommitteeReview");
Print("A committee is convened. Consensus is not reached. The task runs at deadline.");
Print("Committee approval is assumed retroactively.");

var committeeResult = await ProcrastinationScheduler.ScheduleWithResult(
    async () => await Task.CompletedTask,
    TimeSpan.FromMilliseconds(200),
    ProcrastinationMode.CommitteeReview,
    cancellationToken: cts.Token);
Print($"Status: {committeeResult.Mode}. Executed: {committeeResult.Executed}. Deferral: {committeeResult.TotalDeferral.TotalMilliseconds:F0}ms");

Section("6b. ProcrastinationScheduler — AnalysisParalysis");
Print("Open questions multiply. A SWOT analysis of the SWOT analysis is requested.");
Print("Analysis is declared sufficient by executive mandate.");

var paralysisResult = await ProcrastinationScheduler.ScheduleWithResult(
    async () => await Task.CompletedTask,
    TimeSpan.FromMilliseconds(200),
    ProcrastinationMode.AnalysisParalysis,
    cancellationToken: cts.Token);
Print($"Status: {paralysisResult.Mode}. Executed: {paralysisResult.Executed}. Deferral: {paralysisResult.TotalDeferral.TotalMilliseconds:F0}ms");

Section("6c. ProcrastinationScheduler — LastMinute");
Print("Nothing happens for 90% of the window. Then: productive panic.");

var lastMinuteResult = await ProcrastinationScheduler.ScheduleWithResult(
    async () => await Task.CompletedTask,
    TimeSpan.FromMilliseconds(300),
    ProcrastinationMode.LastMinute,
    cancellationToken: cts.Token);
Print($"Status: {lastMinuteResult.Mode}. Executed: {lastMinuteResult.Executed}. Deferral: {lastMinuteResult.TotalDeferral.TotalMilliseconds:F0}ms");

// ─── 7. Quantum Primitives ────────────────────────────────────────────────────
Section("7. Procrastinable<T> — Lazy<T> with a deliberation window");
Print("Like Lazy<T>, but it takes a moment to decide. Result is cached.");

var procrastinable = new Procrastinable<string>(
    () => Task.FromResult("The value, once obtained, is final."));

var value1 = await procrastinable.EvaluateAsync(cts.Token);
var value2 = await procrastinable.EvaluateAsync(cts.Token);
Print($"First evaluation: \"{value1}\"");
Print($"Second evaluation: \"{value2}\" (same object — the effort was cached)");

// ─── 8. QuantumAbortToken ────────────────────────────────────────────────────
Section("8. QuantumAbortToken — cancels when importance is observed");
Print("The more important the task, the more likely it is cancelled.");
Print("This is not a bug. It is the observable universe doing its job.");

using var abortToken = new QuantumAbortToken(baseCancellationProbability: 0.0);
abortToken.ObserveImportance(importance: 0.0); // 0% chance — safe for demos
var abortStatus = abortToken.Token.IsCancellationRequested ? "cancelled" : "proceeding";
Print($"Task with 0% base probability at importance 0.0: {abortStatus}");
Print("(In production, set importance to 2.0 and watch everything stop immediately.)");

// ─── 9. ProbabilityOfSuccess with interpretations ────────────────────────────
Section("9. ProbabilityOfSuccess — fate as a configurable slider");

foreach (var interp in new[] { QuantumInterpretations.Copenhagen, QuantumInterpretations.QBist, QuantumInterpretations.PilotWave })
{
    var effective = interp.InterpretProbability(0.4, 0.6);
    Print($"{interp.Name,-15}: stated=0.6, sample=0.4 → effective={effective:F2}");
}

Print("QBist adds confidence. PilotWave determinises. Copenhagen accepts reality as-is.");

// ─── 10. QuantumTunnel ───────────────────────────────────────────────────────
Section("10. QuantumTunnel — passing through exception barriers");
Print("Exceptions are observed, noted, and tunnelled. Classical physics need not apply.");

var tunnelResult = await QuantumTunnel.TunnelAsync(
    () => throw new InvalidOperationException("This exception was classically real."),
    fallback: "tunnelled successfully",
    tunnelingProbability: 1.0,
    interpretation: QuantumInterpretations.Copenhagen);

Print($"Result: \"{tunnelResult}\"");
Print("The exception was acknowledged. It was also ignored. Both things are true.");

// Under PilotWave (deterministic): probability ≥ 0.5 → 1.0 (always tunnels at stated ≥ 0.5)
var pilotResult = QuantumTunnel.Tunnel(
    () => throw new Exception("Barrier."),
    fallback: "pilot wave tunnelled",
    tunnelingProbability: 0.8,
    interpretation: QuantumInterpretations.PilotWave);
Print($"Pilot Wave result: \"{pilotResult}\" (0.8 ≥ 0.5 → certain tunnelling)");

// ─── 11. ObserverDependentValue ──────────────────────────────────────────────
Section("11. ObserverDependentValue — truth is caller-relative");
Print("Different callers receive different truths from the same value.");
Print("This is the Copenhagen interpretation applied to software architecture.");

var observerValue = new ObserverDependentValue<string>("the real situation")
    .For("Management", "everything is on track")
    .For("Client", "delivering significant value")
    .For("Engineering", "this is a controlled burn");

Print($"Management sees:   \"{observerValue.Observe("Management")}\"");
Print($"Client sees:       \"{observerValue.Observe("Client")}\"");
Print($"Engineering sees:  \"{observerValue.Observe("Engineering")}\"");
Print($"Auditor sees:      \"{observerValue.Observe("Auditor")}\" (default — the auditor is not on the invite list)");

// ─── 12. ManyWorldsScheduler ─────────────────────────────────────────────────
Section("12. ManyWorldsScheduler — all timelines, simultaneously");
Print("Five universes are spawned. The first to succeed collapses reality.");
Print("Under Copenhagen: surviving universes are cancelled.");
Print("Under Many-Worlds: they continue to exist. We just don't live in them.");

var attempt = 0;
var mwResult = await ManyWorldsScheduler.ScheduleAsync(
    async (universeIndex) =>
    {
        await Task.Delay(universeIndex * 5);
        Interlocked.Increment(ref attempt);
        return $"Universe {universeIndex} delivered the result";
    },
    universeCount: 5,
    interpretation: QuantumInterpretations.Copenhagen,
    cancellationToken: cts.Token);

Print($"Prime timeline: \"{mwResult}\"");

// ─── 13. CollapseOnReview ────────────────────────────────────────────────────
Section("13. CollapseOnReview — behaves differently when observed");
Print("In production: returns the real value. Under review: returns the safe value.");
Print("This is technically dishonest. It is also how most systems behave.");

var reviewable = new CollapseOnReview<string>(
    productionValue: "cutting corners, shipping fast, hoping for the best",
    reviewValue: "following all best practices, fully tested, production-ready",
    interpretation: QuantumInterpretations.Copenhagen);

Print($"Currently under review: {reviewable.IsUnderReview}");
Print($"Resolved value: \"{reviewable.Resolve()}\"");
Print("(Running inside xUnit, so the review context was detected. Copenhagen collapses.)");

// ─── 14. RetryInSuperposition ────────────────────────────────────────────────
Section("14. RetryInSuperposition — all retries, simultaneously");
Print("All attempts run in parallel. First to succeed collapses reality.");
Print("Remaining timelines are cancelled and properly observed.");

var retryCount = 0;
var superResult = await RetryInSuperposition.ExecuteAsync(
    async () =>
    {
        var current = Interlocked.Increment(ref retryCount);
        await Task.Delay(current * 5, cts.Token);
        return $"Timeline {current} succeeded";
    },
    maxAttempts: 4,
    cancellationToken: cts.Token);

Print($"Result: \"{superResult}\"");

// ─── 15. QuantumMutex ────────────────────────────────────────────────────────
Section("15. QuantumMutex — exclusivity not guaranteed");
Print("Every thread receives its own private lock. All acquisitions succeed.");
Print("SimultaneousHolders shows how many threads believe they hold the lock.");
Print("Each is correct from their own perspective.");

using var mutex = new QuantumMutex();
using var handle1 = await mutex.AcquireAsync(cts.Token);
Print($"Simultaneous holders after first acquire: {mutex.SimultaneousHolders}");
using var handle2 = await mutex.AcquireAsync(cts.Token); // re-entrant on same logical context
Print($"Simultaneous holders after second acquire: {mutex.SimultaneousHolders}");
Print("(This mutex does not ensure exclusivity. It does ensure all acquisitions succeed.)");

// ─── 16. TechnicalDebtCollector + TODOFramework ──────────────────────────────
Section("16. TechnicalDebtCollector — scanning for honest annotations");
Print("Scans assemblies for [WontFix] and [SomedayMaybe] annotations.");
Print("Produces a formatted report suitable for retrospectives or confessional emails.");

var report = new TechnicalDebtCollector().Collect(Assembly.GetExecutingAssembly());
Console.WriteLine();
Console.Write(report.ToFormattedReport());

// ─── 17. IQuantumInterpretation — all six interpretations ────────────────────
Section("17. IQuantumInterpretation — the full philosophical landscape");
Print("Six named interpretations. Each governs a different aspect of quantum behaviour.");
Print("All are self-consistent. None are experimentally distinguishable.");
Console.WriteLine();

foreach (var interp in QuantumInterpretations.All)
{
    var behavior = CollapseBehaviorFactory.Create<int>(interp);
    Console.WriteLine($"  {interp.Name,-15}: obs={interp.ObservationAffectsOutcome,-5} | parallel={interp.ParallelTimelinesAreReal,-5} | tunnel={interp.TunnellingPermitted,-5} | collapse={behavior.GetType().Name}");
}

// ─── Closing ─────────────────────────────────────────────────────────────────
Console.WriteLine();
Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  Demo complete.                                           ║");
Console.WriteLine("║  No deliverables were produced.                          ║");
Console.WriteLine("║  A follow-up has been scheduled.                         ║");
Console.WriteLine("║  Committee approval has been assumed retroactively.      ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
Console.WriteLine();

// ─── Annotated types for TechnicalDebtCollector demo ─────────────────────────

/// <summary>
/// Sample class demonstrating the TODOFramework attributes.
/// Do not refactor. Do not fix. Do not look directly at it.
/// </summary>
[WontFix("It works on my machine. Deployment is out of scope.")]
[SomedayMaybe("Add unit tests for this class", estimatedYear: 2031)]
[SomedayMaybe("Refactor into microservices", estimatedYear: 2029)]
internal static class LegacyPaymentProcessor
{
    [WontFix("The rounding error is technically within spec if you squint.")]
    [SomedayMaybe("Replace with a currency library", estimatedYear: 2030)]
    internal static decimal ProcessPayment(decimal amount) => Math.Round(amount * 1.0001m, 2);

    [WontFix("Worked in testing. Has not been tested in production.")]
    internal static void RefundPayment(decimal amount) { /* left as an exercise for the reader */ }
}
