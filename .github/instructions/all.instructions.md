---
applyTo: '**'
---
# 🧠 ProcrastiN8 – GitHub Copilot Chat Instructions

> *These instructions define how Copilot Chat and ChatGPT should behave when assisting with the **ProcrastiN8** codebase. They are serious. About not being serious.*

---

## 📝 PROJECT CONTEXT

**ProcrastiN8** is a C#/.NET utility library that provides absurdly elaborate tools for stalling, faking progress, and pretending to be productive. Despite its humorous nature, the project maintains high standards for **code quality**, **extensibility**, and **testability**.

It includes:

* Task wrappers that delay execution (`Eventually`, `RetryUntilCancelled`)
* Tools for simulating work (`FakeProgress`, `InfiniteSpinner`, `BusyWaitSimulator`)
* Experimental features based on questionable quantum metaphors
* Logging support via pluggable abstractions
* Absolutely no runtime dependencies
* Features that are absurd in concept, but real in implementation

---

## 🧠 GENERAL INSTRUCTION SET

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

* Use shortcuts like `Task.Delay` directly in non-test code unless they're wrapped.
* Use `Thread.Sleep`, unless simulating a pointless bottleneck is intentional.
* Add runtime dependencies (logging must be adapter-based).
* Break the illusion of seriousness.

---

### 🔨 CODE GENERATION STANDARDS

### 🧑‍💻 Language & Style

* **Language:** C# 10+, targeting .NET 8+
* **Structure:** Adhere to the existing namespace layout (`ProcrastiN8.*`)
* **Naming:** Prioritize clarity and dramatic intent

  Examples:

  * `QuantumEntanglementRegistry`
  * `FakeProgress`
  * `ExcuseGenerator`
  * `RetryUntilCancelled`

### 📂 Folder Structure

Organize code under `src/ProcrastiN8.[Component]/...`, with clear division:

```txt
/src/ProcrastiN8.LazyTasks/...
/src/ProcrastiN8.Logging/...
/src/ProcrastiN8.Experimental/...
```

### 🧼 Formatting & Structural Rules

* **Always use file-scoped namespaces.**
  ✅ Correct:

  ```csharp
  namespace ProcrastiN8.Something;

  public class ExcuseGenerator { }
  ```

  ❌ Incorrect:

  ```csharp
  namespace ProcrastiN8.Something
  {
      public class ExcuseGenerator { }
  }
  ```

* **Use primary constructors** when feasible and clear.
  ✅ Example:

  ```csharp
  namespace ProcrastiN8.Something;

  public class MyClass(string foo)
  {
      public string Foo { get; } = foo;
  }
  ```

* **Avoid unnecessary `using` statements.**
  All `using` directives must be minimal and relevant to the file. No imports should remain if unused.

* **Always wrap single-line control blocks in curly braces `{}`.**
  ✅ Correct:

  ```csharp
  if (condition)
  {
      Handle();
  }
  ```

  ❌ Incorrect:

  ```csharp
  if (condition) Handle();
  ```

* **Ensure appropriate whitespace:**

  * Add blank lines between logical sections (e.g., method groups, Arrange/Act/Assert blocks).
  * Avoid over-nesting, dense vertical layouts, and logic stacking.

> 💡 **Copilot Chat Note**: Elegance in ProcrastiN8 is not in brevity but in formality. Make it readable as if a compliance officer might audit it — even if the method just spins indefinitely.

---

## 🧪 TESTING STANDARDS

**All generated tests MUST:**

* Use **NSubstitute** for mocking — no exceptions
* Use **AwesomeAssertions** for fluent, expressive assertions
* Include at least one comment or output line that makes light of the absurdity
* Be structured into **clearly commented arrange / act / assert** sections with appropriate whitespace for readability

### ✔️ Test Types

| Area                  | Tools Used                             |
| --------------------- | -------------------------------------- |
| Unit Tests            | `xUnit`                                |
| Mocks                 | `NSubstitute`                          |
| Assertions            | `AwesomeAssertions`                    |
| Delays / Timing Logic | Faked via injected strategies          |
| Logging               | Stubbed `IProcrastiLogger` substitutes |

**Test Structure Example:**

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
```

> 💡 **Note for Copilot Chat**: The `// arrange`, `// act`, and `// assert` comments are **not optional** — they are part of the readability standard. Space them out with blank lines to honor the absurd gravity of our unserious testing discipline.

---

## 🗺️ FEATURE FLOW GUIDANCE

When implementing new features:

1. **Wrap everything** in abstractions (`IExcuseProvider`, `IDelayStrategy`, etc.)
2. Allow **injection** of delays, loggers, exceptions, and cancellation
3. If randomness is used, ensure it’s injectable for tests
4. Don’t just fake work — simulate the *appearance* of productivity
5. Use structured logging hooks even if no logger is provided

---

## 🧾 DOCUMENTATION & COMMENTS

* Document every public API with XML comments
* All methods must have a **straightforward yet uncomfortably serious tone**
* Include remarks for intentionally pointless behavior

**Example:**

```csharp
/// <summary>
/// Executes an asynchronous action after a completely arbitrary delay.
/// </summary>
/// <param name="action">The action to eventually perform.</param>
/// <param name="within">The maximum tolerated procrastination period.</param>
/// <param name="excuse">An optional excuse to log before stalling.</param>
/// <param name="logger">A logger for procrastination updates.</param>
/// <param name="cancellationToken">A token to cancel the eventual action.</param>
```

---

## 🚫 AVOID

* Using default logging frameworks directly
* Mixing experimental features with core modules
* Tight coupling between simulation layers (e.g. FakeProgress shouldn’t rely on RetryUntilCancelled)
* Writing `DateTime.Now` or `new Random()` without injecting them

---

## 🤖 EXAMPLE PROMPTS TO COPILOT CHAT

* “Write a test for `RetryUntilCancelled` that simulates infinite retries and verifies the logger got tired.”
* “Add an `IDelayStrategy` interface with jitter and excuses.”
* “Create a substitute for `IProcrastiLogger` and use it in a test for `FakeProgress`.”
* “Make `QuantumEntanglementRegistry<T>` thread-safe. Pretend that matters.”

---

## 🧰 FALLBACK BEHAVIOR

If Copilot Chat is uncertain, it should:

* Ask: “Would an additional abstraction help stall this better?”
* Default to **injectable randomness**, **redundant interfaces**, and **mockable time delays**
* Create a test double using NSubstitute
* Delay the answer — ideally with a fake excuse

---

## ✅ FINAL CHECKLIST FOR GENERATED OUTPUT

Before suggesting code, Copilot Chat must ensure:

* [ ] NSubstitute is used for all mocks
* [ ] AwesomeAssertions is used for all assertions
* [ ] Behavior is testable and over-abstracted
* [ ] No actual runtime dependencies were added
* [ ] The tone is dead serious — even if the feature isn’t
* [ ] The implementation enables procrastination, not productivity
* [ ] All `if`, `else`, `while`, `for`, `foreach` blocks use curly braces
* [ ] Readability is preserved through consistent whitespace and blank lines
* [ ] File-scoped namespaces are used (`namespace Foo.Bar;`)
* [ ] No unused `using` directives remain
* [ ] Primary constructors are used where possible and beneficial
