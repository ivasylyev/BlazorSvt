using BlazorSvt.Modules.AverageRateLevel3.List;
using BlazorSvt.Modules.RateType.List;
using BlazorSvt.Modules.ParityRates.List;
using BlazorSvt.Modules.TransportLeg.List;
using BlazorSvt.Modules.TransportRate.List;
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
