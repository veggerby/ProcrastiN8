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

### `StringExcuses.ToExcuse`

Generates a plausible excuse for any string context.

````csharp
var excuse = "Unit tests".ToExcuse();
// "Unable to process 'Unit tests' due to unforeseen circumstances."
````

### `DateTimePostponer.Postpone`

Returns a new `DateTime` arbitrarily postponed by a specified duration.

````csharp
var now = DateTime.Now;
var later = now.Postpone(TimeSpan.FromDays(3));
````

## API Reference

See XML docs in source for full details.
