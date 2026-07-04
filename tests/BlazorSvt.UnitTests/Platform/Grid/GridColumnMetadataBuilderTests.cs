using BlazorSvt.Modules.LocationsNodes.List;
using BlazorSvt.Modules.TransportLeg.List;
using BlazorSvt.Modules.TransportRate.List;
using BlazorSvt.Platform.Grid.Services;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace BlazorSvt.UnitTests.Platform.Grid;

[Trait("Category", "Unit")]
public class GridColumnMetadataBuilderTests
{
    [Theory]
    [InlineData(typeof(TransportLegDto), "v2.TransportLeg_Snapshot", nameof(TransportLegDto.TransportLegId))]
    [InlineData(typeof(TransportRateDto), "v2.TransportRate_Snapshot", nameof(TransportRateDto.TransportRateId))]
    [InlineData(typeof(LocationsNodesDto), "v2.LocationsNodes_Snapshot", nameof(LocationsNodesDto.LocationsNodesId))]
    public void GetMetadata_ReturnsSnapshotTableAndEntityKey(Type dtoType, string expectedTable, string expectedEntityKey)
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(dtoType);

        metadata.TableName.Should().Be(expectedTable);
        metadata.EntityKeyPropertyName.Should().Be(expectedEntityKey);
        metadata.Columns.Should().NotBeEmpty();
        metadata.Columns.Should().ContainSingle(c => c.IsEntityKey);
    }

    [Fact]
    public void BuildAllowedColumnsJson_IncludesOnlyFilterableColumns()
    {
        var json = GridColumnMetadataBuilder.BuildAllowedColumnsJson(typeof(TransportLegDto));
        var columns = JArray.Parse(json);

        columns.Should().NotBeEmpty();
        columns.Select(c => c["ColumnName"]!.Value<string>()).Should().Contain(nameof(TransportLegDto.Code));
        columns.Select(c => c["ColumnName"]!.Value<string>()).Should().NotContain(nameof(TransportLegDto.TransportKindCode));
    }

    [Fact]
    public void BuildAllowedColumnsJson_UsesSqlColumnNameWhenSpecified()
    {
        var json = GridColumnMetadataBuilder.BuildAllowedColumnsJson(typeof(TransportLegDto));
        var columns = JArray.Parse(json);

        var shipmentType = columns.Single(c => c["ColumnName"]!.Value<string>() == nameof(TransportLegDto.ShipmentTypeIdRu));
        shipmentType["SqlColumnName"]!.Value<string>().Should().Be("ShipmentTypeId");
        shipmentType["ColumnType"]!.Value<string>().Should().Be("ID");
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
}
