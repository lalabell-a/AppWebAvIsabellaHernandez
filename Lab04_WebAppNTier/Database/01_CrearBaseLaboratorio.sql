/* Base exclusiva del laboratorio 04. No modifica BDD_PoliMusic ni la aplicacion PoliMusic. */
USE master;
GO
IF DB_ID(N'BDD_PoliMusic_Lab04') IS NULL
    CREATE DATABASE BDD_PoliMusic_Lab04;
GO
USE BDD_PoliMusic_Lab04;
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.TBL_USER',N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TBL_USER (
        ID_USER int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Lab04_USER PRIMARY KEY,
        USERNAME varchar(50) NOT NULL,
        PASSWORD varchar(50) NOT NULL,
        EMAIL varchar(50) NOT NULL,
        BIRTHDAY datetime NULL,
        USER_PHOTO varchar(255) NULL,
        USER_TYPE int NOT NULL CONSTRAINT DF_Lab04_USER_TYPE DEFAULT(0),
        CONSTRAINT UQ_Lab04_USERNAME UNIQUE(USERNAME),
        CONSTRAINT UQ_Lab04_EMAIL UNIQUE(EMAIL),
        CONSTRAINT CK_Lab04_USER_TYPE CHECK(USER_TYPE IN(0,1))
    );
END;
IF OBJECT_ID(N'dbo.TBL_SONG',N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TBL_SONG (
        ID_SONG int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Lab04_SONG PRIMARY KEY,
        SONG_NAME varchar(50) NOT NULL,
        SONG_PATH varchar(255) NOT NULL,
        PLAYS int NULL CONSTRAINT DF_Lab04_PLAYS DEFAULT(0)
    );
END;
/* Solo se incluye el audio existente en los recursos originales. */
IF NOT EXISTS(SELECT 1 FROM dbo.TBL_SONG WHERE SONG_PATH='~/SongFiles/bensound-adventure.mp3')
    INSERT INTO dbo.TBL_SONG(SONG_NAME,SONG_PATH,PLAYS) VALUES('Adventure','~/SongFiles/bensound-adventure.mp3',0);
COMMIT;
GO
SELECT DB_NAME() AS BASE_LABORATORIO,COUNT(*) AS CANCIONES FROM dbo.TBL_SONG;
GO
/* Crear tu cuenta desde SignUp.aspx. No se insertan contrasenas de prueba. */
