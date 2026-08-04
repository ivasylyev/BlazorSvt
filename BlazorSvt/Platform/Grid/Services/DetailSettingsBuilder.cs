using System.Linq.Expressions;
using BlazorBootstrap;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Helps build detail field settings without repeating Name/Display/Visible boilerplate.
/// Empty-field hiding is applied in <c>GenericDetailView</c> via <c>Grid:HideEmptyDetailFields</c>.
/// </summary>
public sealed class DetailSettingsBuilder<T>(IStringLocalizer<PlatformResources> platform)
{
    private readonly List<DetailSetting<T>> fields = [];

    public DetailSettingsCollection<T> Build() => new(fields);

    public DetailSettingsBuilder<T> Add<TProp>(
        string groupHeader,
        Expression<Func<T, TProp>> property,
        string header,
        Func<T, bool>? visible = null,
        bool hasMargin = false,
        Func<T, object>? display = null)
    {
        var compiled = property.Compile();
        fields.Add(new DetailSetting<T>
        {
            Name = MemberName(property),
            Header = header,
            GroupHeader = groupHeader,
            DisplaySelector = display ?? (dto => compiled(dto) is { } value ? value : string.Empty),
            VisibleSelector = visible ?? (_ => true),
            HasMargin = hasMargin
        });
        return this;
    }

    public DetailSettingsBuilder<T> AddLocalized<TProp>(
        bool isRu,
        string groupHeader,
        Expression<Func<T, TProp>> ruProperty,
        Expression<Func<T, TProp>> enProperty,
        string header,
        Func<T, bool>? visible = null,
        bool hasMargin = false)
    {
        return Add(groupHeader, isRu ? ruProperty : enProperty, header, visible, hasMargin);
    }

    public DetailSettingsBuilder<T> AddEnum<TRu, TEn>(
        bool isRu,
        string groupHeader,
        Expression<Func<T, TRu>> ruProperty,
        Expression<Func<T, TEn>> enProperty,
        string header,
        Func<T, bool>? visible = null,
        bool hasMargin = false)
        where TRu : Enum
        where TEn : Enum
    {
        if (isRu)
        {
            var compiled = ruProperty.Compile();
            return Add(
                groupHeader,
                ruProperty,
                header,
                visible,
                hasMargin,
                dto => typeof(TRu).GetDisplayName(compiled(dto).ToString()) ?? string.Empty);
        }

        var compiledEn = enProperty.Compile();
        return Add(
            groupHeader,
            enProperty,
            header,
            visible,
            hasMargin,
            dto => typeof(TEn).GetDisplayName(compiledEn(dto).ToString()) ?? string.Empty);
    }

    public DetailSettingsBuilder<T> AddYesNo(
        string groupHeader,
        Expression<Func<T, bool>> property,
        string header,
        Func<T, bool>? visible = null,
        bool hasMargin = false)
    {
        var compiled = property.Compile();
        return Add(
            groupHeader,
            property,
            header,
            visible,
            hasMargin,
            dto => compiled(dto) ? platform["Common.Yes"] : platform["Common.No"]);
    }

    public DetailSettingsBuilder<T> AddYesNo(
        string groupHeader,
        Expression<Func<T, bool?>> property,
        string header,
        Func<T, bool>? visible = null,
        bool hasMargin = false)
    {
        var compiled = property.Compile();
        return Add(
            groupHeader,
            property,
            header,
            visible,
            hasMargin,
            dto => compiled(dto) switch
            {
                true => platform["Common.Yes"],
                false => platform["Common.No"],
                null => string.Empty
            });
    }

    public DetailSettingsBuilder<T> AddArchiveStatus(
        string groupHeader,
        Expression<Func<T, bool>> property,
        string header,
        Func<T, bool>? visible = null,
        bool hasMargin = false)
    {
        var compiled = property.Compile();
        return Add(
            groupHeader,
            property,
            header,
            visible,
            hasMargin,
            dto => compiled(dto) ? platform["Common.Archive"] : platform["Common.Active"]);
    }

    private static string MemberName<TProp>(Expression<Func<T, TProp>> property) =>
        property.Body switch
        {
            MemberExpression member => member.Member.Name,
            UnaryExpression { Operand: MemberExpression member } => member.Member.Name,
            _ => throw new ArgumentException($"Expression must be a property access: {property}", nameof(property))
        };
}
