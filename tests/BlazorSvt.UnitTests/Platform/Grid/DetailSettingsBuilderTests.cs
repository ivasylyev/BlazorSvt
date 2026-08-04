using BlazorSvt.Platform.Grid.Models;
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
    public void Add_VisibleSelector_IsBusinessOnly_EmptyStringsRemainVisible()
    {
        var settings = new DetailSettingsBuilder<SampleDto>(platform)
            .Add("G", x => x.Name, "Name")
            .Build();

        var field = settings.GroupSettings["G"].Single();
        field.VisibleSelector(new SampleDto { Name = null }).Should().BeTrue();
        field.VisibleSelector(new SampleDto { Name = "   " }).Should().BeTrue();
        field.VisibleSelector(new SampleDto { Name = "A" }).Should().BeTrue();
    }

    [Fact]
    public void Add_RespectsExplicitVisible()
    {
        var settings = new DetailSettingsBuilder<SampleDto>(platform)
            .Add("G", x => x.Name, "Name", visible: dto => dto.Flag)
            .Build();

        var field = settings.GroupSettings["G"].Single();
        field.VisibleSelector(new SampleDto { Flag = true, Name = "" }).Should().BeTrue();
        field.VisibleSelector(new SampleDto { Flag = false, Name = "X" }).Should().BeFalse();
    }

    [Fact]
    public void AddYesNo_KeepsBoolVisible_AndLocalizes()
    {
        var settings = new DetailSettingsBuilder<SampleDto>(platform)
            .AddYesNo("G", x => x.Flag, "Flag")
            .Build();

        var field = settings.GroupSettings["G"].Single();
        var dto = new SampleDto { Flag = false };
        field.VisibleSelector(dto).Should().BeTrue();
        field.DisplaySelector(dto).ToString().Should().Be("Common.No");
    }

    [Fact]
    public void AddYesNo_NullableBool_LocalizesValues()
    {
        var settings = new DetailSettingsBuilder<SampleDto>(platform)
            .AddYesNo("G", x => x.NullableFlag, "Flag")
            .Build();

        var field = settings.GroupSettings["G"].Single();
        field.VisibleSelector(new SampleDto { NullableFlag = null }).Should().BeTrue();
        field.DisplaySelector(new SampleDto { NullableFlag = null }).Should().Be(string.Empty);
        field.DisplaySelector(new SampleDto { NullableFlag = true }).ToString().Should().Be("Common.Yes");
        field.DisplaySelector(new SampleDto { NullableFlag = false }).ToString().Should().Be("Common.No");
    }

    private sealed class SampleDto
    {
        public string? Name { get; set; }
        public bool Flag { get; set; }
        public bool? NullableFlag { get; set; }
    }
}

[Trait("Category", "Unit")]
public class DetailDisplayValueTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData("x", true)]
    public void HasMeaningfulValue_Strings(string? value, bool expected) =>
        DetailDisplayValue.HasMeaningfulValue(value).Should().Be(expected);

    [Fact]
    public void HasMeaningfulValue_NonString_IsTrue()
    {
        DetailDisplayValue.HasMeaningfulValue(0).Should().BeTrue();
        DetailDisplayValue.HasMeaningfulValue(false).Should().BeTrue();
    }

    [Fact]
    public void IsVisible_RespectsHideEmptyAndBusinessVisible()
    {
        var setting = new DetailSetting<Sample>
        {
            Name = "Name",
            Header = "Name",
            GroupHeader = "G",
            DisplaySelector = s => s.Name ?? string.Empty,
            VisibleSelector = s => s.Allowed
        };

        DetailDisplayValue.IsVisible(setting, new Sample { Allowed = true, Name = "A" }, hideEmptyFields: true)
            .Should().BeTrue();
        DetailDisplayValue.IsVisible(setting, new Sample { Allowed = true, Name = "" }, hideEmptyFields: true)
            .Should().BeFalse();
        DetailDisplayValue.IsVisible(setting, new Sample { Allowed = true, Name = "" }, hideEmptyFields: false)
            .Should().BeTrue();
        DetailDisplayValue.IsVisible(setting, new Sample { Allowed = false, Name = "A" }, hideEmptyFields: false)
            .Should().BeFalse();
    }

    private sealed class Sample
    {
        public bool Allowed { get; set; }
        public string? Name { get; set; }
    }
}
