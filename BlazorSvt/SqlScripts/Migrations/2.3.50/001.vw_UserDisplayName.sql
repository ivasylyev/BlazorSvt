USE [mdm];
GO

CREATE OR ALTER VIEW v2.vw_UserDisplayName
AS
    SELECT
        u.Id                                                AS UserId,
        LEFT(ISNULL(NULLIF(u.Name, N''), u.Login), 255)     AS DisplayName
    FROM dbo.[User] u (NOLOCK);
GO
