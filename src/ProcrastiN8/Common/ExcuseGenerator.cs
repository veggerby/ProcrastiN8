
using ProcrastiN8.JustBecause;
namespace ProcrastiN8.Common;

public static class ExcuseGenerator
{
    // The year used in API-related excuses
    private const int ApiYear = 2017;

    private static readonly string[] Openings =
    [
        "Still",
        "Currently",
        "Temporarily",
        "Reluctantly",
        "Accidentally",
        "Emotionally",
        "Spiritually",
        "Hypothetically",
        "Existentially"
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

    private static IRandomProvider _randomProvider = new ProcrastiN8.JustBecause.RandomProvider();

    public static void SetRandomProvider(ProcrastiN8.JustBecause.IRandomProvider provider)
    {
        _randomProvider = provider;
    }

    public static string GetRandomExcuse()
    {
        var prefix = GetRandom(Prefixes);
        var opening = GetRandom(Openings);
        var verb = GetRandom(Verbs);
        var noun = GetRandom(Nouns);
        var ending = GetRandom(Endings);

        return $"{prefix}{opening} {verb} {noun}{ending}";
    }

    private static string GetRandom(string[] array) => array[_randomProvider.Next(array.Length)];
}