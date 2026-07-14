using System.Linq.Expressions;
using BlazorBootstrap;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Helps build default grid column settings without repeating Name/Display/Sort boilerplate.
/// </summary>
public sealed class GridColumnSettingsBuilder<T>(IStringLocalizer<PlatformResources> platform)
{
    private readonly List<GridColumnSetting<T>> _columns = [];

    public List<GridColumnSetting<T>> Build() => _columns;

    public GridColumnSettingsBuilder<T> Add<TProp>(
        Expression<Func<T, TProp>> property,
        string header,
        bool visible = true,
        bool filterable = true,
        Func<T, object>? display = null)
    {
        var compiled = property.Compile();
        _columns.Add(new GridColumnSetting<T>
        {
            Name = MemberName(property),
            Header = header,
            DisplaySelector = display ?? (dto => compiled(dto) is { } value ? value : string.Empty),
            SortSelector = ToSort(property),
            Visible = visible,
            Filterable = filterable
        });
        return this;
    }

    public GridColumnSettingsBuilder<T> AddLocalized<TProp>(
        bool isRu,
        Expression<Func<T, TProp>> ruProperty,
        Expression<Func<T, TProp>> enProperty,
        string header,
        bool visible = true,
        bool filterable = true)
    {
        return Add(isRu ? ruProperty : enProperty, header, visible, filterable);
    }

    public GridColumnSettingsBuilder<T> AddEnum<TRu, TEn>(
        bool isRu,
        Expression<Func<T, TRu>> ruProperty,
        Expression<Func<T, TEn>> enProperty,
        string header,
        bool visible = true,
        bool filterable = true)
        where TRu : Enum
        where TEn : Enum
    {
        if (isRu)
        {
            var compiled = ruProperty.Compile();
            return Add(
                ruProperty,
                header,
                visible,
                filterable,
                dto => typeof(TRu).GetDisplayName(compiled(dto).ToString()) ?? string.Empty);
        }

        var compiledEn = enProperty.Compile();
        return Add(
            enProperty,
            header,
            visible,
            filterable,
            dto => typeof(TEn).GetDisplayName(compiledEn(dto).ToString()) ?? string.Empty);
    }

    public GridColumnSettingsBuilder<T> AddEnum<TEnum>(
        Expression<Func<T, TEnum>> property,
        string header,
        bool visible = true,
        bool filterable = true)
        where TEnum : Enum
    {
        var compiled = property.Compile();
        return Add(
            property,
            header,
            visible,
            filterable,
            dto => typeof(TEnum).GetDisplayName(compiled(dto).ToString()) ?? string.Empty);
    }

    public GridColumnSettingsBuilder<T> AddYesNo(
        Expression<Func<T, bool>> property,
        string header,
        bool visible = true,
        bool filterable = true)
    {
        var compiled = property.Compile();
        return Add(
            property,
            header,
            visible,
            filterable,
            dto => compiled(dto) ? platform["Common.Yes"] : platform["Common.No"]);
    }

    public GridColumnSettingsBuilder<T> AddDateOnly(
        Expression<Func<T, DateOnly>> property,
        string header,
        bool visible = true,
        bool filterable = true)
    {
        var compiled = property.Compile();
        return Add(
            property,
            header,
            visible,
            filterable,
            dto => compiled(dto).ToShortDateString());
    }

    public GridColumnSettingsBuilder<T> AddSystemColumns(
        Expression<Func<T, DateTime>> creationDate,
        Expression<Func<T, DateTime>> lastChangeDate,
        Expression<Func<T, bool>> isArchive,
        string creationDateHeader,
        string lastChangeDateHeader,
        string isArchiveHeader)
    {
        Add(creationDate, creationDateHeader, visible: false);
        Add(lastChangeDate, lastChangeDateHeader, visible: false);

        var compiledArchive = isArchive.Compile();
        var archive = new GridColumnSetting<T>
        {
            Name = MemberName(isArchive),
            Header = isArchiveHeader,
            DisplaySelector = dto => compiledArchive(dto)
                ? platform["Common.Archive"]
                : platform["Common.Active"],
            SortSelector = ToSort(isArchive),
            Visible = false,
            Filterable = true,
            FilterValue = "False"
        };
        _columns.Add(archive);
        return this;
    }

    private static string MemberName<TProp>(Expression<Func<T, TProp>> property) =>
        property.Body switch
        {
            MemberExpression member => member.Member.Name,
            UnaryExpression { Operand: MemberExpression member } => member.Member.Name,
            _ => throw new ArgumentException($"Expression must be a property access: {property}", nameof(property))
        };

    private static Expression<Func<T, IComparable>> ToSort<TProp>(Expression<Func<T, TProp>> property)
    {
        var getter = property.Compile();
        return dto => ToComparable(getter(dto));
    }

    private static IComparable ToComparable<TProp>(TProp value) =>
        value is IComparable comparable ? comparable : string.Empty;
}
