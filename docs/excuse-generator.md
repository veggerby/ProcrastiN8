# ExcuseGenerator

> Provides a rich set of excuses for procrastination, suitable for any occasion.

## Overview

`ExcuseGenerator` is a utility for generating plausible, elaborate, or existential excuses. It is used throughout ProcrastiN8 to justify delays, retries, and general lack of progress.

## Usage

```csharp
using ProcrastiN8.Common;

var excuse = ExcuseGenerator.GetRandomExcuse();
Console.WriteLine($"Excuse: {excuse}");
```

## API

### `GetRandomExcuse`

```csharp
public static string GetRandomExcuse()
```

- Returns a random excuse string.

## Remarks

- Excuses are curated for maximum plausibility and minimum accountability.
- Used by all core procrastination features.
