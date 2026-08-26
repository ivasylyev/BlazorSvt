USE [mdm];
GO

/*
    Разовый data-fix архивных TransportLeg (PrimitiveEntityDataStateId = 2):
    заполнение NULL в обязательных полях (TransportKind / LegIsActive / NodeFrom / NodeTo),
    чтобы строки входили в snapshot/грид и в легаси можно было повесить NOT NULL.

    Идемпотентен. Не трогает Actual (1) и Deleted (3).
    Не меняет LastChangeDate / LastChangeUserId.
    Не трогает v2.TransportLeg_Snapshot — инкрементальный sync подхватит RowVer.

    TransportKind (только две строки, только если ещё NULL):
      Code = N'A70066_A70066_Auto'           → 543760 (Auto)
      Code = N'R369649_R244509_No_Movement'  → 543761 (Rail)

    LegIsActive: NULL → 0

    NodeFrom / NodeTo: сегменты Code до 1-го / между 1-м и 2-м '_';
      алиасы сегментов: AC → BR-AC, EU → LV (всегда);
      lookup vw_LocationsNodes.Code, state IN (1, 2),
      TOP 1 ORDER BY PrimitiveEntityDataStateId (Actual раньше Archival).
      Не нашли — оставляем NULL (строки выводятся в отчёте).
*/

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;
SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Missing NVARCHAR(1000) = N'';

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_TransportKind WITH (NOLOCK) WHERE Id = 543760)
        SET @Missing = @Missing + N'TransportKind 543760 (Auto); ';

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_TransportKind WITH (NOLOCK) WHERE Id = 543761)
        SET @Missing = @Missing + N'TransportKind 543761 (Rail); ';

    IF NOT EXISTS (
        SELECT 1 FROM dbo.vw_LocationsNodes WITH (NOLOCK)
        WHERE Code = N'BR-AC' AND PrimitiveEntityDataStateId IN (1, 2)
    )
        SET @Missing = @Missing + N'LocationsNodes Code BR-AC (alias for AC); ';

    IF NOT EXISTS (
        SELECT 1 FROM dbo.vw_LocationsNodes WITH (NOLOCK)
        WHERE Code = N'LV' AND PrimitiveEntityDataStateId IN (1, 2)
    )
        SET @Missing = @Missing + N'LocationsNodes Code LV (alias for EU); ';

    IF @Missing <> N''
    BEGIN
        DECLARE @GuardMsg NVARCHAR(1200) = N'Константа/узел не найдены: ' + @Missing;
        ;THROW 50000, @GuardMsg, 1;
    END

    UPDATE l
    SET
        LegIsActive = ISNULL(l.LegIsActive, 0),
        TransportKind = CASE
            WHEN l.TransportKind IS NOT NULL THEN l.TransportKind
            WHEN l.Code = N'A70066_A70066_Auto' THEN CAST(543760 AS BIGINT)
            WHEN l.Code = N'R369649_R244509_No_Movement' THEN CAST(543761 AS BIGINT)
            ELSE l.TransportKind
        END,
        NodeFrom = COALESCE(l.NodeFrom, ln_from.Id),
        NodeTo = COALESCE(l.NodeTo, ln_to.Id)
    FROM dbo.vw_TransportLeg AS l
    CROSS APPLY (
        SELECT
            CASE
                WHEN CHARINDEX(N'_', l.Code) > 1
                THEN LEFT(l.Code, CHARINDEX(N'_', l.Code) - 1)
            END AS FromCodeRaw,
            CASE
                WHEN CHARINDEX(N'_', l.Code) > 0
                 AND CHARINDEX(N'_', l.Code, CHARINDEX(N'_', l.Code) + 1)
                     > CHARINDEX(N'_', l.Code) + 1
                THEN SUBSTRING(
                        l.Code,
                        CHARINDEX(N'_', l.Code) + 1,
                        CHARINDEX(N'_', l.Code, CHARINDEX(N'_', l.Code) + 1)
                            - CHARINDEX(N'_', l.Code) - 1
                     )
            END AS ToCodeRaw
    ) AS raw_tokens
    CROSS APPLY (
        SELECT
            CASE
                WHEN raw_tokens.FromCodeRaw = N'AC' THEN N'BR-AC'
                WHEN raw_tokens.FromCodeRaw = N'EU' THEN N'LV'
                ELSE raw_tokens.FromCodeRaw
            END AS FromCode,
            CASE
                WHEN raw_tokens.ToCodeRaw = N'AC' THEN N'BR-AC'
                WHEN raw_tokens.ToCodeRaw = N'EU' THEN N'LV'
                ELSE raw_tokens.ToCodeRaw
            END AS ToCode
    ) AS tokens
    OUTER APPLY (
        SELECT TOP (1) ln.Id
        FROM dbo.vw_LocationsNodes AS ln WITH (NOLOCK)
        WHERE tokens.FromCode IS NOT NULL
          AND ln.Code = tokens.FromCode
          AND ln.PrimitiveEntityDataStateId IN (1, 2)
        ORDER BY ln.PrimitiveEntityDataStateId, ln.Id
    ) AS ln_from
    OUTER APPLY (
        SELECT TOP (1) ln.Id
        FROM dbo.vw_LocationsNodes AS ln WITH (NOLOCK)
        WHERE tokens.ToCode IS NOT NULL
          AND ln.Code = tokens.ToCode
          AND ln.PrimitiveEntityDataStateId IN (1, 2)
        ORDER BY ln.PrimitiveEntityDataStateId, ln.Id
    ) AS ln_to
    WHERE l.PrimitiveEntityDataStateId = 2
      AND (
            l.LegIsActive IS NULL
         OR (
                l.TransportKind IS NULL
            AND l.Code IN (N'A70066_A70066_Auto', N'R369649_R244509_No_Movement')
            )
         OR (l.NodeFrom IS NULL AND ln_from.Id IS NOT NULL)
         OR (l.NodeTo IS NULL AND ln_to.Id IS NOT NULL)
          );

    PRINT CONCAT(N'Updated archival TransportLeg rows: ', @@ROWCOUNT);

    COMMIT TRANSACTION;

    -- Контроль: NULL по патчимым полям у архива
    SELECT
        SUM(CASE WHEN LegIsActive IS NULL THEN 1 ELSE 0 END) AS NullLegIsActive,
        SUM(CASE WHEN TransportKind IS NULL THEN 1 ELSE 0 END) AS NullTransportKind,
        SUM(CASE WHEN NodeFrom IS NULL THEN 1 ELSE 0 END) AS NullNodeFrom,
        SUM(CASE WHEN NodeTo IS NULL THEN 1 ELSE 0 END) AS NullNodeTo
    FROM dbo.vw_TransportLeg WITH (NOLOCK)
    WHERE PrimitiveEntityDataStateId = 2;

    PRINT N'Remaining archival TransportLeg with TransportKind IS NULL:';
    SELECT Id, Code, TransportKind, NodeFrom, NodeTo, LegIsActive, LastChangeDate
    FROM dbo.vw_TransportLeg WITH (NOLOCK)
    WHERE PrimitiveEntityDataStateId = 2
      AND TransportKind IS NULL
    ORDER BY Code;

    PRINT N'Remaining archival TransportLeg with NodeFrom IS NULL:';
    SELECT Id, Code, NodeFrom, NodeTo, TransportKind, LegIsActive, LastChangeDate
    FROM dbo.vw_TransportLeg WITH (NOLOCK)
    WHERE PrimitiveEntityDataStateId = 2
      AND NodeFrom IS NULL
    ORDER BY Code;

    PRINT N'Remaining archival TransportLeg with NodeTo IS NULL:';
    SELECT Id, Code, NodeFrom, NodeTo, TransportKind, LegIsActive, LastChangeDate
    FROM dbo.vw_TransportLeg WITH (NOLOCK)
    WHERE PrimitiveEntityDataStateId = 2
      AND NodeTo IS NULL
    ORDER BY Code;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH
GO
