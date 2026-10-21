using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Platform.Api.Catalogs;

/// <summary>Поля периода действия записи (для <c>asOf</c>): начало ≤ дата ≤ конец.</summary>
public sealed record CatalogPeriod(string FromProperty, string ToProperty);

/// <summary>Регистрация справочника в API: код в URL, домен, заголовок, DTO и источник свежести.</summary>
/// <param name="Code">Код в URL (<c>kebab-case</c>, мн. число), напр. <c>transport-rates</c>.</param>
/// <param name="Domain">Домен (как в меню).</param>
/// <param name="TitleResourceKey">Ключ заголовка в Platform.resx (как у пункта меню).</param>
/// <param name="ItemType">List-DTO (<see cref="GridSnapshotAttribute"/>).</param>
/// <param name="DetailType">Detail-DTO (<see cref="DetailSourceAttribute"/>).</param>
/// <param name="SyncEntity">Значение <c>v2.SyncState.Entity</c> — источник <c>dataAsOf</c>.</param>
/// <param name="Period">Поля периода; <c>null</c> — справочник без периода, <c>asOf</c> недоступен.</param>
public sealed record CatalogApiDescriptor(
    string Code,
    CatalogDomain Domain,
    string TitleResourceKey,
    Type ItemType,
    Type DetailType,
    string SyncEntity,
    CatalogPeriod? Period);
