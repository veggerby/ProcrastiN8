using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ProcrastiN8.Services.Diagnostics;

/// <summary>Diagnostic & metrics surface for procrastination events.</summary>
public static class ProcrastinationDiagnostics
{
    public const string ActivitySourceName = "ProcrastiN8.Procrastination";
    public const string MeterName = "ProcrastiN8.Procrastination";
    private static readonly ActivitySource _activitySource = new(ActivitySourceName);
    private static readonly Meter _meter = new(MeterName, "1.0.0");
    private static readonly Counter<int> _cyclesCounter = _meter.CreateCounter<int>("procrastination.cycles");
    private static readonly Counter<int> _excusesCounter = _meter.CreateCounter<int>("procrastination.excuses");
    private static readonly Counter<int> _executionsCounter = _meter.CreateCounter<int>("procrastination.executions");
    private static readonly Counter<int> _abandonedCounter = _meter.CreateCounter<int>("procrastination.abandoned");
    private static readonly Counter<int> _triggeredCounter = _meter.CreateCounter<int>("procrastination.triggered");

    public static Activity? StartActivity(string name, ProcrastinationResult? result = null)
    {
        var act = _activitySource.StartActivity(name, ActivityKind.Internal);
        if (act != null && result != null)
        {
            act.SetTag("procrastination.mode", result.Mode.ToString());
            act.SetTag("procrastination.executed", result.Executed);
            act.SetTag("procrastination.triggered", result.Triggered);
            act.SetTag("procrastination.abandoned", result.Abandoned);
            act.SetTag("procrastination.cycles", result.Cycles);
            act.SetTag("procrastination.excuses", result.ExcuseCount);
        }
        return act;
    }

    public static void RecordEvent(ProcrastinationObserverEvent evt)
    {
        switch (evt.EventType)
        {
            case "cycle": _cyclesCounter.Add(1); break;
            case "excuse": _excusesCounter.Add(1); break;
            case "executed": _executionsCounter.Add(1); break;
            case "abandoned": _abandonedCounter.Add(1); break;
            case "triggered": _triggeredCounter.Add(1); break;
        }
    }
}