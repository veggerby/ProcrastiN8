# Temporal Gaslighting & Time Travel Module

## Overview

The Temporal Gaslighting & Time Travel Module introduces sophisticated mechanisms for manipulating the perception and flow of time within ProcrastiN8. This module enables deadlines to move when observed, time to slow near critical dates, and tasks to exist in alternate timelines where they are perpetually "complete."

## Features

### Time Providers

#### `RelativisticTimeProvider`

Simulates relativistic time dilation effects near deadlines. As the current time approaches a configured deadline, time appears to slow down exponentially, creating the illusion of more available time.

**Usage:**

```csharp
var deadline = DateTimeOffset.UtcNow.AddHours(2);
var provider = new RelativisticTimeProvider(
    deadline: deadline,
    dilationFactor: 2.0  // Higher values = more extreme slowdown
);

var perceivedTime = provider.GetUtcNow();
// perceivedTime will be earlier than real time, and increasingly so as deadline approaches
```

**Key Properties:**
- `dilationFactor`: Controls intensity of time dilation (0.5 - 5.0 typical range)
- Never quite reaches the deadline, even when past it in real time
- Includes quantum jitter for observer effects

#### `NostalgicTimeProvider`

Permanently sets the current year to a configured nostalgic year (defaults to 2019). Time progresses normally within the year but loops back to January 1st upon exceeding December 31st.

**Usage:**

```csharp
var provider = new NostalgicTimeProvider(2019);

var time = provider.GetUtcNow();
// time.Year will always be 2019

// Check if time has looped at least once
bool hasLooped = provider.HasLooped();
```

**Key Features:**
- Default nostalgic year: 2019 (the last good year)
- Time loops within the year indefinitely
- Maintains temporal consistency with elapsed time

#### `MercuryRetrogradeProvider`

Randomly rewinds time based on configurable probability, simulating Mercury retrograde chaos. Multiple consecutive rewinds trigger `TemporalWhiplashException`.

**Usage:**

```csharp
var provider = new MercuryRetrogradeProvider(
    retrogradeProbability: 0.3,  // 30% chance of rewind per observation
    maxRewindDuration: TimeSpan.FromHours(1)
);

try
{
    var time = provider.GetUtcNow();
    // May be earlier than previous observation!
}
catch (TemporalWhiplashException ex)
{
    // Too many consecutive rewinds
    provider.ResetRetrogradeState();
}
```

**Key Features:**
- Configurable retrograde probability (0.0 - 1.0)
- Safety mechanism triggers `TemporalWhiplashException` after 5+ consecutive rewinds
- `ResetRetrogradeState()` method for recovery

### Deadline Distortion

#### `IDeadlineDistortionStrategy`

Interface for distorting deadline perception. Deadlines can move, shift, or paradoxically change based on observation patterns.

```csharp
public interface IDeadlineDistortionStrategy
{
    DateTimeOffset DistortDeadline(DateTimeOffset originalDeadline, DateTimeOffset currentTime, int observationCount);
    bool IsParadoxical(DateTimeOffset originalDeadline, DateTimeOffset distortedDeadline);
}
```

#### `QuantumObserverDeadlineStrategy`

Implements the quantum observer effect for deadlines: each observation pushes the deadline further into the future. After a paradox threshold, the deadline collapses back to its original position.

**Usage:**

```csharp
var strategy = new QuantumObserverDeadlineStrategy(
    shiftPerObservation: TimeSpan.FromHours(1),
    paradoxThreshold: 10  // Collapse after 10 observations
);

var originalDeadline = DateTimeOffset.UtcNow.AddDays(7);
var distorted = strategy.DistortDeadline(originalDeadline, DateTimeOffset.UtcNow, observationCount: 3);

// distorted will be approximately 3 hours after originalDeadline

if (strategy.IsParadoxical(originalDeadline, distorted))
{
    // Handle paradox
}
```

### Time Flux

The `TimeFlux` struct represents temporal distortion intensity and direction.

**Usage:**

```csharp
var flux = new TimeFlux(magnitude: 0.5, direction: TimeFluxDirection.Forward);

// Apply flux to a duration
var actualDuration = TimeSpan.FromHours(2);
var perceivedDuration = flux.Apply(actualDuration);
// perceivedDuration = 1 hour (half of actual)

// Check for causality violations
if (flux.IsParadoxical())
{
    // Handle backward time or negative magnitude
}
```

**Special Values:**
- `TimeFlux.Normal`: No distortion (magnitude = 1.0, forward)
- `TimeFlux.Frozen`: Complete temporal stasis (magnitude = 0.0)

### Timeline Branching

Timeline branches enable quantum promises to exist and be observed in alternate realities without affecting the main timeline.

**Usage:**

```csharp
var promise = new QuantumPromise<string>(() => Task.FromResult("done"), TimeSpan.FromSeconds(5));

// Observe in an alternate timeline
var result = await promise.ObserveInBranchAsync("alternate-reality-1");

// Check if observed in specific branch
bool isObserved = promise.IsObservedInBranch("alternate-reality-1");

// Get or create a timeline branch
var branch = TimelineBranchExtensions.GetOrCreateBranch("reality-2");
var divergenceIndex = branch.GetDivergenceIndex();
bool isParadoxical = branch.IsParadoxical();
```

**Key Features:**
- Multiple promises can be observed in the same branch
- Divergence index tracks distance from main timeline
- Paradox detection for excessive divergence

### Temporal Exceptions

#### `CausalityViolationException`

Thrown when an operation violates the laws of causality (e.g., observing an effect before its cause).

#### `TemporalWhiplashException`

Thrown when rapid or unexpected changes in temporal flow cause disorientation or instability. Common with Mercury retrograde.

**Example:**

```csharp
try
{
    var retrograde = new MercuryRetrogradeProvider(retrogradeProbability: 0.8);
    for (int i = 0; i < 10; i++)
    {
        var time = retrograde.GetUtcNow();
    }
}
catch (TemporalWhiplashException ex)
{
    Console.WriteLine($"Temporal whiplash: {ex.Message}");
}
```

### Metrics & Telemetry

The `TemporalMetrics` class exposes OpenTelemetry metrics for monitoring temporal distortions:

- `temporal.paradoxes`: Counter of detected causality violations
- `temporal.timeline_divergences`: Counter of timeline branch/divergence events
- `temporal.divergence_index`: Gauge showing current distance from baseline timeline
- `temporal.retrograde_rewinds`: Counter of Mercury retrograde time rewind events
- `temporal.whiplash_events`: Counter of temporal whiplash exceptions
- `temporal.dilation_magnitude`: Histogram of relativistic time dilation multipliers
- `temporal.deadline_distortions`: Counter of deadline distortion applications
- `temporal.deadline_shift`: Histogram of deadline shift magnitudes in hours

**Usage:**

```csharp
using ProcrastiN8.Metrics;

// Metrics are automatically recorded by temporal providers
var provider = new RelativisticTimeProvider(deadline);
provider.GetUtcNow();  // Automatically records dilation metrics

// Manually update divergence index
TemporalMetrics.UpdateDivergenceIndex(42.0);
```

## Integration with Existing Features

### QuantumPromise Integration

Quantum promises can leverage temporal providers for time-dependent behavior:

```csharp
var nostalgicTime = new NostalgicTimeProvider(2019);
var promise = new QuantumPromise<int>(
    () => Task.FromResult(42),
    schrodingerWindow: TimeSpan.FromSeconds(10),
    timeProvider: nostalgicTime
);

// Promise operates in 2019
var result = await promise.ObserveAsync();
```

### LazyTasks Integration

Temporal providers can be injected into delay strategies and other lazy task components:

```csharp
var relativistic = new RelativisticTimeProvider(deadline);
var strategy = new DefaultDelayStrategy(timeProvider: relativistic);

// Delays are calculated using relativistic time
await strategy.DelayAsync();
```

## Best Practices

1. **Choose the Right Provider:**
   - Use `RelativisticTimeProvider` for deadline-based procrastination
   - Use `NostalgicTimeProvider` for temporal denial
   - Use `MercuryRetrogradeProvider` for maximum chaos

2. **Handle Temporal Exceptions:**
   - Always catch `TemporalWhiplashException` when using `MercuryRetrogradeProvider`
   - Consider `CausalityViolationException` when time flows backward

3. **Monitor Metrics:**
   - Track paradox counts to detect excessive temporal manipulation
   - Monitor timeline divergence index for stability

4. **Test with Doubles:**
   - Use substitutes for `ITimeProvider` in tests
   - Mock `IRandomProvider` for deterministic temporal behavior

## Examples

### Procrastinating Until the End of Time

```csharp
var deadline = DateTimeOffset.UtcNow.AddDays(1);
var relativistic = new RelativisticTimeProvider(deadline, dilationFactor: 10.0);

// Time slows dramatically near the deadline
while (true)
{
    var perceivedTime = relativistic.GetUtcNow();
    if (perceivedTime >= deadline)
    {
        break;  // This will never happen
    }
    await Task.Delay(1000);
}
```

### Living in the Past

```csharp
var nostalgic = new NostalgicTimeProvider(2015);

Console.WriteLine($"Current year: {nostalgic.GetUtcNow().Year}");  // Always 2015
Console.WriteLine("Everything was simpler back then...");
```

### Deadline Quantum Superposition

```csharp
var strategy = new QuantumObserverDeadlineStrategy();
var deadline = DateTimeOffset.UtcNow.AddDays(7);

for (int i = 0; i < 15; i++)
{
    var distorted = strategy.DistortDeadline(deadline, DateTimeOffset.UtcNow, i);
    Console.WriteLine($"Observation {i}: Deadline is {distorted}");
    
    if (i >= 10)
    {
        Console.WriteLine("Paradox threshold exceeded - deadline collapsed!");
    }
}
```

### Alternate Timeline Completion

```csharp
var task = new QuantumPromise<string>(
    () => Task.FromResult("Actually done"),
    TimeSpan.FromSeconds(5)
);

// Complete in alternate reality
await task.ObserveInBranchAsync("parallel-universe");

// Still incomplete in main timeline
Console.WriteLine("Task is done... in another timeline");
```

## Why It Fits

Procrastination is fundamentally about manipulating one's perception of time. The Temporal Gaslighting & Time Travel Module makes this manipulation explicit, systematic, and measurable. By providing tools to slow time near deadlines, live in nostalgic past, introduce temporal chaos, and complete work in alternate timelines, ProcrastiN8 achieves its core mission: making procrastination easier when time itself is unreliable.

## See Also

- [ITimeProvider Documentation](./just-because-utilities.md)
- [Quantum Entanglement](./quantum-entanglement.md)
- [LazyTasks](./retry-until-cancelled.md)
