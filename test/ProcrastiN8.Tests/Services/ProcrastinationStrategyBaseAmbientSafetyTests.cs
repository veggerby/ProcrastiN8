using ProcrastiN8.Services;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.Common;

namespace ProcrastiN8.Tests.Services;

/// <summary>
/// Validates ambient safety adoption and per-strategy overrides in <see cref="ProcrastinationStrategyBase"/>.
/// </summary>
public class ProcrastinationStrategyBaseAmbientSafetyTests
{
    private sealed class InstantDelayStrategy : IDelayStrategy
    {
        public Task DelayAsync(TimeSpan? minDelay = null, TimeSpan? maxDelay = null, Func<double, bool>? beforeCallback = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
    private sealed class CountingStrategy : ProcrastinationStrategyBase
    {
        protected override async Task ExecuteCoreAsync(Func<Task> task, TimeSpan initialDelay, IExcuseProvider? excuseProvider, IDelayStrategy delayStrategy, IRandomProvider randomProvider, ITimeProvider timeProvider, CancellationToken cancellationToken)
        {
            while (!SafetyCapReached())
            {
                IncrementCycle();
                await NotifyCycleAsync(ControlContext, cancellationToken);
            }
            // never execute task to dramatize the cap.
        }
    }

    private sealed class TinyAmbient(int max) : IExecutionSafetyOptions { public int MaxCycles { get; } = max; }

    [Fact]
    public async Task AmbientSafety_Applied_When_Not_Explicitly_Configured()
    {
        // arrange
        var s = new CountingStrategy();
        s.ConfigureSafety(new TinyAmbient(3));
        Func<Task> task = () => Task.CompletedTask;

        // act
        await s.ExecuteAsync(task, TimeSpan.Zero, null, new InstantDelayStrategy(), Substitute.For<IRandomProvider>(), SystemTimeProvider.Default, CancellationToken.None);
        var r = s.LastResult;

        // assert
        r.Cycles.Should().Be(3);
        r.Executed.Should().BeFalse();

    // cleanup none (explicit safety)
    }

    private sealed class OverrideSafety(int max) : IExecutionSafetyOptions { public int MaxCycles { get; } = max; }

    [Fact]
    public async Task ExplicitSafety_Override_Ignores_Ambient()
    {
        // arrange
        var s = new CountingStrategy();
        s.ConfigureSafety(new OverrideSafety(5));
        Func<Task> task = () => Task.CompletedTask;

        // act
        await s.ExecuteAsync(task, TimeSpan.Zero, null, new InstantDelayStrategy(), Substitute.For<IRandomProvider>(), SystemTimeProvider.Default, CancellationToken.None);
        var r = s.LastResult;

        // assert
        r.Cycles.Should().Be(5);

        // cleanup
        ProcrastinationStrategyBase.ResetAmbientSafety();
    }
}
