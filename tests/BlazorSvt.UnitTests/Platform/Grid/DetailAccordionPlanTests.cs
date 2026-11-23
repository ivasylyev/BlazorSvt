using BlazorSvt.Platform.Grid.Services;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Grid;

[Trait("Category", "Unit")]
public class DetailAccordionPlanTests
{
    [Fact]
    public void Build_PlacesChildTablesBeforeFieldGroups_InSourceOrder()
    {
        var sections = DetailAccordionPlan.Build(
            [new DetailChildAccordionInput(false), new DetailChildAccordionInput(true)],
            visibleFieldGroupCount: 2);

        sections.Should().Equal(
            new DetailAccordionSection(DetailAccordionSectionKind.ChildTable, 0, false),
            new DetailAccordionSection(DetailAccordionSectionKind.ChildTable, 1, true),
            new DetailAccordionSection(DetailAccordionSectionKind.FieldGroup, 0, true),
            new DetailAccordionSection(DetailAccordionSectionKind.FieldGroup, 1, true));
    }

    [Fact]
    public void AllExpanded_IsFalseWhenAChildTableStartsCollapsed()
    {
        var sections = DetailAccordionPlan.Build([new DetailChildAccordionInput(false)], visibleFieldGroupCount: 2);

        DetailAccordionPlan.AllExpanded(sections).Should().BeFalse();
    }

    [Fact]
    public void AllExpanded_IsTrueWhenNoChildTableStartsCollapsed()
    {
        DetailAccordionPlan.AllExpanded(DetailAccordionPlan.Build([], visibleFieldGroupCount: 2)).Should().BeTrue();
        DetailAccordionPlan.AllExpanded(
            DetailAccordionPlan.Build([new DetailChildAccordionInput(true)], visibleFieldGroupCount: 1))
            .Should().BeTrue();
    }
}
