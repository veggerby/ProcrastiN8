namespace ProcrastiN8.JustBecause;

/// <summary>
/// Generates random exceptions for scenarios where failure is not only possible, but expected.
/// </summary>
/// <remarks>
/// The <see cref="RandomExceptionGenerator"/> is intended for use in testing, chaos engineering, and existential exercises.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="RandomExceptionGenerator"/> class.
/// </remarks>
/// <param name="randomProvider">An injectable source of randomness.</param>
/// <param name="exceptionFactories">A list of exception factories to choose from. If null, a default set is used.</param>
public class RandomExceptionGenerator(IRandomProvider randomProvider, IEnumerable<Func<Exception>>? exceptionFactories = null)
{
    private readonly IRandomProvider _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
    private readonly IReadOnlyList<Func<Exception>> _exceptionFactories = (exceptionFactories?.ToList() ?? GetDefaultFactories()).AsReadOnly();

    /// <summary>
    /// Throws a randomly selected exception from the available factories.
    /// </summary>
    public void ThrowRandom()
    {
        var index = _randomProvider.Next(_exceptionFactories.Count);
        throw _exceptionFactories[index]();
    }

    private static List<Func<Exception>> GetDefaultFactories() => new()
    {
        () => new InvalidOperationException("Operation was invalid, but not unexpected."),
        () => new NotSupportedException("This feature is not supported, and never will be."),
        () => new ArgumentException("The argument was fine, but the method wasn't."),
        () => new ApplicationException("A generic application error occurred for no particular reason."),
        () => new TimeoutException("Operation timed out while waiting for motivation.")
    };
}
