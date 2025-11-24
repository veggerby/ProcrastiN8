namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Defines a strategy for distorting deadline perception, enabling deadlines to move, shift, or paradoxically
/// change based on observation patterns.
/// </summary>
/// <remarks>
/// Implementations of this interface provide various mechanisms for manipulating deadline expectations,
/// including quantum observation effects, relativistic time dilation near critical dates, and spontaneous
/// deadline migration. All distortions are fully deterministic in hindsight.
/// </remarks>
public interface IDeadlineDistortionStrategy
{
    /// <summary>
    /// Applies distortion to the given deadline based on the current observation context.
    /// </summary>
    /// <param name="originalDeadline">The original, undistorted deadline.</param>
    /// <param name="currentTime">The current time from which distortion is calculated.</param>
    /// <param name="observationCount">The number of times the deadline has been observed (may influence distortion).</param>
    /// <returns>The distorted deadline, which may differ significantly from the original.</returns>
    /// <remarks>
    /// Deadlines that move when observed are a natural consequence of procrastination physics.
    /// Multiple observations may cause the deadline to drift further or collapse to a fixed point.
    /// </remarks>
    DateTimeOffset DistortDeadline(DateTimeOffset originalDeadline, DateTimeOffset currentTime, int observationCount);

    /// <summary>
    /// Determines whether the distortion strategy has reached a paradoxical state.
    /// </summary>
    /// <param name="originalDeadline">The original deadline.</param>
    /// <param name="distortedDeadline">The deadline after distortion.</param>
    /// <returns><c>true</c> if a paradox has been detected; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// A paradox occurs when distorted deadlines violate causality, such as a deadline moving
    /// to a point before the task was created, or oscillating indefinitely with each observation.
    /// </remarks>
    bool IsParadoxical(DateTimeOffset originalDeadline, DateTimeOffset distortedDeadline);
}
