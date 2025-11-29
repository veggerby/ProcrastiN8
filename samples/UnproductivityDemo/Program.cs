using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using ProcrastiN8;
using ProcrastiN8.Unproductivity;

var serviceName = "ProcrastiN8.Samples.UnproductivityDemo";
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
Console.WriteLine("🐢 ProcrastiN8 Unproductivity Demo Starting...");
Console.WriteLine("===========================================");
Console.WriteLine();

// Create a logger adapter for ProcrastiN8 logging
var logger = new DefaultLogger();

// -------------------------------------------
// Demo 1: FakeProgress
// -------------------------------------------
Console.WriteLine("📌 Demo 1: FakeProgress");
Console.WriteLine("  Simulating progress with fake updates...");
Console.WriteLine();

try
{
    await FakeProgress.ShowFakeProgressAsync(
        stepDuration: TimeSpan.FromMilliseconds(500),
        steps: 5,
        logger: logger);

    Console.WriteLine("  ✅ FakeProgress: Simulation complete (results inconclusive).");
}
catch (Exception ex)
{
    Console.WriteLine($"  ❌ FakeProgress failed: {ex.Message}");
}

Console.WriteLine();

// -------------------------------------------
// Demo 2: InfiniteSpinner
// -------------------------------------------
Console.WriteLine("📌 Demo 2: InfiniteSpinner");
Console.WriteLine("  Starting infinite spinner (will cancel after 3 seconds)...");
Console.WriteLine();

using var spinnerCts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

try
{
    await InfiniteSpinner.SpinForeverAsync(
        logger: logger,
        tickRate: TimeSpan.FromMilliseconds(500),
        cancellationToken: spinnerCts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("  ⏹️ InfiniteSpinner: Cancelled (productivity threat detected).");
}

Console.WriteLine();

// -------------------------------------------
// Demo 3: BusyWaitSimulator
// -------------------------------------------
Console.WriteLine("📌 Demo 3: BusyWaitSimulator");
Console.WriteLine("  Burning CPU cycles for the illusion of hard work...");
Console.WriteLine();

try
{
    BusyWaitSimulator.BurnCpuCycles(
        duration: TimeSpan.FromSeconds(2),
        logger: logger);

    Console.WriteLine("  ✅ BusyWaitSimulator: CPU burn complete. Heat generated: emotional.");
}
catch (Exception ex)
{
    Console.WriteLine($"  ❌ BusyWaitSimulator failed: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("===========================================");
Console.WriteLine("🎉 Unproductivity Demo Completed!");
Console.WriteLine("===========================================");

tracerProvider?.Dispose();
meterProvider?.Dispose();
loggerFactory.Dispose();
