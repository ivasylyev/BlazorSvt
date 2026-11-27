using BlazorBootstrap;
using BlazorSvt.Modules.HomeRatePivot.Data;
using BlazorSvt.Platform.Grid.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using System.Globalization;

namespace BlazorSvt.Modules.HomeRatePivot.Components;

public partial class HomeRatePivotWidget : SvtComponentBase, IDisposable
{
    private static readonly string[] MonthResourceKeys =
    [
        "Month.Jan", "Month.Feb", "Month.Mar", "Month.Apr", "Month.May", "Month.Jun",
        "Month.Jul", "Month.Aug", "Month.Sep", "Month.Oct", "Month.Nov", "Month.Dec"
    ];

    // Первый ряд графика — ДНК СИБУР (sibur_style.md)
    private const string BarColor = "#008C95";
    // Названия осей — тёмно-зелёный, подписи делений — серый 50% (#5A5A5A), сетка — линия --svt-line (app.css)
    private const string TextColor = "#00313C";
    private const string TickColor = "#5A5A5A";
    private const string GridColor = "#D6DEE0";

    [Inject]
    private IHomeRatePivotService PivotService { get; set; } = default!;

    [Inject]
    private PageTimingService TimingService { get; set; } = default!;

    [Inject]
    private ILogger<HomeRatePivotWidget> Logger { get; set; } = default!;

    [Inject]
    private IStringLocalizer<Resources.HomeRatePivot> EL { get; set; } = default!;

    private readonly CancellationTokenSource _loadCancellationTokenSource = new();
    private HomeRatePivotTable? _table;
    private string[] _monthHeaders = [];
    private bool _isLoading = true;
    private string? _error;

    private BarChart _barChart = default!;
    private BarChartOptions _barChartOptions = default!;
    private ChartData _chartData = default!;
    private bool _chartInitialized;
    private HomeRatePivotRow? _selectedRow;

    protected override async Task OnInitializedAsync()
    {
        var cancellationToken = _loadCancellationTokenSource.Token;
        try
        {
            using (new StopwatchTransaction(TimingService))
            {
                var stopwatch = Stopwatch.StartNew();
                var useRussianNames = Lang.Equals("ru", StringComparison.OrdinalIgnoreCase);
                _table = await PivotService.GetTableAsync(useRussianNames, cancellationToken);
                _monthHeaders = _table.Months.Select(FormatMonthHeader).ToArray();
                Logger.LogDebug(
                    "Home rate pivot loaded {RowCount} rows in {ElapsedMs} ms",
                    _table.Rows.Count,
                    stopwatch.ElapsedMilliseconds);
            }

            _selectedRow = _table.Rows.FirstOrDefault();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // component disposed while loading
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load home rate pivot");
            _error = EL["HomeRatePivotWidget.LoadError"];
        }
        finally
        {
            _isLoading = false;
        }
    }

    public void Dispose()
    {
        _loadCancellationTokenSource.Cancel();
        _loadCancellationTokenSource.Dispose();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_table is null || _chartInitialized || _barChart is null || _selectedRow is null)
        {
            return;
        }

        _chartData = BuildChartData(_selectedRow);
        _barChartOptions = BuildChartOptions();

        await _barChart.InitializeAsync(_chartData, _barChartOptions);
        _chartInitialized = true;
    }

    private Task<GridDataProviderResult<HomeRatePivotRow>> GridDataProvider(
        GridDataProviderRequest<HomeRatePivotRow> request)
    {
        var rows = _table?.Rows ?? Array.Empty<HomeRatePivotRow>();
        return Task.FromResult(request.ApplyTo(rows));
    }

    private async Task OnGridRowClickAsync(GridRowEventArgs<HomeRatePivotRow> args)
    {
        _selectedRow = args.Item;
        _chartData = BuildChartData(args.Item);

        if (!_chartInitialized || _barChart is null)
        {
            return;
        }

        await _barChart.UpdateAsync(_chartData, _barChartOptions);
    }

    private string GetRowClass(HomeRatePivotRow row) =>
        ReferenceEquals(row, _selectedRow) ? "table-active" : string.Empty;

    private ChartData BuildChartData(HomeRatePivotRow row)
    {
        var data = row.RatesByMonth
            .Select(rate => rate is null ? (double?)null : (double)rate.Value)
            .ToList();

        var averageRateLabel = EL["HomeRatePivotWidget.AverageRate"].Value;
        var dataset = new BarChartDataset
        {
            Label = averageRateLabel,
            Data = data,
            BackgroundColor = [BarColor],
            BorderColor = [BarColor],
            BorderWidth = [0]
        };

        return new ChartData
        {
            Labels = _monthHeaders.ToList(),
            Datasets = [dataset]
        };
    }

    private BarChartOptions BuildChartOptions()
    {
        var options = new BarChartOptions
        {
            Responsive = true,
            Interaction = new Interaction { Mode = InteractionMode.Index },
            Locale = CultureInfo.CurrentCulture.Name
        };

        options.Scales.X!.Title = new ChartAxesTitle
        {
            Text = EL["HomeRatePivotWidget.Month"].Value,
            Display = true,
            Color = TextColor,
            Font = new ChartFont { Family = "Roboto" }
        };
        options.Scales.Y!.Title = new ChartAxesTitle
        {
            Text = EL["HomeRatePivotWidget.AverageRate"].Value,
            Display = true,
            Color = TextColor,
            Font = new ChartFont { Family = "Roboto" }
        };
        // Подписи делений, сетка и линия оси — цвета палитры СИБУР вместо серых по умолчанию
        options.Scales.X.Ticks = new ChartAxesTicks { Color = TickColor };
        options.Scales.Y.Ticks = new ChartAxesTicks { Color = TickColor };
        options.Scales.X.Grid = new ChartAxesGrid { Color = GridColor };
        options.Scales.Y.Grid = new ChartAxesGrid { Color = GridColor };
        options.Scales.X.Border = new ChartAxesBorder { Color = GridColor };
        options.Scales.Y.Border = new ChartAxesBorder { Color = GridColor };
        options.Plugins.Legend.Display = false;

        return options;
    }

    private string FormatMonthHeader(DateOnly month) =>
        $"{EL[MonthResourceKeys[month.Month - 1]]}.{month.Year % 100:00}";

    private static string FormatRate(decimal? rate) =>
        rate is null
            ? string.Empty
            : rate.Value.ToString("N0", CultureInfo.CurrentCulture);
}
