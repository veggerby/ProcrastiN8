using System.Diagnostics;

using ProcrastiN8.Metrics;
using ProcrastiN8.Services;

namespace ProcrastiN8.Unproductivity;

/// <summary>
/// Simulates progress by emitting fake updates while doing absolutely nothing of consequence.
/// </summary>
public static class FakeProgress
{
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.Unproductivity.FakeProgress");

    private static readonly ExcuseService ExcuseService = new();

    private static readonly string[] FakeStages = new[]
    {
        "Aligning expectations...",
        "Calibrating metrics...",
        "Pretending to load data...",
        "Synchronizing with imaginary server...",
        "Consulting committee of doubts...",
        "Negotiating with productivity demons...",
        "Almost there. Probably.",
    };

    // Default step duration for fake progress (ms)
    private const int DefaultStepDurationMs = 750;

    /// <summary>
    /// Fakes a sequence of progress updates, optionally configurable in length and pacing.
    /// </summary>
    /// <param name="stepDuration">Duration per fake step. Defaults to 750ms.</param>
    /// <param name="steps">Number of steps to fake. Defaults to the built-in stage count.</param>
    /// <param name="logger">Optional logger for ceremonial progress updates.</param>
    /// <param name="commentaryService">Optional commentary service for motivational interjections. If not provided, a default is used.</param>
    /// <param name="cancellationToken">Token to cancel the performance mid-act.</param>
    public static async Task ShowFakeProgressAsync(
        TimeSpan? stepDuration = null,
        int? steps = null,
        IProcrastiLogger? logger = null,
        ICommentaryService? commentaryService = null,
        CancellationToken cancellationToken = default)
    {
        logger ??= new DefaultLogger();
        stepDuration ??= TimeSpan.FromMilliseconds(DefaultStepDurationMs);
        steps ??= FakeStages.Length;
        commentaryService ??= new CommentaryService();

        using var activity = ActivitySource.StartActivity("ProcrastiN8.FakeProgress.Show", ActivityKind.Internal);
        activity?.SetTag("progress.steps", steps);
        activity?.SetTag("progress.stepDuration.ms", stepDuration.Value.TotalMilliseconds);

        logger.Info("[FakeProgress] Initiating pretend progress sequence...");

        try
        {
            for (int i = 0; i < steps && !cancellationToken.IsCancellationRequested; i++)
            {
                var message = i < FakeStages.Length ? FakeStages[i] : $"Stage {i + 1}: Doing a thing...";
                logger.Info("[FakeProgress] Step {Step}/{Total}: {Message}", i + 1, steps, message);

                ProcrastinationMetrics.ExcusesGenerated.Add(1,
                    KeyValuePair.Create<string, object?>("category", "fake-progress"));

                commentaryService.LogRandomRemark();

                ProcrastinationMetrics.TotalTimeProcrastinated.Add(
                    (long)stepDuration.Value.TotalSeconds,
                    KeyValuePair.Create<string, object?>("component", "FakeProgress"));

                await Task.Delay(stepDuration.Value, cancellationToken);
            }

            logger.Info("[FakeProgress] Fake progress complete. Results inconclusive.");
            ProcrastinationMetrics.TasksCompleted.Add(1);
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (OperationCanceledException)
        {
            logger.Info("[FakeProgress] Cancelled mid-pretend. Classic.");
            ProcrastinationMetrics.TasksNeverDone.Add(1);
            activity?.SetStatus(ActivityStatusCode.Ok, "Cancelled");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "[FakeProgress] Something broke while simulating progress.");
            ProcrastinationMetrics.TasksNeverDone.Add(1);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}