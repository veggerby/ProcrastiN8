# ProcrastinationScheduler

The `ProcrastinationScheduler` defers tasks using deliberately over-engineered strategies while exposing formal instrumentation. The ceremony is real; the productivity is optional.

## Core Static API

```csharp
Task Schedule(...)
Task<ProcrastinationResult> ScheduleWithResult(...)
ProcrastinationHandle ScheduleWithHandle(...)
```

All overloads accept optional `IExcuseProvider`, `IDelayStrategy`, `IRandomProvider`, `ITimeProvider`, `IProcrastinationStrategyFactory`, and `IEnumerable<IProcrastinationObserver>` (for the result / handle variants).

### Key Parameters

- `task` – The asynchronous work eventually (maybe) executed.
- `initialDelay` – Baseline deferment window supplied to strategies.
- `mode` – Built-in procrastination mode (enum). Custom composition uses factories.
- Provider overrides – Inject test doubles or bespoke behaviors (deterministic randomness, simulated time, custom excuses, etc.).
- `observers` – Receive lifecycle callbacks: cycle, excuse, triggered, abandoned, executed.

### Result Object (`ProcrastinationResult`)

| Property      | Meaning |
|---------------|---------|
| `Mode`        | Strategy requested. |
| `Executed`    | Underlying task ran. |
| `Triggered`   | Execution was forced early via `TriggerNow()`. |
| `Abandoned`   | Execution skipped due to `Abandon()`. |
| `TotalDeferral` | Wall-clock procrastination span. |
| `Cycles`      | Count of deferment iterations. |
| `ExcuseCount` | Number of excuses fetched. |

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
    .AddObserver(new LoggingProcrastinationObserver(new DefaultLogger()))
    .Build();

var r = await scheduler.ScheduleWithResult(
    () => Task.CompletedTask,
    TimeSpan.FromMilliseconds(50),
    ProcrastinationMode.MovingTarget);
```


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


### MovingTarget

- Delay increases by 10–25% after each postponement.
- Task executes after a maximum delay cap or stable observations.

### InfiniteEstimation

- Logs “Estimated time to start: 5 minutes.” repeatedly.
- Task starts only if `TriggerNow()` is called or canceled.

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
