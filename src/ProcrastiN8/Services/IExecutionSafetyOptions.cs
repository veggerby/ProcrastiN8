namespace ProcrastiN8.Services;

/// <summary>Configurable safety limits to prevent runaway deferral loops.</summary>
public interface IExecutionSafetyOptions
{
    /// <summary>Maximum cycles before forced termination.</summary>
    int MaxCycles { get; }
}

/// <summary>Default safety configuration.</summary>
public sealed class DefaultExecutionSafetyOptions : IExecutionSafetyOptions
{
    public static readonly DefaultExecutionSafetyOptions Instance = new();
    public int MaxCycles => 500;
}