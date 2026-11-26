USE [mdm];
GO

/*
    ФИО пользователя легаси для колонок «Автор» и «Автор изм.» (SVT-5320).
    Name, при пустом Name — Login. Ключ — dbo.[User].Id
    (CreationUserId / LastChangeUserId в dbo.vw_*).
*/

CREATE OR ALTER VIEW v2.vw_UserDisplayName
AS
    SELECT
        u.Id                                                AS UserId,
        LEFT(ISNULL(NULLIF(u.Name, N''), u.Login), 255)     AS DisplayName
    FROM dbo.[User] u (NOLOCK);
GO
