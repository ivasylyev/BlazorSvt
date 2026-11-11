using BlazorSvt.Modules.SmartRateLookup.Contracts;
using BlazorSvt.Modules.SmartRateLookup.Services;
using BlazorSvt.Platform.Api;
using BlazorSvt.Platform.Api.Mcp;

namespace BlazorSvt.Modules.SmartRateLookup.Api;

/// <summary>
/// <c>smart_rate_lookup</c> — тот же сервис и ответ, что <c>POST /api/v1/rates/smart-lookup</c>.
/// Профиля во входной схеме нет: сервис подставляет «Импорт в CRM».
/// </summary>
public sealed class SmartRateLookupMcpTool(IHttpContextAccessor httpContextAccessor) : SvtMcpTool(
    httpContextAccessor,
    "smart_rate_lookup",
    "Умный подбор ставки",
    "Одна наиболее подходящая средневзвешенная ставка 3-го уровня по маршруту, продукту и типу транспорта. " +
    "Если точного совпадения нет — подъём продукта до группы, узла назначения до региона, типа транспорта до вида " +
    "(уровни 1–8), затем узлы отправления DPO. Ставка не найдена — found=false (это не ошибка). " +
    "Нет курса для пересчёта в валюту — ошибка currency_rate_missing.",
    """
    {
      "type": "object",
      "properties": {
        "nodeFromCode": { "type": "string", "description": "Код узла отправления (справочник locations-nodes)" },
        "nodeToCode": { "type": "string", "description": "Код узла назначения (справочник locations-nodes)" },
        "productCode": { "type": "string", "description": "Код продукта (МТР)" },
        "transportTypeCode": { "type": "string", "description": "Код типа транспорта 3-го уровня" },
        "currencyCode": { "type": "string", "description": "Код валюты ответа, напр. RUB" },
        "date": { "type": "string", "format": "date", "description": "Дата действия ставки и курса, yyyy-MM-dd; по умолчанию — сегодня" }
      },
      "required": ["nodeFromCode", "nodeToCode", "productCode", "transportTypeCode", "currencyCode"],
      "additionalProperties": false
    }
    """,
    """
    {
      "type": "object",
      "properties": {
        "found": { "type": "boolean", "description": "false — ставка не найдена ни на одном уровне" },
        "rate": { "type": ["object", "null"], "description": "Найденная ставка" },
        "match": { "type": ["object", "null"], "description": "Как найдена: уровень и подъёмы" },
        "request": { "type": "object", "description": "Разрешённые параметры запроса" }
      },
      "required": ["found", "rate", "match", "request"]
    }
    """)
{
    protected override async Task<object> ExecuteAsync(McpToolContext context, CancellationToken cancellationToken)
    {
        var args = context.Arguments;
        var request = new SmartRateLookupRequest
        {
            NodeFromCode = args.GetString("nodeFromCode", required: true) ?? "",
            NodeToCode = args.GetString("nodeToCode", required: true) ?? "",
            ProductCode = args.GetString("productCode", required: true) ?? "",
            TransportTypeCode = args.GetString("transportTypeCode", required: true) ?? "",
            CurrencyCode = args.GetString("currencyCode", required: true) ?? "",
            Date = args.GetDate("date")
        };
        args.ThrowIfInvalid();

        var response = await context.GetRequiredService<SmartRateLookupService>().LookupAsync(request, cancellationToken);
        ApiCallLog.SetRowCount(context.HttpContext, response.Found ? 1 : 0);
        return response;
    }
}
