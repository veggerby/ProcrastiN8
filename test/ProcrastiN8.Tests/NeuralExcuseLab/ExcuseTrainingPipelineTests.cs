using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.NeuralExcuseLab;

namespace ProcrastiN8.Tests.NeuralExcuseLab;

public class ExcuseTrainingPipelineTests
{
    [Fact]
    public async Task TrainAsync_Should_CompleteWithoutError()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var delayProvider = Substitute.For<IDelayProvider>();
        delayProvider.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var pipeline = new ExcuseTrainingPipeline(delayProvider: delayProvider, logger: logger);

        // act
        await pipeline.TrainAsync("fake-dataset.csv", epochs: 2);

        // assert
        logger.Received().Info(Arg.Is<string>(s => s.Contains("Neural Excuse Fine-Tuning Pipeline")));
        logger.Received().Info(Arg.Is<string>(s => s.Contains("Training pipeline completed successfully")));
    }

    [Fact]
    public async Task TrainAsync_Should_LogEpochProgress()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var delayProvider = Substitute.For<IDelayProvider>();
        delayProvider.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var pipeline = new ExcuseTrainingPipeline(delayProvider: delayProvider, logger: logger);

        // act
        await pipeline.TrainAsync("test-data.csv", epochs: 3);

        // assert
        logger.Received().Info(Arg.Is<string>(s => s.Contains("Epoch 1/3")));
        logger.Received().Info(Arg.Is<string>(s => s.Contains("Epoch 2/3")));
        logger.Received().Info(Arg.Is<string>(s => s.Contains("Epoch 3/3")));
    }

    [Fact]
    public async Task TrainAsync_WithCancellation_Should_StopGracefully()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var pipeline = new ExcuseTrainingPipeline(logger: logger);
        
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(100));

        // act & assert
        // The training should be cancelled by throwing TaskCanceledException
        await Assert.ThrowsAsync<TaskCanceledException>(() => pipeline.TrainAsync("test-data.csv", epochs: 100, cts.Token));
    }

    [Fact]
    public async Task TrainAsync_Should_LogDatasetLoading()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var delayProvider = Substitute.For<IDelayProvider>();
        delayProvider.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var pipeline = new ExcuseTrainingPipeline(delayProvider: delayProvider, logger: logger);

        // act
        await pipeline.TrainAsync("jira-comments.csv", epochs: 1);

        // assert
        logger.Received().Info(Arg.Is<string>(s => s.Contains("Loading backlog comments from jira-comments.csv")));
        logger.Received().Info(Arg.Is<string>(s => s.Contains("Loaded") && s.Contains("training samples")));
    }

    [Fact]
    public async Task TrainAsync_Should_LogModelCheckpoint()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var delayProvider = Substitute.For<IDelayProvider>();
        delayProvider.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var pipeline = new ExcuseTrainingPipeline(delayProvider: delayProvider, logger: logger);

        // act
        await pipeline.TrainAsync("test-data.csv", epochs: 1);

        // assert
        logger.Received().Info(Arg.Is<string>(s => s.Contains("Model checkpoint saved")));
        logger.Received().Info(Arg.Is<string>(s => s.Contains("No actual training occurred")));
    }
}
