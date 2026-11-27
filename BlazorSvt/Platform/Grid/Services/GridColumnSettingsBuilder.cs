using System.Linq.Expressions;
using System.Reflection;
using BlazorBootstrap;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Helps build default grid column settings without repeating Name/Display/Sort boilerplate.
/// </summary>
public sealed class GridColumnSettingsBuilder<T>(IStringLocalizer<PlatformResources> platform)
{
    private readonly List<GridColumnSetting<T>> columns = [];

    public List<GridColumnSetting<T>> Build() => columns;

    public GridColumnSettingsBuilder<T> Add<TProp>(
        Expression<Func<T, TProp>> property,
        string header,
        bool visible = true,
        bool filterable = true,
        Func<T, object>? display = null,
        bool multiSelect = false)
    {
        var compiled = property.Compile();
        columns.Add(new GridColumnSetting<T>
        {
            Name = MemberName(property),
            Header = header,
            DisplaySelector = display ?? (dto => GridValueFormatter.Format(compiled(dto))),
            SortSelector = ToSort(property),
            Visible = visible,
            Filterable = filterable,
            WithoutFullText = IsWithoutFullText(property),
            FilterMultiSelect = multiSelect
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
        bool filterable = true,
        IReadOnlyList<TRu>? filterValuesRu = null,
        IReadOnlyList<TEn>? filterValuesEn = null,
        bool multiSelect = false)
        where TRu : Enum
        where TEn : Enum
    {
        if (isRu)
        {
            var compiled = ruProperty.Compile();
            Add(
                ruProperty,
                header,
                visible,
                filterable,
                dto => typeof(TRu).GetDisplayName(compiled(dto).ToString()) ?? string.Empty,
                multiSelect);
            columns[^1].FilterEnumValues = filterValuesRu?.Cast<Enum>().ToList();
            return this;
        }

        var compiledEn = enProperty.Compile();
        Add(
            enProperty,
            header,
            visible,
            filterable,
            dto => typeof(TEn).GetDisplayName(compiledEn(dto).ToString()) ?? string.Empty,
            multiSelect);
        columns[^1].FilterEnumValues = filterValuesEn?.Cast<Enum>().ToList();
        return this;
    }

    public GridColumnSettingsBuilder<T> AddEnum<TEnum>(
        Expression<Func<T, TEnum>> property,
        string header,
        bool visible = true,
        bool filterable = true,
        IReadOnlyList<TEnum>? filterValues = null,
        bool multiSelect = false)
        where TEnum : Enum
    {
        var compiled = property.Compile();
        Add(
            property,
            header,
            visible,
            filterable,
            dto => typeof(TEnum).GetDisplayName(compiled(dto).ToString()) ?? string.Empty,
            multiSelect);
        columns[^1].FilterEnumValues = filterValues?.Cast<Enum>().ToList();
        return this;
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
        Expression<Func<T, string?>> creationUser,
        Expression<Func<T, DateTime>> creationDate,
        Expression<Func<T, string?>> lastChangeUser,
        Expression<Func<T, DateTime>> lastChangeDate,
        Expression<Func<T, bool>> isArchive,
        string creationDateHeader,
        string lastChangeDateHeader,
        string isArchiveHeader)
    {
        AddCreationUser(creationUser);
        Add(creationDate, creationDateHeader, visible: false);
        AddLastChangeUser(lastChangeUser);
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
        columns.Add(archive);
        return this;
    }

    /// <summary>Колонка «Автор» (перед датой создания): скрыта по умолчанию, заголовок из Platform-ресурсов.</summary>
    public GridColumnSettingsBuilder<T> AddCreationUser(Expression<Func<T, string?>> creationUser) =>
        Add(creationUser, platform["Common.CreationUser"], visible: false);

    /// <summary>Колонка «Автор изм.» (перед датой изменения): скрыта по умолчанию, заголовок из Platform-ресурсов.</summary>
    public GridColumnSettingsBuilder<T> AddLastChangeUser(Expression<Func<T, string?>> lastChangeUser) =>
        Add(lastChangeUser, platform["Common.LastChangeUser"], visible: false);

    private static bool IsWithoutFullText<TProp>(Expression<Func<T, TProp>> property)
    {
        var member = property.Body switch
        {
            MemberExpression direct => direct.Member,
            UnaryExpression { Operand: MemberExpression inner } => inner.Member,
            _ => null
        };

        return member is PropertyInfo propertyInfo
            && propertyInfo.GetCustomAttribute<GridColumnAttribute>()?.WithoutFullText == true;
    }

    private static string MemberName<TProp>(Expression<Func<T, TProp>> property) =>
        property.Body switch
        {
            MemberExpression member => member.Member.Name,
            UnaryExpression { Operand: MemberExpression member } => member.Member.Name,
            _ => throw new ArgumentException($@"Expression must be a property access: {property}", nameof(property))
        };

    private static Expression<Func<T, IComparable>> ToSort<TProp>(Expression<Func<T, TProp>> property)
    {
        var getter = property.Compile();
        return dto => ToComparable(getter(dto));
    }

    private static IComparable ToComparable<TProp>(TProp value) =>
        value as IComparable ?? string.Empty;
}
