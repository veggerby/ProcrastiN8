# 📋 ProcrastiN8 Prioritized Backlog

*Generated: 2025-07-30*
Legend:
🟢 = High Priority (Core, funny, easy to use)
🟡 = Medium Priority (Funny, moderately complex or niche)
🔴 = Low Priority (Very absurd, edge-case or high effort)

---

## 🟢 High Priority – Core Expansion

### 🫣 `UncertaintyDelay`

> A delay that changes unpredictably each time you check it.

```csharp
public static Task WaitAsync(TimeSpan maxDelay, CancellationToken cancellationToken = default)
```

* Uses `IRandomProvider` for testable delay logic.
* Logs whimsical reasons for each delay variation.

---

### 🎚️ `ProcrastinationScheduler` – Detailed Specification

**Goal:** Provide an overengineered way to defer tasks based on ridiculous but technically sound delay strategies.

---

#### 📦 Class + API (Implemented)

```csharp
// Static facade
Task ProcrastinationScheduler.Schedule(...);
Task<ProcrastinationResult> ProcrastinationScheduler.ScheduleWithResult(...);
ProcrastinationHandle ProcrastinationScheduler.ScheduleWithHandle(...);

// Instance / DI usage
var scheduler = ProcrastinationSchedulerBuilder
    .Create()
    .WithRandomProvider(RandomProvider.Default)
    .AddObserver(new LoggingProcrastinationObserver(new DefaultLogger()))
    .Build();
```

Result flags now include `Triggered` and `Abandoned` for forensic clarity post-execution (or non-execution). External intervention provided via handle methods `TriggerNow()` and `Abandon()`.

---

#### 🧠 `ProcrastinationMode` Enum

```csharp
public enum ProcrastinationMode
{
    MovingTarget,
    InfiniteEstimation,
    WeekendFallback
}
```

---

##### 🏃 `MovingTarget`

> “Every time you check when it will run, it moves further away.”

##### Behavior (MovingTarget)

* Delay increases by **10–25%** (randomized) after each postponement check.
* The task is only executed after `N` stable observations or if max delay cap (e.g. 1 hour) is hit.
* Every time you call `Schedule(...)`, log output like:

```txt
[Scheduler] New delay: 17.3s. Reason: 'Still waiting for product alignment.'
```

##### Implementation Notes (MovingTarget)

* Use `IRandomProvider` to pick percentage increase.
* Optionally allow `maxTotalDelay` and `maxAttempts` parameters.
* Delay can be multiplied or added based on configuration.

---

#### ♾️ `InfiniteEstimation`

> “Always just 5 minutes away.”

##### Behavior (InfiniteEstimation)

* Keeps logging: “Estimated time to start: 5 minutes.”
* Actually never starts **unless** a `TriggerNow()` method is called, or the user gives up and cancels.
* Can optionally set a flag `AllowSurpriseStart = true` to allow a random chance to start anyway.

##### Implementation Notes (InfiniteEstimation)

* Loop using `Task.Delay(...)` with a 5-minute step or shorter for debugging.
* Optionally provide a `ProcrastinationSchedulerHandle` object with `.TriggerNow()` or `.Abandon()` APIs.
* Great for test scenarios or a long-running placeholder.

---

#### 📆 `WeekendFallback`

> “Only runs if it’s been >72 hours since it was scheduled — or if today is Saturday after 3pm.”

##### Behavior

* If `DateTime.UtcNow - scheduledAt >= 72 hours`, run it.
* OR if `now.DayOfWeek == DayOfWeek.Saturday && now.Hour >= 15`, allow early execution.
* Otherwise, log excuses and wait in hourly polling loop.

##### Implementation Notes

* Use a `Timer` or loop with 60-minute sleeps.
* Integrate `IExcuseProvider` to give contextual reasons like:

  * “Still waiting for everyone to be OOO.”
  * “Avoiding midweek misalignment.”
* Optional: support a `WeekendCutoffHour` (default = 15).

---

#### Example Logs per Mode

**MovingTarget**:

```txt
[Scheduler] Rescheduled: New delay = 22.1s (increased by 15%).
[Excuse] “Still waiting for sprint planning to end.”
```

**InfiniteEstimation**:

```txt
[Scheduler] Estimated time to start: 5 minutes.
[Scheduler] Still 5 minutes to go. Please wait...
```

**WeekendFallback**:

```txt
[Scheduler] Today is Thursday. Not running yet.
[Excuse] “Waiting for everyone to mentally check out.”
```

---

### 🧯 `QuantumAbortToken`

> Cancels your task the moment it becomes important.

```csharp
public sealed class QuantumAbortToken
{
    public CancellationToken Token { get; }
    public void Observe();
}
```

* Cancellation is probabilistic, context-sensitive, and rude.

---

### 📉 `ProbabilityOfSuccess<T>`

> Returns a result... or nothing. Depends on fate.

```csharp
public static Task<T> ExecuteAsync<T>(Func<Task<T>> operation, double successProbability, IRandomProvider random = null)
```

* May return `default(T)` or throw `QuantumUncertaintyException`.

---

### 🗂️ `ExcuseCache`

> Remembers your best excuses — until you get caught.

```csharp
public Task<string> GetExcuseAsync(IExcuseProvider provider, string context);
public void Invalidate(string context);
```

* Integrates with `OpenAIExcuseProvider`.

---

## 🟡 Medium Priority – Playful Core Utilities

### 🧪 `CollapseOnReview<T>`

> Works only in code review context.

```csharp
CollapseOnReview<T> : QuantumPromise<T>
```

* Only collapses if `CodeReviewContext.IsActive == true`.

---

### 🌀 `QuantumMutex`

> Every thread gets a different lock. All are valid. None are safe.

```csharp
public Task<IDisposable> AcquireAsync();
```

* Simulates parallel universes and shared delusion.

---

### 🔁 `RetryInSuperposition<T>`

> Executes all retries *at once*, in superposition.

```csharp
public static Task<T> RetryInSuperposition<T>(Func<Task<T>> operation, int maxAttempts)
```

* First success collapses reality. The rest cancel themselves… maybe.

---

### 🗃️ `Procrastinable<T>`

> Deferred execution that pretends it's intentional.

```csharp
public class Procrastinable<T>
{
    public Procrastinable(Func<Task<T>> factory);
    public Task<T> EvaluateAsync();
}
```

* Think `Lazy<T>` meets `I’ll do it tomorrow`.

---

## 🔴 Low Priority – Meta/Absurdist Edge Features

### 🪞 `ObserverDependentValue<T>`

> Returns different values depending on *who* is calling.

```csharp
public T Resolve();
```

* Uses `StackTrace` or `ExecutionContext` to personalize response.
* Optional audit log to track contradictory truths.

---

### 🔮 `[QuantumDependent]` (Attribute)

> Changes behavior depending on system state, phase of moon, etc.

```csharp
[QuantumDependent(EntropySource = EntropySource.MoonPhase | EntropySource.CpuLoad)]
```

* Requires AOP or source generation.
* Great for haunting DevOps engineers.

---

## ✅ Suggested Order of Implementation

| Priority | Feature                     | Rationale                                  |
| -------- | --------------------------- | ------------------------------------------ |
| 🟢       | `UncertaintyDelay`          | Easy, funny, and universally applicable    |
| 🟢       | `ProcrastinationScheduler`  | Central scheduling logic for the package   |
| 🟢       | `QuantumAbortToken`         | Great for adding chaos to everyday code    |
| 🟢       | `ExcuseCache`               | Supports excuse generators and memoization |
| 🟢       | `ProbabilityOfSuccess<T>`   | Simple, testable, and hilarious            |
| 🟡       | `CollapseOnReview<T>`       | Plays well with existing `QuantumPromise`  |
| 🟡       | `QuantumMutex`              | Technically absurd, thematically perfect   |
| 🟡       | `RetryInSuperposition<T>`   | Wild, but manageable                       |
| 🟡       | `Procrastinable<T>`         | Logical follow-up to `Eventually.Do(...)`  |
| 🔴       | `ObserverDependentValue<T>` | Hard to test, very quantum                 |
| 🔴       | `[QuantumDependent]`        | Source-gen or runtime injection needed     |
