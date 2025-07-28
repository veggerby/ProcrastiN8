---
title: "Extensions"
sidebar_position: 12
---

> Because even productivity helpers need to stall sometimes.

The **ProcrastiN8.Extensions** namespace provides extension methods for common .NET types, enabling new heights of plausible procrastination.

## Components

### `EnumerableExtensions.LoopForever`

Repeats the elements of any enumerable endlessly, simulating infinite productivity.

````csharp
var numbers = new[] { 1, 2, 3 };
foreach (var n in numbers.LoopForever().Take(7))
{
    Console.WriteLine(n); // 1 2 3 1 2 3 1
}
````

### `StringExcuses.ToExcuse` and `ToExcuseWithPrefix`

Generates a plausible excuse for any string context, with support for context-aware, provider-driven, and prefix-enhanced excuses.

````csharp
var excuse = "Unit tests".ToExcuse();
// "Unable to process 'Unit tests' due to unforeseen circumstances."

var customExcuse = "Merge conflict".ToExcuse(myExcuseProvider);
// "Unable to process 'Merge conflict' because: [elaborate excuse from provider]"

var prefixed = "Deadline".ToExcuseWithPrefix("Heads up: ", myExcuseProvider);
// "Heads up: Unable to process 'Deadline' because: [excuse]"
````

- Excuses can be customized via `IExcuseProvider` for maximum deniability.
- Prefixes add gravitas and plausible professionalism.

### `DateTimePostponer.Postpone` and `PostponeAsync`

Returns a new `DateTime` arbitrarily postponed by a specified duration, with full support for pluggable strategies, excuses, logging, and async deliberation.

````csharp
var now = DateTime.Now;
var later = now.Postpone(TimeSpan.FromDays(3));

// With custom strategy, excuse provider, and logger
var customLater = now.Postpone(TimeSpan.FromHours(2), myStrategy, myExcuseProvider, myLogger);

// Asynchronous, simulating deliberation
var asyncLater = await now.PostponeAsync(TimeSpan.FromMinutes(42), myStrategy, myExcuseProvider, myLogger, cancellationToken);
````

- Strategies (`IDateTimePostponeStrategy`) allow arbitrary, over-engineered postponement logic.
- Excuses and logging are fully pluggable for audit trails and plausible deniability.
- Async variant simulates the appearance of careful consideration.

## API Reference

See XML docs in source for full details.
