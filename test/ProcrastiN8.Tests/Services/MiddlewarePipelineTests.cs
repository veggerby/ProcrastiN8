using ProcrastiN8.Services;
using ProcrastiN8.Services.Diagnostics;

namespace ProcrastiN8.Tests.Services;

public class MiddlewarePipelineTests
{
    private sealed class OrderRecordingMiddleware : IProcrastinationMiddleware
    {
        private readonly string _id;
        private readonly List<string> _sink;
        public OrderRecordingMiddleware(string id, List<string> sink) { _id = id; _sink = sink; }
        public async Task InvokeAsync(ProcrastinationExecutionContext context, Func<Task> next, CancellationToken cancellationToken)
        {
            _sink.Add($"before:{_id}");
            await next();
            _sink.Add($"after:{_id}:{context.Result?.Executed}");
        }
    }

    private sealed class CorrelationCaptureMiddleware : IProcrastinationMiddleware
    {
        public Guid? BeforeId; public Guid? AfterId;
        public async Task InvokeAsync(ProcrastinationExecutionContext context, Func<Task> next, CancellationToken cancellationToken)
        {
            BeforeId = context.CorrelationId;
            await next();
            AfterId = context.Result?.CorrelationId;
        }
    }

    private sealed class ExceptionThrowingMiddleware : IProcrastinationMiddleware
    {
        public async Task InvokeAsync(ProcrastinationExecutionContext context, Func<Task> next, CancellationToken cancellationToken)
        {
            await next();
            throw new InvalidOperationException("post-exec failure");
        }
    }

    [Fact]
    public async Task Middleware_Should_Execute_In_Declared_Order_And_See_Result()
    {
        // arrange
        var records = new List<string>();
        var builder = ProcrastinationSchedulerBuilder.Create()
            .AddMiddleware(new OrderRecordingMiddleware("a", records))
            .AddMiddleware(new OrderRecordingMiddleware("b", records))
            .AddObserver(new MetricsObserver())
            .Build();

        bool ran = false;
        // act
        var result = await builder.ScheduleWithResult(() => { ran = true; return Task.CompletedTask; }, TimeSpan.Zero, ProcrastinationMode.MovingTarget);

        // assert
        ran.Should().BeTrue("the underlying task must execute despite elaborate stalling layers");
        result.Executed.Should().BeTrue();
        // Expect order: a before, b before, (task), b after, a after
        records.Should().HaveCount(4);
        records[0].Should().Be("before:a");
        records[1].Should().Be("before:b");
        records[2].Should().StartWith("after:b:");
        records[3].Should().StartWith("after:a:");
    }

    [Fact]
    public async Task CorrelationId_Should_Be_Stable_Between_Context_And_Result()
    {
        var capture = new CorrelationCaptureMiddleware();
        var scheduler = ProcrastinationSchedulerBuilder.Create()
            .AddMiddleware(capture)
            .Build();

        var result = await scheduler.ScheduleWithResult(() => Task.CompletedTask, TimeSpan.Zero, ProcrastinationMode.MovingTarget);

        capture.BeforeId.Should().NotBeNull();
        capture.AfterId.Should().Be(result.CorrelationId);
        capture.BeforeId.Should().Be(result.CorrelationId, "correlation id should unify pipeline and result");
    }

    [Fact]
    public async Task Middleware_Before_Phase_Has_Provisional_Result_Even_If_Later_Middleware_Throws()
    {
        ProcrastinationResult? beforeObserved = null;
        var observerMw = new DelegateMiddleware(async (ctx, next, ct) =>
        {
            beforeObserved = ctx.Result; // provisional assignment
            await next();
        });
        var scheduler = ProcrastinationSchedulerBuilder.Create()
            .AddMiddleware(observerMw)
            .AddMiddleware(new ExceptionThrowingMiddleware())
            .Build();

        var ex = await Record.ExceptionAsync(async () => await scheduler.ScheduleWithResult(() => Task.CompletedTask, TimeSpan.Zero, ProcrastinationMode.MovingTarget));
        ex.Should().BeOfType<InvalidOperationException>();
        beforeObserved.Should().NotBeNull();
    }

    private sealed class DelegateMiddleware : IProcrastinationMiddleware
    {
        private readonly Func<ProcrastinationExecutionContext, Func<Task>, CancellationToken, Task> _d;
        public DelegateMiddleware(Func<ProcrastinationExecutionContext, Func<Task>, CancellationToken, Task> d) { _d = d; }
        public Task InvokeAsync(ProcrastinationExecutionContext context, Func<Task> next, CancellationToken cancellationToken) => _d(context, next, cancellationToken);
    }
}