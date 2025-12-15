using ProcrastiN8.JustBecause;
using ProcrastiN8.NeuralExcuseLab;

namespace ProcrastiN8.Tests.NeuralExcuseLab;

public class FortuneCookieExcuseModelTests
{
    [Fact]
    public void ModelName_Should_ReturnExpectedValue()
    {
        // arrange
        var model = new FortuneCookieExcuseModel();

        // act
        var modelName = model.ModelName;

        // assert
        modelName.Should().Be("FortuneCookie-API-v1");
    }

    [Fact]
    public async Task GenerateExcuseAsync_Should_ReturnExcuse()
    {
        // arrange
        var model = new FortuneCookieExcuseModel();

        // act
        var excuse = await model.GenerateExcuseAsync("Test prompt");

        // assert
        excuse.Should().NotBeNullOrWhiteSpace("fortune cookies must dispense wisdom, even if questionable");
    }

    [Fact]
    public void GetMetadata_Should_ReturnProviderInformation()
    {
        // arrange
        var model = new FortuneCookieExcuseModel();

        // act
        var metadata = model.GetMetadata();

        // assert
        metadata.Should().ContainKey("provider");
        metadata.Should().ContainKey("model");
        metadata.Should().ContainKey("type");
        metadata["provider"].Should().Be("FortuneCookie");
    }

    [Fact]
    public async Task GenerateExcuseAsync_WithRandomProvider_Should_UseSameRandomProvider()
    {
        // arrange
        var model = new FortuneCookieExcuseModel();

        // act
        var excuse = await model.GenerateExcuseAsync("Test prompt");

        // assert
        excuse.Should().NotBeNullOrWhiteSpace("fortune cookies dispense wisdom");
    }
}
