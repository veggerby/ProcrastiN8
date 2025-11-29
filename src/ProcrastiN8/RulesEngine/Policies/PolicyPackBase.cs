namespace ProcrastiN8.RulesEngine.Policies;

/// <summary>
/// Base implementation of a policy pack.
/// </summary>
public abstract class PolicyPackBase : IPolicyPack
{
    private readonly List<IRule> _rules = new();

    /// <inheritdoc />
    public abstract string Id { get; }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public abstract string Description { get; }

    /// <inheritdoc />
    public abstract Version Version { get; }

    /// <inheritdoc />
    public IReadOnlyList<IRule> Rules => _rules.AsReadOnly();

    /// <inheritdoc />
    public IDictionary<string, object?> Metadata { get; } = new Dictionary<string, object?>();

    /// <summary>
    /// Adds a rule to this policy pack.
    /// </summary>
    /// <param name="rule">The rule to add.</param>
    protected void AddRule(IRule rule)
    {
        _rules.Add(rule ?? throw new ArgumentNullException(nameof(rule)));
    }
}
