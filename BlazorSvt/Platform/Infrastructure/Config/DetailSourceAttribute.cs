namespace BlazorSvt.Platform.Infrastructure.Config;

/// <summary>
/// Помечает detail-DTO источником детализации (view, не snapshot).
/// </summary>
/// <param name="name">Имя view, напр. <c>v2.vw_TransportRate_Detail</c>.</param>
/// <param name="keyColumn">Колонка ключа во view (обычно совпадает с entity key snapshot).</param>
/// <remarks>
/// Grid читает список из snapshot; detail/export полного отчёта — из этого view
/// (часто богаче snapshot и на переходном периоде может тянуть legacy <c>dbo.vw_*</c>).
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class DetailSourceAttribute(string name, string keyColumn) : Attribute
{
    public string Name { get; } = name;

    public string KeyColumn { get; } = keyColumn;
}
