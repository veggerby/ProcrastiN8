# FakeProgress

> Simulates progress by emitting fake updates while doing absolutely nothing of consequence.

## Overview

`FakeProgress` is a utility for simulating the appearance of productivity. It emits progress updates, logs commentary, and supports cancellation, all while achieving nothing.

## Usage

```csharp
using ProcrastiN8.Unproductivity;

await FakeProgress.RunAsync(
    "Deploying to production",
    steps: 5,
    logger: null,
    cancellationToken: CancellationToken.None);
```

### Expected Output

```txt
[FakeProgress] Aligning expectations...
[FakeProgress] Calibrating metrics...
[FakeProgress] Pretending to load data...
[FakeProgress] Synchronizing with imaginary server...
[FakeProgress] Consulting committee of doubts...
[FakeProgress] Progress: 100% (allegedly)
```

## API

### `RunAsync`

```csharp
public static Task RunAsync(
    string taskDescription,
    int steps = 10,
    IProcrastiLogger? logger = null,
    CancellationToken cancellationToken = default)
```

- **taskDescription**: Description of the fake task.
- **steps**: Number of fake progress steps.
- **logger**: Logger for progress updates.
- **cancellationToken**: Cancels the fake progress.

## Remarks

- Commentary and progress are fully customizable.
- Designed for maximum optics and minimum output.
