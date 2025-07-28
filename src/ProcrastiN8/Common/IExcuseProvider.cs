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
    /// Gets an excuse for the current delay or non-action.
    /// </summary>
    /// <returns>A string containing a plausible excuse.</returns>
    string GetExcuse();
}