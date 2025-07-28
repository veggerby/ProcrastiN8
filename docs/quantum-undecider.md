# QuantumUndecider

> Embodies quantum procrastination by entangling outcomes and randomly collapsing decision waveforms, occasionally throwing exceptions when too much certainty is attempted.

## Overview

`QuantumUndecider` is a static class for simulating indecision at a quantum level. It provides methods for observing decisions, delaying inevitability, and triggering entangled callbacks.

## Usage

```csharp
using ProcrastiN8.JustBecause;

try
{
    var result = await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(true));
    Console.WriteLine($"Decision: {result}");
}
catch (SuperpositionCollapseException ex)
{
    Console.WriteLine($"Collapse failed: {ex.Message}");
}
```

### Possible Outcomes

- Returns a definitive or partial decision string.
- Throws `SuperpositionCollapseException` if observed too aggressively.

## API

### `ObserveDecisionAsync`

```csharp
public static Task<string> ObserveDecisionAsync(
    Func<Task<bool>> costlyComputation,
    IProcrastiLogger? logger = null,
    CancellationToken cancellationToken = default)
```

- **costlyComputation**: The computation to observe.
- **logger**: Logger for decision updates.
- **cancellationToken**: Cancels the observation.

### `DelayUntilInevitabilityAsync`

```csharp
public static Task<string> DelayUntilInevitabilityAsync(
    TimeSpan maxDelay,
    IProcrastiLogger? logger = null,
    CancellationToken cancellationToken = default)
```

- **maxDelay**: Maximum delay before a decision is inevitable.
- **logger**: Logger for updates.
- **cancellationToken**: Cancels the delay.

## Remarks

- Designed for maximum indecision and minimum determinism.
- All randomness is injectable for testability.
