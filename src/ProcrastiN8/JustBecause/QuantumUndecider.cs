using System.Diagnostics;

using ProcrastiN8.LazyTasks;
using ProcrastiN8.Metrics;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// Simulates quantum indecision by providing ambiguous or delayed responses to queries.
/// </summary>
/// <remarks>
/// The QuantumUndecider is a cornerstone of ProcrastiN8's philosophy, embodying the art of productive stalling.
/// </remarks>
public static class QuantumUndecider
{
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.JustBecause.QuantumUndecider");

    private static readonly string[] DecisionStates =
    {
        "Maybe.",
        "Not right now.",
        "It depends.",
        "I'm in a superposition of Yes and No.",
        "¯\\_(ツ)_/¯",
        "Let's ask Schrödinger’s cat.",
        "Awaiting entangled twin’s input...",
    };

    public static event Action<string>? OnEntangledDecision;

    /// <summary>
    /// Observes the decision and collapses the waveform. May throw if observation is too aggressive.
    /// </summary>
    public static async Task<string> ObserveDecisionAsync(
        Func<Task<bool>> costlyComputation,
        IProcrastiLogger? logger = null,
        IDelayStrategy? delayStrategy = null,
        IRandomProvider? randomProvider = null,
        CancellationToken cancellationToken = default)
    {
        randomProvider ??= RandomProvider.Default;
        delayStrategy ??= new DefaultDelayStrategy(randomProvider: randomProvider);

        QuantumUndeciderMetrics.Observations.Add(1);

        using var activity = ActivitySource.StartActivity("ProcrastiN8.QuantumUndecider.ObserveDecision", ActivityKind.Internal);
        var sw = Stopwatch.StartNew();

        logger?.Info("[QuantumUndecider] Initiating uncertain observation protocol...");
        LogPhilosophicalUncertainty(randomProvider, logger);

        await delayStrategy.DelayAsync(minDelay: TimeSpan.FromMilliseconds(800), maxDelay: TimeSpan.FromMilliseconds(4200), cancellationToken: cancellationToken);

        if (randomProvider.GetDouble() < 0.15)
        {
            QuantumUndeciderMetrics.Failures.Add(1, new KeyValuePair<string, object?>("reason", "CollapseTooEarly"));
            activity?.SetTag("collapse.status", "failure");
            activity?.SetTag("collapse.reason", "CollapseTooEarly");

            logger?.Error("[QuantumUndecider] Observation collapsed too early.");
            TriggerEntangledCallback("CollapseTooEarly", logger); // Trigger callback before throwing
            throw new SuperpositionCollapseException("The waveform collapsed under scrutiny. Try being less assertive.");
        }

        if (randomProvider.GetDouble() < 0.1)
        {
            logger?.Error("[QuantumUndecider] Observation was too aggressive. Quantum instability triggered.");
            throw new CollapseTooEarlyException("Observation was too aggressive. Quantum instability triggered.");
        }

        if (randomProvider.GetDouble() < 0.25)
        {
            var undecided = "It depends."; // Ensure consistent callback for test expectations

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
    /// Delays until a decision is inevitable or entropy wins.
    /// </summary>
    /// <param name="maxDelay">Maximum delay before a decision is inevitable.</param>
    /// <param name="logger">Logger for updates.</param>
    /// <param name="cancellationToken">Cancels the delay.</param>
    /// <returns>A task that completes with a decision or throws an exception.</returns>
    public static async Task<string> DelayUntilInevitabilityAsync(
        TimeSpan maxDelay,
        IRandomProvider? randomProvider = null,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        randomProvider ??= RandomProvider.Default;
        logger?.Info("[QuantumUndecider] Delaying until inevitability...");

        var delay = randomProvider.GetRandom(0, (int)maxDelay.TotalMilliseconds);
        await Task.Delay(delay, cancellationToken);

        if (randomProvider.GetDouble() < 0.2)
        {
            logger?.Error("[QuantumUndecider] Entropy won. Decision collapsed.");
            QuantumUndeciderMetrics.Failures.Add(1, new KeyValuePair<string, object?>("reason", "EntropyCollapse"));
            throw new SuperpositionCollapseException("Entropy caused the decision to collapse.");
        }

        var outcome = DecisionStates[randomProvider.GetRandom(DecisionStates.Length)];
        logger?.Info($"[QuantumUndecider] Inevitability reached: {outcome}");
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

    private static void LogPhilosophicalUncertainty(IRandomProvider randomProvider, IProcrastiLogger? logger)
    {
        var musings = new[]
        {
            "Certainty is the enemy of progress.",
            "Maybe we should wait for another timeline.",
            "Some decisions are only real if someone regrets them.",
            "Time is a flat circle. So is indecision.",
            "If all outcomes exist, is choosing just cruel?"
        };

        logger?.Debug($"[QuantumUndecider] {musings[randomProvider.GetRandom(musings.Length)]}");
    }
}