# BusyWaitSimulator

> Simulates doing heavy work by consuming CPU in a loop without yielding. Looks busy, achieves nothing.

## Overview

`BusyWaitSimulator` is a utility for simulating CPU-bound work. It spins in a tight loop, logs commentary, and supports cancellation, all while producing no useful output.

## Usage

```csharp
using ProcrastiN8.Unproductivity;

await BusyWaitSimulator.SimulateAsync(
    duration: TimeSpan.FromSeconds(5),
    logger: null,
    cancellationToken: CancellationToken.None);
```

### Expected Output

```txt
[BusyWaitSimulator] Starting busy wait for 5s...
[BusyWaitSimulator] Still busy... (1s)
[BusyWaitSimulator] Still busy... (2s)
[BusyWaitSimulator] Still busy... (3s)
[BusyWaitSimulator] Still busy... (4s)
[BusyWaitSimulator] Busy wait complete. No actual work was done.
```

## API

### `SimulateAsync`

```csharp
public static Task SimulateAsync(
    TimeSpan duration,
    IProcrastiLogger? logger = null,
    CancellationToken cancellationToken = default)
```

- **duration**: Duration of the busy wait.
- **logger**: Logger for busy wait updates.
- **cancellationToken**: Cancels the busy wait.

## Remarks

- Designed for maximum CPU usage and minimum productivity.
- All logging is adapter-based for testability.
