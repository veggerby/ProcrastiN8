using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using ProcrastiN8;
using ProcrastiN8.LazyTasks;

var serviceName = "ProcrastiN8.Samples.LazyTasksDemo";
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
Console.WriteLine("🐢 ProcrastiN8 LazyTasks Demo Starting...");
Console.WriteLine("===========================================");
Console.WriteLine();

// Create a logger adapter for ProcrastiN8 logging
var logger = new DefaultLogger();

// -------------------------------------------
// Demo 1: Eventually
// -------------------------------------------
Console.WriteLine("📌 Demo 1: Eventually");
Console.WriteLine("  Executing a task 'eventually' with a random delay...");
Console.WriteLine();

try
{
    await Eventually.Do(
        action: async () =>
        {
            Console.WriteLine("  ✅ Eventually: Task executed after procrastination!");
            await Task.CompletedTask;
        },
        within: TimeSpan.FromSeconds(3),
        excuse: "Blocked by philosophical uncertainty",
        logger: logger);
}
catch (Exception ex)
{
    Console.WriteLine($"  ❌ Eventually failed: {ex.Message}");
}

Console.WriteLine();

// -------------------------------------------
// Demo 2: RetryUntilCancelled
// -------------------------------------------
Console.WriteLine("📌 Demo 2: RetryUntilCancelled");
Console.WriteLine("  Retrying a flaky operation until success or max retries...");
Console.WriteLine();

var retryAttempt = 0;
var maxSuccess = 3;

try
{
    await RetryUntilCancelled.RunForever(
        action: async () =>
        {
            retryAttempt++;
            Console.WriteLine($"  🔄 Attempt {retryAttempt}...");

            if (retryAttempt < maxSuccess)
            {
                throw new InvalidOperationException("Simulated failure for demo purposes.");
            }

            Console.WriteLine("  ✅ RetryUntilCancelled: Operation succeeded!");
            await Task.CompletedTask;
        },
        initialDelay: TimeSpan.FromMilliseconds(200),
        maxRetries: 5,
        logger: logger);
}
catch (RetryUntilCancelled.RetryExhaustedException ex)
{
    Console.WriteLine($"  ❌ Retry exhausted after {ex.Attempts} attempts: {ex.Message}");
}

Console.WriteLine();

// -------------------------------------------
// Demo 3: UncertaintyDelay
// -------------------------------------------
Console.WriteLine("📌 Demo 3: UncertaintyDelay");
Console.WriteLine("  Introducing unpredictable delays to simulate quantum uncertainty...");
Console.WriteLine();

try
{
    await UncertaintyDelay.WaitAsync(
        maxDelay: TimeSpan.FromMilliseconds(500),
        rounds: 3,
        logger: logger,
        enableCommentary: true);

    Console.WriteLine("  ✅ UncertaintyDelay: All uncertain delays completed!");
}
catch (OperationCanceledException)
{
    Console.WriteLine("  ⏹️ UncertaintyDelay: Operation was cancelled.");
}

Console.WriteLine();
Console.WriteLine("===========================================");
Console.WriteLine("🎉 LazyTasks Demo Completed!");
Console.WriteLine("===========================================");

tracerProvider?.Dispose();
meterProvider?.Dispose();
loggerFactory.Dispose();
