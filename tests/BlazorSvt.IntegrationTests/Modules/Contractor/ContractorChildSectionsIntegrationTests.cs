using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.Contractor.Detail;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Services;
using BlazorSvt.Platform.Infrastructure.Config;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace BlazorSvt.IntegrationTests.Modules.Contractor;

[Collection("Database")]
[Trait("Category", "Integration")]
public class ContractorChildSectionsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    /// <summary>Контрагент, у которого неархивные адреса имеют статус, вид, регион и страну.</summary>
    private const long ContractorWithAddressesId = 9901790;

    [SkippableFact]
    public async Task Addresses_ReturnRowsWithResolvedStatusKindRegionAndCountry()
    {
        var page = await CreateService().GetRowsAsync<AddressDto>(
            ContractorWithAddressesId,
            DetailChildRowStates.For(parentIsArchive: false),
            CancellationToken.None);

        page.TotalCount.Should().BeGreaterThan(0);
        page.Rows.Should().NotBeEmpty();
        page.Rows.Should().OnlyContain(r =>
            r.ContractorId == ContractorWithAddressesId
            && r.PrimitiveEntityDataStateId == 1
            && IsDefined(r.StatusIdRu)
            && IsDefined(r.StatusIdEn));
        page.Rows.Should().Contain(r =>
            !string.IsNullOrWhiteSpace(r.AddressKindName)
            && !string.IsNullOrWhiteSpace(r.RegionNameRu)
            && !string.IsNullOrWhiteSpace(r.RegionNameEn)
            && !string.IsNullOrWhiteSpace(r.CountryNameRu)
            && !string.IsNullOrWhiteSpace(r.CountryNameEn));
    }

    private static bool IsDefined<TEnum>(TEnum? value) where TEnum : struct, Enum =>
        value is { } v && Enum.IsDefined(v);

    private DetailChildDataService CreateService(int rowLimit = 100) =>
        new(
            Options.Create(new DatabaseOptions { MdmDb = RequireConnectionString() }),
            Options.Create(new GridOptions { ChildSectionRowLimit = rowLimit }),
            NullLogger<DetailChildDataService>.Instance,
            Substitute.For<IAccessGuard>());
}
