using ProcrastiN8.Common;
using ProcrastiN8.Metrics;

namespace ProcrastiN8.Services;

public class ExcuseService
{
    private static readonly string[] Categories = { "existential", "technical debt", "calendar", "weather" };
    // Increment value for excuse metric
    private const int ExcuseIncrement = 1;

    public static void SetRandomProvider(ProcrastiN8.JustBecause.IRandomProvider provider)
    {
        ExcuseGenerator.SetRandomProvider(provider);
    }

    public string GenerateExcuse(string category = "existential")
    {
        ProcrastinationMetrics.ExcusesGenerated.Add(ExcuseIncrement,
            KeyValuePair.Create<string, object?>("category", category));

        return ExcuseGenerator.GetRandomExcuse();
    }
}