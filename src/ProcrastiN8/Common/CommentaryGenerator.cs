using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Common;

public static class CommentaryGenerator
{
    private static readonly string[] Intros =
    [
        "This is fine:",
        "Update:",
        "FYI:",
        "Note to future self:",
        "Low-key reminder:",
        "Cosmic insight:",
        "From the department of delays:",
        "According to no plan:",
        "Based on emotional analytics:"
    ];

    private static readonly string[] Phrases =
    [
        "the task has entered a Schrödinger state",
        "we're now operating on borrowed time from a parallel universe",
        "the deliverable has been reclassified as a philosophical concept",
        "progress is being measured in quantum leaps",
        "the deadline has achieved sentience and is negotiating terms",
        "we've outsourced this task to a black hole",
        "the project is now a case study in chaos theory",
        "we're pivoting to a Schrödinger's deliverable model",
        "the timeline has been folded into a Möbius strip",
        "we're currently beta-testing reality"
    ];

    private static readonly string[] Suffixes =
    [
        ".",
        "… allegedly.",
        "™",
        "for legal reasons.",
        "until morale improves.",
        "with absolutely no regrets.",
        "as dictated by the prophecy.",
        "and that's Agile.",
        "because reasons.",
        "don't question it."
    ];

    private static readonly string[] Emojis =
    [
        "📉", "🌀", "🔁", "💤", "😵‍💫", "🫠", "🙃", "🛸", "🤹‍♂️", ""
    ];

    public static string GetRandomCommentary(IRandomProvider? randomProvider = null)
    {
        randomProvider ??= RandomProvider.Default;
        var intro = GetRandom(Intros, randomProvider);
        var phrase = GetRandom(Phrases, randomProvider);
        var suffix = GetRandom(Suffixes, randomProvider);
        var emoji = GetRandom(Emojis, randomProvider);

        return $"{intro} {phrase}{suffix} {emoji}".Trim();
    }

    public static void LogRandomCommentary(IProcrastiLogger? logger, string context = "ProcrastiN8", IRandomProvider? randomProvider = null)
    {
        var message = GetRandomCommentary(randomProvider);
        logger?.Debug($"[{context}] {message}");
    }

    private static string GetRandom(string[] array, IRandomProvider randomProvider) => array[randomProvider.GetRandom(array.Length)];
}