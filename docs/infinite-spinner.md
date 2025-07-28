# InfiniteSpinner

> Simulates endless activity without any meaningful output. Perfect for appearing busy while achieving absolutely nothing.

## Overview

`InfiniteSpinner` is a utility for simulating perpetual motion in the workplace. It emits spinner updates, logs commentary, and supports cancellation, all while never completing.

## Usage

```csharp
using ProcrastiN8.Unproductivity;

await InfiniteSpinner.SpinForeverAsync(
    logger: null,
    commentaryService: null,
    cancellationToken: CancellationToken.None);
```

### Expected Output

```txt
[InfiniteSpinner] Spinning... (frame 1)
[InfiniteSpinner] Spinning... (frame 2)
[InfiniteSpinner] Spinning... (frame 3)
... (forever, or until cancelled)
```

## API

### `SpinForeverAsync`

```csharp
public static Task SpinForeverAsync(
    IProcrastiLogger? logger = null,
    CommentaryService? commentaryService = null,
    CancellationToken cancellationToken = default)
```

- **logger**: Logger for spinner updates.
- **commentaryService**: Service for emitting random commentary.
- **cancellationToken**: Cancels the spinner.

## Remarks

- Commentary and spinner frames are fully customizable.
- Designed for maximum appearance of activity with minimum actual work.
