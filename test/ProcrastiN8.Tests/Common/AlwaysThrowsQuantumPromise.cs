using System;
using System.Threading;
using System.Threading.Tasks;
using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.Common
{
    /// <summary>
    /// A deterministic test double for IQuantumPromise that always throws the provided exception.
    /// </summary>
    public class AlwaysThrowsQuantumPromise<T> : IQuantumPromise<T>
    {
        private readonly Exception _exception;
        public AlwaysThrowsQuantumPromise(Exception exception)
        {
            _exception = exception;
        }
        public Task<T> ObserveAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromException<T>(_exception);
        }
        public override string ToString() => $"[AlwaysThrowsQuantumPromise: {_exception.GetType().Name}]";
    }
}
