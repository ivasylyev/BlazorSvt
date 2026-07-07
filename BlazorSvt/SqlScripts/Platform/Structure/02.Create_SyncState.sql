USE [mdm];
GO

/*
    Служебная таблица-курсор инкрементальной синхронизации legacy -> v2 snapshot.

    НЕ журнал изменений: хранит только "докуда прочитан RowVer" по каждой паре
    (справочник, источник). Источников у одного справочника несколько — основная
    таблица + справочные таблицы для каскадной инвалидации денормализованных полей.

    Пример:
      Entity        SourceName                     LastRowVersion
      TransportLeg  dbo.PrimitiveEntityData_2007   0x000000000012AB01   (основная)
      TransportLeg  dbo.PrimitiveEntityData_1008   0x000000000012AAF0   (Region, каскад)
      TransportLeg  dbo.PrimitiveEntityData_1014   0x000000000012AAF0   (LocationsNodes, каскад)
*/

DROP TABLE IF EXISTS v2.SyncState;
GO

CREATE TABLE v2.SyncState
(
    Entity              NVARCHAR(100)  NOT NULL,
    SourceName          NVARCHAR(200)  NOT NULL,
    LastRowVersion      BINARY(8)      NOT NULL,
    LastRunUtc          DATETIME2(3)   NOT NULL CONSTRAINT DF_SyncState_LastRunUtc DEFAULT (SYSUTCDATETIME()),
    LastAffectedCount   INT            NULL,
    LastReconcileUtc    DATETIME2(3)   NULL,
    CONSTRAINT PK_SyncState PRIMARY KEY CLUSTERED (Entity, SourceName)
);
GO
