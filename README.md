# 🐢 ProcrastiN8

ProcrastiN8 is a C#/.NET utility library for simulating productivity, stalling, and quantum-level procrastination. All features are absurd in concept, but real in implementation.

## ✨ Project Status

![SHIT Compliant](https://img.shields.io/badge/style-SHIT-green?style=flat-square)
![Build](https://img.shields.io/github/actions/workflow/status/veggerby/ProcrastiN8/ci-release.yml?label=build&style=flat-square)
![Coverage](https://img.shields.io/codecov/c/github/veggerby/ProcrastiN8?style=flat-square)
![NuGet](https://img.shields.io/nuget/vpre/ProcrastiN8?label=nuget&style=flat-square)

---

## 🚀 Getting Started

Install via NuGet:

```sh
# .NET CLI
dotnet add package ProcrastiN8 --prerelease
# Because release-readiness is a social construct.
```

or

```powershell
# NuGet Package Manager
Install-Package ProcrastiN8 -Prerelease
# Because final releases are for the overly decisive.
```

Minimal usage:

```csharp
using ProcrastiN8;

var registry = new QuantumEntanglementRegistry<int>();
var promise = new QuantumPromise<int>(() => Task.FromResult(42), TimeSpan.FromSeconds(2));
registry.Entangle(promise);
var result = await promise.ObserveAsync();
Console.WriteLine($"Collapse result: {result}");
```

### ProcrastinationScheduler

Defer execution ceremonially using configurable procrastination strategies while emitting structured diagnostics worthy of an internal audit.

Diagnostics & Metrics:

* ActivitySource: `ProcrastiN8.Procrastination` (spans per scheduling session if you start one)
* Meter counters (unconditional emission): `procrastination.cycles`, `procrastination.excuses`, `procrastination.executions`, `procrastination.triggered`, `procrastination.abandoned`
* Structured events: `ProcrastinationObserverEvent` (types: cycle, excuse, triggered, abandoned, executed) automatically recorded by the strategy base
* Correlation: every run has a `CorrelationId` (GUID) for trace stitching

Result enrichment fields:

* `StartedUtc`, `CompletedUtc`, `TotalDeferral`, `CyclesPerSecond`
* Flags: `Executed`, `Triggered`, `Abandoned`
* Counters: `Cycles`, `ExcuseCount`
* `ProductivityIndex` (satirical: `Executed ? 1 / (1 + ExcuseCount + Cycles) : 0`)

Basic fire-and-forget:

```csharp
await ProcrastiN8.Services.ProcrastinationScheduler.Schedule(
    () => DoImportantWorkAsync(),
    initialDelay: TimeSpan.FromSeconds(5),
    mode: ProcrastinationMode.MovingTarget);
```

Capture metrics:

```csharp
var result = await ProcrastinationScheduler.ScheduleWithResult(
    () => DoImportantWorkAsync(),
    TimeSpan.FromMilliseconds(250),
    ProcrastinationMode.WeekendFallback);

Console.WriteLine($"Executed={result.Executed} Cycles={result.Cycles} Excuses={result.ExcuseCount} Triggered={result.Triggered} Abandoned={result.Abandoned}");
```

Interactive control (handle):

```csharp
var handle = ProcrastinationScheduler.ScheduleWithHandle(
    () => DoImportantWorkAsync(),
    TimeSpan.FromSeconds(30),
    ProcrastinationMode.InfiniteEstimation);

// later externally force progress
handle.TriggerNow();
var summary = await handle.Completion; // includes Triggered=true
```

Fluent builder (for DI friendliness) with observers:

```csharp
var scheduler = ProcrastinationSchedulerBuilder
    .Create()
    .WithSafety(new CustomSafety(maxCycles: 200)) // ambient safety override (optional)
    .WithMetrics() // optional: auto-metrics already emit counters; observer adds extensibility
    .AddObserver(new LoggingProcrastinationObserver(new DefaultLogger()))
    .Build();

var r = await scheduler.ScheduleWithResult(
    () => DoImportantWorkAsync(),
    TimeSpan.FromMilliseconds(100),
    ProcrastinationMode.MovingTarget);
```

#### Middleware Pipeline

Wrap strategy execution with reusable cross‑cutting procrastination enhancers:

```csharp
// Custom middleware that annotates before/after execution
sealed class AnnotationMiddleware : IProcrastinationMiddleware
{
    public async Task InvokeAsync(ProcrastinationExecutionContext ctx, Func<Task> next, CancellationToken ct)
    {
        Console.WriteLine($"[MW] entering {ctx.Mode} {ctx.CorrelationId}");
        await next();
        // ctx.Result may be populated after the core strategy completes
        Console.WriteLine($"[MW] leaving; executed={ctx.Result?.Executed}");
    }
}

var scheduler = ProcrastinationSchedulerBuilder.Create()
    .AddMiddleware(new AnnotationMiddleware())
    .AddObserver(new MetricsObserver()) // forwards structured events to counters
    .Build();

var outcome = await scheduler.ScheduleWithResult(
    () => DoImportantWorkAsync(),
    TimeSpan.FromMilliseconds(50),
    ProcrastinationMode.MovingTarget);
```

Middleware order is preserved: added first = runs outermost (its `before` executes first, its `after` executes last). Middleware should be side‑effect minimal and respect the supplied `CancellationToken`.
`context.Result` is assigned immediately after core strategy completion and is never null post‑execution (it may indicate `Executed=false` for strategies that exit without running the task such as ambient safety cap or untriggered InfiniteEstimation).

#### Metrics Observer

`MetricsObserver` is optional; counters already increment automatically. Attach it if you need to observe events for custom sinks.

#### Extension Points (Excerpt)

| Surface | Purpose |
|---------|---------|
| `IExcuseProvider` | Generate ceremonial rationalizations. |
| `IDelayStrategy` | Control temporal pacing (inject fakes in tests). |
| `IProcrastinationStrategyFactory` | Provide custom / composite / conditional strategies. |
| `IProcrastinationObserver` | Observe lifecycle transitions and structured events. |
| `IProcrastinationMiddleware` | Wrap execution with cross‑cutting logic (metrics, logging, chaos). |
| `IProcrastinationSchedulerBuilder` | Fluent, DI‑friendly construction with observers & middleware. |

See full details (sequence diagram, FAQ) in [docs/procrastination-scheduler.md](docs/procrastination-scheduler.md).

##### Safety & Correlation

Ambient safety (`WithSafety`) sets a process-wide `MaxCycles` applied to strategies lacking an explicit override. Correlation IDs are assigned before strategy execution so middleware, observers, and results share the same GUID even if execution is abandoned, capped, or triggered early.

##### Extended Factory Example

```csharp
sealed class CustomFactory : IProcrastinationStrategyFactory
{
    public IProcrastinationStrategy Create(ProcrastinationMode mode) => mode switch
    {
        ProcrastinationMode.MovingTarget => new CompositeProcrastinationStrategy(
            new MovingTargetStrategy(),
            new WeekendFallbackStrategy()),
        ProcrastinationMode.InfiniteEstimation => new ConditionalProcrastinationStrategy(
            new InfiniteEstimationStrategy(),
            new MovingTargetStrategy(),
            tp => tp.GetUtcNow().DayOfWeek == DayOfWeek.Friday),
        _ => new MovingTargetStrategy()
    };
}

var scheduler = ProcrastinationSchedulerBuilder.Create()
    .WithFactory(new CustomFactory())
    .WithSafety(new CustomSafety(300))
    .Build();
```

---

## 🧑‍💻 Usage & API

### QuantumEntanglementRegistry

Manages a set of entangled `QuantumPromise<T>` instances. Only the registry coordinates the collapse of entangled promises according to the configured quantum behavior. Thread-safe, for those who pretend it matters.

```csharp
var registry = new QuantumEntanglementRegistry<int>();
var promise1 = new QuantumPromise<int>(() => Task.FromResult(42), TimeSpan.FromSeconds(2));
var promise2 = new QuantumPromise<int>(() => Task.FromResult(99), TimeSpan.FromSeconds(2));
registry.Entangle(promise1);
registry.Entangle(promise2);
var result = await promise1.ObserveAsync(); // Collapses all entangled promises if entangled
```

Expected output:

```txt
[QuantumEntanglement] Entangled set: 2 promises
[QuantumEntanglement] Collapsing one promise...
[QuantumEntanglement] Ripple collapse triggered for remaining entangled promises
Collapse result: 42
```

### QuantumPromise

Represents a value in quantum superposition until observed. If entangled in a registry, calling `ObserveAsync()` will trigger the registry's collapse behavior for all entangled promises. Forced collapse to a specific value is only possible via the registry and the appropriate collapse behavior (e.g., Copenhagen). Observing a promise outside a registry will not trigger entanglement effects.

### Eventually

```csharp
await Eventually.Do(async () =>
{
    Console.WriteLine("Actually doing something now...");
});
```

Expected output:

```txt
[Eventually] Thinking about it... Might do it in 17.3s.
[Eventually] Reason for delay: Blocked by a third-party library.
[Eventually] Just five more minutes...
...
[Eventually] Finally did the thing. You're welcome.
```

### RetryUntilCancelled

```csharp
await RetryUntilCancelled.Do(
    async () => { Console.WriteLine("Trying again..."); },
    logger: null,
    excuseProvider: null,
    retryStrategy: null,
    cancellationToken: CancellationToken.None);
```

Expected output:

```txt
[Retry] Attempt 1 failed. Retrying in 2.1s...
[Retry] Excuse: Still waiting for the backlog.
[Retry] Attempt 2 failed. Retrying in 4.2s...
... (until cancelled)
```

### FakeProgress, InfiniteSpinner, BusyWaitSimulator

```csharp
await FakeProgress.RunAsync("Deploying to production", steps: 5, logger: null, cancellationToken: CancellationToken.None);
```

Expected output:

```txt
[FakeProgress] Aligning expectations...
[FakeProgress] Calibrating metrics...
[FakeProgress] Pretending to load data...
[FakeProgress] Synchronizing with imaginary server...
[FakeProgress] Consulting committee of doubts...
[FakeProgress] Progress: 100% (allegedly)
```

---

## 🤖 OpenAI Excuse Provider

ProcrastiN8 includes an `OpenAIExcuseProvider` that fetches creative excuses from OpenAI's ChatGPT API. This provider is ideal for generating topical and humorous excuses for procrastination.

### Usage

```csharp
using ProcrastiN8.Common;

var excuseProvider = new OpenAIExcuseProvider("your-api-key");
var excuse = await excuseProvider.GetExcuseAsync();
Console.WriteLine(excuse);
```

> **Note:** You need an OpenAI API key to use this feature.

---

## 📦 NuGet

ProcrastiN8 is available on [NuGet](https://www.nuget.org/packages/ProcrastiN8/#readme-body-tab)

---

## 📝 License

MIT License. See [LICENSE](LICENSE).
