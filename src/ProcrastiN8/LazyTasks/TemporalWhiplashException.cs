namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Thrown when rapid or unexpected changes in temporal flow cause disorientation or instability.
/// </summary>
/// <remarks>
/// Temporal whiplash occurs when time accelerates, decelerates, or reverses too quickly for
/// stable operation. Common causes include Mercury retrograde conditions, relativistic effects
/// near deadlines, or excessive timeline branching. Side effects may include confusion, missed
/// deadlines, and an unsettling sense that it's always 2019.
/// </remarks>
public class TemporalWhiplashException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalWhiplashException"/> class.
    /// </summary>
    public TemporalWhiplashException()
        : base("Temporal whiplash detected. Time changed too quickly, causing disorientation.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalWhiplashException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the temporal whiplash event.</param>
    public TemporalWhiplashException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalWhiplashException"/> class with a specified error message
    /// and a reference to the inner exception that caused this whiplash.
    /// </summary>
    /// <param name="message">The message that describes the temporal whiplash event.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public TemporalWhiplashException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
