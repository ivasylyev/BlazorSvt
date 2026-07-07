USE [mdm];
GO

/*
    Проекция snapshot TransportLeg из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.TransportLeg_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Один и тот же SELECT для обоих путей гарантирует, что инкремент и полная
    пересборка дают идентичный результат.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.TransportLeg_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).

    Дополнительно к grid-колонкам проекция отдаёт скрытые FK-колонки
    (*_Id) — они нужны только синхронизации для каскадной инвалидации
    (изменение Region/Node -> затронутые плечи). В грид не отдаются.
*/

CREATE OR ALTER VIEW v2.vw_TransportLeg_SnapshotSource
AS
    SELECT
        CAST(l.Id AS INT)      AS TransportLegId,
        l.Code                 AS Code,

        CASE WHEN ISNULL(l.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        ISNULL(l.LegIsActive, 0)        AS CanBeUsed,

        CAST(st.Id AS INT)              AS ShipmentTypeId,

        CAST(l.TransportKind AS INT)    AS TransportKindId,
        LEFT(tk.Code, 5)                AS TransportKindCode,

        LEFT(l.SearchTimeT, 20)         AS SearchTimeT,
        LEFT(l.LoadTimeT, 20)           AS LoadTimeT,
        LEFT(l.TravelTimeT, 20)         AS TravelTimeT,
        LEFT(l.DaysWaitingT, 20)        AS DaysWaitingT,
        LEFT(l.UnLoadTimeT, 20)         AS UnLoadTimeT,
        LEFT(l.TransportationTimeT, 20) AS TransportationTimeT,

        LEFT(nf.Code, 10)               AS NodeFromCode,
        LEFT(nf.Name_en, 30)            AS NodeFromNameEn,
        LEFT(nf.Name_ru, 30)            AS NodeFromNameRu,
        LEFT(rf.Code, 10)               AS RegionFromCode,
        LEFT(rf.Name_en, 60)            AS RegionFromNameEn,
        LEFT(rf.Name_ru, 60)            AS RegionFromNameRu,

        LEFT(np.Code, 10)               AS ProxyNodeCode,
        LEFT(np.Name_en, 30)            AS ProxyNodeNameEn,
        LEFT(np.Name_ru, 30)            AS ProxyNodeNameRu,
        LEFT(rp.Code, 10)               AS ProxyRegionCode,
        LEFT(rp.Name_en, 30)            AS ProxyRegionNameEn,
        LEFT(rp.Name_ru, 30)            AS ProxyRegionNameRu,

        LEFT(nt.Code, 10)               AS NodeToCode,
        LEFT(nt.Name_en, 30)            AS NodeToNameEn,
        LEFT(nt.Name_ru, 30)            AS NodeToNameRu,
        LEFT(rt.Code, 10)               AS RegionToCode,
        LEFT(rt.Name_en, 30)            AS RegionToNameEn,
        LEFT(rt.Name_ru, 30)            AS RegionToNameRu,

        l.CreationDate                  AS CreationDate,
        ISNULL(l.LastChangeDate, l.CreationDate) AS LastChangeDate,

        -- Скрытые FK для каскадной инвалидации (не в гриде)
        CAST(nf.Id AS INT)              AS NodeFromId,
        CAST(nt.Id AS INT)              AS NodeToId,
        CAST(np.Id AS INT)              AS ProxyNodeId,
        CAST(rf.Id AS INT)              AS RegionFromId,
        CAST(rt.Id AS INT)              AS RegionToId,
        CAST(rp.Id AS INT)              AS ProxyRegionId

    FROM vw_TransportLeg l (NOLOCK)
    JOIN vw_TransportKind tk (NOLOCK) ON l.TransportKind = tk.Id
    JOIN vw_ShipmentType st (NOLOCK) ON l.ShipmentTypeCodeT = st.Code

    JOIN vw_LocationsNodes nf (NOLOCK) ON l.NodeFrom = nf.Id
    JOIN vw_Region rf (NOLOCK) ON rf.Id = nf.Region
    JOIN vw_LocationsNodes nt (NOLOCK) ON l.NodeTo = nt.Id
    JOIN vw_Region rt (NOLOCK) ON rt.Id = nt.Region
    LEFT JOIN vw_LocationsNodes np (NOLOCK) ON l.ProxyNode = np.Id
    LEFT JOIN vw_Region rp (NOLOCK) ON rp.Id = np.Region
GO
