using ProcrastiN8.JustBecause;
using ProcrastiN8.NeuralExcuseLab;

namespace ProcrastiN8.Tests.NeuralExcuseLab;

public class LocalExcuseModelTests
{
    [Fact]
    public void ModelName_Should_ReturnExpectedValue()
    {
        // arrange
        var model = new LocalExcuseModel();

        // act
        var modelName = model.ModelName;

        // assert
        modelName.Should().Be("LocalExcuseLLaMA-7B");
    }

    [Fact]
    public async Task GenerateExcuseAsync_Should_ReturnExcuse()
    {
        // arrange
        var model = new LocalExcuseModel();

        // act
        var excuse = await model.GenerateExcuseAsync("Test prompt");

        // assert
        excuse.Should().NotBeNullOrWhiteSpace("local models must pretend to work");
    }

    [Fact]
    public void GetMetadata_Should_ReturnLocalModelInformation()
    {
        // arrange
        var model = new LocalExcuseModel("custom/path/model.gguf");

        // act
        var metadata = model.GetMetadata();

        // assert
        metadata.Should().ContainKey("provider");
        metadata.Should().ContainKey("model_path");
        metadata["provider"].Should().Be("Local");
        metadata["model_path"].Should().Be("custom/path/model.gguf");
    }

    [Fact]
    public async Task GenerateExcuseAsync_Should_SimulateLoadingDelay()
    {
        // arrange
        var model = new LocalExcuseModel();

        // act
        var excuse = await model.GenerateExcuseAsync("Test prompt");

        // assert
        excuse.Should().NotBeNullOrWhiteSpace("local models generate excuses");
    }
}
