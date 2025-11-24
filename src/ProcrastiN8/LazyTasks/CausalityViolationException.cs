namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Thrown when an operation violates the laws of causality, such as observing an effect before its cause.
/// </summary>
/// <remarks>
/// This exception indicates that the temporal order of events has been compromised, typically due to
/// aggressive time manipulation or deadline distortion strategies. While causality violations are
/// theoretically impossible, they remain a frequent occurrence in procrastination workflows.
/// </remarks>
public class CausalityViolationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CausalityViolationException"/> class.
    /// </summary>
    public CausalityViolationException()
        : base("Causality has been violated. The effect preceded the cause, which is awkward.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CausalityViolationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the causality violation.</param>
    public CausalityViolationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CausalityViolationException"/> class with a specified error message
    /// and a reference to the inner exception that caused this violation.
    /// </summary>
    /// <param name="message">The message that describes the causality violation.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CausalityViolationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
