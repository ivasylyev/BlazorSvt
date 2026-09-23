using System.Globalization;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Services;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using NSubstitute;
using PlatformResources = BlazorSvt.Platform.Resources.Platform;

namespace BlazorSvt.UnitTests.Platform.Grid;

[Trait("Category", "Unit")]
public class GridColumnSettingsBuilderTests
{
    private readonly IStringLocalizer<PlatformResources> platform =
        Substitute.For<IStringLocalizer<PlatformResources>>();

    public GridColumnSettingsBuilderTests()
    {
        platform[Arg.Any<string>()].Returns(call => new LocalizedString(call.Arg<string>(), call.Arg<string>()));
    }

    [Fact]
    public void AddSystemColumns_WhenBuilt_HidesAuditColumnsAndLocalizesArchive()
    {
        var columns = new GridColumnSettingsBuilder<ColumnFixture>(platform)
            .AddSystemColumns(
                x => x.CreationDate,
                x => x.LastChangeDate,
                x => x.Flag,
                "Created",
                "Changed",
                "Archive")
            .Build();

        columns.Select(c => c.Name).Should().Equal(
            nameof(ColumnFixture.CreationDate),
            nameof(ColumnFixture.LastChangeDate),
            nameof(ColumnFixture.Flag));
        columns.Should().OnlyContain(c => c.Visible == false);

        var archive = columns.Single(c => c.Name == nameof(ColumnFixture.Flag));
        archive.Filterable.Should().BeTrue();
        archive.FilterValue.Should().Be("False");
        archive.DisplaySelector(new ColumnFixture { Flag = true }).ToString().Should().Be("Common.Archive");
        archive.DisplaySelector(new ColumnFixture { Flag = false }).ToString().Should().Be("Common.Active");
    }

    [Fact]
    public void AddYesNo_WhenBool_LocalizesYesAndNo()
    {
        var column = new GridColumnSettingsBuilder<ColumnFixture>(platform)
            .AddYesNo(x => x.Flag, "Flag")
            .Build()
            .Single();

        column.DisplaySelector(new ColumnFixture { Flag = true }).ToString().Should().Be("Common.Yes");
        column.DisplaySelector(new ColumnFixture { Flag = false }).ToString().Should().Be("Common.No");
    }

    [Fact]
    public void AddDateOnly_WhenCultureIsRu_FormatsShortDate()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");

            var column = new GridColumnSettingsBuilder<ColumnFixture>(platform)
                .AddDateOnly(x => x.StartDate, "Start")
                .Build()
                .Single();

            column.DisplaySelector(new ColumnFixture { StartDate = new DateOnly(2026, 9, 22) })
                .ToString()
                .Should()
                .Be("22.09.2026");
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void AddEnum_WhenTransportKindRuAuto_UsesDisplayName()
    {
        var column = new GridColumnSettingsBuilder<ColumnFixture>(platform)
            .AddEnum(x => x.KindRu, "Kind")
            .Build()
            .Single();

        column.Name.Should().Be(nameof(ColumnFixture.KindRu));
        column.DisplaySelector(new ColumnFixture { KindRu = TransportKindRu.Auto })
            .ToString()
            .Should()
            .Be("Автомобильный");
    }

    [Theory]
    [InlineData(true, nameof(ColumnFixture.KindRu), "Автомобильный")]
    [InlineData(false, nameof(ColumnFixture.KindEn), "Truck")]
    public void AddEnum_WhenLocalizedPair_UsesRequestedLanguage(bool isRu, string propertyName, string display)
    {
        var column = new GridColumnSettingsBuilder<ColumnFixture>(platform)
            .AddEnum(isRu, x => x.KindRu, x => x.KindEn, "Kind")
            .Build()
            .Single();

        column.Name.Should().Be(propertyName);
        column.DisplaySelector(new ColumnFixture
        {
            KindRu = TransportKindRu.Auto,
            KindEn = TransportKindEn.Auto
        }).ToString().Should().Be(display);
    }

    private sealed class ColumnFixture
    {
        public string? Name { get; set; }
        public bool Flag { get; set; }
        public DateOnly StartDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastChangeDate { get; set; }
        public TransportKindRu KindRu { get; set; }
        public TransportKindEn KindEn { get; set; }
    }
}
