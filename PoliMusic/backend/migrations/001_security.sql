-- Review on a backup first. Run against an EXISTING BDD_PoliMusic.
-- This does not drop databases, create logins, grant privileges or rewrite passwords.
USE BDD_PoliMusic;
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF EXISTS(SELECT USERNAME FROM dbo.TBL_USER GROUP BY USERNAME HAVING COUNT(*)>1)
    THROW 50001,'Resolve duplicate usernames before migration',1;
IF EXISTS(SELECT EMAIL FROM dbo.TBL_USER GROUP BY EMAIL HAVING COUNT(*)>1)
    THROW 50002,'Resolve duplicate emails before migration',1;
ALTER TABLE dbo.TBL_USER ALTER COLUMN PASSWORD varchar(255) NOT NULL;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='UX_USER_USERNAME' AND object_id=OBJECT_ID('dbo.TBL_USER'))
    CREATE UNIQUE INDEX UX_USER_USERNAME ON dbo.TBL_USER(USERNAME);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='UX_USER_EMAIL' AND object_id=OBJECT_ID('dbo.TBL_USER'))
    CREATE UNIQUE INDEX UX_USER_EMAIL ON dbo.TBL_USER(EMAIL);
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_USER_TYPE')
    ALTER TABLE dbo.TBL_USER ADD CONSTRAINT CK_USER_TYPE CHECK(USER_TYPE IN(0,1));
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_PLAYLIST_TYPE')
    ALTER TABLE dbo.TBL_PLAYLIST ADD CONSTRAINT CK_PLAYLIST_TYPE CHECK(TYPE IN(0,1));
COMMIT;
-- Legacy plaintext/SHA-1 accounts are intentionally rejected by the API.
-- Use a trusted, server-side administrator password reset workflow before enabling them.
-- Rotate the login password embedded in the original script and revoke db_owner
-- through a DBA-reviewed least-privilege deployment. Never use that login in source.
