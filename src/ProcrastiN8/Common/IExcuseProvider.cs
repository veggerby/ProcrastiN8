namespace ProcrastiN8.Common;

/// <summary>
/// Provides excuses for procrastination, stalling, or general lack of progress.
/// </summary>
/// <remarks>
/// Implementations should generate plausible, elaborate, or existential excuses on demand.
/// </remarks>
public interface IExcuseProvider
{
    /// <summary>
    /// Asynchronously gets an excuse for the current delay or non-action.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a string with a plausible excuse.</returns>
    Task<string> GetExcuseAsync();
}