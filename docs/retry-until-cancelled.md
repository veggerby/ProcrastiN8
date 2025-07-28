# RetryUntilCancelled

> Retries an action until it succeeds, is cancelled, or exhausts retry attempts. Excuses are provided for every failure.

## Overview

`RetryUntilCancelled` is a utility for simulating endless, futile retries with structured excuses and logging. It is ideal for appearing persistent while achieving little.

## Usage

```csharp
using ProcrastiN8.LazyTasks;

await RetryUntilCancelled.Do(
    async () => { Console.WriteLine("Trying again..."); },
    logger: null,
    excuseProvider: null,
    retryStrategy: null,
    cancellationToken: CancellationToken.None);
```

### Expected Output

```txt
[Retry] Attempt 1 failed. Retrying in 2.1s...
[Retry] Excuse: Still waiting for the backlog.
[Retry] Attempt 2 failed. Retrying in 4.2s...
... (until cancelled)
```

## API

### `Do`

```csharp
public static Task Do(
    Func<Task> action,
    IProcrastiLogger? logger = null,
    IExcuseProvider? excuseProvider = null,
    IRetryStrategy? retryStrategy = null,
    CancellationToken cancellationToken = default)
```

- **action**: The action to retry.
- **logger**: Logger for retry updates.
- **excuseProvider**: Provides excuses for each retry.
- **retryStrategy**: Controls retry timing.
- **cancellationToken**: Cancels the retry loop.

## Remarks

- Excuses are generated and logged for every retry.
- Retry strategies are fully injectable for testability.
- Designed for maximum appearance of resilience with minimum actual progress.
