using System.Diagnostics;

using ProcrastiN8.Metrics;
using ProcrastiN8.Services;

namespace ProcrastiN8.Unproductivity;

/// <summary>
/// Simulates endless, misleading percentage-based progress that eventually completes (if you wait long enough).
/// </summary>
public static class FakeIndeterminateProgress
{
    // Activity source for tracing fake progress
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.Unproductivity.FakeIndeterminateProgress");

    // Random number generator for progress simulation
    private static readonly Random Rng = new();
    // Service for generating excuses (used in progress commentary)
    private static readonly ExcuseService ExcuseService = new();
    // Service for logging random commentary
    private static CommentaryService CommentaryService = new();

    // Default update interval for fake indeterminate progress (ms)
    private const int DefaultUpdateIntervalMs = 800;
    // Minimum minutes before fake progress can complete
    private const int MinCompletionMinutes = 4;
    // Maximum minutes before fake progress can complete
    private const int MaxCompletionMinutes = 15;

    /// <summary>
    /// Begins simulating misleading progress with regressions, stalling, and eventual surprise completion.
    /// </summary>
    public static async Task ShowAsync(
        IProcrastiLogger? logger = null,
        TimeSpan? updateInterval = null,
        TimeSpan? minTimeBeforeCompletion = null,
        Action<double>? reportProgress = null,
        CancellationToken cancellationToken = default)
    {
        logger ??= new DefaultLogger();
        updateInterval ??= TimeSpan.FromMilliseconds(DefaultUpdateIntervalMs);
        minTimeBeforeCompletion ??= TimeSpan.FromMinutes(Rng.Next(MinCompletionMinutes, MaxCompletionMinutes + 1)); // 4–15 minutes

        using var activity = ActivitySource.StartActivity("ProcrastiN8.FakeIndeterminateProgress.Show", ActivityKind.Internal);
        activity?.SetTag("progress.style", "infinite-windows-style");
        activity?.SetTag("progress.updateInterval.ms", updateInterval.Value.TotalMilliseconds);
        activity?.SetTag("progress.minCompletionTime.ms", minTimeBeforeCompletion.Value.TotalMilliseconds);

        logger.Info("[FakeProgress] Launching misleading progress loop with eventual closure.");
        logger.Info("[FakeProgress] Completion will not occur before {Minutes} minutes.", minTimeBeforeCompletion.Value.TotalMinutes);

        double progress = 0;
        var startTime = DateTime.UtcNow;
        bool stalled = false;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!stalled)
                {
                    // Simulate progress with random jumps and occasional regressions
                    double delta = Rng.NextDouble() * 10; // Random progress increment (0-10%)
                    bool regress = Rng.Next(0, 10) == 0; // 10% chance to regress

                    if (regress && progress > 10)
                    {
                        progress -= Rng.NextDouble() * 5; // Random regression (0-5%)
                    }
                    else
                    {
                        progress += delta;
                    }

                    progress = Math.Min(progress, 99.9);

                    if (progress >= 99.9)
                    {
                        progress = 99.9;
                        stalled = true;
                        logger.Info("[FakeProgress] Progress reached 99.9%. Entering eternal patience phase...");
                    }
                }
                else if (DateTime.UtcNow - startTime > minTimeBeforeCompletion)
                {
                    // Surprise! It finishes.
                    progress = 100.0;
                    logger.Info("[FakeProgress] 🎉 Progress reached 100%. You win... nothing.");
                    ProcrastinationMetrics.TasksCompleted.Add(1);
                    reportProgress?.Invoke(progress);
                    activity?.SetStatus(ActivityStatusCode.Ok);
                    return;
                }

                logger.Info("[FakeProgress] Progress: {Progress:0.0}%", progress);
                reportProgress?.Invoke(progress);

                ProcrastinationMetrics.TotalTimeProcrastinated.Add(
                    (long)updateInterval.Value.TotalSeconds,
                    KeyValuePair.Create<string, object?>("component", "FakeIndeterminateProgress"));

                ProcrastinationMetrics.ExcusesGenerated.Add(1,
                    KeyValuePair.Create<string, object?>("category", "fake-indeterminate"));

                CommentaryService.LogRandomRemark();

                await Task.Delay(updateInterval.Value, cancellationToken);
            }

            logger.Info("[FakeProgress] Cancelled before reaching 100%. That's on you.");
            ProcrastinationMetrics.TasksNeverDone.Add(1);
            activity?.SetStatus(ActivityStatusCode.Ok, "Cancelled");
        }
        catch (OperationCanceledException)
        {
            logger.Info("[FakeProgress] Gracefully cancelled during procrastination cycle.");
            ProcrastinationMetrics.TasksNeverDone.Add(1);
            activity?.SetStatus(ActivityStatusCode.Ok, "Cancelled (OperationCanceledException)");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "[FakeProgress] Unexpected error while faking progress.");
            ProcrastinationMetrics.TasksNeverDone.Add(1);
        }
    }

    /// <summary>
    /// Allows test code to inject a custom CommentaryService for mocking or fault injection.
    /// </summary>
    public static void SetCommentaryService(CommentaryService service)
    {
        CommentaryService = service ?? throw new ArgumentNullException(nameof(service));
    }
}