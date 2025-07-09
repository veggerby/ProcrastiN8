using ProcrastiN8.Common;
using ProcrastiN8.Metrics;

namespace ProcrastiN8.Services;

public class CommentaryService
{
    public void LogRandomRemark(IProcrastiLogger? logger = null)
    {
        ProcrastinationMetrics.CommentaryTotal.Add(1);
        CommentaryGenerator.LogRandomCommentary(logger);
    }
}