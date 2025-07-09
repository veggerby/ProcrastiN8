using System.Diagnostics;

using ProcrastiN8.Metrics;
using ProcrastiN8.Services;

namespace ProcrastiN8.Unproductivity;

/// <summary>
/// Simulates doing heavy work by consuming CPU in a loop without yielding. Looks busy, achieves nothing.
/// </summary>
public static class BusyWaitSimulator
{
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.Unproductivity.BusyWaitSimulator");

    private static readonly CommentaryService CommentaryService = new();

    /// <summary>
    /// Runs a CPU-bound busy-wait loop for the given duration or until cancelled.
    /// </summary>
    public static void BurnCpuCycles(
        TimeSpan duration,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        logger ??= new DefaultLogger();

        using var activity = ActivitySource.StartActivity("BusyWaitSimulator.BurnCpuCycles", ActivityKind.Internal);
        activity?.SetTag("busywait.duration.ms", duration.TotalMilliseconds);

        logger.Info("[BusyWaitSimulator] Engaging in intense CPU-based non-productivity for {Seconds:0.0}s...", duration.TotalSeconds);

        var stopwatch = Stopwatch.StartNew();
        var spinStart = Stopwatch.GetTimestamp();
        long durationTicks = (long)(duration.TotalSeconds * Stopwatch.Frequency);

        try
        {
            while (!cancellationToken.IsCancellationRequested &&
                   Stopwatch.GetTimestamp() - spinStart < durationTicks)
            {
                // Periodically log excuses and commentary
                if (stopwatch.ElapsedMilliseconds % 1500 < 10)
                {
                    CommentaryService.LogRandomRemark();

                    ProcrastinationMetrics.CommentaryTotal.Add(1,
                        KeyValuePair.Create<string, object?>("source", "BusyWaitSimulator"));

                    ProcrastinationMetrics.ExcusesGenerated.Add(1,
                        KeyValuePair.Create<string, object?>("category", "cpu-bound-delay"));
                }

                // Waste CPU – do nothing in a tight loop
                Math.Sqrt(new Random().NextDouble() * 9999); // token calculation to avoid optimizations
            }

            stopwatch.Stop();
            ProcrastinationMetrics.TotalTimeProcrastinated.Add((long)stopwatch.Elapsed.TotalSeconds,
                KeyValuePair.Create<string, object?>("component", "BusyWaitSimulator"));

            logger.Info("[BusyWaitSimulator] Finished CPU burn. Heat generated: emotional.");
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "[BusyWaitSimulator] Burnout encountered.");
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}