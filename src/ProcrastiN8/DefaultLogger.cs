
namespace ProcrastiN8;

public sealed class DefaultLogger : IProcrastiLogger
{
    public static readonly DefaultLogger Instance = new();

    public void Debug(string message, params object?[] args)
    {
    }

    public void Error(string message, params object?[] args)
    {
    }

    public void Error(Exception ex, string message, params object?[] args)
    {
    }

    public void Warn(string message, params object?[] args)
    {
    }

    public void Info(string message, params object?[] args)
    {
    }
}