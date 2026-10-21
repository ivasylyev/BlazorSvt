namespace BlazorSvt.Modules.SmartRateLookup.Domain;

/// <summary>Дополнительный узел отправления из справочника «Организации DPO».</summary>
public sealed record DpoNode(long NodeId, string? NodeCode);

/// <summary>Параметры запроса, разрешённые в идентификаторы легаси.</summary>
public sealed record ResolvedLookupInput(
    long NodeFromId,
    long NodeToId,
    long? RegionNodeToId,
    long ProductId,
    long? ProductGroupId,
    long TransportTypeId,
    long TransportKindId,
    IReadOnlyList<DpoNode> DpoNodes);

/// <summary>
/// Шаг поиска: уровень 1–8 для основного узла отправления, затем те же уровни для узлов DPO.
/// <see cref="Order"/> — приоритет шага (меньше — лучше).
/// </summary>
/// <remarks>
/// Ровно одно из <see cref="ProductId"/>/<see cref="ProductGroupId"/> и одно из
/// <see cref="TransportTypeId"/>/<see cref="TransportKindId"/> заполнено.
/// </remarks>
public sealed record LookupStep(
    int Order,
    int Level,
    LookupLifts Lifts,
    long StartNodeId,
    DpoNode? ReplacedDepartureNode,
    long EndNodeId,
    long? ProductId,
    long? ProductGroupId,
    long? TransportTypeId,
    long? TransportKindId);

/// <summary>Кандидат — действующая на дату средневзвешенная ставка 3-го уровня с приоритетом типа ставки по профилю.</summary>
public sealed record RateCandidate(
    long AverageRateLevel3Id,
    long NodeFromId,
    long NodeToId,
    long? ProductId,
    long? ProductGroupId,
    long TransportTypeId,
    long TransportKindId,
    long RateTypeId,
    decimal RateTypePriority,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal RateLevel3)
{
    /// <summary>Ключ группы для средней цены — как <c>GROUP BY</c> в процедуре.</summary>
    public (long, long, DateOnly, DateOnly, long, long?, long) AverageGroupKey =>
        (NodeFromId, NodeToId, StartDate, EndDate, TransportTypeId, ProductId ?? ProductGroupId, RateTypeId);
}

/// <summary>Выбранная ставка и шаг, на котором она найдена.</summary>
public sealed record RankedRate(RateCandidate Candidate, LookupStep Step);
