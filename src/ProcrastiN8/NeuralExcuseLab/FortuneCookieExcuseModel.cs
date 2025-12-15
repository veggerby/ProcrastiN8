using ProcrastiN8.JustBecause;

namespace ProcrastiN8.NeuralExcuseLab;

/// <summary>
/// An <see cref="IExcuseModel"/> that generates excuses with the profound wisdom of fortune cookies.
/// </summary>
/// <remarks>
/// The most cost-effective AI solution available. Wisdom not guaranteed.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="FortuneCookieExcuseModel"/> class.
/// </remarks>
/// <param name="randomProvider">Random provider for fortune selection.</param>
/// <param name="logger">Optional logger for fortune cookie philosophy.</param>
public class FortuneCookieExcuseModel(IRandomProvider? randomProvider = null, IProcrastiLogger? logger = null) : IExcuseModel
{
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;
    private readonly IProcrastiLogger? _logger = logger;

    private static readonly string[] FortuneCookieExcuses =
    [
        "Ancient proverb says: 'He who waits, avoids work.'",
        "Confucius say: 'Better to procrastinate than to fail quickly.'",
        "Fortune smiles upon those who delay indefinitely.",
        "The stars align against productivity today.",
        "Wise person knows: tomorrow is also a day.",
        "Your destiny is not to complete tasks today.",
        "In bed... you will remain all day.",
        "A journey of a thousand miles begins with not starting.",
        "He who hesitates is... actually quite sensible.",
        "The Tao of procrastination flows through you."
    ];

    /// <inheritdoc />
    public string ModelName => "FortuneCookie-API-v1";

    /// <inheritdoc />
    public async Task<string> GenerateExcuseAsync(string prompt, CancellationToken cancellationToken = default)
    {
        _logger?.Info($"[{ModelName}] Consulting ancient wisdom for: {prompt}");
        
        // Simulate network latency to a fictional fortune cookie API
        await Task.Delay(TimeSpan.FromMilliseconds(_randomProvider.GetRandom(20, 80)), cancellationToken);
        
        var fortune = FortuneCookieExcuses[_randomProvider.GetRandom(FortuneCookieExcuses.Length)];
        
        _logger?.Info($"[{ModelName}] Fortune retrieved: {fortune}");
        
        return fortune;
    }

    /// <inheritdoc />
    public IDictionary<string, object> GetMetadata()
    {
        return new Dictionary<string, object>
        {
            { "provider", "FortuneCookie" },
            { "model", "ancient-wisdom-v1" },
            { "type", "api" },
            { "wisdom_level", "profound" },
            { "cost_per_call", 0.001 },
            { "requires_api_key", false },
            { "includes_lucky_numbers", false }
        };
    }
}
