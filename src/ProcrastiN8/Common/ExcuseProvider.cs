namespace ProcrastiN8.Common;

/// <summary>
/// Default implementation of <see cref="IExcuseProvider"/> that wraps <see cref="ExcuseGenerator"/>.
/// </summary>
/// <remarks>
/// This provider delegates excuse generation to <see cref="ExcuseGenerator"/>, ensuring congruence and testability.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="ExcuseProvider"/> class.
/// </remarks>
/// <param name="customExcuseFunc">An optional custom excuse generator function.</param>
public class ExcuseProvider(Func<string>? customExcuseFunc = null) : IExcuseProvider
{
    private readonly Func<string>? _customExcuseFunc = customExcuseFunc;

    /// <inheritdoc />
    public Task<string> GetExcuseAsync()
    {
        return Task.FromResult(_customExcuseFunc?.Invoke() ?? ExcuseGenerator.GetRandomExcuse());
    }
}