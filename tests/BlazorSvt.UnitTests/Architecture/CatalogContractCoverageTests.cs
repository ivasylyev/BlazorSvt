using System.Reflection;
using BlazorSvt.Modules.RateType.List;
using BlazorSvt.Platform.Infrastructure.Config;
using BlazorSvt.Platform.Sync;
using BlazorSvt.UnitTests.Platform.Sync;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Architecture;

[Trait("Category", "Unit")]
public class CatalogContractCoverageTests
{
    private static readonly Assembly App = typeof(ISnapshotSyncJob).Assembly;

    [Fact]
    public void WhenAssemblyDeclaresSnapshotSyncJobs_TypesMatchRegisteredJobs()
    {
        var implemented = App.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .Where(type => typeof(ISnapshotSyncJob).IsAssignableFrom(type))
            .ToArray();

        var registered = SnapshotSyncJobContractTests.RegisteredJobs
            .Select(row => row[0]!.GetType())
            .ToArray();

        implemented.Should().BeEquivalentTo(registered);
    }

    [Fact]
    public void WhenAssemblyDeclaresGridSnapshotDtos_TypesMatchRegisteredJobs()
    {
        var stableWithoutSyncJob = new[] { typeof(RateTypeDto) };

        var attributed = App.GetTypes()
            .Where(type => type.IsDefined(typeof(GridSnapshotAttribute), inherit: false))
            .Except(stableWithoutSyncJob)
            .ToArray();

        var registered = SnapshotSyncJobContractTests.RegisteredJobs
            .Select(row => (Type)row[1]!)
            .ToArray();

        attributed.Should().BeEquivalentTo(registered);
    }
}
