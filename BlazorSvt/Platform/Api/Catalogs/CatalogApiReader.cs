namespace BlazorSvt.Platform.Api.Catalogs;

/// <summary>
/// Чтение справочников по коду: список, схема, поиск, карточка. Общий для HTTP-маршрутов <c>/api/v1</c>
/// и инструментов MCP — одни проверки и одни сервисы. Выгрузка Excel сюда не входит: она только у HTTP.
/// </summary>
/// <remarks>Scoped: сервисы данных берутся из области запроса, где <c>ApiAccessMiddleware</c> уже определил пользователя.</remarks>
public sealed class CatalogApiReader(IServiceProvider services, IEnumerable<ICatalogApiSource> sources)
{
    public const string ExportCapability = "export";

    private readonly Dictionary<string, ICatalogApiSource> byCode =
        sources.ToDictionary(s => s.Descriptor.Code, StringComparer.OrdinalIgnoreCase);

    /// <param name="includeExport">MCP выгрузку не даёт — и не объявляет её в <c>capabilities</c>.</param>
    public CatalogListResponse List(bool includeExport)
    {
        var items = byCode.Values
            .Select(s => s.Descriptor)
            .OrderBy(c => c.Domain)
            .ThenBy(c => c.Code, StringComparer.Ordinal)
            .Select(c => new CatalogInfo(
                c.Code,
                CatalogTitles.GetRu(c.TitleResourceKey),
                CatalogTitles.GetEn(c.TitleResourceKey),
                ApiJson.EnumCode(c.Domain.ToString()),
                Capabilities(c, includeExport)))
            .ToList();
        return new CatalogListResponse(items);
    }

    /// <summary>Регистрация справочника по коду; нет такого — <c>catalog_not_found</c>.</summary>
    public CatalogApiDescriptor Find(string? code) => Source(code).Descriptor;

    public CatalogSchemaResponse GetSchema(string code) => Source(code).GetSchema(services);

    /// <param name="maxPageSize">Предел страницы для этого входа (REST и MCP разные).</param>
    /// <param name="priorErrors">Ошибки аргументов, найденные вызывающим: уходят одним ответом с ошибками запроса.</param>
    public Task<CatalogSearchResponse<object>> SearchAsync(
        string code,
        ICatalogQueryRequest request,
        int page,
        int pageSize,
        CancellationToken cancellationToken,
        int maxPageSize = CatalogQueryTranslator.MaxPageSize,
        IReadOnlyDictionary<string, string[]>? priorErrors = null)
    {
        var source = Source(code);
        var query = CatalogQueryTranslator.Translate(
            source.Descriptor.ItemType,
            request,
            source.Descriptor.Period,
            page,
            pageSize,
            maxPageSize,
            priorErrors);
        return source.SearchAsync(services, query, cancellationToken);
    }

    /// <summary>Карточка по ключу записи; нет записи — <c>item_not_found</c>.</summary>
    public async Task<object> GetItemAsync(string code, long id, CancellationToken cancellationToken)
    {
        var item = await Source(code).FindAsync(services, id, cancellationToken);
        return item ?? throw ApiException.NotFound(ApiErrorCodes.ItemNotFound, id);
    }

    private ICatalogApiSource Source(string? code) =>
        code is not null && byCode.TryGetValue(code, out var source)
            ? source
            : throw ApiException.NotFound(ApiErrorCodes.CatalogNotFound);

    private static IReadOnlyList<string> Capabilities(CatalogApiDescriptor descriptor, bool includeExport)
    {
        var capabilities = new List<string> { "schema", "search", "item" };
        if (includeExport)
        {
            capabilities.Add(ExportCapability);
        }

        if (descriptor.Period is not null)
        {
            capabilities.Add("asOf");
        }

        return capabilities;
    }
}
