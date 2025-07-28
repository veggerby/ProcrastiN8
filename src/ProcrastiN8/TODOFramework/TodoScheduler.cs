using System.Reflection;

using ProcrastiN8.Common;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.TODOFramework;

/// <summary>
/// Schedules and manages the execution of methods and actions marked with <see cref="TodoAttribute"/>, ensuring they are never actually completed.
/// </summary>
/// <remarks>
/// The <see cref="TodoScheduler"/> is designed to perpetually defer execution, providing detailed logging and plausible excuses for each postponement.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="TodoScheduler"/> class.
/// </remarks>
/// <param name="logger">An optional logger for procrastination updates.</param>
/// <param name="excuseProvider">An optional excuse provider for creative stalling.</param>
/// <param name="delayStrategy">An optional delay strategy for arbitrary postponement.</param>
public class TodoScheduler(IProcrastiLogger? logger = null, IExcuseProvider? excuseProvider = null, IDelayStrategy? delayStrategy = null)
{
    private readonly IProcrastiLogger? _logger = logger;
    private readonly IExcuseProvider? _excuseProvider = excuseProvider;
    private readonly IDelayStrategy? _delayStrategy = delayStrategy;

    /// <summary>
    /// Attempts to schedule all methods marked with <see cref="TodoAttribute"/> in the given type, but never actually executes them.
    /// </summary>
    /// <param name="type">The type to scan for TODOs.</param>
    /// <param name="cancellationToken">A token to cancel the scheduling process.</param>
    public async Task ScheduleAllAsync(Type type, CancellationToken cancellationToken = default)
    {
        var todoMethods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => m.GetCustomAttributes(typeof(TodoAttribute), true).Any())
            .ToList();

        foreach (var method in todoMethods)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var excuse = _excuseProvider?.GetExcuse() ?? "Pending further review.";
            _logger?.Info($"Deferring TODO: {method.Name} — {excuse}");
            if (_delayStrategy != null)
            {
                await _delayStrategy.DelayAsync(cancellationToken);
            }
        }
    }
}