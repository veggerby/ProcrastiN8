using ProcrastiN8.JustBecause;
using ProcrastiN8.NeuralExcuseLab;

namespace ProcrastiN8.Tests.NeuralExcuseLab;

public class ExcuseModelFactoryTests
{
    [Fact]
    public void CreateModel_WithLocalProvider_Should_ReturnLocalModel()
    {
        // arrange
        var httpClient = new HttpClient();
        var factory = new ExcuseModelFactory(httpClient);

        // act
        var model = factory.CreateModel("local");

        // assert
        model.Should().BeOfType<LocalExcuseModel>();
        model.ModelName.Should().Be("LocalExcuseLLaMA-7B");
    }

    [Fact]
    public void CreateModel_WithFortuneProvider_Should_ReturnFortuneCookieModel()
    {
        // arrange
        var httpClient = new HttpClient();
        var factory = new ExcuseModelFactory(httpClient);

        // act
        var model = factory.CreateModel("fortune");

        // assert
        model.Should().BeOfType<FortuneCookieExcuseModel>();
        model.ModelName.Should().Be("FortuneCookie-API-v1");
    }

    [Fact]
    public void CreateModel_WithOpenAIProvider_Should_ReturnOpenAIModel()
    {
        // arrange
        var httpClient = new HttpClient();
        var factory = new ExcuseModelFactory(httpClient);
        var config = new Dictionary<string, object>
        {
            { "api_key", "test-key-12345" }
        };

        // act
        var model = factory.CreateModel("openai", config);

        // assert
        model.Should().BeOfType<OpenAIExcuseModel>();
        model.ModelName.Should().Be("OpenAI-GPT4");
    }

    [Fact]
    public void CreateModel_WithUnknownProvider_Should_ThrowException()
    {
        // arrange
        var httpClient = new HttpClient();
        var factory = new ExcuseModelFactory(httpClient);

        // act & assert
        var exception = Assert.Throws<ArgumentException>(() => factory.CreateModel("unknown-provider"));
        exception.Message.Should().Contain("Unknown provider");
    }

    [Fact]
    public void CreateModel_WithOpenAIProviderAndNoApiKey_Should_ThrowException()
    {
        // arrange
        var httpClient = new HttpClient();
        var factory = new ExcuseModelFactory(httpClient);

        // act & assert
        var exception = Assert.Throws<ArgumentException>(() => factory.CreateModel("openai"));
        exception.Message.Should().Contain("API key is required");
    }

    [Fact]
    public void GetRegisteredProviders_Should_ReturnAllProviders()
    {
        // arrange
        var httpClient = new HttpClient();
        var factory = new ExcuseModelFactory(httpClient);

        // act
        var providers = factory.GetRegisteredProviders();

        // assert
        providers.Should().Contain("openai");
        providers.Should().Contain("local");
        providers.Should().Contain("fortune");
    }

    [Fact]
    public void CreateModel_WithCustomModelPath_Should_UseCustomPath()
    {
        // arrange
        var httpClient = new HttpClient();
        var factory = new ExcuseModelFactory(httpClient);
        var config = new Dictionary<string, object>
        {
            { "model_path", "custom/model.gguf" }
        };

        // act
        var model = factory.CreateModel("local", config);
        var metadata = model.GetMetadata();

        // assert
        metadata["model_path"].Should().Be("custom/model.gguf");
    }
}
