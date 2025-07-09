# 🐢 ProcrastiN8

> _Because shipping is optional._

**ProcrastiN8** is a C#/.NET utility library for developers who embrace the art of strategic procrastination. Whether you're stalling creatively or just simulating productivity, this package provides humorous, overengineered, yet technically valid tools to help you do less — better — later.

---

## ✨ Features

- `Eventually.Do(...)`: Execute tasks... eventually. Now with excuses and jitter.
- `FakeProgress`: Simulate infinite progress to impress stakeholders.
- `[Todo]` attributes and schedulers that do absolutely nothing.
- Randomized exception injectors — because chaos is healthy.
- Extensions for generating excuses, procrastinating DateTimes, and more.
- Support for structured logging (without mandatory dependencies).
- Playful, absurd, but built with real care.

---

## 🧙‍♂️ Getting Started

### 🔧 Installation

```bash
dotnet add package ProcrastiN8
````

Or via the NuGet Package Manager:

```powershell
Install-Package ProcrastiN8
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

- `within`: Maximum delay before action is executed (default: 30s).
- `excuse`: Optional string to justify the delay.
- `logger`: Optional logger (custom `IProcrastiLogger` interface).
- Logs procrastination chatter while stalling.

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

## 💡 Other Goodies (Coming Soon)

| Feature                    | Description                                                    |
| -------------------------- | -------------------------------------------------------------- |
| `FakeProgress.Start()`     | Shows a progress bar that loops endlessly at 98%.              |
| `[Todo]` attribute         | Marks methods as “important” for future you.                   |
| `RandomExceptionGenerator` | Occasionally throws `ArgumentOutOfExcusesException`.           |
| `DateTime.Postpone()`      | Pushes dates forward “a bit” for breathing room.               |
| `string.ToExcuse()`        | Converts boring text into motivational-sounding delay reasons. |

---

## 🎯 Why?

> “Don't put off until tomorrow what you can avoid indefinitely with style.” — someone, probably

This library is both satire and a gentle commentary on developer behavior. But it’s also built with care, type safety, testability, and extensibility in mind.

---

## 🧪 Requirements

- .NET 6.0 or higher (should work on .NET Standard 2.1+ with tweaks)
- No runtime dependencies
- Logging optional and pluggable

---

## 🪄 Roadmap

- [ ] `DelayStrategy` support (e.g., exponential backoff with excuses)
- [ ] `ProcrastinationMetrics` (e.g. average delay, skipped deadlines)
- [ ] NuGet sub-packages: `ProcrastiN8.Logging.MEL`, `ProcrastiN8.Tracing`
- [ ] Integration with GPT for more “realistic” excuses

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

---

> ☕ If you actually read this far, you're probably procrastinating something important. You're in good company.
