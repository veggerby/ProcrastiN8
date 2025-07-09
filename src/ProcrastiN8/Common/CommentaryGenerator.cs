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
        "productivity is a myth perpetuated by calendars",
        "we've officially entered the Maybe Phase",
        "task has transcended priority and entered the void",
        "effort is currently rate-limited by vibes",
        "delay has been successfully ritualized",
        "this is now someone else's spiritual burden",
        "the deadline is just a polite suggestion",
        "progress is currently being simulated",
        "motivation is currently garbage collected",
        "we're pivoting to a non-actionable framework"
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

    private static readonly Random Rng = new();

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

    private static string GetRandom(string[] array) => array[Rng.Next(array.Length)];
}