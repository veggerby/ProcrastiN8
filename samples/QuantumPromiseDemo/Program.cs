// QuantumPromiseDemo — A guided tour of ProcrastiN8's quantum mechanics capabilities.
//
// This demo demonstrates the QuantumPromise<T> API and the IQuantumInterpretation system
// in a way that is technically accurate, philosophically irreverent, and enterprise-grade.
//
// Running time: approximately as long as your patience holds.
// Deliverables: none. Outcomes: uncertain by design.

using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║   PROCRASTIN8 QUANTUM PROMISE DEMO                       ║");
Console.WriteLine("║   Uncertainty, Delivered with Confidence™                ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
Console.WriteLine();

// ─── 1. QuantumPromise basics ────────────────────────────────────────────────
Console.WriteLine("1. BASIC QUANTUM PROMISE");
Console.WriteLine("   A QuantumPromise is a value that exists in superposition until observed.");
Console.WriteLine("   Unlike a regular promise, it has philosophical backing.");
Console.WriteLine();

var promise = new QuantumPromise<string>(
    () => Task.FromResult("The deliverable exists."),
    TimeSpan.FromMilliseconds(50));

Console.WriteLine("   [Creating promise...]");
var observed = await promise.ObserveAsync();
Console.WriteLine($"   Observed: \"{observed}\"");
Console.WriteLine("   The wavefunction collapsed. The deliverable is real. For now.");
Console.WriteLine();

// ─── 2. IQuantumInterpretation and CollapseBehavior bridge ──────────────────
Console.WriteLine("2. QUANTUM INTERPRETATIONS → COLLAPSE BEHAVIORS");
Console.WriteLine("   Each IQuantumInterpretation governs which ICollapseBehavior is used.");
Console.WriteLine("   CollapseBehaviorFactory.Create<T>(interpretation) bridges both systems.");
Console.WriteLine();

foreach (var interpretation in QuantumInterpretations.All)
{
    var behavior = CollapseBehaviorFactory.Create<string>(interpretation);
    Console.WriteLine($"   {interpretation.Name,-15} → {behavior.GetType().Name}");
}

Console.WriteLine();
Console.WriteLine("   All six interpretations have a corresponding collapse behavior.");
Console.WriteLine("   None of them are experimentally distinguishable.");
Console.WriteLine("   All are held with considerable conviction.");
Console.WriteLine();

// ─── 3. Observer-dependent collapse ─────────────────────────────────────────
Console.WriteLine("3. ENTANGLEMENT ACROSS INTERPRETATIONS");
Console.WriteLine("   Two promises, entangled under the Copenhagen interpretation.");
Console.WriteLine("   Observing one collapses the other. This is intentional.");
Console.WriteLine();

var p1 = new QuantumPromise<int>(() => Task.FromResult(1), TimeSpan.FromMilliseconds(10));
var p2 = new QuantumPromise<int>(() => Task.FromResult(2), TimeSpan.FromMilliseconds(10));

var copenhagenBehavior = CollapseBehaviorFactory.Create<int>(QuantumInterpretations.Copenhagen);
p1.Entangle(copenhagenBehavior, p2);

var v1 = await p1.ObserveAsync();
var v2 = await p2.ObserveAsync();
Console.WriteLine($"   promise1 observed: {v1}");
Console.WriteLine($"   promise2 observed: {v2}");
Console.WriteLine("   (Under Copenhagen, both collapse to the same value. This is the price of observation.)");
Console.WriteLine();

// ─── 4. Many-Worlds: parallel collapse ──────────────────────────────────────
Console.WriteLine("4. MANY-WORLDS: PARALLEL TIMELINES");
Console.WriteLine("   Under Many-Worlds, all outcomes exist simultaneously.");
Console.WriteLine("   We simply inhabit one. The others are not our problem.");
Console.WriteLine();

var manyWorldsBehavior = CollapseBehaviorFactory.Create<string>(QuantumInterpretations.ManyWorlds);
Console.WriteLine($"   Behavior: {manyWorldsBehavior.GetType().Name}");
Console.WriteLine($"   Parallel timelines real: {QuantumInterpretations.ManyWorlds.ParallelTimelinesAreReal}");
Console.WriteLine("   (Surviving timelines are left to complete. Cancelling them would be philosophically incorrect.)");
Console.WriteLine();

// ─── 5. Pilot Wave: determinism ─────────────────────────────────────────────
Console.WriteLine("5. PILOT WAVE: DETERMINISTIC PROBABILITY");
Console.WriteLine("   In the Pilot Wave interpretation, randomness is epistemic only.");
Console.WriteLine("   The outcome was fixed at initialization. We just don't know which one.");
Console.WriteLine();

var pw = QuantumInterpretations.PilotWave;
Console.WriteLine($"   InterpretProbability(0.3, 0.8) = {pw.InterpretProbability(0.3, 0.8):F1}  (≥0.5 → 1.0)");
Console.WriteLine($"   InterpretProbability(0.9, 0.3) = {pw.InterpretProbability(0.9, 0.3):F1}  (<0.5 → 0.0)");
Console.WriteLine("   Probability is either certain or impossible. The pilot wave has spoken.");
Console.WriteLine();

// ─── 6. QuantumInterpretations.ByName ───────────────────────────────────────
Console.WriteLine("6. INTERPRETATION LOOKUP BY NAME");
Console.WriteLine();

var byName = QuantumInterpretations.ByName("Relational");
Console.WriteLine($"   ByName(\"Relational\") → {byName.Name}: {byName.Description}");

var unknown = QuantumInterpretations.ByName("FlatEarth");
Console.WriteLine($"   ByName(\"FlatEarth\") → {unknown.Name} (default: unknown interpretations collapse to Copenhagen)");
Console.WriteLine();

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║   Demo complete. No waveforms were harmed.               ║");
Console.WriteLine("║   Consensus was not reached. This was expected.          ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

