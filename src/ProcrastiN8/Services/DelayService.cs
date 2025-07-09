using System.Diagnostics;

using ProcrastiN8.Metrics;

namespace ProcrastiN8.Services;

public class DelayService
{
    public async Task DelayWithProcrastinationAsync(string reason, TimeSpan delay, CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();

        await Task.Delay(delay, ct);

        stopwatch.Stop();

        ProcrastinationMetrics.TotalTimeProcrastinated.Add(
            (long)stopwatch.Elapsed.TotalSeconds,
            KeyValuePair.Create<string, object?>("reason", reason));

        ProcrastinationMetrics.DelaysTotal.Add(1);
        ProcrastinationMetrics.SnoozeDurations.Record(stopwatch.Elapsed.TotalSeconds);
    }
}