using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using ProcrastiN8;
using ProcrastiN8.Services;

var serviceName = "ProcrastiN8.Samples.ProcrastinationSchedulerDemo";
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
Console.WriteLine("🐢 ProcrastiN8 ProcrastinationScheduler Demo Starting...");
Console.WriteLine("===========================================");
Console.WriteLine();

// Create a logger adapter for ProcrastiN8 logging
var logger = new DefaultLogger();

// -------------------------------------------
// Demo 1: Basic Schedule (fire-and-forget)
// -------------------------------------------
Console.WriteLine("📌 Demo 1: Basic Schedule (fire-and-forget)");
Console.WriteLine("  Scheduling a task with MovingTarget mode...");
Console.WriteLine();

try
{
    await ProcrastinationScheduler.Schedule(
        task: async () =>
        {
            Console.WriteLine("  ✅ Task executed via basic Schedule!");
            await Task.CompletedTask;
        },
        initialDelay: TimeSpan.FromMilliseconds(200),
        mode: ProcrastinationMode.MovingTarget);
}
catch (Exception ex)
{
    Console.WriteLine($"  ❌ Basic Schedule failed: {ex.Message}");
}

Console.WriteLine();

// -------------------------------------------
// Demo 2: ScheduleWithResult (capture metrics)
// -------------------------------------------
Console.WriteLine("📌 Demo 2: ScheduleWithResult (capture metrics)");
Console.WriteLine("  Scheduling a task with WeekendFallback mode and capturing results...");
Console.WriteLine();

try
{
    var result = await ProcrastinationScheduler.ScheduleWithResult(
        task: async () =>
        {
            Console.WriteLine("  ✅ Task executed via ScheduleWithResult!");
            await Task.CompletedTask;
        },
        initialDelay: TimeSpan.FromMilliseconds(100),
        mode: ProcrastinationMode.WeekendFallback);

    Console.WriteLine($"  📊 Result Metrics:");
    Console.WriteLine($"     - Executed: {result.Executed}");
    Console.WriteLine($"     - Mode: {result.Mode}");
    Console.WriteLine($"     - Cycles: {result.Cycles}");
    Console.WriteLine($"     - ExcuseCount: {result.ExcuseCount}");
    Console.WriteLine($"     - TotalDeferral: {result.TotalDeferral.TotalMilliseconds:F2}ms");
    Console.WriteLine($"     - Triggered: {result.Triggered}");
    Console.WriteLine($"     - Abandoned: {result.Abandoned}");
    Console.WriteLine($"     - ProductivityIndex: {result.ProductivityIndex:F4}");
    Console.WriteLine($"     - CorrelationId: {result.CorrelationId}");
}
catch (Exception ex)
{
    Console.WriteLine($"  ❌ ScheduleWithResult failed: {ex.Message}");
}

Console.WriteLine();

// -------------------------------------------
// Demo 3: ScheduleWithHandle (interactive control)
// -------------------------------------------
Console.WriteLine("📌 Demo 3: ScheduleWithHandle (interactive control)");
Console.WriteLine("  Scheduling a task with InfiniteEstimation mode and triggering early...");
Console.WriteLine();

try
{
    var handle = ProcrastinationScheduler.ScheduleWithHandle(
        task: async () =>
        {
            Console.WriteLine("  ✅ Task executed via ScheduleWithHandle!");
            await Task.CompletedTask;
        },
        initialDelay: TimeSpan.FromSeconds(5),
        mode: ProcrastinationMode.InfiniteEstimation);

    // Wait a short moment, then trigger early
    await Task.Delay(500);
    Console.WriteLine("  🔔 Triggering early execution...");
    handle.TriggerNow();

    var handleResult = await handle.Completion;
    Console.WriteLine($"  📊 Handle Result Metrics:");
    Console.WriteLine($"     - Executed: {handleResult.Executed}");
    Console.WriteLine($"     - Triggered: {handleResult.Triggered}");
    Console.WriteLine($"     - Cycles: {handleResult.Cycles}");
    Console.WriteLine($"     - TotalDeferral: {handleResult.TotalDeferral.TotalMilliseconds:F2}ms");
}
catch (Exception ex)
{
    Console.WriteLine($"  ❌ ScheduleWithHandle failed: {ex.Message}");
}

Console.WriteLine();

// -------------------------------------------
// Demo 4: With Observer (logging lifecycle events)
// -------------------------------------------
Console.WriteLine("📌 Demo 4: With Observer (logging lifecycle events)");
Console.WriteLine("  Scheduling with a logging observer to track lifecycle events...");
Console.WriteLine();

try
{
    var observer = new LoggingProcrastinationObserver(logger);
    var observers = new[] { observer };

    var observerResult = await ProcrastinationScheduler.ScheduleWithResult(
        task: async () =>
        {
            Console.WriteLine("  ✅ Task executed with observer tracking!");
            await Task.CompletedTask;
        },
        initialDelay: TimeSpan.FromMilliseconds(150),
        mode: ProcrastinationMode.MovingTarget,
        observers: observers);

    Console.WriteLine($"  📊 Observer-tracked Result:");
    Console.WriteLine($"     - Executed: {observerResult.Executed}");
    Console.WriteLine($"     - ProductivityIndex: {observerResult.ProductivityIndex:F4}");
}
catch (Exception ex)
{
    Console.WriteLine($"  ❌ Observer demo failed: {ex.Message}");
}

Console.WriteLine();

// -------------------------------------------
// Demo 5: With Custom Middleware
// -------------------------------------------
Console.WriteLine("📌 Demo 5: With Custom Middleware");
Console.WriteLine("  Scheduling with a custom middleware to wrap execution...");
Console.WriteLine();

try
{
    var middleware = new AnnotationMiddleware();
    var middlewares = new[] { middleware };

    var middlewareResult = await ProcrastinationScheduler.ScheduleWithResult(
        task: async () =>
        {
            Console.WriteLine("  ✅ Task executed with middleware wrapping!");
            await Task.CompletedTask;
        },
        initialDelay: TimeSpan.FromMilliseconds(100),
        mode: ProcrastinationMode.MovingTarget,
        middlewares: middlewares);

    Console.WriteLine($"  📊 Middleware-wrapped Result:");
    Console.WriteLine($"     - Executed: {middlewareResult.Executed}");
    Console.WriteLine($"     - CorrelationId: {middlewareResult.CorrelationId}");
}
catch (Exception ex)
{
    Console.WriteLine($"  ❌ Middleware demo failed: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("===========================================");
Console.WriteLine("🎉 ProcrastinationScheduler Demo Completed!");
Console.WriteLine("===========================================");

tracerProvider?.Dispose();
meterProvider?.Dispose();
loggerFactory.Dispose();

/// <summary>
/// Custom middleware that annotates before/after execution.
/// </summary>
sealed class AnnotationMiddleware : IProcrastinationMiddleware
{
    public async Task InvokeAsync(ProcrastinationExecutionContext ctx, Func<Task> next, CancellationToken ct)
    {
        Console.WriteLine($"  [MW] Entering {ctx.Mode} (CorrelationId: {ctx.CorrelationId})");
        await next();
        Console.WriteLine($"  [MW] Leaving; Executed={ctx.Result?.Executed}");
    }
}
