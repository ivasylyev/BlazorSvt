using BlazorSvt.Modules.SmartRateLookup.Domain;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Modules.SmartRateLookup;

[Trait("Category", "Unit")]
public class SmartRateRankerTests
{
    private static readonly DateOnly Start = new(2026, 10, 1);
    private static readonly DateOnly End = new(2026, 12, 31);

    private static readonly IReadOnlyList<LookupStep> Steps =
        LookupStepBuilder.Build(LookupStepBuilderTests.Input(region: 300, group: 50, new DpoNode(901, "D1")));

    [Fact]
    public void Pick_WhenNoCandidates_ReturnsNull()
    {
        SmartRateRanker.Pick(Steps, []).Should().BeNull();
    }

    [Fact]
    public void Pick_ExactMatch_IsLevelOne()
    {
        var best = SmartRateRanker.Pick(Steps, [Rate(1, product: 10)]);

        best!.Step.Level.Should().Be(1);
    }

    [Fact]
    public void Pick_RateTypePriority_BeatsLevel()
    {
        var exactLowPriority = Rate(1, product: 10, priority: 2);
        var liftedHighPriority = Rate(2, nodeTo: 300, product: null, group: 50, type: 8, kind: 70, priority: 1);

        var best = SmartRateRanker.Pick(Steps, [exactLowPriority, liftedHighPriority]);

        best!.Candidate.AverageRateLevel3Id.Should().Be(2);
        best.Step.Level.Should().Be(8);
    }

    [Fact]
    public void Pick_SamePriority_LowerLevelWins()
    {
        var groupLevel = Rate(1, product: null, group: 50);
        var exact = Rate(2, product: 10);

        SmartRateRanker.Pick(Steps, [groupLevel, exact])!.Candidate.AverageRateLevel3Id.Should().Be(2);
    }

    [Fact]
    public void Pick_MainNode_BeatsDpoNode()
    {
        var dpoExact = Rate(1, nodeFrom: 901, product: 10);
        var mainLifted = Rate(2, nodeTo: 300, product: null, group: 50, type: 8, kind: 70);

        var best = SmartRateRanker.Pick(Steps, [dpoExact, mainLifted]);

        best!.Candidate.AverageRateLevel3Id.Should().Be(2);
        best.Step.ReplacedDepartureNode.Should().BeNull();
    }

    [Fact]
    public void Pick_WithinGroup_ClosestToAverageWins_ThenLarger()
    {
        // Средняя по группе = 200: 100 и 300 равноудалены, 210 ближе всех.
        var candidates = new[] { Rate(1, product: 10, value: 100), Rate(2, product: 10, value: 300), Rate(3, product: 10, value: 200 + 10) };
        SmartRateRanker.Pick(Steps, candidates)!.Candidate.AverageRateLevel3Id.Should().Be(3);

        var tie = new[] { Rate(1, product: 10, value: 100), Rate(2, product: 10, value: 300) };
        SmartRateRanker.Pick(Steps, tie)!.Candidate.AverageRateLevel3Id.Should().Be(2);
    }

    [Fact]
    public void Pick_AverageIsPerGroup()
    {
        // Разные группы (разные периоды): средняя у каждой своя, расстояние до неё 0 у обеих — побеждает бо́льшая.
        var a = Rate(1, product: 10, value: 100);
        var b = Rate(2, product: 10, value: 500) with { StartDate = new DateOnly(2026, 9, 1) };

        SmartRateRanker.Pick(Steps, [a, b])!.Candidate.AverageRateLevel3Id.Should().Be(2);
    }

    [Fact]
    public void Matches_GroupLevel_OnlyGroupRatesWithoutProduct()
    {
        var groupStep = Steps.First(s => s.Level == 2);

        SmartRateRanker.Matches(groupStep, Rate(1, product: null, group: 50)).Should().BeTrue();
        SmartRateRanker.Matches(groupStep, Rate(2, product: 11, group: 50)).Should().BeFalse();
    }

    [Fact]
    public void Matches_KindLevel_AcceptsOtherTypeOfSameKind()
    {
        var kindStep = Steps.First(s => s.Level == 5);

        SmartRateRanker.Matches(kindStep, Rate(1, product: 10, type: 8, kind: 70)).Should().BeTrue();
        SmartRateRanker.Matches(kindStep, Rate(2, product: 10, type: 8, kind: 71)).Should().BeFalse();
        SmartRateRanker.Matches(Steps.First(s => s.Level == 1), Rate(3, product: 10, type: 8, kind: 70)).Should().BeFalse();
    }

    private static RateCandidate Rate(
        long id,
        long nodeFrom = 100,
        long nodeTo = 200,
        long? product = 10,
        long? group = 50,
        long type = 7,
        long kind = 70,
        decimal priority = 1,
        decimal value = 100) =>
        new(id, nodeFrom, nodeTo, product, group, type, kind, RateTypeId: 1, priority, Start, End, value);
}
