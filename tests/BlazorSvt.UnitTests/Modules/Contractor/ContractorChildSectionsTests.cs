using System.Globalization;
using BlazorSvt.Modules.Contractor;
using BlazorSvt.Modules.Contractor.Detail;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using BlazorSvt.Platform.Grid.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using ContractorResources = BlazorSvt.Modules.Contractor.Resources.Contractor;
using PlatformResources = BlazorSvt.Platform.Resources.Platform;

namespace BlazorSvt.UnitTests.Modules.Contractor;

[Trait("Category", "Unit")]
public class ContractorChildSectionsTests
{
    private const long ContractorId = 9901790;

    private readonly IDetailChildDataService dataService = Substitute.For<IDetailChildDataService>();
    private readonly IStringLocalizer<ContractorResources> localizer = KeyLocalizer<ContractorResources>();
    private readonly IStringLocalizer<PlatformResources> platform = KeyLocalizer<PlatformResources>();

    [Fact]
    public async Task AddressSection_ActiveParent_ColumnsInOrder()
    {
        var row = FilledRow();
        GivenRows(row);
        var section = new AddressSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: false), "en", CancellationToken.None);

        section.Title.Should().Be("AddressDto.Title");
        table.Headers().Should().Equal(
            "AddressDto.Code",
            "AddressDto.AddressType",
            "AddressDto.City",
            "AddressDto.Status",
            "AddressDto.AddressKind",
            "AddressDto.Pobox",
            "AddressDto.RegionCode",
            "AddressDto.District_ru",
            "AddressDto.Street",
            "AddressDto.House",
            "AddressDto.OfficeApart",
            "AddressDto.FullAddress_ru",
            "AddressDto.CoordinateW",
            "AddressDto.CoordinateL",
            "AddressDto.FIASCodeRegion",
            "AddressDto.FIASCodeDistrict",
            "AddressDto.FIASCodeSettlement",
            "AddressDto.Region",
            "AddressDto.Country");
        table.Cells().Single().Should().Equal(
            "ADDR-1",
            "Legal",
            "Tobolsk",
            "Active",
            "Фактический",
            "626150",
            "72",
            "Тобольский",
            "Ленина",
            "1",
            "10",
            "полный",
            Text(row.CoordinateW),
            Text(row.CoordinateL),
            "fias-region",
            "fias-district",
            "fias-settlement",
            "Tyumen Region",
            "Russia");
    }

    [Fact]
    public async Task AddressSection_ArchivedParent_AddsArchiveColumnLast()
    {
        GivenRows(
            new AddressDto { ContractorId = ContractorId, Id = 1, PrimitiveEntityDataStateId = 1 },
            new AddressDto { ContractorId = ContractorId, Id = 2, PrimitiveEntityDataStateId = 2 });
        var section = new AddressSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: true), "ru", CancellationToken.None);

        table.Headers().Should().HaveCount(20).And.EndWith("AddressDto.IsArchive");
        table.Columns.Select(c => c.Name).Should().OnlyHaveUniqueItems();
        table.Cells().Select(r => r[^1]).Should().Equal("Common.Active", "Common.Archive");
        table.Columns[^1].Name.Should().Be("IsArchive");
        table.Rows.Select(r => r.IsArchive).Should().Equal(false, true);
    }

    [Fact]
    public void Module_RegistersSectionsInGroupOrder()
    {
        var services = new ServiceCollection();
        services.AddContractorModule();

        services
            .Where(d => d.ServiceType == typeof(IDetailChildSection<ContractorDetailDto>))
            .Select(d => d.ImplementationType)
            .Should().Equal(typeof(AddressSection));
    }

    [Theory]
    [InlineData("ru", "ru-RU",
        "Адреса контрагентов",
        new[]
        {
            "Идентификатор адреса", "Тип адреса", "Город/населенный пункт", "Статус адреса", "Вид адреса",
            "Индекс", "Код региона", "Район", "Улица", "Дом", "Офис/квартира", "Полный адрес",
            "Географические координаты широты", "Географические координаты долготы",
            "Код ФИАС региона", "Код ФИАС района", "Код ФИАС города/населенного пункта",
            "Регион", "Страна", "Архив"
        })]
    [InlineData("en", "en-US",
        "Addresses of contractors",
        new[]
        {
            "Address ID", "Address type", "City/town", "Address status", "Address type",
            "Pobox", "Region code", "District", "Street", "House", "Office/apartment", "Full address",
            "Latitude", "Longitude",
            "FIAS region code", "FIAS district code", "FIAS settlement code",
            "Region", "Country", "Archive"
        })]
    public async Task Section_UsesResxTitleAndHeaders(string lang, string culture, string title, string[] headers)
    {
        var previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
        try
        {
            var factory = new ResourceManagerStringLocalizerFactory(
                Options.Create(new LocalizationOptions()), NullLoggerFactory.Instance);
            var resxLocalizer = new StringLocalizer<ContractorResources>(factory);
            var resxPlatform = new StringLocalizer<PlatformResources>(factory);
            GivenRows(new AddressDto { ContractorId = ContractorId, Id = 1, PrimitiveEntityDataStateId = 1 });
            var section = new AddressSection(dataService, resxLocalizer, resxPlatform);

            var table = await section.LoadAsync(Parent(isArchive: true), lang, CancellationToken.None);

            section.Title.Should().Be(title);
            table.Headers().Should().Equal(headers);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }

    private void GivenRows(params AddressDto[] rows) =>
        dataService
            .GetRowsAsync<AddressDto>(Arg.Any<object>(), Arg.Any<IReadOnlyCollection<int>>(), Arg.Any<CancellationToken>())
            .Returns(new DetailChildPage<AddressDto>(rows, rows.Length));

    private static AddressDto FilledRow() => new()
    {
        ContractorId = ContractorId,
        Id = 1,
        PrimitiveEntityDataStateId = 1,
        Code = "ADDR-1",
        AddressType = "Legal",
        City = "Tobolsk",
        StatusIdRu = StatusElementRu.Active,
        StatusIdEn = StatusElementEn.Active,
        AddressKindName = "Фактический",
        Pobox = "626150",
        RegionCode = "72",
        District_ru = "Тобольский",
        Street = "Ленина",
        House = "1",
        OfficeApart = "10",
        FullAddress_ru = "полный",
        CoordinateW = 58.201000m,
        CoordinateL = 68.254000m,
        FIASCodeRegion = "fias-region",
        FIASCodeDistrict = "fias-district",
        FIASCodeSettlement = "fias-settlement",
        RegionNameRu = "Тюменская область",
        RegionNameEn = "Tyumen Region",
        CountryNameRu = "Россия",
        CountryNameEn = "Russia"
    };

    private static ContractorDetailDto Parent(bool isArchive) => new()
    {
        ContractorId = ContractorId,
        IsArchive = isArchive,
        Code = "C"
    };

    private static string Text(decimal? value) =>
        Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;

    private static IStringLocalizer<T> KeyLocalizer<T>()
    {
        var localizer = Substitute.For<IStringLocalizer<T>>();
        localizer[Arg.Any<string>()].Returns(call => new LocalizedString(call.Arg<string>(), call.Arg<string>()));
        return localizer;
    }
}
