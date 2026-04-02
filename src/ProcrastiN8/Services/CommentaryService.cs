using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.Metrics;

namespace ProcrastiN8.Services;

/// <summary>
/// Generates and logs random remarks that acknowledge the procrastination without doing anything about it.
/// </summary>
public class CommentaryService(IRandomProvider? randomProvider = null) : ICommentaryService
{
    // Increment value for commentary metric
    private const int CommentaryIncrement = 1;
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;

    /// <inheritdoc />
    public virtual void LogRandomRemark(IProcrastiLogger? logger = null)
    {
        ProcrastinationMetrics.CommentaryTotal.Add(CommentaryIncrement);
        CommentaryGenerator.LogRandomCommentary(logger, randomProvider: _randomProvider);
    }
}