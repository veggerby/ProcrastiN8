using ProcrastiN8.RulesEngine;
using ProcrastiN8.RulesEngine.Actions;
using ProcrastiN8.RulesEngine.Conditions;
using ProcrastiN8.RulesEngine.Policies;

namespace ProcrastiN8.Tests.RulesEngine;

public class PolicyHostTests
{
    [Fact]
    public void PolicyHost_LoadsPolicy()
    {
        // arrange
        var host = new PolicyHost();
        var policy = new TestPolicyPack("TEST-001", "Test Policy", new Version(1, 0, 0));

        // act
        host.LoadPolicy(policy);

        // assert
        host.LoadedPolicies.Should().Contain(policy, "policy should be loaded");
        host.LoadedPolicies.Should().HaveCount(1);
    }

    [Fact]
    public void PolicyHost_UnloadsPolicy()
    {
        // arrange
        var host = new PolicyHost();
        var policy = new TestPolicyPack("TEST-001", "Test Policy", new Version(1, 0, 0));
        host.LoadPolicy(policy);

        // act
        var result = host.UnloadPolicy("TEST-001");

        // assert
        result.Should().BeTrue("policy should be successfully unloaded");
        host.LoadedPolicies.Should().BeEmpty("no policies should remain after unload");
    }

    [Fact]
    public void PolicyHost_UnloadPolicy_ReturnsFalseForUnknownPolicy()
    {
        // arrange
        var host = new PolicyHost();

        // act
        var result = host.UnloadPolicy("NONEXISTENT");

        // assert
        result.Should().BeFalse("cannot unload a policy that was never loaded");
    }

    [Fact]
    public void PolicyHost_AllRules_AggregatesFromAllPolicies()
    {
        // arrange
        var host = new PolicyHost();
        var policy1 = new TestPolicyPack("P1", "Policy 1", new Version(1, 0, 0));
        policy1.AddTestRule(new ProcrastinationRule("R1", "Rule 1", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(5)), 10));

        var policy2 = new TestPolicyPack("P2", "Policy 2", new Version(1, 0, 0));
        policy2.AddTestRule(new ProcrastinationRule("R2", "Rule 2", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(10)), 20));

        host.LoadPolicy(policy1);
        host.LoadPolicy(policy2);

        // act
        var allRules = host.AllRules;

        // assert
        allRules.Should().HaveCount(2, "both rules from both policies should be aggregated");
        allRules.Select(r => r.Id).Should().Contain("R1").And.Contain("R2");
    }

    [Fact]
    public void PolicyHost_AllRules_SortedByPriority()
    {
        // arrange
        var host = new PolicyHost();
        var policy = new TestPolicyPack("P1", "Policy 1", new Version(1, 0, 0));
        policy.AddTestRule(new ProcrastinationRule("R1", "Low Priority", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(5)), 100));
        policy.AddTestRule(new ProcrastinationRule("R2", "High Priority", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(5)), 10));
        policy.AddTestRule(new ProcrastinationRule("R3", "Medium Priority", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(5)), 50));

        host.LoadPolicy(policy);

        // act
        var allRules = host.AllRules;

        // assert - lower priority value = higher priority, so should be first
        allRules[0].Id.Should().Be("R2", "highest priority rule should be first");
        allRules[1].Id.Should().Be("R3", "medium priority rule should be second");
        allRules[2].Id.Should().Be("R1", "lowest priority rule should be last");
    }

    [Fact]
    public void PolicyHost_CreateSnapshot_CreatesRestorableState()
    {
        // arrange
        var host = new PolicyHost();
        var policy1 = new TestPolicyPack("P1", "Policy 1", new Version(1, 0, 0));
        host.LoadPolicy(policy1);

        // act
        var snapshotId = host.CreateSnapshot();

        // assert
        snapshotId.Should().NotBeNullOrEmpty("snapshot ID should be generated");
    }

    [Fact]
    public void PolicyHost_RollbackTo_RestoresPreviousState()
    {
        // arrange
        var host = new PolicyHost();
        var policy1 = new TestPolicyPack("P1", "Policy 1", new Version(1, 0, 0));
        host.LoadPolicy(policy1);

        var snapshotId = host.CreateSnapshot();

        var policy2 = new TestPolicyPack("P2", "Policy 2", new Version(1, 0, 0));
        host.LoadPolicy(policy2);

        host.LoadedPolicies.Should().HaveCount(2, "two policies should be loaded before rollback");

        // act
        var result = host.RollbackTo(snapshotId);

        // assert
        result.Should().BeTrue("rollback should succeed");
        host.LoadedPolicies.Should().HaveCount(1, "should only have policies from snapshot");
        host.LoadedPolicies.Single().Id.Should().Be("P1", "should be the policy from snapshot");
    }

    [Fact]
    public void PolicyHost_RollbackTo_ReturnsFalseForUnknownSnapshot()
    {
        // arrange
        var host = new PolicyHost();

        // act
        var result = host.RollbackTo("NONEXISTENT");

        // assert
        result.Should().BeFalse("cannot rollback to unknown snapshot");
    }

    [Fact]
    public void PolicyHost_GetHistory_RecordsPolicyChanges()
    {
        // arrange
        var host = new PolicyHost();
        var policy = new TestPolicyPack("P1", "Policy 1", new Version(1, 0, 0));

        // act
        host.LoadPolicy(policy);
        host.UnloadPolicy("P1");

        var history = host.GetHistory();

        // assert
        history.Should().HaveCount(2, "load and unload should be recorded");
        history[0].ChangeType.Should().Be(PolicyChangeType.Loaded);
        history[1].ChangeType.Should().Be(PolicyChangeType.Unloaded);
    }

    [Fact]
    public void PolicyHost_RaisesPolicyLoadedEvent()
    {
        // arrange
        var host = new PolicyHost();
        var policy = new TestPolicyPack("P1", "Policy 1", new Version(1, 0, 0));
        IPolicyPack? loadedPolicy = null;

        host.PolicyLoaded += (sender, args) => loadedPolicy = args.PolicyPack;

        // act
        host.LoadPolicy(policy);

        // assert
        loadedPolicy.Should().Be(policy, "event should be raised with loaded policy");
    }

    [Fact]
    public void PolicyHost_RaisesPolicyUnloadedEvent()
    {
        // arrange
        var host = new PolicyHost();
        var policy = new TestPolicyPack("P1", "Policy 1", new Version(1, 0, 0));
        string? unloadedPolicyId = null;

        host.PolicyUnloaded += (sender, args) => unloadedPolicyId = args.PolicyId;
        host.LoadPolicy(policy);

        // act
        host.UnloadPolicy("P1");

        // assert
        unloadedPolicyId.Should().Be("P1", "event should be raised with unloaded policy ID");
    }

    private sealed class TestPolicyPack : PolicyPackBase
    {
        public override string Id { get; }
        public override string Name { get; }
        public override string Description => "Test policy pack for unit tests.";
        public override Version Version { get; }

        public TestPolicyPack(string id, string name, Version version)
        {
            Id = id;
            Name = name;
            Version = version;
        }

        public void AddTestRule(IRule rule)
        {
            AddRule(rule);
        }
    }
}
