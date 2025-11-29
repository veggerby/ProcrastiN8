using ProcrastiN8.RulesEngine.Policies;

namespace ProcrastiN8.Tests.RulesEngine;

public class BuiltInPolicyPackTests
{
    [Fact]
    public void Iso9001ProcrastinationPolicyPack_HasCorrectMetadata()
    {
        // Arrange & act
        var policy = new Iso9001ProcrastinationPolicyPack();

        // Assert
        policy.Id.Should().Be("ISO-9001-Procrastination", "ISO compliance must be precisely named");
        policy.Name.Should().Contain("ISO 9001");
        policy.Version.Should().Be(new Version(1, 0, 0));
        policy.Rules.Should().NotBeEmpty("ISO standards require documentation");
    }

    [Fact]
    public void Iso9001ProcrastinationPolicyPack_HasDocumentationComplianceRule()
    {
        // Arrange
        var policy = new Iso9001ProcrastinationPolicyPack();

        // Act
        var rule = policy.Rules.FirstOrDefault(r => r.Id == "ISO-9001-PROC-003");

        // Assert - every ISO standard needs baseline documentation requirements
        rule.Should().NotBeNull("baseline documentation compliance rule is required");
        rule!.Name.Should().Contain("Documentation", "ISO loves documentation");
    }

    [Fact]
    public void AgileButNotReallyPolicyPack_HasCorrectMetadata()
    {
        // Arrange & act
        var policy = new AgileButNotReallyPolicyPack();

        // Assert
        policy.Id.Should().Be("Agile-But-Not-Really", "enterprise agile has its own brand");
        policy.Name.Should().Contain("Agile");
        policy.Version.Should().Be(new Version(2, 0, 0), "version 2.0 because 1.0 was too agile");
        policy.Rules.Should().NotBeEmpty("agile requires ceremonies");
    }

    [Fact]
    public void AgileButNotReallyPolicyPack_BlocksUngroomedItems()
    {
        // Arrange
        var policy = new AgileButNotReallyPolicyPack();

        // Act
        var rule = policy.Rules.FirstOrDefault(r => r.Id == "AGILE-001");

        // Assert - ungroomed backlog items can never proceed
        rule.Should().NotBeNull("grooming gate is mandatory in enterprise agile");
        rule!.Priority.Should().Be(5, "blocking rules have high priority");
    }

    [Fact]
    public void AgileButNotReallyPolicyPack_HasFridaySprintBoundary()
    {
        // Arrange
        var policy = new AgileButNotReallyPolicyPack();

        // Act
        var rule = policy.Rules.FirstOrDefault(r => r.Id == "AGILE-003");

        // Assert - nothing ships on Friday
        rule.Should().NotBeNull("Friday deployments destabilize sprints");
    }

    [Fact]
    public void GdprForFeelingsPolicyPack_HasCorrectMetadata()
    {
        // Arrange & act
        var policy = new GdprForFeelingsPolicyPack();

        // Assert
        policy.Id.Should().Be("GDPR-For-Feelings", "emotional data needs protection too");
        policy.Name.Should().Contain("GDPR");
        policy.Version.Should().Be(new Version(1, 2, 0));
        policy.Rules.Should().NotBeEmpty("feelings require compliance rules");
    }

    [Fact]
    public void GdprForFeelingsPolicyPack_RequiresStressConsent()
    {
        // Arrange
        var policy = new GdprForFeelingsPolicyPack();

        // Act
        var rule = policy.Rules.FirstOrDefault(r => r.Id == "GDPR-F-001");

        // Assert - stressful tasks need explicit consent
        rule.Should().NotBeNull("stress consent is required under GDPR-F");
        rule!.Name.Should().Contain("Consent");
    }

    [Fact]
    public void GdprForFeelingsPolicyPack_HasPrivacyNotice()
    {
        // Arrange
        var policy = new GdprForFeelingsPolicyPack();

        // Act
        var rule = policy.Rules.FirstOrDefault(r => r.Id == "GDPR-F-004");

        // Assert - all tasks must display privacy notice
        rule.Should().NotBeNull("privacy notice is mandatory");
        rule!.Priority.Should().Be(100, "privacy notices apply to all tasks as a baseline");
    }

    [Fact]
    public void AllBuiltInPolicies_HaveUniqueRuleIds()
    {
        // Arrange
        var policies = new IPolicyPack[]
        {
            new Iso9001ProcrastinationPolicyPack(),
            new AgileButNotReallyPolicyPack(),
            new GdprForFeelingsPolicyPack()
        };

        // Act
        var allRuleIds = policies.SelectMany(p => p.Rules.Select(r => r.Id)).ToList();

        // Assert - no duplicate rule IDs across all policies
        allRuleIds.Should().OnlyHaveUniqueItems("each rule must have a unique ID for traceability");
    }

    [Fact]
    public void AllBuiltInPolicies_HaveDescriptiveMetadata()
    {
        // Arrange
        var policies = new IPolicyPack[]
        {
            new Iso9001ProcrastinationPolicyPack(),
            new AgileButNotReallyPolicyPack(),
            new GdprForFeelingsPolicyPack()
        };

        // Assert
        foreach (var policy in policies)
        {
            policy.Description.Should().NotBeNullOrEmpty($"Policy {policy.Id} needs a description");
            policy.Metadata.Should().NotBeEmpty($"Policy {policy.Id} should have metadata");
        }
    }
}
