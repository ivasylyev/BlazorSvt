
using Blazored.LocalStorage;
using BlazorSvt.Host.Components;
using BlazorSvt.Import;
using BlazorSvt.Modules.AverageRateLevel3;
using BlazorSvt.Modules.HomeRatePivot;
using BlazorSvt.Modules.LocationsNodes;
using BlazorSvt.Modules.ParityRates;
using BlazorSvt.Modules.TransportLeg;
using BlazorSvt.Modules.TransportRate;
using BlazorSvt.Platform;
using BlazorSvt.Platform.Infrastructure.Data;
using BlazorSvt.Platform.Sync;
using Dapper;
using Serilog;

SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddBlazorBootstrap();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddControllers();
builder.Services.AddLocalization();

builder.Services.AddPlatform(builder.Configuration);
builder.Services.AddSnapshotSync(builder.Configuration);
builder.Services.AddImportModule();
builder.Services.AddTransportRateModule();
builder.Services.AddParityRatesModule();
builder.Services.AddAverageRateLevel3Module();
builder.Services.AddTransportLegModule();
builder.Services.AddLocationsNodesModule();
builder.Services.AddHomeRatePivotModule();

var supportedCultures = new[] { "ru-RU", "en-US" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

var pathBase = builder.Configuration["PathBase"];

var app = builder.Build();

if (!string.IsNullOrEmpty(pathBase) && pathBase != "/")
{
    app.UsePathBase(pathBase);
}

// Edge/Chrome запрещают unload по умолчанию; blazor.web.js всё ещё регистрирует обработчик.
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        const string headerName = "Permissions-Policy";
        const string unloadDirective = "unload=(self)";
        var headers = context.Response.Headers;
        if (headers.TryGetValue(headerName, out var existing) && existing.Count > 0)
        {
            var current = existing.ToString();
            if (!current.Contains("unload=", StringComparison.OrdinalIgnoreCase))
            {
                headers[headerName] = $"{current}, {unloadDirective}";
            }
        }
        else
        {
            headers[headerName] = unloadDirective;
        }

        return Task.CompletedTask;
    });

    await next();
});

app.UseRequestLocalization(localizationOptions);
app.MapControllers();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.Use(MiddlewareDecorator.MiddlewareShortCorrelationId);
app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = SerilogRequestLogLevel.GetLevel;
});
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
