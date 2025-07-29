using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.Metrics;

namespace ProcrastiN8.Services;

public class CommentaryService(IRandomProvider? randomProvider = null)
{
    // Increment value for commentary metric
    private const int CommentaryIncrement = 1;
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;

    public virtual void LogRandomRemark(IProcrastiLogger? logger = null)
    {
        ProcrastinationMetrics.CommentaryTotal.Add(CommentaryIncrement);
        CommentaryGenerator.LogRandomCommentary(logger, randomProvider: _randomProvider);
    }
}