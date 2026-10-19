USE BDD_PoliMusic;
GO
IF OBJECT_ID('dbo.TBL_PLAY_EVENT','U') IS NULL
CREATE TABLE dbo.TBL_PLAY_EVENT(
 EVENT_ID uniqueidentifier NOT NULL PRIMARY KEY,
 ID_USER int NOT NULL REFERENCES dbo.TBL_USER(ID_USER),
 ID_SONG int NOT NULL REFERENCES dbo.TBL_SONG(ID_SONG),
 STARTED_AT datetime2 NOT NULL,
 COUNTED bit NOT NULL DEFAULT 0
);
-- Add a maintenance job to remove old receipts after your chosen retention period.
-- No individual playback-history feature is exposed by this migration.
