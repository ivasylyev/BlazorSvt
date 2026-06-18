
using Blazored.LocalStorage;
using Dapper;
using BlazorSvt.Components;
using BlazorSvt.Models.Config;
using BlazorSvt.Models.Dto;
using BlazorSvt.Services.Legs;
using BlazorSvt.Services.Rates;
using BlazorSvt.Services.Shared;
using BlazorSvt.Utils;
using FluentValidation;
using Serilog;


SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddBlazorBootstrap();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddControllers();
builder.Services.AddLocalization();

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
builder.Services.AddScoped<PageTimingService>();

builder.Services.AddScoped(typeof(IGridDataService<,>), typeof(GridDataService<,>));

builder.Services.AddScoped<IExcelParser, ExcelParser>();
builder.Services.AddScoped<IExcelErrorWriter, ExcelErrorWriter>();
builder.Services.AddScoped<IGridExcelExporter, GridExcelExporter>();
builder.Services.AddScoped(typeof(IStagingRepository<>), typeof(StagingRepository<>));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddScoped<IGridSettingsService<RateDto>, RatesGridSettingsService>();
builder.Services.AddScoped<IDetailSettingsService<RateDetailDto>, RatesDetailSettingsService>();

builder.Services.AddScoped<IGridSettingsService<LegDto>, LegsGridSettingsService>();
builder.Services.AddScoped<IDetailSettingsService<LegDetailDto>, LegsDetailSettingsService>();

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
app.UseRequestLocalization(localizationOptions);
app.MapControllers();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.Use(MiddlewareDecorator.MiddlewareShortCorrelationId);
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
