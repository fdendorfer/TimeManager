-- --------------------------------------------------
-- **************************************************
-- Author:		Florian Dendorfer
-- Date:		09.10.2018
-- Description:	Test DB for IPA
-- **************************************************

-- **************************************************
-- Delete when finished DB
-- Dropping old database to create new
-- **************************************************
USE master
IF EXISTS(SELECT * FROM sys.databases WHERE name='TimeManager')
DROP DATABASE TimeManager
GO

CREATE DATABASE TimeManager
GO

USE TimeManager
GO
-- **************************************************
-- --------------------------------------------------

-- **************************************************
-- Table Permission
-- **************************************************
CREATE TABLE [Permission] (
	[ID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[Level] TINYINT,
	[Description] NVARCHAR(100),
	[DescriptionLong] NVARCHAR(MAX)
)
GO
-- **************************************************

-- **************************************************
-- Table User
-- **************************************************
CREATE TABLE [User] (
	[ID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[IdPermission] UNIQUEIDENTIFIER,
	[Firstname] NVARCHAR(100),
	[Lastname] NVARCHAR(100),
	[Username] NVARCHAR(100),
	[Password] VARBINARY(8000),
  [Department] NVARCHAR(20)
)
GO
-- **************************************************

-- **************************************************
-- Table AbsenceDetail
-- **************************************************
CREATE TABLE [AbsenceDetail] (	
	[ID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[Reason] NVARCHAR(100)
)
GO
-- **************************************************

-- **************************************************
-- Table OvertimeDetail
-- **************************************************
CREATE TABLE OvertimeDetail (
	[ID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[Description] NVARCHAR(100),
	[Rate] TINYINT
)
GO
-- **************************************************

-- **************************************************
-- Table Absence
-- **************************************************
CREATE TABLE Absence (
	[ID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[IdUser] UNIQUEIDENTIFIER,
	[IdAbsenceDetail] UNIQUEIDENTIFIER,
	[AbsentFrom] SMALLDATETIME,
	[AbsentTo] SMALLDATETIME,
	[Reason] NVARCHAR(100),
	[Approved] BIT,
)
GO
-- **************************************************

-- **************************************************
-- Table Overtime
-- **************************************************
CREATE TABLE Overtime (
	[ID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[IdOvertimeDetail] UNIQUEIDENTIFIER,
	[IdUser] UNIQUEIDENTIFIER,
	[Customer] NVARCHAR(100),
	[Date] SMALLDATETIME,
	[Hours] DECIMAL(18, 2)
)
GO
-- **************************************************

-- --------------------------------------------------
-- INSERTS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
-- --------------------------------------------------

-- **************************************************
-- Insert Users
-- **************************************************
INSERT INTO [Permission] ([Level], [Description], [DescriptionLong])
	VALUES (1, 'Normal', 'Users, who can only see their own times'),
	(2, 'Advanced', 'Can approve absences of Normal users'),
	(3, 'High', 'Can make changes to Details and Users')
GO
-- **************************************************

-- **************************************************
-- Insert Users
-- **************************************************
INSERT INTO [USER] ([IdPermission], [Firstname], [Lastname], [Username], [Password])
	VALUES ((SELECT [ID] FROM [Permission] WHERE [Level] = 1), 'Florian', 'Dendorfer', 'fdendorfer', ENCRYPTBYPASSPHRASE('SECURE1',N'fd1')),
	((SELECT [ID] FROM [Permission] WHERE [Level] = 2), 'Roman', 'Bleisch', 'rbleisch', ENCRYPTBYPASSPHRASE('SECURE1',N'rb1')),
	((SELECT [ID] FROM [Permission] WHERE [Level] = 3), 'Marco', 'Andreoli', 'mandreoli', ENCRYPTBYPASSPHRASE('SECURE1',N'ma1'))
GO
-- **************************************************

-- **************************************************
-- Insert Users
-- **************************************************
INSERT INTO [AbsenceDetail] ([Reason])
	VALUES ('Ferien'),
	('Krank'),
	('Militär'),
	('Schule'),
	('Unfall')
GO
-- **************************************************

-- **************************************************
-- Insert Users
-- **************************************************
INSERT INTO [OvertimeDetail] ([Description], [Rate])
	VALUES ('Werktag, VOR 7:00 und NACH 18:00 Uhr' + CHAR(13) + 'ohne Zuschlag', 0),
	('SAMSTAG, ganzer Tag' + CHAR(13) + 'mit 25% Zuschlag', 25),
	('SONNTAG, ganzer Tag' + CHAR(13) + 'mit 50% Zuschlag', 50)
GO
-- **************************************************

-- --------------------------------------------------