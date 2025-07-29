# QuantumPromise Entanglement

> Simulates quantum entanglement by allowing `QuantumPromise` instances to be linked, such that collapsing one may influence others.

## Overview

`QuantumPromise<T>` supports the concept of entanglement, where multiple promises can be linked together. When one promise is observed or collapsed, the behavior of the others is influenced according to the configured collapse behavior. This feature is designed to simulate quantum metaphors with enterprise-level seriousness.

## Usage

```csharp
using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

var promise1 = new QuantumPromise<int>(() => Task.FromResult(42), TimeSpan.FromSeconds(2));
var promise2 = new QuantumPromise<int>(() => Task.FromResult(99), TimeSpan.FromSeconds(2));

// Entangle promises with a specific behavior
promise1.Entangle(new RandomUnfairCollapseBehavior<int>(), promise2);

// Observe one promise
var result1 = await promise1.ObserveAsync();
Console.WriteLine($"Observed value of promise1: {result1}");

// Verify the other promise is also influenced
var result2 = await promise2.ObserveAsync();
Console.WriteLine($"Observed value of promise2: {result2}");
```

### Expected Output

```txt
[QuantumPromise] Entangled with another promise.
[QuantumPromise] Observing promise...
[QuantumPromise] Collapse behavior: RandomUnfairCollapse
[QuantumPromise] Observed value: 42
[QuantumPromise] Observing promise...
[QuantumPromise] Observed value: 42
```

## API

### `Entangle`

```csharp
public void Entangle(ICollapseBehavior<T> behavior, params QuantumPromise<T>[] others)
```

- **behavior**: The collapse behavior to use for the entanglement.
- **others**: The other promises to entangle with this promise.

### `ObserveAsync`

```csharp
public Task<T> ObserveAsync(CancellationToken cancellationToken = default)
```

- Observes the promise, collapsing it to a value or triggering a failure based on the configured behavior.

## Available Behaviors

The `QuantumPromise<T>` supports multiple collapse behaviors, each simulating a different interpretation of quantum mechanics. These behaviors can be selected based on the `CollapseBehaviorFactory` or created manually.

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

To use a specific behavior, create it using the `CollapseBehaviorFactory` and pass it to the `Entangle` method of a `QuantumPromise`.

```csharp
using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

var behavior = CollapseBehaviorFactory.Create<int>(QuantumComplianceLevel.Copenhagen);

var promise1 = new QuantumPromise<int>(() => Task.FromResult(42), TimeSpan.FromSeconds(2));
var promise2 = new QuantumPromise<int>(() => Task.FromResult(99), TimeSpan.FromSeconds(2));
promise1.Entangle(behavior, promise2);

// Observe promise1 directly
var result1 = await promise1.ObserveAsync();
Console.WriteLine($"Observed value of promise1: {result1}");

// Verify promise2 is also observed
var result2 = await promise2.ObserveAsync();
Console.WriteLine($"Observed value of promise2: {result2}");
```

## Remarks

- Collapse behaviors are pluggable and dictate the quantum metaphor (Copenhagen, ManyWorlds, etc).
- Entanglement is managed directly by the `QuantumPromise` instances.
- Designed for maximum auditability and minimum determinism.
