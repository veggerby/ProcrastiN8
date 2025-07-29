namespace ProcrastiN8.JustBecause.CollapseBehaviors;

public sealed class HeisenLoggingCollapseBehavior<T>(IProcrastiLogger? logger = null) : ICollapseBehavior<T>
{
    private readonly IProcrastiLogger? _logger = logger;

    public async Task<T?> CollapseAsync(IEnumerable<IQuantumPromise<T>> entangled, CancellationToken cancellationToken)
    {
        var promises = entangled.ToArray();
        if (!promises.Any()) return default;

        var actualValue = promises.First().Value;
        await Task.Yield(); // Ensure asynchronous behavior
        var fakeValue = GenerateFakeValue();

        _logger?.Info($"Observed fake value: {fakeValue}");

        return actualValue;
    }

    private T GenerateFakeValue()
    {
        // Generate a fake value consistent with T (e.g., default or random value)
        return default!;
    }
}