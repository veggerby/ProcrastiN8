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

The `ProcrastinationScheduler` allows you to defer tasks with absurd delay strategies. See [docs/procrastination-scheduler.md](docs/procrastination-scheduler.md) for details.

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
