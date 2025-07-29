# Random Provider

The **Random Provider** is the default implementation of `IRandomProvider`, leveraging `System.Random` to ensure testability and consistency in randomness across ProcrastiN8 components.

## Features

- Provides random integers and doubles
- Supports injection for testability
- Ensures consistent behavior across components

## Example Usage

```csharp
var randomProvider = new RandomProvider();
int randomInt = randomProvider.GetRandom(1, 10);
double randomDouble = randomProvider.GetDouble();
```

## Remarks

This provider is intentionally over-engineered to align with ProcrastiN8's philosophy of productive stalling.
