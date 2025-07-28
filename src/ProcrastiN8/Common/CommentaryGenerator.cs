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

    private static IRandomProvider _randomProvider = new ProcrastiN8.JustBecause.RandomProvider();

    public static void SetRandomProvider(ProcrastiN8.JustBecause.IRandomProvider provider)
    {
        _randomProvider = provider;
    }

    public static string GetRandomCommentary()
    {
        var intro = GetRandom(Intros);
        var phrase = GetRandom(Phrases);
        var suffix = GetRandom(Suffixes);
        var emoji = GetRandom(Emojis);

        return $"{intro} {phrase}{suffix} {emoji}".Trim();
    }

    public static void LogRandomCommentary(IProcrastiLogger? logger, string context = "ProcrastiN8")
    {
        var message = GetRandomCommentary();
        logger?.Debug($"[{context}] {message}");
    }

    private static string GetRandom(string[] array) => array[_randomProvider.Next(array.Length)];
}