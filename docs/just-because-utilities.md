---
title: "Just Because Utilities"
sidebar_position: 11
---

> For when you need to simulate chaos, futility, or strict YAGNI compliance.

The **ProcrastiN8.JustBecause** namespace provides tools for generating random exceptions, chaining pointless operations, and enforcing the principle of "You Aren't Gonna Need It" with the utmost seriousness.

## Components

### `RandomExceptionGenerator`

Throws a randomly selected exception from a configurable set. Useful for chaos engineering, testing, or existential exercises.

````csharp
var random = Substitute.For<IRandomProvider>();
random.Next(Arg.Any<int>()).Returns(2);
var generator = new RandomExceptionGenerator(random);
generator.ThrowRandom(); // Throws ArgumentException
````

### `PointlessChain`

Simulates endless, non-productive asynchronous work.

````csharp
var chain = new PointlessChain(delayStrategy, logger);
await chain.StartAsync(steps: 3, cancellationToken);
````

### `YAGNIValidator`

Throws a `NotSupportedException` if a feature is even considered.

````csharp
YAGNIValidator.Validate("DarkMode"); // Throws NotSupportedException
````

## API Reference

See XML docs in source for full details.
