using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Infrastructure;
using BlazorSvt.Platform.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace BlazorSvt.Modules.HomeRatePivot.Data;

/// <summary>
/// Прототип: читает AverageRateLevel3_Snapshot напрямую (без кеш-таблиц).
/// </summary>
public sealed class HomeRatePivotService(
    IOptions<DatabaseOptions> options,
    ILogger<HomeRatePivotService> logger) : IHomeRatePivotService
{
    public const int MonthWindowSize = 6;
    public const int MonthsBack = 3;

    private const string ProductGroupCode = "T23";
    private static readonly int RateTypeId = (int)RateTypeRu.Agreement;
    private static readonly int TransportKindId = (int)TransportKindRu.Auto;

    private readonly string connectionString = options.Value.MdmDb;
    private readonly int commandTimeoutSeconds = options.Value.DefaultQueryTimeoutSeconds;

    public async Task<HomeRatePivotTable> GetTableAsync(
        bool useRussianNames,
        CancellationToken cancellationToken = default)
    {
        var months = BuildMonthWindow(DateOnly.FromDateTime(DateTime.Today));
        var fromDate = months[0];
        var toDateExclusive = months[^1].AddMonths(1);

        var sql = BuildSql();
        var parameters = new DynamicParameters();
        parameters.Add("FromDate", fromDate.ToDateTime(TimeOnly.MinValue));
        parameters.Add("ToDateExclusive", toDateExclusive.ToDateTime(TimeOnly.MinValue));
        parameters.Add("ProductGroupCode", ProductGroupCode);
        parameters.Add("TransportKindId", TransportKindId);
        parameters.Add("RateTypeId", RateTypeId);

#pragma warning disable CS0618 // System.Data.SqlClient — как в GridDataService
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618
        await connection.OpenAsync(cancellationToken);

        var db = new DbConnectionLogDecorator(connection, logger, commandTimeoutSeconds);
        var facts = (await db.QueryAsync<HomeRatePivotFactRow>(
            sql,
            parameters,
            CommandType.Text,
            cancellationToken)).ToList();

        return Pivot(months, facts, useRussianNames);
    }

    /// <summary>Окно: текущий месяц −3 … текущий месяц +2 (6 месяцев).</summary>
    internal static IReadOnlyList<DateOnly> BuildMonthWindow(DateOnly today)
    {
        var start = new DateOnly(today.Year, today.Month, 1).AddMonths(-MonthsBack);
        return Enumerable.Range(0, MonthWindowSize)
            .Select(i => start.AddMonths(i))
            .ToList();
    }

    private static HomeRatePivotTable Pivot(
        IReadOnlyList<DateOnly> months,
        IReadOnlyList<HomeRatePivotFactRow> facts,
        bool useRussianNames)
    {
        var monthIndex = months
            .Select((m, i) => (Key: (m.Year, m.Month), Index: i))
            .ToDictionary(x => x.Key, x => x.Index);

        var rows = new List<HomeRatePivotRow>(HomeRatePivotDirections.Pairs.Count);

        for (var i = 0; i < HomeRatePivotDirections.Pairs.Count; i++)
        {
            var sortOrder = i + 1;
            var pairFacts = facts.Where(f => f.SortOrder == sortOrder).ToList();
            var sample = pairFacts.FirstOrDefault();
            var (fromCode, toCode) = HomeRatePivotDirections.Pairs[i];

            var fromName = ResolveNodeName(
                useRussianNames,
                sample?.NodeFromNameRu,
                sample?.NodeFromNameEn,
                fromCode);
            var toName = ResolveNodeName(
                useRussianNames,
                sample?.NodeToNameRu,
                sample?.NodeToNameEn,
                toCode);

            var rates = new decimal?[MonthWindowSize];
            foreach (var fact in pairFacts)
            {
                if (fact.Year is null || fact.Month is null || fact.RateLevel3 is null)
                {
                    continue;
                }

                if (monthIndex.TryGetValue((fact.Year.Value, fact.Month.Value), out var idx))
                {
                    rates[idx] = fact.RateLevel3;
                }
            }

            rows.Add(new HomeRatePivotRow
            {
                DirectionLabel = $"{fromName} - {toName}",
                RatesByMonth = rates
            });
        }

        return new HomeRatePivotTable { Months = months, Rows = rows };
    }

    private static string ResolveNodeName(
        bool useRussianNames,
        string? nameRu,
        string? nameEn,
        string code)
    {
        if (useRussianNames)
        {
            return FirstNonEmpty(nameRu, nameEn, code);
        }

        return FirstNonEmpty(nameEn, nameRu, code);
    }

    private static string FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return string.Empty;
    }

    private static string BuildSql()
    {
        var pairsSql = new StringBuilder();
        for (var i = 0; i < HomeRatePivotDirections.Pairs.Count; i++)
        {
            var (fromCode, toCode) = HomeRatePivotDirections.Pairs[i];
            if (i > 0)
            {
                pairsSql.AppendLine("    UNION ALL");
            }

            pairsSql.Append("    SELECT ")
                .Append(i + 1)
                .Append(" AS SortOrder, N'")
                .Append(fromCode)
                .Append("' AS NodeFromCode, N'")
                .Append(toCode)
                .Append("' AS NodeToCode");
        }

        // IsArchive не фильтруем — нужны исторические ставки.
        // Дедуп: IsDefRate=0 → LastChangeDate DESC → RateLevel3 DESC (MAX).
        // ProxyNode всегда NULL. Полный календарный месяц в окне [@FromDate, @ToDateExclusive).
        return $"""
            WITH Pairs AS (
            {pairsSql}
            ),
            Ranked AS (
                SELECT
                    p.SortOrder,
                    YEAR(a.StartDate) AS [Year],
                    MONTH(a.StartDate) AS [Month],
                    a.RateLevel3,
                    ROW_NUMBER() OVER (
                        PARTITION BY p.SortOrder, YEAR(a.StartDate), MONTH(a.StartDate)
                        ORDER BY
                            CASE WHEN a.IsDefRate = 0 THEN 0 ELSE 1 END,
                            a.LastChangeDate DESC,
                            a.RateLevel3 DESC
                    ) AS rn
                FROM Pairs p
                INNER JOIN v2.AverageRateLevel3_Snapshot a
                    ON a.NodeFromCode = p.NodeFromCode
                   AND a.NodeToCode = p.NodeToCode
                WHERE a.ProductGroupCode = @ProductGroupCode
                  AND a.TransportKindId = @TransportKindId
                  AND a.RateTypeId = @RateTypeId
                  AND a.ProxyNodeId IS NULL
                  AND a.StartDate >= @FromDate
                  AND a.StartDate < @ToDateExclusive
                  AND a.StartDate = DATEADD(DAY, 1 - DAY(a.StartDate), a.StartDate)
                  AND a.EndDate = DATEADD(DAY, -1, DATEADD(MONTH, DATEDIFF(MONTH, 0, a.StartDate) + 1, 0))
            ),
            Deduped AS (
                SELECT SortOrder, [Year], [Month], RateLevel3
                FROM Ranked
                WHERE rn = 1
            )
            SELECT
                p.SortOrder,
                p.NodeFromCode,
                p.NodeToCode,
                nf.NameRu AS NodeFromNameRu,
                nf.NameEn AS NodeFromNameEn,
                nt.NameRu AS NodeToNameRu,
                nt.NameEn AS NodeToNameEn,
                d.[Year],
                d.[Month],
                d.RateLevel3
            FROM Pairs p
            LEFT JOIN Deduped d
                ON d.SortOrder = p.SortOrder
            OUTER APPLY (
                SELECT TOP (1) n.NameRu, n.NameEn
                FROM v2.LocationsNodes_Snapshot n
                WHERE n.Code = p.NodeFromCode
                ORDER BY n.IsArchive ASC, n.Id DESC
            ) nf
            OUTER APPLY (
                SELECT TOP (1) n.NameRu, n.NameEn
                FROM v2.LocationsNodes_Snapshot n
                WHERE n.Code = p.NodeToCode
                ORDER BY n.IsArchive ASC, n.Id DESC
            ) nt
            ORDER BY p.SortOrder, d.[Year], d.[Month];
            """;
    }
}
