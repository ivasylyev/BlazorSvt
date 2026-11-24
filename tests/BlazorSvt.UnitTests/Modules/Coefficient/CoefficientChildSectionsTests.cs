using System.Globalization;
using BlazorSvt.Modules.Coefficient;
using BlazorSvt.Modules.Coefficient.Detail;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using BlazorSvt.Platform.Grid.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using CoefficientResources = BlazorSvt.Modules.Coefficient.Resources.Coefficient;
using PlatformResources = BlazorSvt.Platform.Resources.Platform;

namespace BlazorSvt.UnitTests.Modules.Coefficient;

[Trait("Category", "Unit")]
public class CoefficientChildSectionsTests
{
    private const long CoefficientId = 26857587;

    private readonly IDetailChildDataService dataService = Substitute.For<IDetailChildDataService>();
    private readonly IStringLocalizer<CoefficientResources> localizer = KeyLocalizer<CoefficientResources>();
    private readonly IStringLocalizer<PlatformResources> platform = KeyLocalizer<PlatformResources>();

    [Theory]
    [InlineData(false, new[] { 1 })]
    [InlineData(true, new[] { 1, 2 })]
    public async Task ConditionSection_RequestsStatesByParentArchive(bool parentIsArchive, int[] expectedStates)
    {
        GivenConditionRows();
        var section = new CoefficientConditionValueSection(dataService, localizer, platform);

        await section.LoadAsync(Parent(parentIsArchive), "en", CancellationToken.None);

        await dataService.Received(1).GetRowsAsync<CoefficientConditionValueDto>(
            CoefficientId,
            Arg.Is<IReadOnlyCollection<int>>(states => states.SequenceEqual(expectedStates)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConditionSection_ActiveParent_ColumnsInOrderWithoutArchive()
    {
        GivenConditionRows(new CoefficientConditionValueDto
        {
            CoefficientId = CoefficientId,
            Id = 1,
            PrimitiveEntityDataStateId = 1,
            Code = "162",
            Except = false,
            TransportKindIdRu = TransportKindRu.Mix,
            TransportKindIdEn = TransportKindEn.Mix,
            TransportTypeLevel2IdRu = TransportTypeRu.Rail_TK,
            TransportTypeLevel2IdEn = TransportTypeEn.Rail_TK,
            VehicleTypeName = "Wagon",
            FromLocationNameRu = "Откуда",
            FromLocationNameEn = "From node",
            ToLocationNameEn = "To node"
        });
        var section = new CoefficientConditionValueSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: false), "en", CancellationToken.None);

        section.Title.Should().Be("CoefficientConditionValueDto.Title");
        table.Headers().Should().Equal(
            "CoefficientConditionValueDto.Code",
            "CoefficientConditionValueDto.Except",
            "CoefficientConditionValueDto.TransportKindName",
            "CoefficientConditionValueDto.TransportTypeLevel2Name",
            "CoefficientConditionValueDto.VehicleTypeName",
            "CoefficientConditionValueDto.FromLocationName",
            "CoefficientConditionValueDto.ToLocationName");
        table.Cells().Single().Should().Equal(
            "162", "Common.No", "Multimodal", "Rail tank container", "Wagon", "From node", "To node");
    }

    [Fact]
    public async Task ConditionSection_EmptyReferences_RenderEmptyCells()
    {
        GivenConditionRows(new CoefficientConditionValueDto { CoefficientId = CoefficientId, Id = 1, PrimitiveEntityDataStateId = 1 });
        var section = new CoefficientConditionValueSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: false), "ru", CancellationToken.None);

        table.Cells().Single().Should().AllBe(string.Empty);
    }

    [Fact]
    public async Task ConditionSection_ArchivedParent_AddsArchiveColumnLast()
    {
        GivenConditionRows(
            new CoefficientConditionValueDto { CoefficientId = CoefficientId, Id = 1, PrimitiveEntityDataStateId = 1 },
            new CoefficientConditionValueDto { CoefficientId = CoefficientId, Id = 2, PrimitiveEntityDataStateId = 2 });
        var section = new CoefficientConditionValueSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: true), "en", CancellationToken.None);

        table.Headers().Should().HaveCount(8).And.EndWith("CoefficientConditionValueDto.IsArchive");
        table.Columns.Select(c => c.Name).Should().OnlyHaveUniqueItems();
        table.Cells().Select(r => r[^1]).Should().Equal("Common.Active", "Common.Archive");
        table.Columns[^1].Name.Should().Be("IsArchive");
        table.Rows.Select(r => r.IsArchive).Should().Equal(false, true);
    }

    [Fact]
    public async Task ComponentSection_ColumnsAndTotalCount()
    {
        dataService
            .GetRowsAsync<CoefficientComponentValueDto>(Arg.Any<object>(), Arg.Any<IReadOnlyCollection<int>>(), Arg.Any<CancellationToken>())
            .Returns(new DetailChildPage<CoefficientComponentValueDto>(
                [new CoefficientComponentValueDto { CoefficientId = CoefficientId, Id = 1, PrimitiveEntityDataStateId = 1, Code = "670", ComponentName = "General", Value = 1.0500m }],
                TotalCount: 14));
        var section = new CoefficientComponentValueSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: false), "en", CancellationToken.None);

        section.Title.Should().Be("CoefficientComponentValueDto.Title");
        table.Headers().Should().Equal(
            "CoefficientComponentValueDto.Code",
            "CoefficientComponentValueDto.ComponentName",
            "CoefficientComponentValueDto.Value");
        table.Cells().Single().Should().Equal("670", "General", 1.0500m.ToString(CultureInfo.CurrentCulture));
        table.TotalCount.Should().Be(14);
        table.IsTruncated.Should().BeTrue();
    }

    [Fact]
    public void Module_RegistersSectionsInGroupOrder()
    {
        var services = new ServiceCollection();
        services.AddCoefficientModule();

        services
            .Where(d => d.ServiceType == typeof(IDetailChildSection<CoefficientDetailDto>))
            .Select(d => d.ImplementationType)
            .Should().Equal(typeof(CoefficientConditionValueSection), typeof(CoefficientComponentValueSection));
    }

    [Theory]
    [InlineData("ru", "ru-RU",
        "Условие по коэффициенту",
        new[] { "Код", "Исключать", "Вид транспорта", "Тип транспорта (2 уровень)", "Тип подвижного состава", "Направление от", "Направление до", "Архив" },
        "Составляющая по коэффициенту",
        new[] { "Код", "Составляющая", "Значение", "Архив" })]
    [InlineData("en", "en-US",
        "Coefficient conditions",
        new[] { "Code", "Exclude", "Transport kind", "Transport type (level 2)", "Rolling stock type", "From", "To", "Archive" },
        "Coefficient components",
        new[] { "Code", "Component", "Value", "Archive" })]
    public async Task Sections_UseResxTitlesAndHeaders(
        string lang,
        string culture,
        string conditionTitle,
        string[] conditionHeaders,
        string componentTitle,
        string[] componentHeaders)
    {
        var previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
        try
        {
            var factory = new ResourceManagerStringLocalizerFactory(
                Options.Create(new LocalizationOptions()), NullLoggerFactory.Instance);
            var resxLocalizer = new StringLocalizer<CoefficientResources>(factory);
            var resxPlatform = new StringLocalizer<PlatformResources>(factory);
            GivenConditionRows(new CoefficientConditionValueDto { CoefficientId = CoefficientId, Id = 1, PrimitiveEntityDataStateId = 1 });
            dataService
                .GetRowsAsync<CoefficientComponentValueDto>(Arg.Any<object>(), Arg.Any<IReadOnlyCollection<int>>(), Arg.Any<CancellationToken>())
                .Returns(new DetailChildPage<CoefficientComponentValueDto>(
                    [new CoefficientComponentValueDto { CoefficientId = CoefficientId, Id = 1, PrimitiveEntityDataStateId = 1 }],
                    TotalCount: 1));
            var condition = new CoefficientConditionValueSection(dataService, resxLocalizer, resxPlatform);
            var component = new CoefficientComponentValueSection(dataService, resxLocalizer, resxPlatform);

            var conditionTable = await condition.LoadAsync(Parent(isArchive: true), lang, CancellationToken.None);
            var componentTable = await component.LoadAsync(Parent(isArchive: true), lang, CancellationToken.None);

            condition.Title.Should().Be(conditionTitle);
            conditionTable.Headers().Should().Equal(conditionHeaders);
            component.Title.Should().Be(componentTitle);
            componentTable.Headers().Should().Equal(componentHeaders);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }

    private void GivenConditionRows(params CoefficientConditionValueDto[] rows) =>
        dataService
            .GetRowsAsync<CoefficientConditionValueDto>(Arg.Any<object>(), Arg.Any<IReadOnlyCollection<int>>(), Arg.Any<CancellationToken>())
            .Returns(new DetailChildPage<CoefficientConditionValueDto>(rows, rows.Length));

    private static CoefficientDetailDto Parent(bool isArchive) => new()
    {
        CoefficientId = CoefficientId,
        IsArchive = isArchive,
        Code = "C",
        CoefficientTypeIdRu = CoefficientTypeRu.Deflator,
        CoefficientTypeIdEn = CoefficientTypeEn.Deflator,
        RateTypeIdRu = default,
        RateTypeIdEn = default
    };

    private static IStringLocalizer<T> KeyLocalizer<T>()
    {
        var localizer = Substitute.For<IStringLocalizer<T>>();
        localizer[Arg.Any<string>()].Returns(call => new LocalizedString(call.Arg<string>(), call.Arg<string>()));
        return localizer;
    }
}
