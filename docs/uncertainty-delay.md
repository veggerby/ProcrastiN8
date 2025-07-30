# UncertaintyDelay

> A delay that changes unpredictably each time you check it.

## Overview

`UncertaintyDelay` is a utility for introducing unpredictable delays in asynchronous workflows. It leverages `IRandomProvider` for testable randomness and supports excuses for each delay round.

## Usage

```csharp
using ProcrastiN8.LazyTasks;

await UncertaintyDelay.WaitAsync(
    maxDelay: TimeSpan.FromSeconds(5),
    cancellationToken: CancellationToken.None);
```

## Features

- Randomized delay durations.
- Configurable number of delay rounds.
- Optional excuse logging for each delay.

## API

### `WaitAsync`

```csharp
public static Task WaitAsync(
    TimeSpan maxDelay,
    int? rounds = null,
    IRandomProvider? randomProvider = null,
    IExcuseProvider? excuseProvider = null,
    IDelayProvider? delayProvider = null,
    IProcrastiLogger? logger = null,
    CancellationToken cancellationToken = default)
```

- **maxDelay**: The maximum delay duration per round.
- **rounds**: Number of delay rounds to perform. If null or less than 1, a random count (2–5) will be used.
- **randomProvider**: The random provider used to determine delay duration and rounds.
- **excuseProvider**: Optional excuse provider for logging procrastination reasons.
- **delayProvider**: The provider used to introduce delays.
- **logger**: Optional logger for tracking delay execution.
- **cancellationToken**: A token to cancel the delay sequence.

## Remarks

This delay is designed to simulate the uncertainty of procrastination, ensuring that no two checks yield the same result.
