using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.Metrics;

namespace ProcrastiN8.Services;

public class ExcuseService(IRandomProvider? randomProvider = null)
{
    private static readonly string[] Categories = { "existential", "technical debt", "calendar", "weather" };
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;

    // Increment value for excuse metric
    private const int ExcuseIncrement = 1;

    public string GenerateExcuse(string category = "existential")
    {
        ProcrastinationMetrics.ExcusesGenerated.Add(ExcuseIncrement,
            KeyValuePair.Create<string, object?>("category", category));

        return ExcuseGenerator.GetRandomExcuse(_randomProvider);
    }
}