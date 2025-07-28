namespace ProcrastiN8.JustBecause.CollapseBehaviors;

public sealed class DefaultObserverContext : IObserverContext
{
    public bool IsObserved(string context) => false; // No one is watching. Schrödinger wins.
}