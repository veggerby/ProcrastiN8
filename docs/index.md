# 🧠 Concept: ProcrastiN8 – Lazy Utilities for the Productively Avoidant

**ProcrastiN8** is a .NET utility library that celebrates and enables *strategic procrastination* — by offering clever, sarcastic, overengineered, or intentionally roundabout tools to "get things done (eventually)." It walks the fine line between genuinely useful utilities and parody, making it a fun yet real package for developers who appreciate humor, absurdity, and the art of procrastination.

---

## 🧱 Package Structure

```txt
ProcrastiN8
├── LazyTasks/
│   ├── DelayedExecution.cs
│   ├── Eventually.cs
│   └── RetryUntilCancelled.cs
├── Unproductivity/
│   ├── InfiniteSpinner.cs
│   ├── FakeProgress.cs
│   └── BusyWaitSimulator.cs
├── TODOFramework/
│   ├── TodoAttribute.cs
│   ├── TodoScheduler.cs
│   └── NeverDoExecutor.cs
├── JustBecause/
│   ├── RandomExceptionGenerator.cs
│   ├── PointlessChain.cs
│   └── YAGNIValidator.cs
├── Extensions/
│   ├── EnumerableExtensions.cs
│   ├── StringExcuses.cs
│   └── DateTimePostponer.cs
└── README.md
```

---

## 🧰 Example Features

### 1. **LazyTasks.Eventually**

```csharp
await ProcrastiN8.LazyTasks.Eventually.Do(() => DoSomethingImportant(), within: TimeSpan.FromDays(3));
```

> Wraps a task in a humorous delay. Logs "I'll get to it" messages at intervals. Optional `ExcuseGenerator`.

---

### 2. **TODOFramework.TodoAttribute**

```csharp
[Todo("Refactor this someday", Priority = -1)]
public void LegacyThing() => Console.WriteLine("Still works ¯\\_(ツ)_/¯");
```

> A `[Todo]` attribute that plugs into a `TodoScheduler` which... never runs.

---

### 3. **Unproductivity.FakeProgress**

```csharp
var bar = new FakeProgress("Deploying to Production");
bar.Start(); // increments endlessly, loops at 98%
```

> Simulates being productive. Great for impressing managers or livestreams.

---

### 4. **JustBecause.RandomExceptionGenerator**

```csharp
var chaos = new RandomExceptionGenerator();
chaos.MaybeThrow(); // 5% chance of throwing an ArgumentOutOfExcusesException
```

> Encourages resilience through randomness.

---

### 5. **Extensions.StringExcuses**

```csharp
"Unit test not written".ToExcuse()
// => "I was waiting for more requirements"
```

> Converts any string into a semi-believable excuse using GPT-like prompt mapping.

---

## 🧪 Real Use Cases (Almost)

* Decorate legacy code with meaningful-but-ignored `[Todo]` tags
* Delay side-effects in tests with `Eventually`
* Troll teammates with infinite progress bars
* Add realism to internal tools by introducing unexplainable latency
* Practice failure handling with chaos monkey–style `MaybeThrow()`

---

## 📦 NuGet Package Metadata

* **ID:** `ProcrastiN8`
* **Author:** `veggerby.dev`
* **License:** MIT (because who cares?)
* **Version:** `0.0.8-beta` (and it’ll never leave beta)
* **Tags:** `procrastination`, `excuses`, `chaos-engineering`, `humor`, `todo`, `lazy`
