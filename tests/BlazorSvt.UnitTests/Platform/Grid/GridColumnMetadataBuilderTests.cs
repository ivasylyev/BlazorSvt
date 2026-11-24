using BlazorSvt.Modules.AverageRateLevel3.List;
using BlazorSvt.Modules.Coefficient.List;
using BlazorSvt.Modules.Country.List;
using BlazorSvt.Modules.Contractor.List;
using BlazorSvt.Modules.Currency.List;
using BlazorSvt.Modules.MTR.List;
using BlazorSvt.Modules.ProductGroup.List;
using BlazorSvt.Modules.RateAutoSelectionSetting.List;
using BlazorSvt.Modules.RateCalcType.List;
using BlazorSvt.Modules.ReportConstructor.List;
using BlazorSvt.Modules.RateSyncClass.List;
using BlazorSvt.Modules.RateType.List;
using BlazorSvt.Modules.Relevance.List;
using BlazorSvt.Modules.Region.List;
using BlazorSvt.Modules.ParityRates.List;
using BlazorSvt.Modules.TransportLeg.List;
using BlazorSvt.Modules.TransportRate.List;
using BlazorSvt.Modules.TransportRoute.List;
using BlazorSvt.Modules.TransportKind.List;
using BlazorSvt.Modules.TransportTypeLevel3.List;
using BlazorSvt.Platform.Grid.Models;
using BlazorSvt.Platform.Grid.Services;
using BlazorSvt.Platform.Infrastructure.Config;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace BlazorSvt.UnitTests.Platform.Grid;

[Trait("Category", "Unit")]
public class GridColumnMetadataBuilderTests
{
    [Fact]
    public void BuildAllowedColumnsJson_IncludesOnlyFilterableColumns()
    {
        var json = GridColumnMetadataBuilder.BuildAllowedColumnsJson(typeof(TransportLegDto));
        var columns = JArray.Parse(json);

        columns.Should().NotBeEmpty();
        columns.Select(c => c["ColumnName"]!.Value<string>()).Should().Contain(nameof(TransportLegDto.Code));
        columns.Select(c => c["ColumnName"]!.Value<string>()).Should().Contain(nameof(TransportLegDto.TransportKindCode));
        columns.Select(c => c["ColumnName"]!.Value<string>()).Should().NotContain(nameof(TransportLegDto.TransportationTimeT));
    }

    [Fact]
    public void BuildAllowedColumnsJson_DecimalColumns_HaveDecimalColumnType()
    {
        var json = GridColumnMetadataBuilder.BuildAllowedColumnsJson(typeof(TransportRateDto));
        var columns = JArray.Parse(json);

        var totalCostTon = columns.Single(c => c["ColumnName"]!.Value<string>() == nameof(TransportRateDto.TotalCostTon));
        totalCostTon["ColumnType"]!.Value<string>().Should().Be("DECIMAL");

        var totalCostTransport = columns.Single(c => c["ColumnName"]!.Value<string>() == nameof(TransportRateDto.TotalCostTransport));
        totalCostTransport["ColumnType"]!.Value<string>().Should().Be("DECIMAL");
    }

    [Fact]
    public void BuildAllowedColumnsJson_NullableDecimalColumns_HaveDecimalColumnType()
    {
        var json = GridColumnMetadataBuilder.BuildAllowedColumnsJson(typeof(ParityRatesDto));
        var columns = JArray.Parse(json);

        var factRate = columns.Single(c => c["ColumnName"]!.Value<string>() == nameof(ParityRatesDto.FactRate));
        factRate["ColumnType"]!.Value<string>().Should().Be("DECIMAL");
    }

    [Fact]
    public void BuildAllowedColumnsJson_DateTimeColumns_KeepTime_DateOnlyStayDate()
    {
        var json = GridColumnMetadataBuilder.BuildAllowedColumnsJson(typeof(TransportRateDto));
        var columns = JArray.Parse(json);

        columns.Single(c => c["ColumnName"]!.Value<string>() == nameof(TransportRateDto.CreationDate))
            ["ColumnType"]!.Value<string>().Should().Be("DATETIME");
        columns.Single(c => c["ColumnName"]!.Value<string>() == nameof(TransportRateDto.LastChangeDate))
            ["ColumnType"]!.Value<string>().Should().Be("DATETIME");
        columns.Single(c => c["ColumnName"]!.Value<string>() == nameof(TransportRateDto.StartDate))
            ["ColumnType"]!.Value<string>().Should().Be("DATE");
        columns.Single(c => c["ColumnName"]!.Value<string>() == nameof(TransportRateDto.EndDate))
            ["ColumnType"]!.Value<string>().Should().Be("DATE");
    }

    [Fact]
    public void BuildAllowedColumnsJson_UsesSqlColumnNameWhenSpecified()
    {
        var json = GridColumnMetadataBuilder.BuildAllowedColumnsJson(typeof(TransportLegDto));
        var columns = JArray.Parse(json);

        var transportKind = columns.Single(c => c["ColumnName"]!.Value<string>() == nameof(TransportLegDto.TransportKindIdRu));
        transportKind["SqlColumnName"]!.Value<string>().Should().Be("TransportKindId");
        transportKind["ColumnType"]!.Value<string>().Should().Be("ID");
    }

    [Fact]
    public void BuildSelectList_IncludesEntityKeyColumn()
    {
        var selectList = GridColumnMetadataBuilder.BuildSelectList(typeof(TransportLegDto));

        selectList.Should().Contain("TransportLegId");
        selectList.Should().Contain("SELECT");
    }

    [Fact]
    public void BuildSelectList_WhenKeysOnly_ReturnsSingleEntityKeyColumn()
    {
        var selectList = GridColumnMetadataBuilder.BuildSelectList(typeof(TransportLegDto), keysOnly: true);

        selectList.Should().Contain("TransportLegId");
        selectList.Should().NotContain(nameof(TransportLegDto.Code));
    }

    [Fact]
    public void GetMetadata_RateType_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(RateTypeDto));

        metadata.TableName.Should().Be("v2.RateType_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(RateTypeDto.RateTypeId));
    }

    [Fact]
    public void GetMetadata_Relevance_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(RelevanceDto));

        metadata.TableName.Should().Be("v2.Relevance_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(RelevanceDto.RelevanceId));
    }

    [Fact]
    public void GetMetadata_Country_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(CountryDto));

        metadata.TableName.Should().Be("v2.Country_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(CountryDto.CountryId));
    }

    [Fact]
    public void GetMetadata_Region_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(RegionDto));

        metadata.TableName.Should().Be("v2.Region_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(RegionDto.RegionId));
    }

    [Fact]
    public void GetMetadata_RateSyncClass_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(RateSyncClassDto));

        metadata.TableName.Should().Be("v2.RateSyncClass_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(RateSyncClassDto.RateSyncClassId));
    }

    [Fact]
    public void GetMetadata_RateCalcType_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(RateCalcTypeDto));

        metadata.TableName.Should().Be("v2.RateCalcType_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(RateCalcTypeDto.RateCalcTypeId));
    }

    [Fact]
    public void GetMetadata_RateAutoSelectionSetting_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(RateAutoSelectionSettingDto));

        metadata.TableName.Should().Be("v2.RateAutoSelectionSetting_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(RateAutoSelectionSettingDto.RateAutoSelectionSettingId));
    }

    [Fact]
    public void GetMetadata_ReportConstructor_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(ReportConstructorDto));

        metadata.TableName.Should().Be("v2.ReportConstructor_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(ReportConstructorDto.ReportConstructorId));
    }

    [Fact]
    public void GetMetadata_Currency_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(CurrencyDto));

        metadata.TableName.Should().Be("v2.Currency_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(CurrencyDto.CurrencyId));
    }

    [Fact]
    public void GetMetadata_ProductGroup_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(ProductGroupDto));

        metadata.TableName.Should().Be("v2.ProductGroup_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(ProductGroupDto.ProductGroupId));
    }

    [Fact]
    public void GetMetadata_Contractor_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(ContractorDto));

        metadata.TableName.Should().Be("v2.Contractor_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(ContractorDto.ContractorId));
    }

    [Fact]
    public void GetMetadata_Coefficient_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(CoefficientDto));

        metadata.TableName.Should().Be("v2.Coefficient_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(CoefficientDto.CoefficientId));
    }

    [Fact]
    public void GetMetadata_MTR_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(MTRDto));

        metadata.TableName.Should().Be("v2.MTR_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(MTRDto.MTRId));
    }

    [Fact]
    public void GetMetadata_TransportTypeLevel3_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(TransportTypeLevel3Dto));

        metadata.TableName.Should().Be("v2.TransportTypeLevel3_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(TransportTypeLevel3Dto.TransportTypeLevel3Id));
    }

    [Fact]
    public void GetMetadata_TransportRoute_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(TransportRouteDto));

        metadata.TableName.Should().Be("v2.TransportRoute_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(TransportRouteDto.TransportRouteId));
    }

    [Fact]
    public void GetMetadata_TransportKind_UsesSnapshotTableAndEntityKey()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(TransportKindDto));

        metadata.TableName.Should().Be("v2.TransportKind_Snapshot");
        metadata.EntityKeyPropertyName.Should().Be(nameof(TransportKindDto.TransportKindId));
    }

    [Fact]
    public void GetMetadata_WhenDtoHasNoGridSnapshotAttribute_Throws()
    {
        var act = () => GridColumnMetadataBuilder.GetMetadata(typeof(object));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*GridSnapshotAttribute*");
    }

    [Fact]
    public void GetMetadata_IsCachedForSameDtoType()
    {
        var first = GridColumnMetadataBuilder.GetMetadata(typeof(TransportLegDto));
        var second = GridColumnMetadataBuilder.GetMetadata(typeof(TransportLegDto));

        ReferenceEquals(first, second).Should().BeTrue();
    }

    [Fact]
    public void GetMetadata_WhenDtoHasNoGridColumn_Throws()
    {
        var act = () => GridColumnMetadataBuilder.GetMetadata(typeof(NoGridColumnsDto));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*has no properties with GridColumnAttribute*");
    }

    [Fact]
    public void GetMetadata_WhenPropertyHasNoGridColumn_OmitsIt()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(UnmarkedPropertyDto));

        metadata.Columns.Should().ContainSingle()
            .Which.PropertyName.Should().Be(nameof(UnmarkedPropertyDto.Id));
    }

    [Theory]
    [InlineData(typeof(ZeroEntityKeysDto), "found 0")]
    [InlineData(typeof(TwoEntityKeysDto), "found 2")]
    public void GetMetadata_WhenEntityKeyCountIsNotOne_Throws(Type dtoType, string found)
    {
        var act = () => GridColumnMetadataBuilder.GetMetadata(dtoType);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"*must have exactly one GridColumn with IsEntityKey = true, {found}*");
    }

    [Fact]
    public void GetMetadata_WhenFilterablePropertyTypeCannotBeInferred_Throws()
    {
        var act = () => GridColumnMetadataBuilder.GetMetadata(typeof(FilterableGuidDto));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*requires explicit GridColumnType*");
    }

    [Fact]
    public void GetMetadata_WhenUninferablePropertyIsNotFilterable_UsesIdAndOmitsItFromWhitelist()
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(typeof(NonFilterableGuidDto));

        metadata.Columns.Should().ContainSingle(c => c.PropertyName == nameof(NonFilterableGuidDto.Token))
            .Which.ColumnType.Should().Be(GridColumnType.Id);

        var names = JArray.Parse(GridColumnMetadataBuilder.BuildAllowedColumnsJson(typeof(NonFilterableGuidDto)))
            .Select(c => c["ColumnName"]!.Value<string>());

        names.Should().NotContain(nameof(NonFilterableGuidDto.Token));
    }

    [Theory]
    [InlineData(typeof(TransportRateDto))]
    [InlineData(typeof(AverageRateLevel3Dto))]
    [InlineData(typeof(ParityRatesDto))]
    public void BuildSelectList_WhenDateOnlyAndDateTimeColumns_CastsDatesAndLeavesCreationDateBare(Type dtoType)
    {
        var parts = SelectParts(GridColumnMetadataBuilder.BuildSelectList(dtoType));

        parts.Should().Contain("CAST(StartDate AS DATE) AS StartDate");
        parts.Should().Contain("CAST(EndDate AS DATE) AS EndDate");
        parts.Should().Contain("CreationDate");
        parts.Where(part => part.Contains("CreationDate", StringComparison.Ordinal))
            .Should().AllSatisfy(part =>
            {
                part.Should().NotContain("CAST");
                part.Should().NotContain(" AS ");
            });
    }

    private static IReadOnlyList<string> SelectParts(string selectList) =>
        selectList
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Replace("SELECT", "", StringComparison.Ordinal).Trim())
            .ToList();

    [GridSnapshot("v2.Fixture_Snapshot")]
    private sealed class NoGridColumnsDto
    {
        public string Name { get; set; } = "";
    }

    [GridSnapshot("v2.Fixture_Snapshot")]
    private sealed class UnmarkedPropertyDto
    {
        [GridColumn(IsEntityKey = true)]
        public int Id { get; set; }

        public string Note { get; set; } = "";
    }

    [GridSnapshot("v2.Fixture_Snapshot")]
    private sealed class ZeroEntityKeysDto
    {
        [GridColumn]
        public string Name { get; set; } = "";
    }

    [GridSnapshot("v2.Fixture_Snapshot")]
    private sealed class TwoEntityKeysDto
    {
        [GridColumn(IsEntityKey = true)]
        public int Id { get; set; }

        [GridColumn(IsEntityKey = true)]
        public int OtherId { get; set; }
    }

    [GridSnapshot("v2.Fixture_Snapshot")]
    private sealed class FilterableGuidDto
    {
        [GridColumn(IsEntityKey = true)]
        public int Id { get; set; }

        [GridColumn]
        public Guid Token { get; set; }
    }

    [GridSnapshot("v2.Fixture_Snapshot")]
    private sealed class NonFilterableGuidDto
    {
        [GridColumn(IsEntityKey = true)]
        public int Id { get; set; }

        [GridColumn(Filterable = false)]
        public Guid Token { get; set; }
    }
}
