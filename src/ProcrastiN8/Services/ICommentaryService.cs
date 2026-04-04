namespace ProcrastiN8.Services;

/// <summary>
/// Provides ceremonial commentary to underscore the gravity of doing nothing.
/// </summary>
public interface ICommentaryService
{
    /// <summary>
    /// Emits a randomly selected remark acknowledging, but not addressing, the situation at hand.
    /// </summary>
    /// <param name="logger">Optional logger to receive the remark. If omitted, the remark echoes into the void.</param>
    void LogRandomRemark(IProcrastiLogger? logger = null);
}
