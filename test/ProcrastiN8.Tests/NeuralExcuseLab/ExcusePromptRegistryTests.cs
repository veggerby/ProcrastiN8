using ProcrastiN8.NeuralExcuseLab;

namespace ProcrastiN8.Tests.NeuralExcuseLab;

public class ExcusePromptRegistryTests
{
    [Fact]
    public void Constructor_Should_RegisterDefaultPrompts()
    {
        // arrange & act
        var registry = new ExcusePromptRegistry();
        var prompts = registry.GetAllPrompts();

        // assert
        prompts.Should().HaveCountGreaterThan(0, "default prompts should be registered");
    }

    [Fact]
    public void GetActivePrompt_Should_ReturnV1ByDefault()
    {
        // arrange
        var registry = new ExcusePromptRegistry();

        // act
        var activePrompt = registry.GetActivePrompt();

        // assert
        activePrompt.Version.Should().Be("v1.0");
        activePrompt.IsActive.Should().BeTrue();
    }

    [Fact]
    public void RegisterPrompt_Should_AddNewPrompt()
    {
        // arrange
        var registry = new ExcusePromptRegistry();

        // act
        registry.RegisterPrompt("v3.0", "Custom template", "custom-tone");
        var prompt = registry.GetPrompt("v3.0");

        // assert
        prompt.Template.Should().Be("Custom template");
        prompt.Tone.Should().Be("custom-tone");
    }

    [Fact]
    public void SetActiveVersion_Should_ChangeActivePrompt()
    {
        // arrange
        var registry = new ExcusePromptRegistry();

        // act
        registry.SetActiveVersion("v1.1");
        var activePrompt = registry.GetActivePrompt();

        // assert
        activePrompt.Version.Should().Be("v1.1");
    }

    [Fact]
    public void SetActiveVersion_WithInvalidVersion_Should_ThrowException()
    {
        // arrange
        var registry = new ExcusePromptRegistry();

        // act & assert
        var exception = Assert.Throws<ArgumentException>(() => registry.SetActiveVersion("v99.0"));
        exception.Message.Should().Contain("not found");
    }

    [Fact]
    public void GetPrompt_WithInvalidVersion_Should_ThrowException()
    {
        // arrange
        var registry = new ExcusePromptRegistry();

        // act & assert
        var exception = Assert.Throws<ArgumentException>(() => registry.GetPrompt("v99.0"));
        exception.Message.Should().Contain("not found");
    }

    [Fact]
    public void RegisterPrompt_WithActiveFlag_Should_SetAsActive()
    {
        // arrange
        var registry = new ExcusePromptRegistry();

        // act
        registry.RegisterPrompt("v3.0", "Custom template", "custom-tone", isActive: true);
        var activePrompt = registry.GetActivePrompt();

        // assert
        activePrompt.Version.Should().Be("v3.0", "newly registered active prompt should become active");
    }
}
