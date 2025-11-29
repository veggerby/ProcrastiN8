using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

var serviceName = "ProcrastiN8.Samples.QuantumPromiseDemo";
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

Console.WriteLine("===========================================");
Console.WriteLine("🐢 ProcrastiN8 QuantumPromise Demo Starting...");
Console.WriteLine("===========================================");
Console.WriteLine();

// -------------------------------------------
// Demo 1: Basic QuantumPromise observation
// -------------------------------------------
Console.WriteLine("📌 Demo 1: Basic QuantumPromise Observation");
Console.WriteLine("  Creating a quantum promise that exists in superposition...");
Console.WriteLine();

try
{
    // Create a promise with a 5-second Schrödinger window
    var promise = new QuantumPromise<int>(
        lazyInitializer: () => Task.FromResult(42),
        schrodingerWindow: TimeSpan.FromSeconds(5));

    Console.WriteLine($"  🌀 Promise state: {promise}");

    // Wait a bit before observing (must wait at least 200ms but not exceed window)
    await Task.Delay(500);

    Console.WriteLine("  👁️ Observing the promise...");
    var result = await promise.ObserveAsync();

    Console.WriteLine($"  ✅ Observed value: {result}");
    Console.WriteLine($"  📦 Promise state after observation: {promise}");
}
catch (CollapseTooEarlyException ex)
{
    Console.WriteLine($"  ⚠️ Observed too early: {ex.Message}");
}
catch (CollapseTooLateException ex)
{
    Console.WriteLine($"  ⏰ Observed too late: {ex.Message}");
}
catch (CollapseToVoidException ex)
{
    Console.WriteLine($"  🕳️ Collapsed to void: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"  ❌ Unexpected error: {ex.Message}");
}

Console.WriteLine();

// -------------------------------------------
// Demo 2: QuantumPromise with string value
// -------------------------------------------
Console.WriteLine("📌 Demo 2: QuantumPromise with String Value");
Console.WriteLine("  Demonstrating quantum superposition with text...");
Console.WriteLine();

try
{
    var textPromise = new QuantumPromise<string>(
        lazyInitializer: () => Task.FromResult("The answer is within!"),
        schrodingerWindow: TimeSpan.FromSeconds(5));

    // Must wait > 1000ms (MaxObservationDelayMs) for the quantum instability window
    await Task.Delay(1100);

    var textResult = await textPromise.ObserveAsync();
    Console.WriteLine($"  ✅ Quantum text revealed: \"{textResult}\"");
}
catch (Exception ex)
{
    Console.WriteLine($"  ❌ Text observation failed: {ex.Message}");
}

Console.WriteLine();

// -------------------------------------------
// Demo 3: Multiple promises (independent)
// -------------------------------------------
Console.WriteLine("📌 Demo 3: Multiple Independent Promises");
Console.WriteLine("  Creating multiple quantum promises and observing them...");
Console.WriteLine();

try
{
    var promiseA = new QuantumPromise<int>(
        () => Task.FromResult(100),
        TimeSpan.FromSeconds(5));

    var promiseB = new QuantumPromise<int>(
        () => Task.FromResult(200),
        TimeSpan.FromSeconds(5));

    var promiseC = new QuantumPromise<int>(
        () => Task.FromResult(300),
        TimeSpan.FromSeconds(5));

    // Must wait > 1000ms for the quantum instability window
    await Task.Delay(1100);

    Console.WriteLine("  👁️ Observing all promises concurrently...");

    var resultA = await promiseA.ObserveAsync();
    var resultB = await promiseB.ObserveAsync();
    var resultC = await promiseC.ObserveAsync();

    Console.WriteLine($"  ✅ Promise A: {resultA}");
    Console.WriteLine($"  ✅ Promise B: {resultB}");
    Console.WriteLine($"  ✅ Promise C: {resultC}");
    Console.WriteLine($"  📊 Sum: {resultA + resultB + resultC}");
}
catch (Exception ex)
{
    Console.WriteLine($"  ❌ Multiple promises failed: {ex.Message}");
}

Console.WriteLine();

// -------------------------------------------
// Demo 4: Timing edge cases
// -------------------------------------------
Console.WriteLine("📌 Demo 4: Quantum Timing Demonstrations");
Console.WriteLine("  Exploring the boundaries of the Schrödinger window...");
Console.WriteLine();

// Case 4a: Observe too early
Console.WriteLine("  🕐 Test: Observing too early (within quantum instability window)...");
try
{
    var earlyPromise = new QuantumPromise<int>(
        () => Task.FromResult(999),
        TimeSpan.FromSeconds(2));

    // Observe immediately without waiting
    var earlyResult = await earlyPromise.ObserveAsync();
    Console.WriteLine($"  ✅ Early observation succeeded: {earlyResult}");
}
catch (CollapseTooEarlyException)
{
    Console.WriteLine("  ⚠️ Expected: Collapse too early - quantum instability triggered!");
}
catch (Exception ex)
{
    Console.WriteLine($"  📌 Result: {ex.Message}");
}

// Case 4b: Observe within valid window
Console.WriteLine("  🕐 Test: Observing within valid window...");
try
{
    var validPromise = new QuantumPromise<int>(
        () => Task.FromResult(777),
        TimeSpan.FromSeconds(5));

    // Must wait > 1000ms for the quantum instability window
    await Task.Delay(1100);
    var validResult = await validPromise.ObserveAsync();
    Console.WriteLine($"  ✅ Valid window observation: {validResult}");
}
catch (Exception ex)
{
    Console.WriteLine($"  ❌ Valid window test failed: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("===========================================");
Console.WriteLine("🎉 QuantumPromise Demo Completed!");
Console.WriteLine("===========================================");

tracerProvider?.Dispose();
meterProvider?.Dispose();
loggerFactory.Dispose();
