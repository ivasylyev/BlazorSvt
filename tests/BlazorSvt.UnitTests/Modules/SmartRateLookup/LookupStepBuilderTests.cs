using BlazorSvt.Modules.SmartRateLookup.Domain;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Modules.SmartRateLookup;

[Trait("Category", "Unit")]
public class LookupStepBuilderTests
{
    internal static ResolvedLookupInput Input(long? region = 300, long? group = 50, params DpoNode[] dpo) =>
        new(
            NodeFromId: 100,
            NodeToId: 200,
            RegionNodeToId: region,
            ProductId: 10,
            ProductGroupId: group,
            TransportTypeId: 7,
            TransportKindId: 70,
            DpoNodes: dpo);

    [Fact]
    public void Build_WithRegionAndGroup_ReturnsEightLevelsInOrder()
    {
        var steps = LookupStepBuilder.Build(Input());

        steps.Select(s => s.Level).Should().Equal(1, 2, 3, 4, 5, 6, 7, 8);
        steps.Select(s => s.Order).Should().Equal(1, 2, 3, 4, 5, 6, 7, 8);
        steps.Should().OnlyContain(s => s.StartNodeId == 100 && s.ReplacedDepartureNode == null);
    }

    [Fact]
    public void Build_LevelsLiftExpectedAttributes()
    {
        var steps = LookupStepBuilder.Build(Input()).ToDictionary(s => s.Level);

        steps[1].Should().BeEquivalentTo(new { EndNodeId = 200L, ProductId = (long?)10, ProductGroupId = (long?)null, TransportTypeId = (long?)7, TransportKindId = (long?)null });
        steps[2].Should().BeEquivalentTo(new { EndNodeId = 200L, ProductId = (long?)null, ProductGroupId = (long?)50, TransportTypeId = (long?)7 });
        steps[3].Should().BeEquivalentTo(new { EndNodeId = 300L, ProductId = (long?)10, TransportTypeId = (long?)7 });
        steps[5].Should().BeEquivalentTo(new { EndNodeId = 200L, ProductId = (long?)10, TransportTypeId = (long?)null, TransportKindId = (long?)70 });
        steps[8].Should().BeEquivalentTo(new { EndNodeId = 300L, ProductId = (long?)null, ProductGroupId = (long?)50, TransportKindId = (long?)70 });
    }

    [Fact]
    public void Build_WithoutRegion_SkipsRegionLevels()
    {
        var steps = LookupStepBuilder.Build(Input(region: null));

        steps.Select(s => s.Level).Should().Equal(1, 2, 5, 6);
        steps.Select(s => s.Order).Should().Equal(1, 2, 3, 4);
    }

    [Fact]
    public void Build_WithoutProductGroup_SkipsGroupLevels()
    {
        var steps = LookupStepBuilder.Build(Input(group: null));

        steps.Select(s => s.Level).Should().Equal(1, 3, 5, 7);
    }

    [Fact]
    public void Build_DpoNodes_AfterAllMainSteps_LevelMajor()
    {
        var steps = LookupStepBuilder.Build(Input(region: null, group: 50, new DpoNode(901, "D1"), new DpoNode(902, "D2")));

        steps.Take(4).Should().OnlyContain(s => s.StartNodeId == 100);
        steps.Skip(4).Select(s => (s.Level, s.StartNodeId)).Should().Equal(
            (1, 901L), (1, 902L), (2, 901L), (2, 902L), (5, 901L), (5, 902L), (6, 901L), (6, 902L));
        steps.Skip(4).Should().OnlyContain(s => s.ReplacedDepartureNode != null);
        steps.Select(s => s.Order).Should().BeInAscendingOrder().And.OnlyHaveUniqueItems();
    }
}
