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
    public void AddCreationUserAndAddLastChangeUser_UseLongHeaders()
    {
        var fields = new DetailSettingsBuilder<SampleDto>(platform)
            .AddCreationUser("G", x => x.Name, hasMargin: true)
            .AddLastChangeUser("G", x => x.NameRu)
            .Build()
            .GroupSettings["G"]
            .ToList();

        fields.Select(f => f.Name).Should().Equal(nameof(SampleDto.Name), nameof(SampleDto.NameRu));
        fields.Select(f => f.Header).Should().Equal("Common.CreationUser.Long", "Common.LastChangeUser.Long");
        fields.Select(f => f.HasMargin).Should().Equal(true, false);
        fields[0].DisplaySelector(new SampleDto { Name = "Иванов И.И." }).Should().Be("Иванов И.И.");
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
    public void Add_WhenDateTime_ShowsMoscowWallClock_AndNullStaysEmpty()
    {
        var settings = new DetailSettingsBuilder<SampleDto>(platform)
            .Add("G", x => x.Created, "Created")
            .Add("G", x => x.Changed, "Changed")
            .Build();

        var created = settings.GroupSettings["G"].Single(f => f.Name == nameof(SampleDto.Created));
        var changed = settings.GroupSettings["G"].Single(f => f.Name == nameof(SampleDto.Changed));

        created.DisplaySelector(new SampleDto { Created = new DateTime(2026, 11, 20, 22, 0, 0) })
            .Should().Be(new DateTime(2026, 11, 21, 1, 0, 0));
        changed.DisplaySelector(new SampleDto { Changed = null }).Should().Be(string.Empty);
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

    [Theory]
    [InlineData(true, "Red")]
    [InlineData(false, "Rouge")]
    public void AddEnum_NullableEnum_UsesDisplayNameOrEmpty(bool isRu, string expected)
    {
        var settings = new DetailSettingsBuilder<SampleDto>(platform)
            .AddEnum(isRu, "G", x => x.ColorRu, x => x.ColorEn, "Color")
            .Build();

        var field = settings.GroupSettings["G"].Single();
        field.DisplaySelector(new SampleDto { ColorRu = SampleColorRu.Red, ColorEn = SampleColorEn.Red }).Should().Be(expected);
        field.DisplaySelector(new SampleDto()).Should().Be(string.Empty);
    }

    // Сортировка — по значению свойства из Name, не по подписи. Подписи подобраны так, чтобы их порядок
    // был обратным порядку значений: сортировка по подписи тест провалит.

    private IStringLocalizer<PlatformResources> ReversedLabels()
    {
        var localizer = Substitute.For<IStringLocalizer<PlatformResources>>();
        localizer["Common.Yes"].Returns(new LocalizedString("Common.Yes", "A-Yes"));
        localizer["Common.No"].Returns(new LocalizedString("Common.No", "Z-No"));
        localizer["Common.Archive"].Returns(new LocalizedString("Common.Archive", "A-Archive"));
        localizer["Common.Active"].Returns(new LocalizedString("Common.Active", "Z-Active"));
        return localizer;
    }

    private static List<SampleDto> Ordered(DetailSetting<SampleDto> setting, params SampleDto[] rows) =>
        rows.OrderBy(setting.SortKeySelector.Compile()).ToList();

    [Fact]
    public void Add_SortKey_OrdersStringsByValue_NullFirst()
    {
        var setting = new DetailSettingsBuilder<SampleDto>(platform)
            .Add("G", x => x.Name, "Name")
            .Build().GroupSettings["G"].Single();

        var ordered = Ordered(setting,
            new SampleDto { Name = "b" }, new SampleDto { Name = null }, new SampleDto { Name = "a" });

        setting.Name.Should().Be(nameof(SampleDto.Name));
        ordered.Select(x => x.Name).Should().Equal(null, "a", "b");
    }

    [Fact]
    public void Add_SortKey_OrdersNumbersByValue_NotByText()
    {
        var setting = new DetailSettingsBuilder<SampleDto>(platform)
            .Add("G", x => x.Number, "Number")
            .Build().GroupSettings["G"].Single();

        var ordered = Ordered(setting,
            new SampleDto { Number = 100 }, new SampleDto { Number = 9 }, new SampleDto { Number = 10 });

        ordered.Select(x => x.Number).Should().Equal(9, 10, 100);
    }

    [Fact]
    public void Add_SortKey_OrdersDatesByValue_NullFirst()
    {
        var setting = new DetailSettingsBuilder<SampleDto>(platform)
            .Add("G", x => x.Changed, "Changed")
            .Build().GroupSettings["G"].Single();
        var early = new DateTime(2026, 1, 1);
        var late = new DateTime(2026, 12, 31);

        var ordered = Ordered(setting,
            new SampleDto { Changed = late }, new SampleDto { Changed = null }, new SampleDto { Changed = early });

        ordered.Select(x => x.Changed).Should().Equal(null, early, late);
    }

    [Theory]
    [InlineData(true, nameof(SampleDto.NameRu))]
    [InlineData(false, nameof(SampleDto.NameEn))]
    public void AddLocalized_SortKey_UsesPropertyOfCurrentLanguage(bool isRu, string expectedName)
    {
        var setting = new DetailSettingsBuilder<SampleDto>(platform)
            .AddLocalized(isRu, "G", x => x.NameRu, x => x.NameEn, "Name")
            .Build().GroupSettings["G"].Single();

        var ordered = Ordered(setting,
            new SampleDto { NameRu = "я", NameEn = "b" },
            new SampleDto { NameRu = "а", NameEn = "c" },
            new SampleDto { NameRu = "б", NameEn = "a" });

        setting.Name.Should().Be(expectedName);
        var expected = isRu ? new[] { "а", "б", "я" } : new[] { "a", "b", "c" };
        ordered.Select(x => isRu ? x.NameRu : x.NameEn).Should().Equal(expected);
    }

    [Theory]
    [InlineData(true, nameof(SampleDto.SizeRu))]
    [InlineData(false, nameof(SampleDto.SizeEn))]
    public void AddEnum_SortKey_OrdersByEnumValue_NotByLabel(bool isRu, string expectedName)
    {
        var setting = new DetailSettingsBuilder<SampleDto>(platform)
            .AddEnum(isRu, "G", x => x.SizeRu, x => x.SizeEn, "Size")
            .Build().GroupSettings["G"].Single();

        var ordered = Ordered(setting,
            new SampleDto { SizeRu = SampleSizeRu.Large, SizeEn = SampleSizeEn.Large },
            new SampleDto { SizeRu = SampleSizeRu.Small, SizeEn = SampleSizeEn.Small },
            new SampleDto { SizeRu = SampleSizeRu.Medium, SizeEn = SampleSizeEn.Medium });

        setting.Name.Should().Be(expectedName);
        ordered.Select(x => (int)x.SizeRu).Should().Equal(1, 2, 3);
        setting.DisplaySelector(ordered[0]).Should().NotBe(setting.DisplaySelector(ordered[2]));
    }

    [Theory]
    [InlineData(true, nameof(SampleDto.ColorRu))]
    [InlineData(false, nameof(SampleDto.ColorEn))]
    public void AddEnum_NullableEnum_SortKey_OrdersByEnumValue_NullFirst(bool isRu, string expectedName)
    {
        var setting = new DetailSettingsBuilder<SampleDto>(platform)
            .AddEnum(isRu, "G", x => x.ColorRu, x => x.ColorEn, "Color")
            .Build().GroupSettings["G"].Single();

        var ordered = Ordered(setting,
            new SampleDto { ColorRu = SampleColorRu.Blue, ColorEn = SampleColorEn.Blue },
            new SampleDto(),
            new SampleDto { ColorRu = SampleColorRu.Red, ColorEn = SampleColorEn.Red });

        setting.Name.Should().Be(expectedName);
        ordered.Select(x => x.ColorRu).Should().Equal(null, SampleColorRu.Red, SampleColorRu.Blue);
    }

    [Fact]
    public void AddYesNo_SortKey_OrdersByBoolValue_NotByLabel()
    {
        var setting = new DetailSettingsBuilder<SampleDto>(ReversedLabels())
            .AddYesNo("G", x => x.Flag, "Flag")
            .Build().GroupSettings["G"].Single();

        var ordered = Ordered(setting, new SampleDto { Flag = true }, new SampleDto { Flag = false });

        setting.Name.Should().Be(nameof(SampleDto.Flag));
        ordered.Select(x => x.Flag).Should().Equal(false, true);
        setting.DisplaySelector(ordered[0]).ToString().Should().Be("Z-No");
    }

    [Fact]
    public void AddYesNo_NullableBool_SortKey_OrdersByBoolValue_NullFirst()
    {
        var setting = new DetailSettingsBuilder<SampleDto>(ReversedLabels())
            .AddYesNo("G", x => x.NullableFlag, "Flag")
            .Build().GroupSettings["G"].Single();

        var ordered = Ordered(setting,
            new SampleDto { NullableFlag = true }, new SampleDto { NullableFlag = null }, new SampleDto { NullableFlag = false });

        setting.Name.Should().Be(nameof(SampleDto.NullableFlag));
        ordered.Select(x => x.NullableFlag).Should().Equal(null, false, true);
    }

    [Fact]
    public void AddArchiveStatus_SortKey_OrdersByBoolValue_NotByLabel()
    {
        var setting = new DetailSettingsBuilder<SampleDto>(ReversedLabels())
            .AddArchiveStatus("G", x => x.IsArchive, "Archive")
            .Build().GroupSettings["G"].Single();

        var ordered = Ordered(setting, new SampleDto { IsArchive = true }, new SampleDto { IsArchive = false });

        setting.Name.Should().Be(nameof(SampleDto.IsArchive));
        ordered.Select(x => x.IsArchive).Should().Equal(false, true);
        setting.DisplaySelector(ordered[0]).ToString().Should().Be("Z-Active");
    }

    private sealed class SampleDto
    {
        public string? Name { get; set; }
        public string? NameRu { get; set; }
        public string? NameEn { get; set; }
        public int Number { get; set; }
        public bool Flag { get; set; }
        public bool? NullableFlag { get; set; }
        public bool IsArchive { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Changed { get; set; }
        public SampleColorRu? ColorRu { get; set; }
        public SampleColorEn? ColorEn { get; set; }
        public SampleSizeRu SizeRu { get; set; }
        public SampleSizeEn SizeEn { get; set; }
    }

    private enum SampleColorRu
    {
        [System.ComponentModel.DataAnnotations.Display(Name = "Red")]
        Red = 1,

        [System.ComponentModel.DataAnnotations.Display(Name = "Azure")]
        Blue = 2
    }

    private enum SampleColorEn
    {
        [System.ComponentModel.DataAnnotations.Display(Name = "Rouge")]
        Red = 1,

        [System.ComponentModel.DataAnnotations.Display(Name = "Azur")]
        Blue = 2
    }

    private enum SampleSizeRu
    {
        [System.ComponentModel.DataAnnotations.Display(Name = "Zeta")]
        Small = 1,

        [System.ComponentModel.DataAnnotations.Display(Name = "Mu")]
        Medium = 2,

        [System.ComponentModel.DataAnnotations.Display(Name = "Alpha")]
        Large = 3
    }

    private enum SampleSizeEn
    {
        [System.ComponentModel.DataAnnotations.Display(Name = "Zed")]
        Small = 1,

        [System.ComponentModel.DataAnnotations.Display(Name = "Em")]
        Medium = 2,

        [System.ComponentModel.DataAnnotations.Display(Name = "Ay")]
        Large = 3
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
            SortKeySelector = s => s.Name ?? string.Empty,
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
