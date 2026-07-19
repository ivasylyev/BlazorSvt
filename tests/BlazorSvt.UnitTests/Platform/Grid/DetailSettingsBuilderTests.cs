using BlazorSvt.Platform.Grid.Services;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using NSubstitute;
using PlatformResources = BlazorSvt.Platform.Resources.Platform;

namespace BlazorSvt.UnitTests.Platform.Grid;

[Trait("Category", "Unit")]
public class DetailSettingsBuilderTests
{
    private readonly IStringLocalizer<PlatformResources> platform =
        Substitute.For<IStringLocalizer<PlatformResources>>();

    public DetailSettingsBuilderTests()
    {
        platform[Arg.Any<string>()].Returns(call => new LocalizedString(call.Arg<string>(), call.Arg<string>()));
    }

    [Fact]
    public void Add_HidesNullAndEmptyStringValues()
    {
        var settings = new DetailSettingsBuilder<SampleDto>(platform)
            .Add("G", x => x.Name, "Name")
            .Add("G", x => x.Code, "Code")
            .Build();

        var empty = new SampleDto { Name = null, Code = "   " };
        var filled = new SampleDto { Name = "A", Code = "B" };

        var fields = settings.GroupSettings["G"].ToList();
        fields.Should().HaveCount(2);
        fields[0].VisibleSelector(empty).Should().BeFalse();
        fields[1].VisibleSelector(empty).Should().BeFalse();
        fields[0].VisibleSelector(filled).Should().BeTrue();
        fields[1].VisibleSelector(filled).Should().BeTrue();
    }

    [Fact]
    public void Add_HidesWhenCustomDisplayReturnsEmpty()
    {
        var settings = new DetailSettingsBuilder<SampleDto>(platform)
            .Add("G", x => x.Amount, "Amount", display: dto => dto.Amount != 0 ? dto.Amount : string.Empty)
            .Build();

        var field = settings.GroupSettings["G"].Single();
        field.VisibleSelector(new SampleDto { Amount = 0 }).Should().BeFalse();
        field.VisibleSelector(new SampleDto { Amount = 5 }).Should().BeTrue();
    }

    [Fact]
    public void Add_KeepsBoolAndNonEmptyValuesVisible()
    {
        var settings = new DetailSettingsBuilder<SampleDto>(platform)
            .AddYesNo("G", x => x.Flag, "Flag")
            .Add("G", x => x.Amount, "Amount")
            .Build();

        var fields = settings.GroupSettings["G"].ToList();
        var dto = new SampleDto { Flag = false, Amount = 0 };

        fields[0].VisibleSelector(dto).Should().BeTrue();
        fields[1].VisibleSelector(dto).Should().BeTrue();
    }

    [Fact]
    public void Add_RespectsExplicitVisibleAndStillHidesEmpty()
    {
        var settings = new DetailSettingsBuilder<SampleDto>(platform)
            .Add("G", x => x.Name, "Name", visible: dto => dto.Flag)
            .Build();

        var field = settings.GroupSettings["G"].Single();
        field.VisibleSelector(new SampleDto { Flag = true, Name = "X" }).Should().BeTrue();
        field.VisibleSelector(new SampleDto { Flag = true, Name = "" }).Should().BeFalse();
        field.VisibleSelector(new SampleDto { Flag = false, Name = "X" }).Should().BeFalse();
    }

    private sealed class SampleDto
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public bool Flag { get; set; }
        public decimal Amount { get; set; }
    }
}
