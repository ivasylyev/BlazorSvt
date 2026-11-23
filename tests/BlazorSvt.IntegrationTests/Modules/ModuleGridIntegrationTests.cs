using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.AverageRateLevel3.Detail;
using BlazorSvt.Modules.Coefficient.Detail;
using BlazorSvt.Modules.Coefficient.List;
using BlazorSvt.Modules.Country.Detail;
using BlazorSvt.Modules.Country.List;
using BlazorSvt.Modules.Contractor.Detail;
using BlazorSvt.Modules.Contractor.List;
using BlazorSvt.Modules.Currency.Detail;
using BlazorSvt.Modules.Currency.List;
using BlazorSvt.Modules.AverageRateLevel3.List;
using BlazorSvt.Modules.LocationsNodes.Detail;
using BlazorSvt.Modules.LocationsNodes.List;
using BlazorSvt.Modules.ParityRates.Detail;
using BlazorSvt.Modules.MTR.Detail;
using BlazorSvt.Modules.MTR.List;
using BlazorSvt.Modules.ProductGroup.Detail;
using BlazorSvt.Modules.ProductGroup.List;
using BlazorSvt.Modules.RateAutoSelectionSetting.Detail;
using BlazorSvt.Modules.RateAutoSelectionSetting.List;
using BlazorSvt.Modules.ReportConstructor.Detail;
using BlazorSvt.Modules.ReportConstructor.List;
using BlazorSvt.Modules.RateCalcType.Detail;
using BlazorSvt.Modules.RateCalcType.List;
using BlazorSvt.Modules.RateSyncClass.Detail;
using BlazorSvt.Modules.RateSyncClass.List;
using BlazorSvt.Modules.RateType.Detail;
using BlazorSvt.Modules.RateType.List;
using BlazorSvt.Modules.Relevance.Detail;
using BlazorSvt.Modules.Relevance.List;
using BlazorSvt.Modules.Region.Detail;
using BlazorSvt.Modules.Region.List;
using BlazorSvt.Modules.ParityRates.List;
using BlazorSvt.Modules.TransportLeg.Detail;
using BlazorSvt.Modules.TransportLeg.List;
using BlazorSvt.Modules.TransportRate.Detail;
using BlazorSvt.Modules.TransportRate.List;
using BlazorSvt.Modules.TransportRoute.Detail;
using BlazorSvt.Modules.TransportRoute.List;
using BlazorSvt.Modules.TransportTypeLevel3.Detail;
using BlazorSvt.Modules.TransportTypeLevel3.List;
using BlazorSvt.Platform.Grid.Models;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules;

[Collection("Database")]
[Trait("Category", "Integration")]
public class ModuleGridIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task TransportLegGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task TransportRateGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportRateDto>(
            connectionString,
            typeof(TransportRateDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task ParityRatesGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ParityRatesDto>(
            connectionString,
            typeof(ParityRatesDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task LocationsNodesGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<LocationsNodesDto>(
            connectionString,
            typeof(LocationsNodesDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task AverageRateLevel3Grid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<AverageRateLevel3Dto>(
            connectionString,
            typeof(AverageRateLevel3Dto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task TransportLegGrid_SecondPageDiffersFromFirstPage()
    {
        var connectionString = RequireConnectionString();
        var sort = new GridSort(nameof(TransportLegDto.TransportLegId), "ASC");
        var filters = new[] { new GridFilter("IsArchive", "False", GridFilterOperators.EqualsOperator) };

        var (pageOne, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            new GridQuery(1, 10, sort, filters));

        var (pageTwo, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            new GridQuery(2, 10, sort, filters));

        pageOne.Should().NotBeEmpty();
        pageTwo.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(10);
        pageOne[0].TransportLegId.Should().NotBe(pageTwo[0].TransportLegId);
    }

    [SkippableFact]
    public async Task TransportLegDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].TransportLegId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<TransportLegDetailDto>(
            connectionString,
            "v2.vw_TransportLeg_Detail",
            "TransportLegId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.TransportLegId.Should().Be(entityKey);
        detail.Code.Should().NotBeNullOrWhiteSpace();
    }

    [SkippableFact]
    public async Task TransportRateDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportRateDto>(
            connectionString,
            typeof(TransportRateDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].TransportRateId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<TransportRateDetailDto>(
            connectionString,
            "v2.vw_TransportRate_Detail",
            "TransportRateId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.TransportRateId.Should().Be(entityKey);
    }

    [SkippableFact]
    public async Task ParityRatesDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ParityRatesDto>(
            connectionString,
            typeof(ParityRatesDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].ParityRatesId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<ParityRatesDetailDto>(
            connectionString,
            "v2.vw_ParityRates_Detail",
            "ParityRatesId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.ParityRatesId.Should().Be(entityKey);
        detail.Code.Should().NotBeNullOrWhiteSpace();
    }

    [SkippableFact]
    public async Task LocationsNodesDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<LocationsNodesDto>(
            connectionString,
            typeof(LocationsNodesDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].LocationsNodesId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<LocationsNodesDetailDto>(
            connectionString,
            "v2.vw_LocationsNodes_Detail",
            "LocationsNodesId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.LocationsNodesId.Should().Be(entityKey);
        detail.Code.Should().NotBeNullOrWhiteSpace();
    }

    [SkippableFact]
    public async Task AverageRateLevel3DetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<AverageRateLevel3Dto>(
            connectionString,
            typeof(AverageRateLevel3Dto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].AverageRateLevel3Id;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<AverageRateLevel3DetailDto>(
            connectionString,
            "v2.vw_AverageRateLevel3_Detail",
            "AverageRateLevel3Id",
            entityKey);

        detail.Should().NotBeNull();
        detail!.AverageRateLevel3Id.Should().Be(entityKey);
    }

    [SkippableFact]
    public async Task RateTypeGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateTypeDto>(
            connectionString,
            typeof(RateTypeDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task RateTypeDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateTypeDto>(
            connectionString,
            typeof(RateTypeDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].RateTypeId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<RateTypeDetailDto>(
            connectionString,
            "v2.vw_RateType_Detail",
            "RateTypeId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.RateTypeId.Should().Be(entityKey);
        detail.Code.Should().NotBeNullOrWhiteSpace();
    }

    [SkippableFact]
    public async Task CurrencyGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<CurrencyDto>(
            connectionString,
            typeof(CurrencyDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task CurrencyDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<CurrencyDto>(
            connectionString,
            typeof(CurrencyDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].CurrencyId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<CurrencyDetailDto>(
            connectionString,
            "v2.vw_Currency_Detail",
            "CurrencyId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.CurrencyId.Should().Be(entityKey);
        detail.Code.Should().NotBeNullOrWhiteSpace();
    }

    [SkippableFact]
    public async Task ProductGroupGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ProductGroupDto>(
            connectionString,
            typeof(ProductGroupDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task ProductGroupDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ProductGroupDto>(
            connectionString,
            typeof(ProductGroupDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].ProductGroupId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<ProductGroupDetailDto>(
            connectionString,
            "v2.vw_ProductGroup_Detail",
            "ProductGroupId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.ProductGroupId.Should().Be(entityKey);
        detail.Code.Should().NotBeNullOrWhiteSpace();
    }

    [SkippableFact]
    public async Task ContractorGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ContractorDto>(
            connectionString,
            typeof(ContractorDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task ContractorDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ContractorDto>(
            connectionString,
            typeof(ContractorDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].ContractorId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<ContractorDetailDto>(
            connectionString,
            "v2.vw_Contractor_Detail",
            "ContractorId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.ContractorId.Should().Be(entityKey);
        detail.Code.Should().NotBeNullOrWhiteSpace();
    }

    [SkippableFact]
    public async Task CoefficientGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<CoefficientDto>(
            connectionString,
            typeof(CoefficientDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task CoefficientDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<CoefficientDto>(
            connectionString,
            typeof(CoefficientDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].CoefficientId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<CoefficientDetailDto>(
            connectionString,
            "v2.vw_Coefficient_Detail",
            "CoefficientId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.CoefficientId.Should().Be(entityKey);
        detail.Code.Should().NotBeNullOrWhiteSpace();
        detail.CoefficientTypeIdRu.Should().Be(rows[0].CoefficientTypeIdRu);
        detail.RateTypeIdRu.Should().Be(rows[0].RateTypeIdRu);
    }

    [SkippableFact]
    public async Task MTRGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<MTRDto>(
            connectionString,
            typeof(MTRDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task MTRDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<MTRDto>(
            connectionString,
            typeof(MTRDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].MTRId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<MTRDetailDto>(
            connectionString,
            "v2.vw_MTR_Detail",
            "MTRId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.MTRId.Should().Be(entityKey);
        detail.Code.Should().NotBeNullOrWhiteSpace();
    }

    [SkippableFact]
    public async Task TransportTypeLevel3Grid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportTypeLevel3Dto>(
            connectionString,
            typeof(TransportTypeLevel3Dto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task TransportTypeLevel3DetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportTypeLevel3Dto>(
            connectionString,
            typeof(TransportTypeLevel3Dto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].TransportTypeLevel3Id;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<TransportTypeLevel3DetailDto>(
            connectionString,
            "v2.vw_TransportTypeLevel3_Detail",
            "TransportTypeLevel3Id",
            entityKey);

        detail.Should().NotBeNull();
        detail!.TransportTypeLevel3Id.Should().Be(entityKey);
        detail.Code.Should().NotBeNullOrWhiteSpace();
    }

    [SkippableFact]
    public async Task RateSyncClassGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateSyncClassDto>(
            connectionString,
            typeof(RateSyncClassDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task RateSyncClassDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateSyncClassDto>(
            connectionString,
            typeof(RateSyncClassDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].RateSyncClassId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<RateSyncClassDetailDto>(
            connectionString,
            "v2.vw_RateSyncClass_Detail",
            "RateSyncClassId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.RateSyncClassId.Should().Be(entityKey);
        detail.Code.Should().NotBeNullOrWhiteSpace();
    }

    [SkippableFact]
    public async Task RateCalcTypeGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateCalcTypeDto>(
            connectionString,
            typeof(RateCalcTypeDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task RateCalcTypeDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateCalcTypeDto>(
            connectionString,
            typeof(RateCalcTypeDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].RateCalcTypeId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<RateCalcTypeDetailDto>(
            connectionString,
            "v2.vw_RateCalcType_Detail",
            "RateCalcTypeId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.RateCalcTypeId.Should().Be(entityKey);
        detail.Code.Should().NotBeNullOrWhiteSpace();
    }

    [SkippableFact]
    public async Task RateAutoSelectionSettingGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateAutoSelectionSettingDto>(
            connectionString,
            typeof(RateAutoSelectionSettingDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows[0].Code.Should().Be("3");
        rows[0].RateAutoSelectionSettingId.Should().Be(10609266);
    }

    [SkippableFact]
    public async Task RateAutoSelectionSettingDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateAutoSelectionSettingDto>(
            connectionString,
            typeof(RateAutoSelectionSettingDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].RateAutoSelectionSettingId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<RateAutoSelectionSettingDetailDto>(
            connectionString,
            "v2.vw_RateAutoSelectionSetting_Detail",
            "RateAutoSelectionSettingId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.RateAutoSelectionSettingId.Should().Be(entityKey);
        detail.Code.Should().Be(rows[0].Code);
        detail.DestinationNodeToRegion.Should().Be(rows[0].DestinationNodeToRegion);
        detail.AnyTransportKind.Should().Be(rows[0].AnyTransportKind);
    }

    [SkippableFact]
    public async Task ReportConstructorGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ReportConstructorDto>(
            connectionString,
            typeof(ReportConstructorDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().Be(12);
        rows.Should().OnlyContain(r => r.LastChangeDate != default);
        rows.Should().OnlyContain(r => r.Code.Length > 0);
    }

    [SkippableFact]
    public async Task ReportConstructorDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ReportConstructorDto>(
            connectionString,
            typeof(ReportConstructorDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].ReportConstructorId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<ReportConstructorDetailDto>(
            connectionString,
            "v2.vw_ReportConstructor_Detail",
            "ReportConstructorId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.ReportConstructorId.Should().Be(entityKey);
        detail.Code.Should().Be(rows[0].Code);
        detail.ReportTypeIdRu.Should().Be(rows[0].ReportTypeIdRu);
        detail.DateFrom.Should().Be(rows[0].DateFrom);
        detail.LastChangeDate.Should().Be(rows[0].LastChangeDate);
    }

    [SkippableFact]
    public async Task RelevanceGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RelevanceDto>(
            connectionString,
            typeof(RelevanceDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().Be(4);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Code.Length > 0);
    }

    [SkippableFact]
    public async Task RelevanceDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RelevanceDto>(
            connectionString,
            typeof(RelevanceDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].RelevanceId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<RelevanceDetailDto>(
            connectionString,
            "v2.vw_Relevance_Detail",
            "RelevanceId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.RelevanceId.Should().Be(entityKey);
        detail.Code.Should().Be(rows[0].Code);
        detail.Name.Should().Be(rows[0].Name);
    }

    [SkippableFact]
    public async Task CountryGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<CountryDto>(
            connectionString,
            typeof(CountryDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().Be(256);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Code.Length > 0);
    }

    [SkippableFact]
    public async Task CountryDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<CountryDto>(
            connectionString,
            typeof(CountryDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].CountryId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<CountryDetailDto>(
            connectionString,
            "v2.vw_Country_Detail",
            "CountryId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.CountryId.Should().Be(entityKey);
        detail.Code.Should().Be(rows[0].Code);
        detail.NameRu.Should().Be(rows[0].NameRu);
        detail.ISOCode.Should().Be(rows[0].ISOCode);
        detail.StatusIdRu.Should().Be(rows[0].StatusIdRu);
    }

    [SkippableFact]
    public async Task RegionGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RegionDto>(
            connectionString,
            typeof(RegionDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().Be(2019);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Code.Length > 0);
    }

    [SkippableFact]
    public async Task RegionDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RegionDto>(
            connectionString,
            typeof(RegionDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].RegionId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<RegionDetailDto>(
            connectionString,
            "v2.vw_Region_Detail",
            "RegionId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.RegionId.Should().Be(entityKey);
        detail.Code.Should().Be(rows[0].Code);
        detail.NameRu.Should().Be(rows[0].NameRu);
        detail.StatusIdRu.Should().Be(rows[0].StatusIdRu);
        detail.Market.Should().Be(rows[0].Market);
    }

    [SkippableFact]
    public async Task TransportRouteGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportRouteDto>(
            connectionString,
            typeof(TransportRouteDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Code.Length > 0);
        rows.Should().OnlyContain(r => r.IsActive);
    }

    [SkippableFact]
    public async Task TransportRouteDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportRouteDto>(
            connectionString,
            typeof(TransportRouteDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].TransportRouteId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<TransportRouteDetailDto>(
            connectionString,
            "v2.vw_TransportRoute_Detail",
            "TransportRouteId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.TransportRouteId.Should().Be(entityKey);
        detail.Code.Should().Be(rows[0].Code);
        detail.RouteTransportKindIdRu.Should().Be(rows[0].RouteTransportKindIdRu);
        detail.StartNodeNameRu.Should().Be(rows[0].StartNodeNameRu);
        detail.ChainNodes.Should().Be(rows[0].ChainNodes);
    }
}
