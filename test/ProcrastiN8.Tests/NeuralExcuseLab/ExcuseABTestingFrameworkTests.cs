using ProcrastiN8.JustBecause;
using ProcrastiN8.NeuralExcuseLab;

namespace ProcrastiN8.Tests.NeuralExcuseLab;

public class ExcuseABTestingFrameworkTests
{
    [Fact]
    public void RegisterVariant_Should_AddVariant()
    {
        // arrange
        var framework = new ExcuseABTestingFramework();

        // act
        framework.RegisterVariant("control");
        framework.RegisterVariant("variant_a");

        // assert - should not throw
        var selected = framework.SelectVariant();
        selected.Should().BeOneOf("control", "variant_a");
    }

    [Fact]
    public void SelectVariant_WithNoVariants_Should_ThrowException()
    {
        // arrange
        var framework = new ExcuseABTestingFramework();

        // act & assert
        var exception = Assert.Throws<InvalidOperationException>(() => framework.SelectVariant());
        exception.Message.Should().Contain("No variants registered");
    }

    [Fact]
    public async Task RecordImpression_Should_UpdateVariantMetrics()
    {
        // arrange
        var framework = new ExcuseABTestingFramework();
        framework.RegisterVariant("control");

        // act
        framework.RecordImpression("control", 85.0, 42.0);
        framework.RecordImpression("control", 90.0, 38.0);

        // assert - should not throw, metrics will be verified in report
        var report = await framework.GenerateReportAsync();
        report.Should().Contain("control");
    }

    [Fact]
    public void RecordImpression_WithUnregisteredVariant_Should_ThrowException()
    {
        // arrange
        var framework = new ExcuseABTestingFramework();

        // act & assert
        var exception = Assert.Throws<ArgumentException>(() => framework.RecordImpression("unknown", 85.0, 42.0));
        exception.Message.Should().Contain("not registered");
    }

    [Fact]
    public async Task GenerateReportAsync_Should_IncludeVariantStatistics()
    {
        // arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.03);
        var framework = new ExcuseABTestingFramework(randomProvider);
        framework.RegisterVariant("control");
        framework.RecordImpression("control", 85.0, 42.0);

        // act
        var report = await framework.GenerateReportAsync();

        // assert
        report.Should().Contain("Variant: control");
        report.Should().Contain("Impressions: 1");
        report.Should().Contain("Avg Quality Score:");
        report.Should().Contain("Avg Shame Index:");
    }

    [Fact]
    public async Task GenerateReportAsync_WithMultipleVariants_Should_IncludeStatisticalSignificance()
    {
        // arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.03);
        var framework = new ExcuseABTestingFramework(randomProvider);
        framework.RegisterVariant("control");
        framework.RegisterVariant("variant_a");
        framework.RecordImpression("control", 85.0, 42.0);
        framework.RecordImpression("variant_a", 90.0, 38.0);

        // act
        var report = await framework.GenerateReportAsync();

        // assert
        report.Should().Contain("Statistical Significance");
        report.Should().Contain("p-value:");
        report.Should().Contain("Confidence Interval:");
    }

    [Fact]
    public void SelectVariant_Should_DistributeRandomly()
    {
        // arrange
        var framework = new ExcuseABTestingFramework();
        framework.RegisterVariant("control");
        framework.RegisterVariant("variant_a");

        // act
        var selection1 = framework.SelectVariant();
        var selection2 = framework.SelectVariant();

        // assert
        selection1.Should().BeOneOf("control", "variant_a");
        selection2.Should().BeOneOf("control", "variant_a");
    }
}
