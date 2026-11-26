namespace BlazorBootstrap;

public partial class GridColumnFilter : BlazorBootstrapComponentBase
{
    #region Fields and Constants

    /// <summary>
    /// Popper fixed strategy so filter menus escape .table-responsive overflow clipping.
    /// Per-instance: DropdownToggleButton mutates AdditionalAttributes in OnInitialized.
    /// </summary>
    private readonly Dictionary<string, object> fixedStrategyPopperAttributes = new()
    {
        ["data-bs-popper-config"] = "{\"strategy\":\"fixed\"}"
    };

    private int inputKey;

    private int lastResetTrigger;

    private FilterOperator filterOperator;

    private IEnumerable<FilterOperatorInfo>? filterOperators;

    private string? filterValue;

    /// <summary>
    /// Точность DateTime-фильтра: day / hour / minute — определяет шаг диапазона в SQL.
    /// Значения примитивны намеренно: datetime-local не умеет частичные значения.
    /// </summary>
    private string dateTimePrecision = "minute";

    private string? selectedFilterSymbol;

    /// <summary>
    /// Выбранные значения мульти-фильтра enum — имена членов enum.
    /// </summary>
    private readonly HashSet<string> selectedEnumNames = new(StringComparer.OrdinalIgnoreCase);

    #endregion

    #region Methods

    protected override async Task OnInitializedAsync()
    {
        filterOperators = await GetFilterOperatorsAsync(PropertyTypeName!);
        filterOperator = FilterOperator;
        dateTimePrecision = InferDateTimePrecision(FilterValue);
        filterValue = PropertyTypeName == StringConstants.PropertyTypeNameDateTime
            ? AdaptDateTimeValueToPrecision(FilterValue, dateTimePrecision)
            : FilterValue;
        SyncSelectedEnumNames();

        await base.OnInitializedAsync();
    }

    protected override void OnParametersSet()
    {
        if (ResetTrigger != lastResetTrigger)
        {
            lastResetTrigger = ResetTrigger;
            dateTimePrecision = InferDateTimePrecision(FilterValue);
            filterValue = PropertyTypeName == StringConstants.PropertyTypeNameDateTime
                ? AdaptDateTimeValueToPrecision(FilterValue, dateTimePrecision)
                : FilterValue;
            SyncSelectedEnumNames();
            inputKey++;
        }
        SetDefaultFilter();
        SetSelectedFilterSymbol();
    }

    internal void SetDefaultFilter()
    {
        if (PropertyTypeName is StringConstants.PropertyTypeNameInt16
                                or StringConstants.PropertyTypeNameInt32
                                or StringConstants.PropertyTypeNameInt64
                                or StringConstants.PropertyTypeNameSingle // float
                                or StringConstants.PropertyTypeNameDecimal
                                or StringConstants.PropertyTypeNameDouble)
        {
            if (filterOperator is FilterOperator.None or FilterOperator.Clear)
                filterOperator = FilterOperator.Equals;
        }
        else if (PropertyTypeName is StringConstants.PropertyTypeNameString
                                     or StringConstants.PropertyTypeNameChar)
        {
            if (filterOperator is FilterOperator.None or FilterOperator.Clear)
                filterOperator = FilterOperator.Contains;
        }
        else if (PropertyTypeName is StringConstants.PropertyTypeNameDateOnly
                                     or StringConstants.PropertyTypeNameDateTime)
        {
            if (filterOperator is FilterOperator.None or FilterOperator.Clear)
                filterOperator = FilterOperator.Equals;
        }
        else if (PropertyTypeName == StringConstants.PropertyTypeNameBoolean)
        {
            if (filterOperator is FilterOperator.None or FilterOperator.Clear)
                filterOperator = FilterOperator.Equals;
        }
        else if (PropertyTypeName == StringConstants.PropertyTypeNameEnum)
        {
            if (filterOperator is FilterOperator.None or FilterOperator.Clear)
                filterOperator = MultiSelect ? FilterOperator.In : FilterOperator.Equals;
        }
        else if (PropertyTypeName == StringConstants.PropertyTypeNameGuid)
        {
            if (filterOperator is FilterOperator.None or FilterOperator.Clear)
                filterOperator = FilterOperator.Equals;
        }
    }

    private async Task<IEnumerable<FilterOperatorInfo>> GetFilterOperatorsAsync(string propertyTypeName)
    {
        // Мультивыбор enum: список операторов не нужен, значения выбираются чекбоксами.
        // Оставляем только сброс — оператор всегда In.
        if (MultiSelect && propertyTypeName == StringConstants.PropertyTypeNameEnum)
        {
            var clear = new FilterOperatorInfo("x", "Clear", FilterOperator.Clear);
            if (FiltersTranslationProvider is null)
                return new[] { clear };

            var translations = await FiltersTranslationProvider.Invoke();
            return new[] { translations?.FirstOrDefault(x => x.FilterOperator == FilterOperator.Clear) ?? clear };
        }

        if (FiltersTranslationProvider is null)
            return FilterOperatorUtility.GetFilterOperators(PropertyTypeName!, WithoutFullText);

        var filters = await FiltersTranslationProvider.Invoke();

        if (!(filters?.Any() ?? false))
            return FilterOperatorUtility.GetFilterOperators(PropertyTypeName!, WithoutFullText);

        return FilterOperatorUtility.GetFilterOperators(PropertyTypeName!, filters!, WithoutFullText);
    }

    private async Task OnEnumFilterValueChangedAsync(object enumValue)
    {
        filterValue = enumValue?.ToString();

        if (GridColumnFilterChanged.HasDelegate)
            await GridColumnFilterChanged.InvokeAsync(new FilterEventArgs(filterValue!, filterOperator));
    }

    private async Task OnEnumMultiFilterValueChangedAsync(string? enumName)
    {
        if (enumName is null)
            return;

        if (!selectedEnumNames.Remove(enumName))
            selectedEnumNames.Add(enumName);

        // Порядок значений в строке фильтра — как в выпадающем списке, не в порядке кликов:
        // одинаковый состав даёт одинаковый FilterItem (это сравнивает HasSameEffectiveFiltersAsLastQuery).
        filterValue = string.Join(
            ',',
            EnumFilterItems
                .Select(item => Enum.GetName(PropertyType!, item))
                .Where(name => name is not null && selectedEnumNames.Contains(name)));

        if (GridColumnFilterChanged.HasDelegate)
            await GridColumnFilterChanged.InvokeAsync(new FilterEventArgs(filterValue, filterOperator));
    }

    /// <summary>
    /// Восстанавливает отмеченные значения чекбоксов из строки фильтра (после сброса и при инициализации).
    /// </summary>
    private void SyncSelectedEnumNames()
    {
        selectedEnumNames.Clear();

        if (!MultiSelect
            || PropertyTypeName != StringConstants.PropertyTypeNameEnum
            || PropertyType is null
            || string.IsNullOrWhiteSpace(filterValue))
            return;

        var knownNames = Enum.GetNames(PropertyType);
        foreach (var name in filterValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (knownNames.Contains(name, StringComparer.OrdinalIgnoreCase))
                selectedEnumNames.Add(name);
        }
    }

    private string SelectedEnumFilterText =>
        PropertyType is null
            ? string.Empty
            : string.Join(", ", selectedEnumNames.Select(name => PropertyType.GetDisplayName(name)));

    private async Task OnFilterOperatorChangedAsync(FilterOperatorInfo filterOperatorInfo)
    {
        if (filterOperatorInfo.FilterOperator == FilterOperator.Clear)
        {
            SetDefaultFilter();
            selectedEnumNames.Clear();

            if (PropertyTypeName == StringConstants.PropertyTypeNameBoolean)
                filterValue = null;
            else
                filterValue = string.Empty;
        }
        else
        {
            filterOperator = filterOperatorInfo.FilterOperator;
        }

        SetSelectedFilterSymbol();

        if (GridColumnFilterChanged.HasDelegate)
            await GridColumnFilterChanged.InvokeAsync(new FilterEventArgs(EffectiveFilterValue!, filterOperator));
    }

    private async Task OnFilterValueChangedAsync(ChangeEventArgs args)
    {
        filterValue = args?.Value?.ToString();

        if (GridColumnFilterChanged.HasDelegate)
            await GridColumnFilterChanged.InvokeAsync(new FilterEventArgs(EffectiveFilterValue!, filterOperator));
    }

    /// <summary>
    /// Смена точности DateTime-фильтра: значение подгоняется под валидный для контрола формат,
    /// в FilterItem уходит усечённое до выбранной точности значение.
    /// </summary>
    private async Task OnDateTimePrecisionChangedAsync(ChangeEventArgs args)
    {
        dateTimePrecision = args?.Value?.ToString() ?? "minute";
        filterValue = AdaptDateTimeValueToPrecision(filterValue, dateTimePrecision);
        inputKey++;

        if (GridColumnFilterChanged.HasDelegate)
            await GridColumnFilterChanged.InvokeAsync(new FilterEventArgs(EffectiveFilterValue!, filterOperator));
    }

    /// <summary>
    /// Что реально уходит в FilterItem — datetime-local не умеет частичные значения, поэтому точность режем вручную.
    /// </summary>
    private string? EffectiveFilterValue =>
        PropertyTypeName == StringConstants.PropertyTypeNameDateTime && !string.IsNullOrEmpty(filterValue)
            ? dateTimePrecision switch
            {
                "day" => filterValue.Length > 10 ? filterValue[..10] : filterValue,
                "hour" => filterValue.Length > 13 ? filterValue[..13] : filterValue,
                _ => filterValue.Length > 16 ? filterValue[..16] : filterValue
            }
            : filterValue;

    /// <summary>
    /// Точность из длины значения: 10 — день, 13 — час, 16 и больше — минута.
    /// </summary>
    private static string InferDateTimePrecision(string? value) =>
        value?.Length switch
        {
            <= 10 => "day",
            <= 13 => "hour",
            _ => "minute"
        };

    /// <summary>
    /// Приводит значение к валидному для контрола формату: type=date принимает только дату,
    /// datetime-local — дату и минуты (час держится на :00 через step).
    /// </summary>
    private static string? AdaptDateTimeValueToPrecision(string? value, string precision)
    {
        if (string.IsNullOrEmpty(value) || value.Length < 10)
            return value;

        return precision switch
        {
            "day" => value[..10],
            "hour" => (value.Length >= 13 ? value[..13] : value[..10] + "T00") + ":00",
            _ => value.Length >= 16 ? value[..16] : value[..10] + "T00:00"
        };
    }

    private string DateTimeInputType => dateTimePrecision == "day" ? "date" : "datetime-local";

    private string? DateTimeStep => dateTimePrecision == "hour" ? "3600" : null;

    /// <summary>
    /// Точность показываем только для = и != — им шаг диапазона определяет весь интервал.
    /// Остальные операторы сравнивают с мгновением: скрытой точности достаточно усекать значение.
    /// </summary>
    private bool ShowDateTimePrecision =>
        filterOperator is FilterOperator.Equals or FilterOperator.NotEquals;

    private static string DateTimePrecisionLabel(string precision) =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ru"
            ? precision switch { "day" => "День", "hour" => "Час", _ => "Минута" }
            : precision switch { "day" => "Day", "hour" => "Hour", _ => "Minute" };

    private string DateTimePrecisionTitle =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ru" ? "Точность" : "Precision";

    private void SetSelectedFilterSymbol()
    {
        if (PropertyTypeName is StringConstants.PropertyTypeNameInt16
                                or StringConstants.PropertyTypeNameInt32
                                or StringConstants.PropertyTypeNameInt64
                                or StringConstants.PropertyTypeNameSingle // float
                                or StringConstants.PropertyTypeNameDecimal
                                or StringConstants.PropertyTypeNameDouble)
            selectedFilterSymbol = filterOperators?.FirstOrDefault(x => x.FilterOperator == filterOperator)?.Symbol;
        else if (PropertyTypeName is StringConstants.PropertyTypeNameString
                                     or StringConstants.PropertyTypeNameChar)
            selectedFilterSymbol = filterOperators?.FirstOrDefault(x => x.FilterOperator == filterOperator)?.Symbol;
        else if (PropertyTypeName is StringConstants.PropertyTypeNameDateOnly
                                     or StringConstants.PropertyTypeNameDateTime)
            selectedFilterSymbol = filterOperators?.FirstOrDefault(x => x.FilterOperator == filterOperator)?.Symbol;
        else if (PropertyTypeName == StringConstants.PropertyTypeNameBoolean) selectedFilterSymbol = filterOperators?.FirstOrDefault(x => x.FilterOperator == filterOperator)?.Symbol;
        else if (PropertyTypeName == StringConstants.PropertyTypeNameEnum) selectedFilterSymbol = filterOperators?.FirstOrDefault(x => x.FilterOperator == filterOperator)?.Symbol;
        else if (PropertyTypeName == StringConstants.PropertyTypeNameGuid) selectedFilterSymbol = filterOperators?.FirstOrDefault(x => x.FilterOperator == filterOperator)?.Symbol;
    }

    #endregion

    #region Properties, Indexers

    [Parameter] 
    public int ResetTrigger { get; set; }


    [Parameter]
    public string? EnumFilterSelectText { get; set; }

    [Parameter]
    public IReadOnlyList<Enum>? FilterEnumValues { get; set; }

    /// <summary>
    /// If <see langword="true" />, the enum filter renders a checkbox list and sends
    /// <see cref="FilterOperator.In" /> with a comma-separated list of enum names.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    [Parameter]
    public bool MultiSelect { get; set; }

    private IEnumerable<object> EnumFilterItems =>
        FilterEnumValues ?? Enum.GetValues(PropertyType!).Cast<object>();

    /// <summary>
    /// Gets or sets the filter button color.
    /// </summary>
    /// <remarks>
    /// Default value is <see cref="ButtonColor.Light" />.
    /// </remarks>
    [Parameter]
    public ButtonColor FilterButtonColor { get; set; } = ButtonColor.Light;

    /// <summary>
    /// Gets or sets the filter button CSS class.
    /// </summary>
    /// <remarks>
    /// Default value is null.
    /// </remarks>
    [Parameter]
    public string? FilterButtonCSSClass { get; set; }

    private string filterButtonColorString => FilterButtonColor.ToButtonColorClass();

    /// <summary>
    /// NVARCHAR без full-text: подпись «a**» / «Начинается с», оператор остаётся Contains.
    /// </summary>
    [Parameter]
    public bool WithoutFullText { get; set; }

    /// <summary>
    /// Gets or sets filter operator.
    /// </summary>
    [Parameter]
    public FilterOperator FilterOperator { get; set; }

    /// <summary>
    /// Filters transalation is for grid filters to render.
    /// The provider should always return a 'FilterOperatorInfo' collection, and 'null' is not allowed.
    /// </summary>
    [Parameter]
    public GridFiltersTranslationDelegate FiltersTranslationProvider { get; set; } = default!;

    private string filterStyle => FilterWidth > 0 ? $"width:{FilterWidth.ToString(CultureInfo.InvariantCulture)}{Unit.ToCssString()};" : "";

    private bool IsFilterChecked => filterValue is "True" or "true" or "1";

    /// <summary>
    /// Gets or sets filter value.
    /// </summary>
    /// <remarks>
    /// Default value is null.
    /// </remarks>
    [Parameter]
    public string? FilterValue { get; set; }

    /// <summary>
    /// Gets or sets the filter textbox width.
    /// </summary>
    [Parameter]
    public int FilterWidth { get; set; }

    /// <summary>
    /// Gets or sets the grid fixed header.
    /// </summary>
    [Parameter]
    public bool FixedHeader { get; set; }

    [Parameter] public EventCallback<FilterEventArgs> GridColumnFilterChanged { get; set; }

    /// <summary>
    /// Gets or sets the filter property name.
    /// </summary>
    [Parameter]
    public Type? PropertyType { get; set; }

    /// <summary>
    /// Gets or sets the filter property name.
    /// </summary>
    /// <remarks>
    /// Default value is null.
    /// </remarks>
    [Parameter]
    public string? PropertyTypeName { get; set; }

    /// <summary>
    /// Gets or sets the units.
    /// </summary>
    [Parameter]
    public Unit Unit { get; set; }

    #endregion
}
