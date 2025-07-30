# ProcrastinationScheduler

The `ProcrastinationScheduler` is a utility for deferring tasks based on absurd but technically sound delay strategies. It provides three modes of operation:

## API

```csharp
public static class ProcrastinationScheduler
{
    public static Task Schedule(
        Func<Task> task,
        TimeSpan initialDelay,
        ProcrastinationMode mode,
        IExcuseProvider? excuseProvider = null,
        CancellationToken cancellationToken = default);
}
```

### Parameters

- **`task`**: The task to execute.
- **`initialDelay`**: The initial delay before starting the task.
- **`mode`**: The scheduling strategy (`MovingTarget`, `InfiniteEstimation`, `WeekendFallback`).
- **`excuseProvider`**: Optional provider for logging whimsical excuses.
- **`cancellationToken`**: Token to cancel the scheduling process.

## Modes

### MovingTarget

- Delay increases by 10–25% after each postponement.
- Task executes after a maximum delay cap or stable observations.

### InfiniteEstimation

- Logs “Estimated time to start: 5 minutes.” repeatedly.
- Task starts only if `TriggerNow()` is called or canceled.

### WeekendFallback

- Task runs if 72 hours have passed or it’s Saturday after 3 PM.

## Example Usage

```csharp
await ProcrastinationScheduler.Schedule(
    async () => Console.WriteLine("Task executed!"),
    TimeSpan.FromSeconds(10),
    ProcrastinationMode.MovingTarget);
```
