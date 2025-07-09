using ProcrastiN8.Common;
using ProcrastiN8.Metrics;

namespace ProcrastiN8.Services;

public class ExcuseService
{
    private static readonly string[] Categories = { "existential", "technical debt", "calendar", "weather" };

    public string GenerateExcuse(string category = "existential")
    {
        ProcrastinationMetrics.ExcusesGenerated.Add(1,
            KeyValuePair.Create<string, object?>("category", category));

        return ExcuseGenerator.GetRandomExcuse();
    }
}