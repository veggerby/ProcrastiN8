using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Common;

public static class ExcuseGenerator
{
    // The year used in API-related excuses
    private const int ApiYear = 2017;

    private static readonly string[] Openings =
    [
        "In a shocking twist",
        "Due to unforeseen quantum entanglements",
        "After consulting the Oracle",
        "Following a heated debate with my inner monologue",
        "In light of recent cosmic events",
        "As foretold by the prophecy",
        "After a thorough risk assessment by Schrödinger's cat",
        "Because the multiverse demanded it",
        "In a parallel timeline"
    ];

    private static readonly string[] Verbs =
    [
        "waiting for",
        "avoiding",
        "syncing with",
        "refactoring",
        "debugging",
        "questioning",
        "compiling",
        "unit testing",
        "renaming",
        "neglecting"
    ];

    private static readonly string[] Nouns =
    [
        "the backlog",
        "my motivation",
        $"an API from {ApiYear}",
        "a vague requirement",
        "inner demons",
        "a sprint goal",
        "emotional dependencies",
        "the definition of 'done'",
        "Slack notifications",
        "Jira"
    ];

    private static readonly string[] Endings =
    [
        ".",
        "… again.",
        "for the 5th time today.",
        "until further notice.",
        "with extreme prejudice.",
        ", allegedly.",
        "— don't @ me.",
        "as a service.",
        "in production.",
        "because that's Agile, right?"
    ];

    private static readonly string[] Prefixes =
    [
        "¯\\_(ツ)_/¯ ",
        "🤷 ",
        "🥱 ",
        "🚧 ",
        "🧠 ",
        "🔄 ",
        "💤 ",
        ""
    ];

    public static string GetRandomExcuse(IRandomProvider? randomProvider = null)
    {
        randomProvider ??= RandomProvider.Default;

        var prefix = GetRandom(Prefixes, randomProvider);
        var opening = GetRandom(Openings, randomProvider);
        var verb = GetRandom(Verbs, randomProvider);
        var noun = GetRandom(Nouns, randomProvider);
        var ending = GetRandom(Endings, randomProvider);

        return $"{prefix}{opening} {verb} {noun}{ending}";
    }

    private static string GetRandom(string[] array, IRandomProvider randomProvider) => array[randomProvider.GetRandom(array.Length)];
}