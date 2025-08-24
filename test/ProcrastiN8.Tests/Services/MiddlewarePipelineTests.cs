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
}
