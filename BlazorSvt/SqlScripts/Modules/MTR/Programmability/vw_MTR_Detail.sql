USE [mdm];
GO

/*
    Карточка MTR. Поля длинного списка MDM плюс системные.
    Колонки грида — из проекции снимка. Поля только карточки
    (краткое RU, полное EN, статус, код продукта) читаются из dbo.vw_MTR.
*/

CREATE OR ALTER VIEW v2.vw_MTR_Detail
AS
    SELECT
        s.MTRId,
        s.IsArchive,
        s.Code,
        s.NameFullRu,
        LEFT(m.NameShort_ru, 4000) AS NameShortRu,
        LEFT(m.NameFull_en, 4000)  AS NameFullEn,
        s.NameShortEn,
        s.ProductGroupCode,
        s.ProductGroupNameRu,
        s.ProductGroupNameEn,
        s.EtsngNameRu,
        s.EtsngNameEn,
        s.GngCode,
        s.UnitId AS UnitIdRu,
        s.UnitId AS UnitIdEn,
        s.DpoCode,
        s.MtrType,
        s.DangerClassId AS DangerClassIdRu,
        s.DangerClassId AS DangerClassIdEn,
        CAST(m.Status AS BIGINT) AS StatusIdRu,
        CAST(m.Status AS BIGINT) AS StatusIdEn,
        LEFT(m.ProductCode, 50) AS ProductCode,
        s.CreationDate,
        s.LastChangeDate
    FROM v2.vw_MTR_SnapshotSource s
    JOIN dbo.vw_MTR m (NOLOCK) ON m.Id = s.MTRId;
GO
