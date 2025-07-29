# QuantumEntanglementRegistry

> Manages a non-local, non-consensual registry of entangled quantum promises. Collapsing one may trigger collapse or decay in others, with no regard for fairness or quantum ethics.

## Overview

`QuantumEntanglementRegistry<T>` coordinates the entanglement and collapse of multiple `QuantumPromise<T>` instances. It delegates collapse logic to a pluggable behavior, simulating quantum metaphors with enterprise-level seriousness.

## Usage

```csharp
using ProcrastiN8.JustBecause;

var registry = new QuantumEntanglementRegistry<int>();
var promise1 = new QuantumPromise<int>(() => Task.FromResult(42), TimeSpan.FromSeconds(2));
var promise2 = new QuantumPromise<int>(() => Task.FromResult(99), TimeSpan.FromSeconds(2));
registry.Entangle(promise1);
registry.Entangle(promise2);

// Collapse one (and ripple to others, depending on behavior)
var result = await registry.CollapseOneAsync();
Console.WriteLine($"Collapsed value: {result}");
```

### Expected Output

```txt
[QuantumEntanglement] Entangled promise registered.
[QuantumEntanglement] Entangled promise registered.
[QuantumEntanglement] Collapsing one entangled promise...
[QuantumEntanglement] Collapse behavior: Copenhagen (or other)
[QuantumEntanglement] Collapsed value: 42
```

## API

### `Entangle`

```csharp
public void Entangle(QuantumPromise<T> quantum)
```

- **quantum**: The quantum promise to entangle.

### `CollapseOneAsync`

```csharp
public Task<T?> CollapseOneAsync(CancellationToken cancellationToken = default)
```

- Collapses one entangled promise and ripples to others according to the configured behavior.

## Available Behaviors

The `QuantumEntanglementRegistry<T>` supports multiple collapse behaviors, each simulating a different interpretation of quantum mechanics. These behaviors can be selected based on the `QuantumComplianceLevel` provided to the `CollapseBehaviorFactory`.

### Behavior List

1. **CopenhagenCollapseBehavior**
   - **Description:** Implements the Copenhagen interpretation. Observation collapses all entangled promises, but only the chosen one determines the returned value. All other promises are forcibly collapsed to the same value.
   - **Use Case:** When you want deterministic results after observation.

2. **ForkingCollapseBehavior**
   - **Description:** Simulates the Many-Worlds Interpretation. Collapses one promise and forks multiple parallel universes (background tasks) that each recurse independently.
   - **Use Case:** Ideal for burning CPU cycles and confusing stakeholders.

3. **EnterpriseQuantumCollapseBehavior**
   - **Description:** Adds enterprise-level metrics and decorators. Collapses promises while logging deprecation notices and reporting to OKR platforms.
   - **Use Case:** When metrics and optics are more important than results.

4. **RandomUnfairCollapseBehavior**
   - **Description:** Observes one promise randomly and ripples the collapse to others unfairly.
   - **Use Case:** For simulating chaotic and unpredictable outcomes.

5. **SilentFailureCollapseBehavior**
   - **Description:** Simulates cooperation without performing any actual observation. Collapses nothing and returns default values while recording metrics for optics.
   - **Use Case:** When failure is expected but should appear cooperative.

6. **SpookyActionCollapseBehavior**
   - **Description:** Attempts to collapse all entangled promises at once, instantly, and non-locally, simulating "spooky action at a distance."
   - **Use Case:** For scenarios requiring simultaneous and uncorrelated outcomes.

### How to Use Behaviors

To use a specific behavior, create it using the `CollapseBehaviorFactory` and pass it to the `QuantumEntanglementRegistry<T>`.

```csharp
using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

var behavior = CollapseBehaviorFactory.Create<int>(QuantumComplianceLevel.Copenhagen);
var registry = new QuantumEntanglementRegistry<int>(behavior);

var promise1 = new QuantumPromise<int>(() => Task.FromResult(42), TimeSpan.FromSeconds(2));
var promise2 = new QuantumPromise<int>(() => Task.FromResult(99), TimeSpan.FromSeconds(2));
registry.Entangle(promise1);
registry.Entangle(promise2);

// Observe promise1 directly
var result1 = await promise1.ObserveAsync();
Console.WriteLine($"Observed value of promise1: {result1}");

// Verify promise2 is also observed
var result2 = await promise2.ObserveAsync();
Console.WriteLine($"Observed value of promise2: {result2}");
```

## Remarks

- Collapse behaviors are pluggable and dictate the quantum metaphor (Copenhagen, ManyWorlds, etc).
- The registry is the only authority for coordinated collapse.
- Designed for maximum auditability and minimum determinism.
