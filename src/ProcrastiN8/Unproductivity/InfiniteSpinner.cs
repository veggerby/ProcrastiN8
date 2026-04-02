using System.Diagnostics;

using ProcrastiN8.Metrics;
using ProcrastiN8.Services;

namespace ProcrastiN8.Unproductivity;

/// <summary>
/// Simulates endless activity without any meaningful output.
/// Perfect for appearing busy while achieving absolutely nothing.
/// </summary>
public static class InfiniteSpinner
{
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.Unproductivity.InfiniteSpinner");

    // Default tick rate for infinite spinner (ms)
    private const int DefaultTickRateMs = 500;

    /// <summary>
    /// Starts an infinite spinner loop that chews CPU cycles, emits fake activity, and accomplishes nothing.
    /// </summary>
    /// <param name="logger">Optional logger for existential spinner updates.</param>
    /// <param name="tickRate">Interval between spins. Defaults to 500ms.</param>
    /// <param name="cancellationToken">Token to eventually end the suffering.</param>
    /// <param name="commentaryService">Optional commentary service for periodic remarks. If not provided, a default is used.</param>
    public static async Task SpinForeverAsync(
        IProcrastiLogger? logger = null,
        TimeSpan? tickRate = null,
        CancellationToken cancellationToken = default,
        ICommentaryService? commentaryService = null)
    {
        logger ??= new DefaultLogger();
        tickRate ??= TimeSpan.FromMilliseconds(DefaultTickRateMs);
        commentaryService ??= new CommentaryService();

        using var activity = ActivitySource.StartActivity("ProcrastiN8.InfiniteSpinner.Spin", ActivityKind.Internal);
        activity?.SetTag("spinner.tickRate.ms", tickRate.Value.TotalMilliseconds);

        logger.Info("[InfiniteSpinner] Beginning infinite spin cycle...");
        logger.Info("[InfiniteSpinner] Tick rate: {TickMs}ms", tickRate.Value.TotalMilliseconds);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ProcrastinationMetrics.TotalTimeProcrastinated.Add(
                    (long)tickRate.Value.TotalSeconds,
                    KeyValuePair.Create<string, object?>("component", "InfiniteSpinner"));

                commentaryService.LogRandomRemark();

                await Task.Delay(tickRate.Value, cancellationToken);
            }

            activity?.SetStatus(ActivityStatusCode.Ok, "Cancelled");
            logger.Info("[InfiniteSpinner] Spin cycle cancelled. Productivity threat detected.");
        }
        catch (OperationCanceledException)
        {
            activity?.SetStatus(ActivityStatusCode.Ok, "Cancelled by OperationCanceledException");
            logger.Info("[InfiniteSpinner] Spin cycle cancelled. Productivity threat detected.");
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            logger.Error(ex, "[InfiniteSpinner] Something went wrong... while doing nothing.");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            logger.Info("[InfiniteSpinner] Total time wasted: {Seconds:0.0}s", stopwatch.Elapsed.TotalSeconds);
        }
    }
}