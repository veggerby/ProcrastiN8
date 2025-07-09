namespace ProcrastiN8;

public interface IProcrastiLogger
{
    void Info(string message, params object?[] args);
    void Debug(string message, params object?[] args);
    void Warn(string message, params object?[] args);
    void Error(string message, params object?[] args);
    void Error(Exception ex, string message, params object?[] args);
}