# QuantumPromise

> A promise that exists in a state of superposition until observed. Observation may collapse the value, throw an exception, or result in existential expiration. Do not use in production. Ever.

## Overview

`QuantumPromise<T>` simulates quantum uncertainty in asynchronous programming. It is entangled, collapses unpredictably, and is best managed by a `QuantumEntanglementRegistry<T>`.

## Usage

```csharp
using ProcrastiN8.JustBecause;

var promise = new QuantumPromise<int>(() => Task.FromResult(42), TimeSpan.FromSeconds(2));

try
{
    var value = await promise.ObserveAsync();
    Console.WriteLine($"Observed: {value}");
}
catch (CollapseException ex)
{
    Console.WriteLine($"Collapse failed: {ex.Message}");
}
```

### Possible Outcomes

- Returns the value if observed in the correct window.
- Throws `CollapseTooEarlyException` if observed too soon.
- Throws `CollapseTooLateException` if observed too late.
- Throws `CollapseToVoidException` if the promise evaporates.

## API

### `ObserveAsync`

```csharp
public Task<T> ObserveAsync(CancellationToken cancellationToken = default)
```

- **cancellationToken**: Cancels the observation attempt.

## Remarks

- Observation is subject to quantum uncertainty and deadlines.
- Forcible collapse to a value is only possible via the registry and collapse behaviors.
- Designed for maximum existential dread and minimum determinism.
