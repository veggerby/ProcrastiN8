using System.Diagnostics;

using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// A class that embodies quantum procrastination by entangling outcomes and randomly collapsing decision waveforms,
/// occasionally throwing exceptions when too much certainty is attempted.
/// </summary>
public static class QuantumUndecider
{
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.JustBecause.QuantumUndecider");

    private static readonly Random Rng = new();
    private static readonly string[] DecisionStates =
    [
        "Maybe.",
        "Not right now.",
        "It depends.",
        "I'm in a superposition of Yes and No.",
        "¯\\_(ツ)_/¯",
        "Let's ask Schrödinger’s cat.",
        "Awaiting entangled twin’s input...",
    ];

    public static event Action<string>? OnEntangledDecision;

    /// <summary>
    /// Observes the decision and collapses the waveform. May throw if observation is too aggressive.
    /// </summary>
    public static async Task<string> ObserveDecisionAsync(
        Func<Task<bool>> costlyComputation,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        QuantumUndeciderMetrics.Observations.Add(1);

        using var activity = ActivitySource.StartActivity("QuantumUndecider.ObserveDecision", ActivityKind.Internal);
        var sw = Stopwatch.StartNew();

        logger?.Info("[QuantumUndecider] Initiating uncertain observation protocol...");
        LogPhilosophicalUncertainty(logger);

        await Task.Delay(TimeSpan.FromMilliseconds(Rng.Next(800, 4200)), cancellationToken);

        if (Rng.NextDouble() < 0.15)
        {
            QuantumUndeciderMetrics.Failures.Add(1, new KeyValuePair<string, object?>("reason", "CollapseTooEarly"));
            activity?.SetTag("collapse.status", "failure");
            activity?.SetTag("collapse.reason", "CollapseTooEarly");

            logger?.Error("[QuantumUndecider] Observation collapsed too early.");
            throw new SuperpositionCollapseException("The waveform collapsed under scrutiny. Try being less assertive.");
        }

        if (Rng.NextDouble() < 0.25)
        {
            var undecided = DecisionStates[Rng.Next(DecisionStates.Length)];

            QuantumUndeciderMetrics.Outcomes.Add(1, new KeyValuePair<string, object?>("state", "partial"));
            activity?.SetTag("collapse.status", "partial");
            activity?.SetTag("decision", undecided);

            logger?.Warn($"[QuantumUndecider] Partial collapse occurred: {undecided}");
            TriggerEntangledCallback(undecided, logger);

            sw.Stop();
            QuantumUndeciderMetrics.DecisionLatency.Record(sw.Elapsed.TotalMilliseconds);
            return undecided;
        }

        var result = await costlyComputation();
        var outcome = result ? "Yes. Probably." : "No. But it’s complicated.";

        QuantumUndeciderMetrics.Outcomes.Add(1, new KeyValuePair<string, object?>("state", "definitive"));
        activity?.SetTag("collapse.status", "success");
        activity?.SetTag("decision", outcome);

        logger?.Info($"[QuantumUndecider] Waveform fully collapsed to: {outcome}");
        TriggerEntangledCallback(outcome, logger);

        sw.Stop();
        QuantumUndeciderMetrics.DecisionLatency.Record(sw.Elapsed.TotalMilliseconds);
        return outcome;
    }

    /// <summary>
    /// Delays decision until cosmically necessary. May entangle the outcome.
    /// </summary>
    public static async Task<string> DelayUntilInevitabilityAsync(
        TimeSpan maxDelay,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        QuantumUndeciderMetrics.Observations.Add(1);

        using var activity = ActivitySource.StartActivity("QuantumUndecider.DelayUntilInevitability", ActivityKind.Internal);
        var sw = Stopwatch.StartNew();

        var delay = TimeSpan.FromMilliseconds(Rng.Next((int)(maxDelay.TotalMilliseconds * 0.7), (int)maxDelay.TotalMilliseconds));
        logger?.Info($"[QuantumUndecider] Delaying waveform collapse by {delay.TotalSeconds:0.0}s.");

        LogExcuse(logger);
        await Task.Delay(delay, cancellationToken);

        if (Rng.NextDouble() < 0.2)
        {
            QuantumUndeciderMetrics.Failures.Add(1, new KeyValuePair<string, object?>("reason", "EntropyDecay"));
            activity?.SetTag("collapse.status", "failure");
            activity?.SetTag("collapse.reason", "EntropyDecay");

            logger?.Error("[QuantumUndecider] Delayed too long. Decision lost to heat death.");
            throw new SuperpositionCollapseException("Decision decayed due to cosmic entropy.");
        }

        var outcome = DecisionStates[Rng.Next(DecisionStates.Length)];
        QuantumUndeciderMetrics.Outcomes.Add(1, new KeyValuePair<string, object?>("state", "delayed"));

        activity?.SetTag("collapse.status", "success");
        activity?.SetTag("decision", outcome);
        activity?.SetTag("mode", "delayed");

        TriggerEntangledCallback(outcome, logger);

        sw.Stop();
        QuantumUndeciderMetrics.DecisionLatency.Record(sw.Elapsed.TotalMilliseconds);
        return outcome;
    }

    private static void TriggerEntangledCallback(string outcome, IProcrastiLogger? logger)
    {
        if (OnEntangledDecision != null)
        {
            logger?.Debug("[QuantumUndecider] Triggering entangled twin decision update.");
            try
            {
                OnEntangledDecision.Invoke(outcome);
            }
            catch (Exception ex)
            {
                logger?.Warn($"[QuantumUndecider] Entanglement callback threw: {ex.Message}");
            }
        }
    }

    private static void LogPhilosophicalUncertainty(IProcrastiLogger? logger)
    {
        var musings = new[]
        {
            "Certainty is the enemy of progress.",
            "Maybe we should wait for another timeline.",
            "Some decisions are only real if someone regrets them.",
            "Time is a flat circle. So is indecision.",
            "If all outcomes exist, is choosing just cruel?"
        };

        logger?.Debug($"[QuantumUndecider] {musings[Rng.Next(musings.Length)]}");
    }

    private static void LogExcuse(IProcrastiLogger? logger)
    {
        var excuses = new[]
        {
            "Awaiting cosmic alignment.",
            "Still synchronizing with quantum mirror self.",
            "There's a lag in the entanglement buffer.",
            "My uncertainty matrix isn't calibrated yet.",
            "Lost connection to the multiverse API."
        };

        logger?.Info($"[QuantumUndecider] {excuses[Rng.Next(excuses.Length)]}");
    }
}