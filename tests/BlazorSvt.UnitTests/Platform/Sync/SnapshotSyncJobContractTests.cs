using BlazorSvt.Modules.AverageRateLevel3.List;
using BlazorSvt.Modules.AverageRateLevel3.Sync;
using BlazorSvt.Modules.LocationsNodes.List;
using BlazorSvt.Modules.LocationsNodes.Sync;
using BlazorSvt.Modules.ParityRates.List;
using BlazorSvt.Modules.ParityRates.Sync;
using BlazorSvt.Modules.Contractor.List;
using BlazorSvt.Modules.Contractor.Sync;
using BlazorSvt.Modules.MTR.List;
using BlazorSvt.Modules.MTR.Sync;
using BlazorSvt.Modules.ProductGroup.List;
using BlazorSvt.Modules.ProductGroup.Sync;
using BlazorSvt.Modules.TransportLeg.List;
using BlazorSvt.Modules.TransportLeg.Sync;
using BlazorSvt.Modules.TransportRate.List;
using BlazorSvt.Modules.TransportRate.Sync;
using BlazorSvt.Platform.Grid.Services;
using BlazorSvt.Platform.Sync;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Sync;

[Trait("Category", "Unit")]
public class SnapshotSyncJobContractTests
{
    private static readonly string[] StableSourceTables =
    [
        "dbo.PrimitiveEntityData_1007", // TypePlace
        "dbo.PrimitiveEntityData_2132", // TypeNode
        "dbo.PrimitiveEntityData_2008", // TransportKind
        "dbo.PrimitiveEntityData_2142", // ShipmentType
        "dbo.PrimitiveEntityData_2048", // RateType
        "dbo.PrimitiveEntityData_2009", // TransportType
        "dbo.PrimitiveEntityData_2023", // TransportTypeLevel3
        "dbo.PrimitiveEntityData_2016", // Currency
        "dbo.PrimitiveEntityData_2108", // Relevance
        "dbo.PrimitiveEntityData_2019", // ETSNG
    ];

    public static TheoryData<ISnapshotSyncJob, Type, string, string, string, string, string> RegisteredJobs =>
        new()
        {
            {
                new LocationsNodesSyncJob(),
                typeof(LocationsNodesDto),
                "LocationsNodes",
                "v2.LocationsNodes_Snapshot",
                nameof(LocationsNodesDto.LocationsNodesId),
                "v2.vw_LocationsNodes_SnapshotSource",
                "v2.LocationsNodes_PopulateAffectedKeys"
            },
            {
                new TransportLegSyncJob(),
                typeof(TransportLegDto),
                "TransportLeg",
                "v2.TransportLeg_Snapshot",
                nameof(TransportLegDto.TransportLegId),
                "v2.vw_TransportLeg_SnapshotSource",
                "v2.TransportLeg_PopulateAffectedKeys"
            },
            {
                new TransportRateSyncJob(),
                typeof(TransportRateDto),
                "TransportRate",
                "v2.TransportRate_Snapshot",
                nameof(TransportRateDto.TransportRateId),
                "v2.vw_TransportRate_SnapshotSource",
                "v2.TransportRate_PopulateAffectedKeys"
            },
            {
                new ParityRatesSyncJob(),
                typeof(ParityRatesDto),
                "ParityRates",
                "v2.ParityRates_Snapshot",
                nameof(ParityRatesDto.ParityRatesId),
                "v2.vw_ParityRates_SnapshotSource",
                "v2.ParityRates_PopulateAffectedKeys"
            },
            {
                new AverageRateLevel3SyncJob(),
                typeof(AverageRateLevel3Dto),
                "AverageRateLevel3",
                "v2.AverageRateLevel3_Snapshot",
                nameof(AverageRateLevel3Dto.AverageRateLevel3Id),
                "v2.vw_AverageRateLevel3_SnapshotSource",
                "v2.AverageRateLevel3_PopulateAffectedKeys"
            },
            {
                new ProductGroupSyncJob(),
                typeof(ProductGroupDto),
                "ProductGroup",
                "v2.ProductGroup_Snapshot",
                nameof(ProductGroupDto.ProductGroupId),
                "v2.vw_ProductGroup_SnapshotSource",
                "v2.ProductGroup_PopulateAffectedKeys"
            },
            {
                new MTRSyncJob(),
                typeof(MTRDto),
                "MTR",
                "v2.MTR_Snapshot",
                nameof(MTRDto.MTRId),
                "v2.vw_MTR_SnapshotSource",
                "v2.MTR_PopulateAffectedKeys"
            },
            {
                new ContractorSyncJob(),
                typeof(ContractorDto),
                "Contractor",
                "v2.Contractor_Snapshot",
                nameof(ContractorDto.ContractorId),
                "v2.vw_Contractor_SnapshotSource",
                "v2.Contractor_PopulateAffectedKeys"
            },
        };

    public static IEnumerable<object[]> AllJobs()
    {
        foreach (var row in RegisteredJobs)
            yield return [row[0]!];
    }

    [Theory]
    [MemberData(nameof(RegisteredJobs))]
    public void Job_HasExpectedMetadata(
        ISnapshotSyncJob job,
        Type dtoType,
        string entity,
        string snapshotTable,
        string entityKeyColumn,
        string sourceView,
        string populateProc)
    {
        job.Entity.Should().Be(entity);
        job.SnapshotTable.Should().Be(snapshotTable);
        job.EntityKeyColumn.Should().Be(entityKeyColumn);
        job.SourceProjectionView.Should().Be(sourceView);
        job.PopulateAffectedKeysProc.Should().Be(populateProc);

        var gridMetadata = GridColumnMetadataBuilder.GetMetadata(dtoType);
        gridMetadata.TableName.Should().Be(job.SnapshotTable);
        gridMetadata.EntityKeyPropertyName.Should().Be(job.EntityKeyColumn);
    }

    [Theory]
    [MemberData(nameof(RegisteredJobs))]
    public void GetMetadata_ReturnsSnapshotTableAndEntityKey(
        ISnapshotSyncJob job,
        Type dtoType,
        string entity,
        string expectedTable,
        string expectedEntityKey,
        string sourceView,
        string populateProc)
    {
        _ = job;
        _ = entity;
        _ = sourceView;
        _ = populateProc;

        var metadata = GridColumnMetadataBuilder.GetMetadata(dtoType);

        metadata.TableName.Should().Be(expectedTable);
        metadata.EntityKeyPropertyName.Should().Be(expectedEntityKey);
        metadata.Columns.Should().NotBeEmpty();
        metadata.Columns.Should().ContainSingle(c => c.IsEntityKey);
    }

    [Theory]
    [MemberData(nameof(AllJobs))]
    public void Job_SourcesAreUniqueAndNonEmpty(ISnapshotSyncJob job)
    {
        job.Sources.Should().NotBeEmpty();
        job.Sources.Select(s => s.Name).Should().OnlyHaveUniqueItems();
        job.Sources.Should().AllSatisfy(s => s.Name.Should().StartWith("dbo.PrimitiveEntityData_"));
    }

    [Theory]
    [MemberData(nameof(AllJobs))]
    public void Job_SourcesExcludeStableReferences(ISnapshotSyncJob job)
    {
        job.Sources.Select(s => s.Name).Should().NotContain(StableSourceTables);
    }

    [Fact]
    public void TransportRateJob_IncludesMtrSource_ForMembershipFilter()
    {
        var sources = new TransportRateSyncJob().Sources.Select(s => s.Name);

        sources.Should().Contain("dbo.PrimitiveEntityData_1015");
        sources.Should().NotContain("dbo.PrimitiveEntityData_2048");
    }

    [Fact]
    public void ContractorJob_TracksLegalTypeOnly()
    {
        var sources = new ContractorSyncJob().Sources.Select(s => s.Name);

        sources.Should().Equal("dbo.PrimitiveEntityData_2028");
    }

    [Fact]
    public void MTRJob_TracksProductGroupCascade()
    {
        var sources = new MTRSyncJob().Sources.Select(s => s.Name);

        sources.Should().Equal(
            "dbo.PrimitiveEntityData_1015", // MTR (основная)
            "dbo.PrimitiveEntityData_1013"); // ProductGroup
    }

    [Fact]
    public void SnapshotSyncJob_DerivesObjectNamesFromEntity()
    {
        ISnapshotSyncJob job = new NamingConventionJob();

        job.Entity.Should().Be("FooBar");
        job.SnapshotTable.Should().Be("v2.FooBar_Snapshot");
        job.EntityKeyColumn.Should().Be("FooBarId");
        job.SourceProjectionView.Should().Be("v2.vw_FooBar_SnapshotSource");
        job.PopulateAffectedKeysProc.Should().Be("v2.FooBar_PopulateAffectedKeys");
        job.Sources.Select(s => s.Name).Should().Equal(
            "dbo.PrimitiveEntityData_1",
            "dbo.PrimitiveEntityData_2");
    }

    [Fact]
    public void SnapshotSyncJob_RequiresAtLeastOneSource()
    {
        var act = () => new EmptySourcesJob();

        act.Should().Throw<ArgumentException>();
    }

    private sealed class NamingConventionJob() : SnapshotSyncJob(
        "FooBar",
        "dbo.PrimitiveEntityData_1",
        "dbo.PrimitiveEntityData_2");

    private sealed class EmptySourcesJob() : SnapshotSyncJob("FooBar");
}
