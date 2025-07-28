---
title: "TODO Framework"
sidebar_position: 10
---

> Because some things are best left unfinished.

The **ProcrastiN8.TODOFramework** provides a suite of tools for marking, scheduling, and never actually completing work. This includes the `TodoAttribute`, `TodoScheduler`, and `NeverDoExecutor`—all designed to ensure that your backlog remains as aspirational as possible.

## Components

### `TodoAttribute`

A straight-faced attribute for marking code elements as perpetually incomplete.

````csharp
[Todo("Waiting for requirements.")]
public void ImplementFeature() { /* ... */ }
````

### `TodoScheduler`

Discovers and schedules all methods marked with `TodoAttribute`—but never actually executes them. Instead, it logs plausible excuses and defers action indefinitely.

````csharp
var scheduler = new TodoScheduler(logger, excuseProvider, delayStrategy);
await scheduler.ScheduleAllAsync(typeof(MyClass));
````

### `NeverDoExecutor`

A utility for ensuring that certain actions are never performed, regardless of urgency.

````csharp
var neverDo = new NeverDoExecutor(logger, excuseProvider, delayStrategy);
await neverDo.NeverAsync(() => Task.Run(() => { /* ... */ }), "Not today.");
````

## Use Cases

- Marking code for eternal postponement
- Generating audit trails of inaction
- Satisfying compliance requirements for TODOs

## API Reference

See XML docs in source for full details.
