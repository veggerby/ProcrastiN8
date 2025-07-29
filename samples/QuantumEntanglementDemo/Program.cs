using System;
using System.Threading.Tasks;
using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

Console.WriteLine("Quantum Entanglement Demo Starting...");

// Create a collapse behavior
var behavior = CollapseBehaviorFactory.Create<int>(QuantumComplianceLevel.Copenhagen);

// Create promises
var promise1 = new QuantumPromise<int>(() => Task.FromResult(42), TimeSpan.FromSeconds(2));
var promise2 = new QuantumPromise<int>(() => Task.FromResult(99), TimeSpan.FromSeconds(2));

// Entangle promises
promise1.Entangle(behavior, promise2);

// Simulate some delay before observing
await Task.Delay(2500); // Simulate some delay before observing

// Observe promise1 directly
var result1 = await promise1.ObserveAsync();
Console.WriteLine($"Observed value of promise1: {result1}");

// Verify promise2 is also observed - with same value (Copenhagen interpretation)
var result2 = await promise2.ObserveAsync();
Console.WriteLine($"Observed value of promise2: {result2}");

Console.WriteLine("Quantum Entanglement Demo Completed.");