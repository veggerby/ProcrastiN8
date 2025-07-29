using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// Provides extension methods for entangling quantum promises with others.
/// </summary>
/// <remarks>
/// These methods allow for customizable entanglement behaviors, ensuring maximum procrastination potential.
/// </remarks>
public static class QuantumExtensions
{
    /// <summary>
    /// Entangles a quantum promise with others using a specified collapse behavior.
    /// </summary>
    /// <typeparam name="T">The type of the quantum promise.</typeparam>
    /// <param name="promise">The primary quantum promise.</param>
    /// <param name="behavior">The collapse behavior to use.</param>
    /// <param name="others">The other promises to entangle with.</param>
    public static void Entangle<T>(this QuantumPromise<T> promise, ICollapseBehavior<T> behavior, params QuantumPromise<T>[] others)
    {
        promise.Entangle(behavior, null, others);
    }

    /// <summary>
    /// Entangles a quantum promise with others using a specified quantum compliance level.
    /// </summary>
    /// <typeparam name="T">The type of the quantum promise.</typeparam>
    /// <param name="promise">The primary quantum promise.</param>
    /// <param name="complianceLevel">The quantum compliance level to use.</param>
    /// <param name="others">The other promises to entangle with.</param>
    public static void Entangle<T>(this QuantumPromise<T> promise, QuantumComplianceLevel complianceLevel, params QuantumPromise<T>[] others)
    {
        var behavior = CollapseBehaviorFactory.Create<T>(complianceLevel);
        promise.Entangle(behavior, null, others);
    }

    /// <summary>
    /// Entangles a quantum promise with others using a specified random provider.
    /// </summary>
    /// <typeparam name="T">The type of the quantum promise.</typeparam>
    /// <param name="promise">The primary quantum promise.</param>
    /// <param name="randomProvider">The random provider to use.</param>
    /// <param name="others">The other promises to entangle with.</param>
    public static void Entangle<T>(this QuantumPromise<T> promise, IRandomProvider randomProvider, params QuantumPromise<T>[] others)
    {
        promise.Entangle(null, randomProvider, others);
    }

    /// <summary>
    /// Entangles a quantum promise with others using a specified collapse behavior and random provider.
    /// </summary>
    /// <typeparam name="T">The type of the quantum promise.</typeparam>
    /// <param name="promise">The primary quantum promise.</param>
    /// <param name="complianceLevel">The quantum compliance level to use.</param>
    /// <param name="randomProvider">The random provider to use.</param>
    /// <param name="others">The other promises to entangle with.</param>
    public static void Entangle<T>(this QuantumPromise<T> promise, QuantumComplianceLevel complianceLevel, IRandomProvider randomProvider, params QuantumPromise<T>[] others)
    {
        var behavior = CollapseBehaviorFactory.Create<T>(complianceLevel);
        promise.Entangle(behavior, randomProvider, others);
    }
}