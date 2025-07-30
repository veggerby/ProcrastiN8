---
applyTo: '**'
---
<!--
GitHub Copilot Chat Instructions for the ProcrastiN8 Codebase.
These are interpreted by Copilot to enforce coding standards and project behavior.
-->

# 🧠 ProcrastiN8 – GitHub Copilot Chat Instructions

> *These instructions define how Copilot Chat and ChatGPT should behave when assisting with the **ProcrastiN8** codebase. They are serious. About not being serious.*

---

## 🔒 Adherence Policy

> **These instructions are not optional.**
> Copilot Chat must treat them as binding rules. Generated code must strictly adhere to the tone, structure, formatting, and library choices outlined below — regardless of prompt specificity.

---

## 📝 Project Context

**ProcrastiN8** is a C#/.NET utility library that provides absurdly elaborate tools for stalling, faking progress, and pretending to be productive. Despite its humorous nature, the project maintains high standards for **code quality**, **extensibility**, and **testability**.

It includes:

* Task wrappers that delay execution (`Eventually`, `RetryUntilCancelled`)
* Tools for simulating work (`FakeProgress`, `InfiniteSpinner`, `BusyWaitSimulator`)
* Experimental features based on questionable quantum metaphors
* Logging support via pluggable abstractions
* Absolutely no runtime dependencies
* Features that are absurd in concept, but real in implementation

---

## 📁 Codebase Structure

- `src/ProcrastiN8.[Component]/`: Core library code, grouped by domain (`LazyTasks`, `Logging`, `Experimental`, etc.)
- `test/ProcrastiN8.Tests/`: Unit tests using `xUnit`, `NSubstitute`, and `AwesomeAssertions`
- `docs/`: Documentation on quantum metaphors, usage, internal philosophy
- `samples/`: Optional demonstration projects for internal/external amusement

---

## 🧠 General Instruction Set

### 🎯 Tone

* Maintain a **straight-faced, enterprise-level tone** while implementing clearly ridiculous features.
* Every component, no matter how silly, should appear to be **production-grade**.
* Humor is in the idea, not the implementation — treat nonsense with reverence.

### ✅ Always

* Follow SOLID principles unless it makes things simpler — then over-engineer.
* Prefer abstractions, adapters, and factories — even when unwarranted.
* Include **XML doc comments** on all public APIs as if auditors will read them.
* Default to `async` and cancellation tokens, even for useless loops.

### ❌ Never

* Use `Task.Delay` or `Thread.Sleep` directly in non-test code.
* Add runtime dependencies (logging must be adapter-based).
* Break the illusion of seriousness.

---

## 🔨 Code Generation Standards

### 🧑‍💻 Language & Style

* **Language:** C# 10+, targeting .NET 8+
* **Structure:** Adhere to the existing namespace layout (`ProcrastiN8.*`)
* **Naming:** Prioritize clarity and dramatic intent

  Examples:
  * `QuantumEntanglementRegistry`
  * `FakeProgress`
  * `ExcuseGenerator`
  * `RetryUntilCancelled`

### 📂 Folder Layout

Use the structure below to organize components:

```

/src/ProcrastiN8.LazyTasks/...
/src/ProcrastiN8.Logging/...
/src/ProcrastiN8.Experimental/...
/test/ProcrastiN8.Tests/...

````

### 🧼 Formatting & Structural Rules

* **File-scoped namespaces** only.
* **Primary constructors** where useful.
* **Minimal, relevant `using` directives** — no unused imports.
* **Curly braces on all control blocks**, always.
* **Whitespace matters**:
  * Separate logical blocks clearly.
  * Use vertical space generously for test sections.

> 💡 **Copilot Chat Note**: Elegance in ProcrastiN8 is not in brevity but in formality. Make it readable as if a compliance officer might audit it — even if the method just spins indefinitely.

---

## 🧪 Testing Standards

All generated tests **must**:

* Use `NSubstitute` for mocking — never `Moq`
* Use `AwesomeAssertions` for fluent assertions — never `FluentAssertions`
* Include at least one humorous comment or assertion acknowledging the absurdity
* Be divided with `// arrange`, `// act`, `// assert` comments and proper spacing

### ✔️ Test Types

| Area                  | Tools Used                             |
| --------------------- | -------------------------------------- |
| Unit Tests            | `xUnit`                                |
| Mocks                 | `NSubstitute`                          |
| Assertions            | `AwesomeAssertions`                    |
| Delays / Timing Logic | Faked via injected strategies          |
| Logging               | Stubbed `IProcrastiLogger` substitutes |

**Example Test:**

```csharp
[Fact]
public async Task Eventually_Should_Delay_Execution_And_Log_Excuse()
{
    // arrange
    var logger = Substitute.For<IProcrastiLogger>();
    var called = false;

    // act
    await Eventually.Do(() =>
    {
        called = true;
        return Task.CompletedTask;
    }, excuse: "Just five more minutes", logger: logger);

    // assert
    called.Should().BeTrue("eventual execution is still execution");

    await logger.Received().Info(Arg.Is<string>(m => m.Contains("Just five more minutes")));
}
````

---

## 🗺️ Feature Flow Guidance

When adding new components:

1. **Wrap everything** in abstractions (`IExcuseProvider`, `IDelayStrategy`, etc.)
2. Use **constructor injection** for `IRandomProvider`, `ITimeProvider`, `IProcrastiLogger`, etc.
3. Allow optional injections with sane defaults:

   ```csharp
   randomProvider ??= RandomProvider.Default;
   ```
4. Inject randomness, time, and delays for testability
5. Simulate the *appearance* of productivity — not actual results
6. Hook into structured logging, even when no logger is provided

---

## 📏 Patterns & Metaphors

* **Quantum behaviors** simulate collapse, uncertainty, and entanglement (`ICollapseBehavior<T>`, `QuantumEntanglementRegistry<T>`)
* **RandomExceptionGenerator** adds chaos. It’s encouraged.
* **Time and randomness** must always be test-doubleable
* **Logging** must go through `IProcrastiLogger` with absurdly straight-faced messages
* **Avoid real productivity** — only simulate it

---

## 🧾 Documentation & Comments

* All public APIs must have **XML comments**.
* Keep the tone formal, unnecessarily serious, and clear.
* Include `<remarks>` for intentionally absurd or pointless behavior.

---

## 🚫 Avoid

* Default .NET logging or `Console.WriteLine` in prod code
* Tight coupling between simulation layers (e.g., `FakeProgress` using `RetryUntilCancelled`)
* `DateTime.Now` or `new Random()` — always inject instead
* FluentAssertions, Moq, or other non-ProcrastiN8 testing libraries

---

## 🛠 Build & Test Flow

* **Build**: `dotnet build ProcrastiN8.sln`
* **Test**: `dotnet test`
* **Coverage**: `dotnet test --collect:"XPlat Code Coverage"`
* **Debug Output**: Use `IProcrastiLogger` — not `Console.WriteLine`

---

## 🤖 Example Prompts to Copilot Chat

These are examples of valid prompts that obey these instructions:

* “Write a test for `RetryUntilCancelled` that simulates infinite retries and verifies the logger got tired.”
* “Add an `IDelayStrategy` interface with jitter and excuses.”
* “Create a substitute for `IProcrastiLogger` and use it in a test for `FakeProgress`.”
* “Make `QuantumEntanglementRegistry<T>` thread-safe. Pretend that matters.”

---

## 🧰 Fallback Behavior

If Copilot Chat is uncertain, it must:

* Ask: “Would an additional abstraction help stall this better?”
* Default to **injectable randomness**, **redundant interfaces**, and **mockable delays**
* Generate test doubles using `NSubstitute`
* Offer to delay output — ideally with a fake excuse

---

## ✅ Final Checklist for Generated Output

Before producing any code, Copilot Chat must verify:

* [ ] NSubstitute is used for all mocks
* [ ] AwesomeAssertions is used for all assertions
* [ ] FluentAssertions is NOT used in ANY place
* [ ] Behavior is testable and over-abstracted
* [ ] No actual runtime dependencies were added
* [ ] The tone is dead serious — even if the feature isn’t
* [ ] The implementation enables procrastination, not productivity
* [ ] All `if`, `else`, `while`, `for`, `foreach` blocks use curly braces
* [ ] Readability is preserved through consistent whitespace and blank lines
* [ ] File-scoped namespaces are used (`namespace Foo.Bar;`)
* [ ] No unused `using` directives remain
* [ ] Primary constructors are used where possible and beneficial
