using System.Globalization;
using BlazorSvt.Platform.Reporting.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
namespace BlazorSvt.Platform.Api.Catalogs;

/// <summary>
/// Маршруты одного справочника: <c>/catalogs/{code}/schema|items/search|items/{id}|export</c>.
/// Типизированы DTO справочника — в OpenAPI у каждого справочника своя схема ответа.
/// </summary>
public sealed class CatalogApiEndpoints<TItem, TDetail>(CatalogApiDescriptor descriptor) : IApiEndpointContribution
{
    private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public void Map(RouteGroupBuilder api)
    {
        var code = descriptor.Code;
        var name = ToPascal(code);
        var route = $"/catalogs/{code}";
        var group = api.MapGroup(route).WithTags(code);

        group.MapGet("/schema", GetSchema)
            .WithName($"get{name}Schema")
            .WithSummary($"Описание полей справочника {code}")
            .WithDescription("Поля, типы, заголовки (по Accept-Language), операторы фильтра, значения enum, ссылки, период, ключ. Поддерживает ETag.")
            .Produces<CatalogSchemaResponse>()
            .Produces(StatusCodes.Status304NotModified);

        group.MapPost("/items/search", SearchAsync)
            .WithName($"search{name}")
            .WithSummary($"Поиск записей справочника {code}")
            .WithDescription("Фильтры объединяются через И; сортировка по одному полю; pageSize 1–1000. По умолчанию без архивных записей. asOf — записи, действующие на дату.")
            .Produces<CatalogSearchResponse<TItem>>()
            .ProducesValidationProblem();

        group.MapGet("/items/{id:long}", GetItemAsync)
            .WithName($"get{name}Item")
            .WithSummary($"Карточка записи справочника {code}")
            .WithDescription("id — ключ записи (поле entityKey из /schema). Поля — как в карточке СВТ.")
            .Produces<TDetail>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/export", ExportAsync)
            .WithName($"export{name}")
            .WithSummary($"Выгрузка справочника {code} в Excel")
            .WithDescription("Файл .xlsx в ответе. kind=Short — колонки грида, kind=Full — поля карточки. Все колонки, без пользовательских настроек.")
            .Produces(StatusCodes.Status200OK, contentType: XlsxContentType)
            .ProducesValidationProblem();
    }

    private IResult GetSchema(HttpContext context, CatalogApiReader reader)
    {
        ApiCallLog.SetOperation(context, "schema", descriptor.Code);
        return ApiHttp.JsonWithETag(context, reader.GetSchema(descriptor.Code));
    }

    private async Task<IResult> SearchAsync(
        HttpContext context,
        [FromBody] CatalogSearchRequest request,
        CatalogApiReader reader,
        CancellationToken cancellationToken)
    {
        ApiCallLog.SetOperation(context, "search", descriptor.Code);

        var response = await reader.SearchAsync(
            descriptor.Code, request, request.Page, request.PageSize, cancellationToken);

        ApiCallLog.SetRowCount(context, response.Items.Count);
        return Results.Ok(response);
    }

    private async Task<IResult> GetItemAsync(
        HttpContext context,
        long id,
        CatalogApiReader reader,
        CancellationToken cancellationToken)
    {
        ApiCallLog.SetOperation(context, "item", descriptor.Code);

        var item = await reader.GetItemAsync(descriptor.Code, id, cancellationToken);

        ApiCallLog.SetRowCount(context, 1);
        return Results.Ok(item);
    }

    private async Task ExportAsync(
        HttpContext context,
        [FromBody] CatalogExportRequest request,
        GridReportExporter<TItem, TDetail> exporter,
        IGridSettingsService<TItem> gridSettings,
        IDetailSettingsService<TDetail> detailSettings,
        CancellationToken cancellationToken)
    {
        ApiCallLog.SetOperation(context, $"export_{request.Kind.ToString().ToLowerInvariant()}", descriptor.Code);

        var query = CatalogQueryTranslator.Translate(
            typeof(TItem), request, descriptor.Period, page: 1, pageSize: 1);
        var lang = CurrentLang();

        var tempPath = Path.Combine(Path.GetTempPath(), $"svt-api-{Guid.NewGuid():N}.xlsx");
        await using var buffer = new FileStream(
            tempPath,
            FileMode.CreateNew,
            FileAccess.ReadWrite,
            FileShare.None,
            bufferSize: 81920,
            FileOptions.DeleteOnClose | FileOptions.Asynchronous);

        int rows;
        if (request.Kind == CatalogExportKind.Full)
        {
            var columns = detailSettings.GetGridDetailSettings(lang).GroupSettings.Values.SelectMany(s => s);
            rows = await exporter.WriteFullAsync(buffer, query, columns, cancellationToken);
        }
        else
        {
            var columns = gridSettings.GetDefaultSettings(lang)
                .Select(c =>
                {
                    c.Visible = true;
                    return c;
                });
            rows = await exporter.WriteShortAsync(buffer, query, columns, cancellationToken);
        }

        ApiCallLog.SetRowCount(context, rows);

        var fileName = $"{descriptor.Code}_{DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)}.xlsx";
        context.Response.ContentType = XlsxContentType;
        context.Response.ContentLength = buffer.Length;
        context.Response.Headers[HeaderNames.ContentDisposition] =
            new ContentDispositionHeaderValue("attachment") { FileNameStar = fileName, FileName = fileName }.ToString();

        buffer.Position = 0;
        await buffer.CopyToAsync(context.Response.Body, cancellationToken);
    }

    private static string CurrentLang() => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    internal static string ToPascal(string code) =>
        string.Concat(code.Split('-', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => char.ToUpperInvariant(part[0]) + part[1..]));
}
