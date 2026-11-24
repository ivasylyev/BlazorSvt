using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using BlazorBootstrap;
using BlazorSvt.Modules.AverageRateLevel3;
using BlazorSvt.Modules.Coefficient;
using BlazorSvt.Modules.Contractor;
using BlazorSvt.Modules.TransportLeg;
using BlazorSvt.Modules.TransportRate;
using BlazorSvt.Modules.TransportRoute;
using BlazorSvt.Platform.Grid.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace BlazorSvt.UnitTests.Platform.Grid;

/// <summary>
/// Фильтр во вложенной таблице строится для каждой колонки каждой дочерней секции шести модулей:
/// без <c>ArgumentException("Unsupported property type")</c> и без других исключений.
/// </summary>
[Trait("Category", "Unit")]
public class DetailChildSectionFiltersTests
{
    public static TheoryData<string, string> Sections()
    {
        var data = new TheoryData<string, string>();
        foreach (var section in SectionTypes())
            foreach (var lang in new[] { "ru", "en" })
                data.Add(section.Name, lang);
        return data;
    }

    [Fact]
    public void SixModules_RegisterChildSections()
    {
        SectionTypes().Select(type => type.Name).Should().BeEquivalentTo(
            "CoefficientConditionValueSection",
            "CoefficientComponentValueSection",
            "AddressSection",
            "TransportRateTenderServicePackSection",
            "TransportRateCoefficientSection",
            "LegRateSection",
            "LegAverageRateLevel3Section",
            "LeadtimeSection",
            "TransportRouteLegSection",
            "RatesOfAverageRateSection");
    }

    [Theory]
    [MemberData(nameof(Sections))]
    public void EveryColumn_BuildsFilter(string sectionName, string lang)
    {
        var sectionType = SectionTypes().Single(type => type.Name == sectionName);
        var section = CreateSection(sectionType);
        var rowType = RowType(sectionType);

        var columns = ColumnNames(section, lang);

        columns.Should().NotBeEmpty();
        foreach (var column in columns)
            AssertFilterBuilds(rowType, column);
    }

    private static void AssertFilterBuilds(Type rowType, string propertyName)
    {
        var typeName = rowType.GetPropertyTypeName(propertyName);
        typeName.Should().NotBeEmpty($"{rowType.Name}.{propertyName} должен иметь тип, который понимает фильтр");

        var operators = FilterOperatorUtility.GetFilterOperators(typeName)
            .Select(info => info.FilterOperator)
            .Where(op => op != FilterOperator.Clear)
            .ToList();
        operators.Should().NotBeEmpty($"{rowType.Name}.{propertyName} ({typeName}) должен иметь операторы фильтра");

        var value = SampleValue(rowType, propertyName, typeName);
        var row = Activator.CreateInstance(rowType)!;
        var build = typeof(ExpressionExtensions)
            .GetMethod(nameof(ExpressionExtensions.GetExpressionDelegate))!
            .MakeGenericMethod(rowType);

        foreach (var op in operators)
        {
            var parameter = Expression.Parameter(rowType);
            var filter = new FilterItem(propertyName, value, op, StringComparison.OrdinalIgnoreCase);

            var lambda = (LambdaExpression?)build.Invoke(null, [parameter, filter]);

            lambda.Should().NotBeNull($"{rowType.Name}.{propertyName} {op}");
            lambda!.Compile().DynamicInvoke(row).Should().BeOfType<bool>($"{rowType.Name}.{propertyName} {op}");
        }
    }

    private static string SampleValue(Type rowType, string propertyName, string typeName)
    {
        var propertyType = rowType.GetProperty(propertyName)!.PropertyType;
        propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        return typeName switch
        {
            "Enum" => Enum.GetNames(propertyType).First(),
            "Boolean" => "True",
            "DateOnly" => "2026-03-05",
            "DateTime" => "2026-03-05T00:00:00",
            "Int16" or "Int32" or "Int64" => "1",
            "Single" or "Decimal" or "Double" => "1.5",
            _ => "a"
        };
    }

    private static IReadOnlyList<string> ColumnNames(object section, string lang)
    {
        var buildColumns = section.GetType().GetMethod(
            "BuildColumns",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;

        // parentIsArchive: true — полный набор колонок, включая статус архива.
        var collection = buildColumns.Invoke(section, [lang, true])!;
        var groups = collection.GetType().GetProperty("GroupSettings")!.GetValue(collection)!;

        var names = new List<string>();
        foreach (var group in (IEnumerable)groups)
        {
            var settings = (IEnumerable)group.GetType().GetProperty("Value")!.GetValue(group)!;
            foreach (var setting in settings)
                names.Add((string)setting.GetType().GetProperty("Name")!.GetValue(setting)!);
        }

        return names;
    }

    private static object CreateSection(Type sectionType)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddLocalization();
        services.AddSingleton(Substitute.For<IDetailChildDataService>());
        services.AddSingleton(sectionType);

        return services.BuildServiceProvider().GetRequiredService(sectionType);
    }

    private static Type RowType(Type sectionType)
    {
        for (var type = sectionType; type is not null; type = type.BaseType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DetailChildSection<,>))
                return type.GetGenericArguments()[1];
        }

        throw new InvalidOperationException($"{sectionType.Name} не наследует DetailChildSection");
    }

    private static IReadOnlyList<Type> SectionTypes()
    {
        var services = new ServiceCollection();
        services
            .AddCoefficientModule()
            .AddContractorModule()
            .AddTransportRateModule()
            .AddTransportLegModule()
            .AddTransportRouteModule()
            .AddAverageRateLevel3Module();

        return services
            .Where(d => d.ServiceType.IsGenericType
                        && d.ServiceType.GetGenericTypeDefinition() == typeof(IDetailChildSection<>))
            .Select(d => d.ImplementationType!)
            .ToList();
    }
}
