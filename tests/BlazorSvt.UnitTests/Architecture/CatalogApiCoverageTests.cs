using System.Reflection;
using BlazorSvt.Platform.Api;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Infrastructure.Config;
using BlazorSvt.Platform.Sync;
using BlazorSvt.Platform.UI.Navigation;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorSvt.UnitTests.Architecture;

/// <summary>Всё, что есть в СВТ 2.0, доступно через API: справочник без <c>AddCatalogApi</c> валит сборку.</summary>
[Trait("Category", "Unit")]
public class CatalogApiCoverageTests
{
    private static readonly Assembly App = typeof(ISnapshotSyncJob).Assembly;

    private static readonly Lazy<IServiceCollection> Services = new(RegisterAllModules);

    [Fact]
    public void EveryGridSnapshotDto_IsRegisteredInApi()
    {
        var snapshotDtos = App.GetTypes()
            .Where(type => type.IsDefined(typeof(GridSnapshotAttribute), inherit: false))
            .ToArray();

        snapshotDtos.Should().NotBeEmpty();
        Descriptors().Select(d => d.ItemType).Should().BeEquivalentTo(snapshotDtos);
    }

    [Fact]
    public void EveryCatalogMenuItem_HasApiCatalog()
    {
        var menuTitles = Services.Value
            .Where(s => s.ImplementationInstance is CatalogMenuContribution)
            .Select(s => ((CatalogMenuContribution)s.ImplementationInstance!).TextResourceKey);

        Descriptors().Select(d => d.TitleResourceKey).Should().BeEquivalentTo(menuTitles);
    }

    [Fact]
    public void ApiCodes_AreUniqueKebabCase()
    {
        var codes = Descriptors().Select(d => d.Code).ToArray();

        codes.Should().OnlyHaveUniqueItems();
        codes.Should().OnlyContain(c => System.Text.RegularExpressions.Regex.IsMatch(c, "^[a-z0-9]+(-[a-z0-9]+)*$"));
    }

    [Fact]
    public void Period_PointsToExistingDateProperties()
    {
        foreach (var descriptor in Descriptors().Where(d => d.Period is not null))
        {
            var fields = CatalogFields.For(descriptor.ItemType).ToDictionary(f => f.PropertyName);
            fields.Should().ContainKey(descriptor.Period!.FromProperty);
            fields.Should().ContainKey(descriptor.Period.ToProperty);
            fields[descriptor.Period.FromProperty].Type.Should().Be(CatalogFieldTypes.Date);
        }
    }

    [Fact]
    public void EveryCatalog_HasEndpointContribution()
    {
        var contributions = Services.Value
            .Count(s => s.ServiceType == typeof(IApiEndpointContribution) && s.ImplementationInstance?.GetType().IsGenericType == true);

        contributions.Should().Be(Descriptors().Count);
    }

    private static IReadOnlyList<CatalogApiDescriptor> Descriptors() =>
        Services.Value
            .Where(s => s.ImplementationInstance is CatalogApiDescriptor)
            .Select(s => (CatalogApiDescriptor)s.ImplementationInstance!)
            .ToList();

    private static IServiceCollection RegisterAllModules()
    {
        var services = new ServiceCollection();
        var moduleRegistrations = App.GetTypes()
            .Where(t => t is { IsAbstract: true, IsSealed: true }
                        && t.Namespace?.StartsWith("BlazorSvt.Modules.", StringComparison.Ordinal) == true)
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(m => m.Name.StartsWith("Add", StringComparison.Ordinal)
                        && m.Name.EndsWith("Module", StringComparison.Ordinal)
                        && m.GetParameters() is [{ ParameterType: var p }] && p == typeof(IServiceCollection));

        foreach (var method in moduleRegistrations)
        {
            method.Invoke(null, [services]);
        }

        return services;
    }
}
