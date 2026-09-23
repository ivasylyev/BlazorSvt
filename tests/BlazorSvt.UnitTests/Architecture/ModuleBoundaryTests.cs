using System.Reflection;
using System.Text.RegularExpressions;
using BlazorSvt.Platform.Sync;
using FluentAssertions;
using NetArchTest.Rules;

namespace BlazorSvt.UnitTests.Architecture;

[Trait("Category", "Unit")]
public class ModuleBoundaryTests
{
    private static readonly Assembly App = typeof(ISnapshotSyncJob).Assembly;

    [Fact]
    public void Module_does_not_reference_another_module()
    {
        var modules = ModuleNames();
        var violations = new List<string>();

        foreach (var source in modules)
        {
            foreach (var target in modules)
            {
                if (source == target)
                    continue;

                var result = Types.InAssembly(App)
                    .That()
                    .ResideInNamespaceMatching(NamespaceOf(source))
                    .ShouldNot()
                    .HaveDependencyOn(DependencyOf(target))
                    .GetResult();

                if (result.IsSuccessful)
                    continue;

                var types = string.Join(", ", result.FailingTypeNames ?? []);
                violations.Add($"{source} → {target}: {types}");
            }
        }

        violations.Should().BeEmpty(
            "модуль не ссылается на другой модуль:{0}{1}",
            Environment.NewLine,
            string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void Platform_does_not_reference_modules()
    {
        var result = Types.InAssembly(App)
            .That()
            .ResideInNamespaceMatching(@"^BlazorSvt\.Platform($|\.)")
            .ShouldNot()
            .HaveDependencyOn("BlazorSvt.Modules.")
            .GetResult();

        var types = string.Join(", ", result.FailingTypeNames ?? []);
        result.IsSuccessful.Should().BeTrue("Platform не ссылается на Modules: {0}", types);
    }

    private static IReadOnlyList<string> ModuleNames() =>
        App.GetTypes()
            .Select(t => t.Namespace)
            .Where(ns => ns?.StartsWith("BlazorSvt.Modules.", StringComparison.Ordinal) == true)
            .Select(ns => ns!.Split('.')[2])
            .Distinct(StringComparer.Ordinal)
            .Order()
            .ToArray();

    // Корень модуля и дочерние List/Detail/Sync, без совпадения по префиксу имени:
    // TransportRate не захватывает гипотетический TransportRateExtra.
    private static string NamespaceOf(string module) =>
        $@"^BlazorSvt\.Modules\.{Regex.Escape(module)}($|\.)";

    // HaveDependencyOn сравнивает префикс полного имени типа.
    // Точка на конце отделяет Rate от RateType.
    private static string DependencyOf(string module) =>
        $"BlazorSvt.Modules.{module}.";
}
