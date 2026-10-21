namespace BlazorSvt.Modules.SmartRateLookup.Domain;

/// <summary>
/// Выбор одной ставки из кандидатов. Порядок как в <c>CalcTransportRateCRM</c>:
/// приоритет типа ставки → шаг → близость к средней по группе → бо́льшая ставка.
/// </summary>
/// <remarks>
/// Тип и вид транспорта, продукт и группа сравниваются явно (в процедуре — через <c>ISNULL</c>
/// по идентификаторам разных справочников). Последний ключ — id ставки, чтобы выбор был детерминирован.
/// </remarks>
public static class SmartRateRanker
{
    public static RankedRate? Pick(IReadOnlyList<LookupStep> steps, IReadOnlyList<RateCandidate> candidates)
    {
        if (steps.Count == 0 || candidates.Count == 0)
        {
            return null;
        }

        var averages = candidates
            .GroupBy(c => c.AverageGroupKey)
            .ToDictionary(g => g.Key, g => g.Average(c => c.RateLevel3));

        return candidates
            .Select(c => (Candidate: c, Step: steps.FirstOrDefault(s => Matches(s, c))))
            .Where(x => x.Step is not null)
            .OrderBy(x => x.Candidate.RateTypePriority)
            .ThenBy(x => x.Step!.Order)
            .ThenBy(x => Math.Abs(x.Candidate.RateLevel3 - averages[x.Candidate.AverageGroupKey]))
            .ThenByDescending(x => x.Candidate.RateLevel3)
            .ThenBy(x => x.Candidate.AverageRateLevel3Id)
            .Select(x => new RankedRate(x.Candidate, x.Step!))
            .FirstOrDefault();
    }

    public static bool Matches(LookupStep step, RateCandidate candidate)
    {
        if (candidate.NodeFromId != step.StartNodeId || candidate.NodeToId != step.EndNodeId)
        {
            return false;
        }

        var transportMatches = step.TransportTypeId is { } type
            ? candidate.TransportTypeId == type
            : candidate.TransportKindId == step.TransportKindId;
        if (!transportMatches)
        {
            return false;
        }

        return step.ProductId is { } product
            ? candidate.ProductId == product
            : candidate.ProductId is null && candidate.ProductGroupId == step.ProductGroupId;
    }
}
