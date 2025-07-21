# 🐢 ProcrastiN8

> *Because shipping is optional.*

**ProcrastiN8** is a C#/.NET utility library for developers who embrace the art of strategic procrastination. Whether you're stalling creatively or just simulating productivity, this package provides humorous, overengineered, yet technically valid tools to help you do less — better — later.

---

## ✨ Features

* `Eventually.Do(...)`: Execute tasks... eventually. Now with excuses and jitter.
* `FakeProgress`: Simulate infinite progress to impress stakeholders.
* `InfiniteSpinner`: A CPU-hungry animation loop of meaningless motion.
* `BusyWaitSimulator`: Burn cycles under the guise of being *very* busy.
* `RetryUntilCancelled`: Retries operations forever (or until someone gives up).
* `QuantumEntanglementRegistry`: Link async operations through questionable physics metaphors.
* `ExcuseGenerator`: Generate plausible (or implausible) reasons for delay.
* `CommentaryService`: Structured procrastination commentary, fully injectable.
* `DelayStrategy`: Pluggable, testable delay logic (coming soon).
* `ProcrastinationMetrics`: Track and analyze your stalling performance (coming soon).
* `[Todo]` attributes and schedulers that do absolutely nothing.
* Randomized exception injectors — because chaos is healthy.
* Extensions for generating excuses, procrastinating DateTimes, and more.
* Support for structured logging (without mandatory dependencies).
* Playful, absurd, but built with real care.

---

## 🧙‍♂️ Getting Started

### 🔧 Installation

```bash
dotnet add package ProcrastiN8 --prerelease
# Because release-readiness is a social construct.
```

Or via the NuGet Package Manager:

```powershell
Install-Package ProcrastiN8 -Prerelease
# Because final releases are for the overly decisive.
```

---

## 🐌 Usage Example

```csharp
using ProcrastiN8.LazyTasks;

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

---

## 📚 API Highlights

### `Eventually.Do(...)`

```csharp
Task Eventually.Do(
    Func<Task> action,
    TimeSpan? within = null,
    string? excuse = null,
    IProcrastiLogger? logger = null,
    CancellationToken cancellationToken = default)
```

* `within`: Maximum delay before action is executed (default: 30s).
* `excuse`: Optional string to justify the delay (see `ExcuseGenerator`).
* `logger`: Optional logger (custom `IProcrastiLogger` interface).
* Logs procrastination chatter while stalling.

### `RetryUntilCancelled`

```csharp
Task RetryUntilCancelled.Do(
    Func<Task> action,
    IProcrastiLogger? logger = null,
    IExcuseProvider? excuseProvider = null,
    IRetryStrategy? retryStrategy = null,
    CancellationToken cancellationToken = default)
```

* Retries the action until cancellation or despair.
* Fully injectable for testability (excuses, delays, logging).

### `QuantumEntanglementRegistry<T>`

```csharp
public class QuantumEntanglementRegistry<T>
```

* Entangle and collapse async operations in ways that defy reason.
* Thread-safe, for those who pretend it matters.

### `FakeProgress`, `InfiniteSpinner`, `BusyWaitSimulator`

* Simulate the appearance of productivity with endless progress, spinning, or busy-waiting.
* All support cancellation and logging.

---

## 🧪 Testing Standards

ProcrastiN8 is built for testability and over-abstraction:

* **Unit tests:** xUnit
* **Mocks:** NSubstitute (no exceptions)
* **Assertions:** AwesomeAssertions (for expressive, readable tests)
* **Delays/timing:** Always injectable/faked
* **Logging:** Always via `IProcrastiLogger` or test double
* **Absurdity:** Every test must acknowledge the futility of its own existence

Example:

```csharp
[Fact]
public async Task Eventually_Should_Delay_Execution_And_Log_Excuse()
{
    var logger = Substitute.For<IProcrastiLogger>();
    var called = false;

    await Eventually.Do(() =>
    {
        called = true;
        return Task.CompletedTask;
    }, excuse: "Just five more minutes", logger: logger);

    called.Should().BeTrue("eventual execution is still execution");
    await logger.Received().Info(Arg.Is<string>(m => m.Contains("Just five more minutes")));
}
```

---

## 🔄 New & Experimental

| Feature                       | Description                                                                |
| ----------------------------- | -------------------------------------------------------------------------- |
| `InfiniteSpinner.Start()`     | Loops endlessly to simulate doing something. CPU usage is part of the fun. |
| `BusyWaitSimulator.Run()`     | Consumes time with zero productivity.                                      |
| `RetryUntilCancelled`         | Retries until you cancel or give in. No exponential backoff — just hope.   |
| `QuantumEntanglementRegistry` | Links promises. Collapse one and see what happens. No guarantees.          |
| `FakeProgress.Start()`        | Loops endlessly at ~98%. Adds legitimacy to indecision.                    |
| `DelayStrategy`               | Pluggable, testable delay logic (coming soon).                             |
| `ProcrastinationMetrics`      | Track and analyze your stalling performance (coming soon).                 |

---

## 🪵 Logging Support

ProcrastiN8 uses a lightweight, pluggable logger abstraction:

```csharp
public interface IProcrastiLogger
{
    void Info(string message);
    void Debug(string message);
    void Error(string message, Exception? ex = null);
}
```

You can use your own implementation or adapt to `Microsoft.Extensions.Logging`:

```csharp
var logger = new MELAdapter(myILogger); // from ProcrastiN8.Adapters.MEL
await Eventually.Do(action, logger: logger);
```

---

## 🪄 Roadmap

* [x] `QuantumEntanglementRegistry` for chaotic promise behavior
* [x] `FakeProgress`, `InfiniteSpinner`, and `BusyWaitSimulator`
* [x] `RetryUntilCancelled` with structured commentary
* [x] Robust, over-abstracted test suite (NSubstitute, AwesomeAssertions)
* [ ] `DelayStrategy` support (e.g., exponential backoff with excuses)
* [ ] `ProcrastinationMetrics` (e.g. average delay, skipped deadlines)
* [ ] NuGet sub-packages: `ProcrastiN8.Logging.MEL`, `ProcrastiN8.Tracing`
* [ ] Integration with GPT for dynamically generated excuses
* [ ] Remove `--pre-release` from install instructions
  *(*ETA: when the universe reaches heat death*)*

---

## 🎯 Why?

> “Don't put off until tomorrow what you can avoid indefinitely with style.” — someone, probably

This library is both satire and a gentle commentary on developer behavior. But it’s also built with care, type safety, testability, and extensibility in mind.

---

## 🧪 Requirements

* .NET 6.0 or higher
* No runtime dependencies
* Logging is optional and pluggable

---

## 👨‍🔧 Author

Made by [@veggerby](https://github.com/veggerby) — overengineer, software architect, and semi-professional excuse generator.

---

## ⚖️ License

MIT. Free to use, fork, delay, or completely ignore.

---

## 🧠 Bonus Tip

```csharp
var bar = new FakeProgress("Deploying to Production");
bar.Start();
// Just sit back. No one will question a 98% loading bar.
```

Or go quantum:

```csharp
var registry = new QuantumEntanglementRegistry<string>();
registry.Entangle(someQuantumPromise);
await registry.CollapseOneAsync(); // Reality may vary
```

---

> ☕ If you actually read this far, you're probably procrastinating something important. You're in good company.
