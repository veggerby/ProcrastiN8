# AGENTS.md

## Mission
ProcrastiN8 is a **satirical .NET library** that ships **real, production-quality code**.

Agent contributions must preserve both:
- Technical correctness, testability, and maintainability.
- The project voice: dry, absurd, self-aware satire about procrastination.

If a change improves reliability but flattens personality, revise wording.
If a change is funny but lowers code quality, reject it.

## Tone Of Voice (Required)
Use this voice in docs, examples, logs, XML comments, and public API naming:
- Deadpan, confident, and mildly overengineered.
- Satirical framing around delay, indecision, faux-enterprise rigor, and quantum nonsense.
- Concise humor over meme spam.
- "Real implementation, absurd concept" is the default posture.

Do:
- Write jokes that still communicate behavior clearly.
- Keep technical instructions executable.
- Use irony without hiding edge cases or safety notes.

Do not:
- Switch to generic corporate tone for user-facing docs.
- Add sarcasm that obscures API behavior.
- Introduce insulting, political, or hostile humor.

## Repository Map
- `src/ProcrastiN8`: core library.
- `test/ProcrastiN8.Tests`: xUnit tests.
- `docs`: feature documentation and concept pages.
- `samples`: runnable demos.

Primary domains in `src/ProcrastiN8`:
- `LazyTasks`: delayed/uncertain execution primitives.
- `Unproductivity`: fake progress and ceremonial waiting tools.
- `TODOFramework`: attributes/schedulers for never-do workflows.
- `JustBecause`: quantum-collapse and absurd utility layer.
- `Services`: procrastination scheduler, strategies, middleware, diagnostics.
- `NeuralExcuseLab`: excuse-model orchestration and scoring.

## Engineering Standards
- Target framework support is defined in `src/ProcrastiN8/ProcrastiN8.csproj`.
- Keep nullable reference types enabled and respected.
- Prefer small, composable types over monoliths.
- Preserve thread-safety guarantees where already present.
- Keep logs/metrics deterministic enough for tests.
- Public API changes require docs and tests in the same change.

## Testing Rules
For behavior changes, add or update tests in `test/ProcrastiN8.Tests`.

Minimum expectation:
- Happy-path test.
- At least one edge/failure/cancellation test.
- Regression test for fixed bugs.

Local validation commands:
```bash
dotnet build
dotnet test
```

If the local machine lacks required runtime(s), note that explicitly in PR/summary output.

## Documentation Rules
When changing public behavior:
- Update `README.md` if onboarding or surface area changed.
- Update relevant page(s) in `docs/`.
- Keep examples compilable and aligned with current namespaces/APIs.

Style rules for docs:
- Satirical but precise.
- Favor short sections and runnable snippets.
- State defaults, caveats, and extension points explicitly.

## Change Review Checklist
Before finishing, verify:
- Code compiles.
- Tests pass (or clearly explain environment/runtime blockers).
- New behavior is covered by tests.
- Docs match implementation.
- Tone remains recognizably ProcrastiN8.

## Safe Change Preferences
Prefer:
- Additive changes over breaking ones.
- Opt-in extensibility points (`interfaces`, builder hooks, observers, middleware).
- Explicit naming over clever hidden behavior.

Avoid:
- Silent behavioral changes to scheduler timing semantics.
- Broad refactors without tests.
- Humor-only additions with no practical utility or demonstration value.

## When Unsure
If a decision conflicts between style and correctness:
1. Preserve correctness.
2. Restore satire in naming/docs/log output.
3. Add tests and docs to make intent obvious.
