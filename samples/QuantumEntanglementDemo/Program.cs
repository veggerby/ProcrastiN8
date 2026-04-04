using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

var serviceName = "ProcrastiN8.Samples.QuantumEntanglementDemo";
var serviceVersion = "1.0.0";

var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(serviceName)
    .AddSource("ProcrastiN8.*")
    .ConfigureResource(resource =>
        resource.AddService(
          serviceName: serviceName,
          serviceVersion: serviceVersion))
    .AddConsoleExporter()
    .AddOtlpExporter()
    .Build();

var meterProvider = Sdk.CreateMeterProviderBuilder()
    .AddMeter("ProcrastiN8.*")
    .AddConsoleExporter()
    .AddOtlpExporter()
    .Build();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddOpenTelemetry(logging =>
    {
        logging.AddConsoleExporter().AddOtlpExporter();
    });
});

Console.WriteLine("Quantum Entanglement Demo Starting...");
Console.WriteLine();

// ─── Interpretation-driven collapse ─────────────────────────────────────────
// IQuantumInterpretation integrates with ICollapseBehavior via
// CollapseBehaviorFactory.Create<T>(interpretation) — letting the interpretation
// govern the collapse semantics without requiring the caller to know which
// concrete behavior class corresponds to which philosophical worldview.

foreach (var interpretation in QuantumInterpretations.All)
{
    Console.WriteLine($"--- {interpretation.Name} interpretation ---");
    Console.WriteLine($"    {interpretation.Description}");

    // CollapseBehaviorFactory.Create<T>(IQuantumInterpretation) delegates to
    // interpretation.GetCollapseBehavior<T>(), bridging both abstraction layers.
    var behavior = CollapseBehaviorFactory.Create<int>(interpretation);
    Console.WriteLine($"    Collapse behavior: {behavior.GetType().Name}");
    Console.WriteLine($"    Observation affects outcome: {interpretation.ObservationAffectsOutcome}");
    Console.WriteLine($"    Parallel timelines real:     {interpretation.ParallelTimelinesAreReal}");
    Console.WriteLine();
}

// ─── Live quantum entanglement with Copenhagen interpretation ───────────────
Console.WriteLine("--- Live entanglement (Copenhagen) ---");

var copenhagenBehavior = CollapseBehaviorFactory.Create<int>(QuantumInterpretations.Copenhagen);

var promise1 = new QuantumPromise<int>(() => Task.FromResult(42), TimeSpan.FromSeconds(2));
var promise2 = new QuantumPromise<int>(() => Task.FromResult(99), TimeSpan.FromSeconds(2));

promise1.Entangle(copenhagenBehavior, promise2);

await Task.Delay(2500);

var result1 = await promise1.ObserveAsync();
Console.WriteLine($"Observed value of promise1: {result1}");

var result2 = await promise2.ObserveAsync();
Console.WriteLine($"Observed value of promise2: {result2}");
Console.WriteLine("(Both collapsed to the same value. Copenhagen: only one truth survives observation.)");

Console.WriteLine();
Console.WriteLine("Quantum Entanglement Demo Completed.");
Console.WriteLine("No consensus was reached. This is expected.");

tracerProvider.Dispose();
meterProvider.Dispose();
loggerFactory.Dispose();