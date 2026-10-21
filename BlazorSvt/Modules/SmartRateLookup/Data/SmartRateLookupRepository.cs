using System.Data;
using System.Data.SqlClient;
using BlazorSvt.Modules.SmartRateLookup.Domain;
using BlazorSvt.Platform.Infrastructure;
using BlazorSvt.Platform.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Options;

namespace BlazorSvt.Modules.SmartRateLookup.Data;

/// <summary>Справочные данные запроса, разрешённые по кодам.</summary>
public sealed class LookupReferences
{
    public long? TransportTypeId { get; init; }
    public string? TransportTypeName { get; init; }
    public long? TransportKindId { get; init; }
    public string? TransportKindCode { get; init; }
    public long? NodeFromId { get; init; }
    public string? NodeFromName { get; init; }
    public long? NodeToId { get; init; }
    public string? NodeToName { get; init; }
    public long? RegionNodeToId { get; init; }
    public long? ProductId { get; init; }
    public string? ProductName { get; init; }
    public long? ProductGroupId { get; init; }
    public string? ProductGroupCode { get; init; }
    public string? ProductGroupName { get; init; }
    public long? CurrencyId { get; init; }
    public bool ProfileExists { get; init; }
    public IReadOnlyList<DpoNode> DpoNodes { get; init; } = [];
}

/// <summary>Кандидат с полями для ответа.</summary>
public sealed class CandidateRow
{
    public long AverageRateLevel3Id { get; init; }
    public long NodeFromId { get; init; }
    public long NodeToId { get; init; }
    public long? ProductId { get; init; }
    public long? ProductGroupId { get; init; }
    public long TransportTypeId { get; init; }
    public long TransportKindId { get; init; }
    public long RateTypeId { get; init; }
    public decimal RateTypePriority { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal RateLevel3 { get; init; }
    public decimal EffectiveLoad { get; init; }
    public long CurrencyId { get; init; }
    public string CurrencyCode { get; init; } = "";
    public string RateTypeCode { get; init; } = "";
    public string RateTypeName { get; init; } = "";
    public string TransportTypeCode { get; init; } = "";
    public string TransportTypeName { get; init; } = "";
    public string NodeFromCode { get; init; } = "";
    public string NodeToCode { get; init; } = "";
    public string? ProductCode { get; init; }
    public string? ProductGroupCode { get; init; }

    public RateCandidate ToCandidate() =>
        new(
            AverageRateLevel3Id,
            NodeFromId,
            NodeToId,
            ProductId,
            ProductGroupId,
            TransportTypeId,
            TransportKindId,
            RateTypeId,
            RateTypePriority,
            DateOnly.FromDateTime(StartDate),
            DateOnly.FromDateTime(EndDate),
            RateLevel3);
}

public interface ISmartRateLookupRepository
{
    Task<LookupReferences> ResolveAsync(
        string nodeFromCode,
        string nodeToCode,
        string productCode,
        string transportTypeCode,
        string currencyCode,
        string profile,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CandidateRow>> GetCandidatesAsync(
        IReadOnlyList<LookupStep> steps,
        DateOnly date,
        string profile,
        CancellationToken cancellationToken);

    /// <summary>Пересчёт легаси-функцией <c>dbo.fnCurrencyRateConversion</c>; <c>null</c> — курса нет.</summary>
    Task<decimal?> ConvertCurrencyAsync(
        decimal value,
        long fromCurrencyId,
        long toCurrencyId,
        DateOnly date,
        CancellationToken cancellationToken);
}

/// <summary>
/// Данные умного подбора. Кандидаты — из snapshot <c>v2.AverageRateLevel3_Snapshot</c>;
/// справочники запроса (узлы, регион, МТР, группа, тип/вид транспорта, DPO, приоритеты, курсы) —
/// из легаси-view <c>dbo.vw_*</c>: их snapshot ещё нет (техдолг, заменить при переносе справочников).
/// </summary>
public sealed class SmartRateLookupRepository(
    IOptions<DatabaseOptions> options,
    ILogger<SmartRateLookupRepository> logger) : ISmartRateLookupRepository
{
    // Без фильтра по статусу, как в процедуре; при дублях кода предпочитаем активную запись.
    private const string ResolveSql =
        """
        DECLARE @NodeFromId BIGINT, @NodeFromName NVARCHAR(400);
        SELECT TOP (1) @NodeFromId = Id, @NodeFromName = Name_ru
        FROM dbo.vw_LocationsNodes
        WHERE Code = @NodeFromCode
        ORDER BY IIF(PrimitiveEntityDataStateId = 1, 0, 1), Id;

        SELECT TOP (1)
            TT.Id AS TransportTypeId, TT.[Name] AS TransportTypeName, TK.Id AS TransportKindId, TK.Code AS TransportKindCode,
            @NodeFromId AS NodeFromId, @NodeFromName AS NodeFromName
        FROM (SELECT 1 AS One) AS X
        LEFT JOIN dbo.vw_TransportType_level_3 TT ON TT.Code = @TransportTypeCode
        LEFT JOIN dbo.vw_TransportKind TK ON TT.ParentCode_1 = TK.Id
        ORDER BY IIF(TK.Id IS NULL, 1, 0), IIF(TT.PrimitiveEntityDataStateId = 1, 0, 1), TT.Id;

        SELECT TOP (1) LN.Id AS NodeToId, LN.Name_ru AS NodeToName, LNR.Id AS RegionNodeToId
        FROM dbo.vw_LocationsNodes LN
        LEFT JOIN dbo.vw_Region R ON LN.Region = R.Id
        LEFT JOIN dbo.vw_LocationsNodes LNR ON R.RegionCodePlanning = LNR.Code
        WHERE LN.Code = @NodeToCode
        ORDER BY IIF(LN.PrimitiveEntityDataStateId = 1, 0, 1), LN.Id, IIF(LNR.PrimitiveEntityDataStateId = 1, 0, 1), LNR.Id;

        SELECT TOP (1) M.Id AS ProductId, M.NameShort_ru AS ProductName, M.TransportName AS ProductGroupId,
               PG.Code AS ProductGroupCode, PG.ShortName AS ProductGroupName
        FROM dbo.vw_MTR M
        LEFT JOIN dbo.vw_ProductGroup PG ON PG.Id = M.TransportName
        WHERE M.Code = @ProductCode
        ORDER BY IIF(M.PrimitiveEntityDataStateId = 1, 0, 1), M.Id;

        SELECT TOP (1) Id FROM dbo.vw_Currency WHERE Code = @CurrencyCode
        ORDER BY IIF(PrimitiveEntityDataStateId = 1, 0, 1), Id;

        SELECT CAST(IIF(EXISTS (
            SELECT 1 FROM dbo.vw_Priority_Rate WHERE OperationName = @Profile AND PrimitiveEntityDataStateId = 1), 1, 0) AS BIT);

        SELECT v.NodeId, LN.Code AS NodeCode
        FROM dbo.vw_OrgDPO d
        CROSS APPLY (VALUES
            (1, d.NodeCode), (2, d.NodeCode2), (3, d.NodeCode3), (4, d.NodeCode4), (5, d.NodeCode5),
            (6, d.NodeCode6), (7, d.NodeCode7), (8, d.NodeCode8), (9, d.NodeCode9), (10, d.NodeCode10)
        ) AS v(Ord, NodeId)
        OUTER APPLY (SELECT TOP (1) Code FROM dbo.vw_LocationsNodes WHERE Id = v.NodeId) AS LN
        WHERE d.PrimitiveEntityDataStateId = 1
          AND d.MainNode = @NodeFromId
          AND v.NodeId IS NOT NULL
          AND v.NodeId <> d.MainNode
        ORDER BY d.Id, v.Ord;
        """;

    // Кандидаты одним запросом: действующие на дату активные ставки по любому из узлов/типов/видов/продуктов шагов;
    // точное соответствие шагу и ранжирование — в C# (SmartRateRanker).
    private const string CandidatesSql =
        """
        SELECT
            s.AverageRateLevel3Id,
            s.NodeFromId,
            s.NodeToId,
            s.ProductId,
            s.ProductGroupId,
            s.TransportTypeId,
            s.TransportKindId,
            s.RateTypeId,
            pr.[Priority] AS RateTypePriority,
            s.StartDate,
            s.EndDate,
            s.RateLevel3,
            s.EffectiveLoadOfTransportType AS EffectiveLoad,
            s.CurrencyId,
            s.CurrencyCode,
            rt.Code AS RateTypeCode,
            rt.[Name] AS RateTypeName,
            tt.Code AS TransportTypeCode,
            tt.[Name] AS TransportTypeName,
            s.NodeFromCode,
            s.NodeToCode,
            CAST(s.ProductCode AS NVARCHAR(20)) AS ProductCode,
            s.ProductGroupCode
        FROM v2.AverageRateLevel3_Snapshot AS s
        JOIN dbo.vw_Priority_Rate AS pr
            ON pr.RateType = s.RateTypeId
           AND pr.TransportKind = s.TransportKindId
           AND pr.PrimitiveEntityDataStateId = 1
           AND pr.OperationName = @Profile
        JOIN dbo.vw_TransportType_level_3 AS tt
            ON tt.Id = s.TransportTypeId
           AND tt.PrimitiveEntityDataStateId = 1
        JOIN dbo.vw_RateType AS rt
            ON rt.Id = s.RateTypeId
           AND rt.PrimitiveEntityDataStateId = 1
        WHERE s.IsArchive = 0
          AND @Date BETWEEN s.StartDate AND s.EndDate
          AND s.NodeFromId IN @StartNodes
          AND s.NodeToId IN @EndNodes
          AND (s.TransportTypeId IN @TransportTypes OR s.TransportKindId IN @TransportKinds)
          AND (s.ProductId IN @Products OR (s.ProductId IS NULL AND s.ProductGroupId IN @ProductGroups));
        """;

    private const string ConvertSql =
        "SELECT dbo.fnCurrencyRateConversion(@Value, @FromCurrency, @ToCurrency, @Date)";

    public async Task<LookupReferences> ResolveAsync(
        string nodeFromCode,
        string nodeToCode,
        string productCode,
        string transportTypeCode,
        string currencyCode,
        string profile,
        CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("NodeFromCode", nodeFromCode);
        parameters.Add("NodeToCode", nodeToCode);
        parameters.Add("ProductCode", productCode);
        parameters.Add("TransportTypeCode", transportTypeCode);
        parameters.Add("CurrencyCode", currencyCode);
        parameters.Add("Profile", profile);

        return await ExecuteAsync(async db =>
        {
            await using var multi = await db.QueryMultipleAsync(ResolveSql, parameters, CommandType.Text, cancellationToken);

            var transport = await multi.ReadSingleAsync<TransportRow>();
            var nodeTo = await multi.ReadSingleOrDefaultAsync<NodeToRow>();
            var product = await multi.ReadSingleOrDefaultAsync<ProductRow>();
            var currencyId = await multi.ReadSingleOrDefaultAsync<long?>();
            var profileExists = await multi.ReadSingleAsync<bool>();
            var dpo = (await multi.ReadAsync<DpoNodeRow>()).Select(r => new DpoNode(r.NodeId, r.NodeCode)).ToList();

            return new LookupReferences
            {
                TransportTypeId = transport.TransportTypeId,
                TransportTypeName = transport.TransportTypeName,
                TransportKindId = transport.TransportKindId,
                TransportKindCode = transport.TransportKindCode,
                NodeFromId = transport.NodeFromId,
                NodeFromName = transport.NodeFromName,
                NodeToId = nodeTo?.NodeToId,
                NodeToName = nodeTo?.NodeToName,
                RegionNodeToId = nodeTo?.RegionNodeToId,
                ProductId = product?.ProductId,
                ProductName = product?.ProductName,
                ProductGroupId = product?.ProductGroupId,
                ProductGroupCode = product?.ProductGroupCode,
                ProductGroupName = product?.ProductGroupName,
                CurrencyId = currencyId,
                ProfileExists = profileExists,
                DpoNodes = dpo
            };
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<CandidateRow>> GetCandidatesAsync(
        IReadOnlyList<LookupStep> steps,
        DateOnly date,
        string profile,
        CancellationToken cancellationToken)
    {
        if (steps.Count == 0)
        {
            return [];
        }

        var parameters = new DynamicParameters();
        parameters.Add("Profile", profile);
        parameters.Add("Date", date.ToDateTime(TimeOnly.MinValue), DbType.Date);
        parameters.Add("StartNodes", steps.Select(s => s.StartNodeId).Distinct().ToArray());
        parameters.Add("EndNodes", steps.Select(s => s.EndNodeId).Distinct().ToArray());
        parameters.Add("TransportTypes", NonEmpty(steps.Select(s => s.TransportTypeId)));
        parameters.Add("TransportKinds", NonEmpty(steps.Select(s => s.TransportKindId)));
        parameters.Add("Products", NonEmpty(steps.Select(s => s.ProductId)));
        parameters.Add("ProductGroups", NonEmpty(steps.Select(s => s.ProductGroupId)));

        return await ExecuteAsync(
            async db => (await db.QueryAsync<CandidateRow>(CandidatesSql, parameters, CommandType.Text, cancellationToken)).ToList(),
            cancellationToken);
    }

    public Task<decimal?> ConvertCurrencyAsync(
        decimal value,
        long fromCurrencyId,
        long toCurrencyId,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Value", value, DbType.Decimal, precision: 19, scale: 2);
        parameters.Add("FromCurrency", fromCurrencyId);
        parameters.Add("ToCurrency", toCurrencyId);
        parameters.Add("Date", date.ToDateTime(TimeOnly.MinValue), DbType.Date);

        return ExecuteAsync(
            db => db.QuerySingleAsync<decimal?>(ConvertSql, parameters, CommandType.Text, cancellationToken),
            cancellationToken);
    }

    // Пустой IN в Dapper даёт ложное условие; -1 не встречается среди идентификаторов легаси.
    private static long[] NonEmpty(IEnumerable<long?> ids)
    {
        var values = ids.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToArray();
        return values.Length == 0 ? [-1] : values;
    }

    private async Task<T> ExecuteAsync<T>(Func<DbConnectionLogDecorator, Task<T>> action, CancellationToken cancellationToken)
    {
        try
        {
#pragma warning disable CS0618 // Type or member is obsolete
            await using var connection = new SqlConnection(options.Value.MdmDb);
#pragma warning restore CS0618 // Type or member is obsolete
            await connection.OpenAsync(cancellationToken);
            var db = new DbConnectionLogDecorator(connection, logger, options.Value.DefaultQueryTimeoutSeconds);
            return await action(db);
        }
        catch (Exception ex) when (OperationCancellation.IsCancellation(ex, cancellationToken))
        {
            throw new OperationCanceledException(cancellationToken);
        }
    }

    private sealed class TransportRow
    {
        public long? TransportTypeId { get; init; }
        public string? TransportTypeName { get; init; }
        public long? TransportKindId { get; init; }
        public string? TransportKindCode { get; init; }
        public long? NodeFromId { get; init; }
        public string? NodeFromName { get; init; }
    }

    private sealed class NodeToRow
    {
        public long NodeToId { get; init; }
        public string? NodeToName { get; init; }
        public long? RegionNodeToId { get; init; }
    }

    private sealed class ProductRow
    {
        public long ProductId { get; init; }
        public string? ProductName { get; init; }
        public long? ProductGroupId { get; init; }
        public string? ProductGroupCode { get; init; }
        public string? ProductGroupName { get; init; }
    }

    private sealed class DpoNodeRow
    {
        public long NodeId { get; init; }
        public string? NodeCode { get; init; }
    }
}
