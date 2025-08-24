# ProcrastinationScheduler

The `ProcrastinationScheduler` defers tasks using deliberately over-engineered strategies while exposing formal instrumentation. The ceremony is real; the productivity is optional.

## Core Static API

```csharp
Task Schedule(...)
Task<ProcrastinationResult> ScheduleWithResult(...)
ProcrastinationHandle ScheduleWithHandle(...)
```

All overloads accept optional `IExcuseProvider`, `IDelayStrategy`, `IRandomProvider`, `ITimeProvider`, `IProcrastinationStrategyFactory`, `IEnumerable<IProcrastinationObserver>`, and (via builder) `IEnumerable<IProcrastinationMiddleware>`.

### Key Parameters

- `task` – The asynchronous work eventually (maybe) executed.
- `initialDelay` – Baseline deferment window supplied to strategies.
- `mode` – Built-in procrastination mode (enum). Custom composition uses factories.
- Provider overrides – Inject test doubles or bespoke behaviors (deterministic randomness, simulated time, custom excuses, etc.).
- `observers` – Receive lifecycle callbacks: cycle, excuse, triggered, abandoned, executed.

### Result Object (`ProcrastinationResult`)

| Property | Meaning |
|----------|---------|
| `Mode` | Strategy requested. |
| `Executed` | Underlying task ran. |
| `Triggered` | Execution was forced early via `TriggerNow()`. |
| `Abandoned` | Execution skipped due to `Abandon()`. |
| `StartedUtc` | UTC timestamp when procrastination ritual began. |
| `CompletedUtc` | UTC timestamp when ritual concluded. |
| `TotalDeferral` | Wall-clock procrastination span. |
| `Cycles` | Count of deferment iterations. |
| `ExcuseCount` | Number of excuses fetched. |
| `CorrelationId` | Unique GUID for tracing across observers/middleware. |
| `CyclesPerSecond` | Derived throughput of deferment cycles (if any wall-clock time elapsed). |
| `ProductivityIndex` | Satirical metric: `Executed ? 1 / (1 + ExcuseCount + Cycles) : 0`. Lower values indicate more ceremonious deferral. |

`Triggered` and `Abandoned` are mutually exclusive in well-behaved workflows; both can be false if a strategy finished organically.

### Interactive Handle

`ScheduleWithHandle` returns a `ProcrastinationHandle` providing:

```csharp
handle.TriggerNow();  // Force early execution (sets Triggered flag)
handle.Abandon();     // Skip execution entirely (sets Abandoned flag)
await handle.Completion; // Yields populated ProcrastinationResult
```

Status transitions: `Pending -> Deferring -> (Triggered|Abandoned|Executed)`.

### Observers

Implement `IProcrastinationObserver` to hook lifecycle notifications. A built-in `LoggingProcrastinationObserver` emits solemn log entries via `IProcrastiLogger`.

Example:

```csharp
var observers = new [] { new LoggingProcrastinationObserver(new DefaultLogger()) };
var result = await ProcrastinationScheduler.ScheduleWithResult(
    () => Task.CompletedTask,
    TimeSpan.FromMilliseconds(100),
    ProcrastinationMode.MovingTarget,
    observers: observers);
```

### Builder / DI Helper

To avoid static usage in DI-friendly contexts:

```csharp
var scheduler = ProcrastinationSchedulerBuilder
    .Create()
    .WithRandomProvider(RandomProvider.Default)
    .WithSafety(new CustomSafety(maxCycles: 250)) // ambient safety override
    .WithMetrics() // optional, auto-metrics already emit counters; this adds observer-based transformation
    .AddObserver(new LoggingProcrastinationObserver(new DefaultLogger()))
    .Build();

var r = await scheduler.ScheduleWithResult(
    () => Task.CompletedTask,
    TimeSpan.FromMilliseconds(50),
    ProcrastinationMode.MovingTarget);
```


### Middleware

`IProcrastinationMiddleware` components wrap execution, enabling cross-cutting instrumentation, logging, chaos engineering, or synthetic latency injection.

Execution ordering: Added first = outermost wrapper (`before` runs first, `after` runs last). Middlewares receive a `ProcrastinationExecutionContext` containing the `Mode`, `CorrelationId`, and (post-core) the `Result`.

Result availability: `context.Result` is assigned immediately after the core strategy completes and BEFORE middleware "after" phases run.
For strategies that end without executing the underlying task (e.g., InfiniteEstimation cancelled, ambient safety cap, early abandon) the
`Result` is still non-null with `Executed=false` (and flags like `Abandoned` / `Triggered` as appropriate).

Example timing decorator:

```csharp
sealed class TimingMiddleware : IProcrastinationMiddleware
{
    public async Task InvokeAsync(ProcrastinationExecutionContext ctx, Func<Task> next, CancellationToken ct)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await next();
        sw.Stop();
        Console.WriteLine($"[Timing] {ctx.Mode} {ctx.CorrelationId} took {sw.ElapsedMilliseconds}ms deferral span (exec={ctx.Result?.Executed})");
    }
}
```

Register via builder:

```csharp
var scheduler = ProcrastinationSchedulerBuilder.Create()
    .AddMiddleware(new TimingMiddleware())
    .AddObserver(new MetricsObserver())
    .Build();
```

### Metrics & Auto‑Instrumentation

Automatic metrics emission occurs directly in `ProcrastinationStrategyBase` (events forwarded into `ProcrastinationDiagnostics`). Attaching a
`MetricsObserver` is now optional and primarily useful for custom aggregation or side-channel logging. The builder convenience `.WithMetrics()`
adds it explicitly.

Ambient safety: Calling `.WithSafety(IExecutionSafetyOptions)` on the builder establishes a process-wide ambient safety configuration adopted by
strategies that have not explicitly customized their safety options (e.g., global max cycle cap). Individual strategies constructed manually can
still call `ConfigureSafety(...)` to override.

---

## Built-in Modes

## Diagnostics & Metrics

ActivitySource: `ProcrastiN8.Procrastination`

Meter (counters):

- `procrastination.cycles`
- `procrastination.excuses`
- `procrastination.executions`
- `procrastination.triggered`
- `procrastination.abandoned`

Observer structured events (`ProcrastinationObserverEvent`) deliver immutable payloads for each lifecycle occurrence.
`MetricsObserver` may be attached to transform these events into counter increments automatically.

## Strategy Comparison Matrix

| Strategy | Core Delay Pattern | Executes Automatically | External Trigger Effect | Safety Caps Applied | Notes |
|----------|--------------------|------------------------|-------------------------|---------------------|-------|
| MovingTarget | Jitter growth (~0.5–15% per cycle) with upper delay & cycle ceilings | Yes (after ceilings or loop exit) | Forces immediate execution, sets `Triggered` | Max cycles (`IExecutionSafetyOptions`), absolute deadline in tests | Uses growth & ceiling policies; excuses each cycle. |
| InfiniteEstimation | Conceptual 5m constant; implemented as ~10ms micro-delay loops | No (never by itself) | Forces one-off task execution then marks `Triggered` | Max cycles + absolute 2s deadline (tests) | Preserves ideal of perpetual estimation while remaining test-friendly. |
| WeekendFallback | Hourly polling until weekend window or 72h elapsed | Yes (when weekend window or 72h cap) | Forces immediate execution ahead of window | Max cycles | Simulates culturally convenient deferral rituals. |

All strategies report structured lifecycle events and update `ProcrastinationResult` with correlation metadata.


### MovingTarget

- Delay increases by approximately 0.5–15% (bounded jitter) each postponement.
- Task executes after a maximum delay cap or safety ceiling.

### InfiniteEstimation

- Conceptually keeps reporting “Estimated time to start: 5 minutes.” forever.
- Implementation uses tiny (≈10ms) synthetic delays plus an absolute safety deadline in tests to avoid multi-minute hangs.
- Task only runs if externally triggered (otherwise it purposefully never executes).

### WeekendFallback

- Task runs if 72 hours have passed or it’s Saturday after 3 PM.

## Simple Example

```csharp
await ProcrastinationScheduler.Schedule(
    async () => Console.WriteLine("Task executed!"),
    TimeSpan.FromSeconds(10),
    ProcrastinationMode.MovingTarget);
```

## Advanced Strategies & Composition

You can assemble higher-order procrastination behaviors without modifying the enum:

```csharp
var composite = new CompositeProcrastinationStrategy(
    new MovingTargetStrategy(),
    new WeekendFallbackStrategy());

await composite.ExecuteAsync(task, TimeSpan.FromSeconds(1), excuseProvider, delayStrategy, randomProvider, SystemTimeProvider.Default, CancellationToken.None);

var conditional = new ConditionalProcrastinationStrategy(
    new MovingTargetStrategy(),
    new InfiniteEstimationStrategy(),
    timeProvider => timeProvider.GetUtcNow().DayOfWeek == DayOfWeek.Friday);

await conditional.ExecuteAsync(task, TimeSpan.FromSeconds(1), excuseProvider, delayStrategy, randomProvider, SystemTimeProvider.Default, CancellationToken.None);
```

Provide a custom `IProcrastinationStrategyFactory` to surface composites or conditionals under your own external configuration scheme.

## Extensibility Summary

| Extension Point | Interface | Purpose |
|-----------------|-----------|---------|
| Strategy Factory | `IProcrastinationStrategyFactory` | Swap / inject custom strategies. |
| Excuses | `IExcuseProvider` | Supply thematic rationalizations. |
| Time | `ITimeProvider` | Deterministic temporal control. |
| Randomness | `IRandomProvider` | Deterministic stochastic rituals. |
| Delay | `IDelayStrategy` | Custom pacing algorithms. |
| Observers | `IProcrastinationObserver` | Telemetry & logging hooks. |
| Scheduler Builder | `IProcrastinationSchedulerBuilder` | Instance-based wiring for DI. |

## Design Rationale

The system intentionally over-abstracts trivial delay loops to maximize testability, extensibility, and the appearance of architectural gravitas. Result flags (`Triggered`, `Abandoned`) provide post-hoc narrative clarity for auditors investigating why nothing happened sooner.

---
Return to [README](../README.md)
