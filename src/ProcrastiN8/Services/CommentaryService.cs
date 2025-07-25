using ProcrastiN8.Common;
using ProcrastiN8.Metrics;

namespace ProcrastiN8.Services;

public class CommentaryService
{
    // Increment value for commentary metric
    private const int CommentaryIncrement = 1;

    public virtual void LogRandomRemark(IProcrastiLogger? logger = null)
    {
        ProcrastinationMetrics.CommentaryTotal.Add(CommentaryIncrement);
        CommentaryGenerator.LogRandomCommentary(logger);
    }
}