# Eventually

> Executes an asynchronous action after a completely arbitrary, but production-grade, delay.

## Overview

`Eventually` is a utility for simulating procrastination by wrapping an async action in a configurable, over-engineered delay. It supports excuses, logging, and cancellation tokens for maximum stalling.

## Usage

```csharp
using ProcrastiN8.LazyTasks;

await Eventually.Do(async () =>
{
    Console.WriteLine("Actually doing something now...");
},
within: TimeSpan.FromSeconds(30),
excuse: "Blocked by a third-party library.",
logger: null,
cancellationToken: CancellationToken.None);
```

### Expected Output

```txt
[Eventually] Thinking about it... Might do it in 17.3s.
[Eventually] Reason for delay: Blocked by a third-party library.
[Eventually] Just five more minutes...
...
[Eventually] Finally did the thing. You're welcome.
```

## API

### `Do`

```csharp
public static Task Do(
    Func<Task> action,
    TimeSpan? within = null,
    string? excuse = null,
    IProcrastiLogger? logger = null,
    CancellationToken cancellationToken = default)
```

- **action**: The action to eventually perform.
- **within**: The maximum tolerated procrastination period.
- **excuse**: An optional excuse to log before stalling.
- **logger**: A logger for procrastination updates.
- **cancellationToken**: A token to cancel the eventual action.

## Remarks

- All delays are mockable and injectable for testability.
- Excuses are logged with the utmost seriousness.
- Designed for maximum appearance of productivity with minimum actual output.
