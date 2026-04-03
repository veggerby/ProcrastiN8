using System.Reflection;

using ProcrastiN8.Services;
using ProcrastiN8.TODOFramework;

namespace ProcrastiN8.Tests.Services;

// A deliberately debt-laden class for scanning in tests
[WontFix("The class design is a strategic architectural decision.")]
[SomedayMaybe("Refactor this entire class", estimatedYear: 2035)]
file sealed class DebtLadenClass
{
    [WontFix("Performance is within acceptable parameters for a class that does nothing.")]
    public void DoNothing() { }

    [SomedayMaybe("Add some functionality here")]
    public int AlsoDoNothing() => 0;

    [WontFix]
    [SomedayMaybe("Remove this method", estimatedYear: 2040)]
    public void DoEvenLessNothing() { }
}

public class TechnicalDebtCollectorTests
{
    private static readonly Assembly TestAssembly = Assembly.GetExecutingAssembly();

    [Fact]
    public void Collect_FindsWontFixAttributes()
    {
        // arrange
        var collector = new TechnicalDebtCollector();

        // act
        var report = collector.Collect(TestAssembly);

        // assert
        report.WontFixCount.Should().BeGreaterThan(0, "the test assembly contains deliberate WontFix annotations");
    }

    [Fact]
    public void Collect_FindsSomedayMaybeAttributes()
    {
        // arrange
        var collector = new TechnicalDebtCollector();

        // act
        var report = collector.Collect(TestAssembly);

        // assert
        report.SomedayMaybeCount.Should().BeGreaterThan(0, "the test assembly contains deliberate SomedayMaybe annotations");
    }

    [Fact]
    public void Collect_TotalDebt_IsSum()
    {
        // arrange
        var collector = new TechnicalDebtCollector();

        // act
        var report = collector.Collect(TestAssembly);

        // assert
        report.TotalDebt.Should().Be(
            report.WontFixCount + report.SomedayMaybeCount,
            "total debt is the honest sum of WontFix and SomedayMaybe items");
    }

    [Fact]
    public void Collect_Reports_ScannedAssemblies()
    {
        // arrange
        var collector = new TechnicalDebtCollector();

        // act
        var report = collector.Collect(TestAssembly);

        // assert
        report.ScannedAssemblies.Should().Contain(TestAssembly,
            "the provided assembly is recorded in the report for accountability");
    }

    [Fact]
    public void ToFormattedReport_ContainsDebtSummary()
    {
        // arrange
        var collector = new TechnicalDebtCollector();
        var report = collector.Collect(TestAssembly);

        // act
        var formatted = report.ToFormattedReport();

        // assert
        formatted.Should().Contain("# Technical Debt Report", "the report has a title suitable for executive review");
        formatted.Should().Contain("WontFix", "WontFix items are listed");
        formatted.Should().Contain("SomedayMaybe", "SomedayMaybe items are listed");
    }

    [Fact]
    public void Collect_EmptyAssemblyList_ScansCallingAssembly()
    {
        // arrange — when no assemblies are specified, the collector should scan something
        var collector = new TechnicalDebtCollector();

        // act — calling with no arguments scans the calling assembly
        var report = collector.Collect(TestAssembly);

        // assert
        report.Should().NotBeNull("a report is always produced, even for pristine codebases");
        report.GeneratedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, precision: TimeSpan.FromSeconds(5));
    }
}
