-- **************************************************
-- Author:		    Florian Dendorfer
-- Last updated:  24.01.2018
-- Description:	  TimeManager DB for IPA
-- **************************************************

-- **************************************************
-- Dropping old database to create new
USE master
IF EXISTS(SELECT * FROM sys.databases WHERE name='TimeManager')
DROP DATABASE TimeManager
GO
CREATE DATABASE TimeManager
GO
USE TimeManager
GO

-- **************************************************
CREATE TABLE [Permission] (
	[ID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[Level] TINYINT,
	[Description] NVARCHAR(100),
	[DescriptionLong] NVARCHAR(MAX),
)
GO

-- **************************************************
CREATE TABLE [User] (
	[ID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[IdPermission] UNIQUEIDENTIFIER,
	[Firstname] NVARCHAR(100),
	[Lastname] NVARCHAR(100),
	[Username] NVARCHAR(100),
	[Password] VARBINARY(8000),
  [Department] NVARCHAR(20),
  [Holidays] DECIMAL(18, 2),
  [Deactivated] bit,
)
GO

-- **************************************************
CREATE TABLE [AbsenceDetail] (	
	[ID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[Reason] NVARCHAR(100),
)
GO

-- **************************************************
CREATE TABLE OvertimeDetail (
	[ID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[Description] NVARCHAR(100),
	[Rate] DECIMAL(4, 2),
)
GO

-- **************************************************
CREATE TABLE Absence (
	[ID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[IdUser] UNIQUEIDENTIFIER,
	[IdAbsenceDetail] UNIQUEIDENTIFIER,
	[AbsentFrom] SMALLDATETIME,
	[AbsentTo] SMALLDATETIME,
  [Negative] BIT,
	[Reason] NVARCHAR(100),
	[Approved] BIT,
	[CreatedOn] DATETIME,
)
GO

-- **************************************************
CREATE TABLE Overtime (
	[ID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[IdOvertimeDetail] UNIQUEIDENTIFIER,
	[IdUser] UNIQUEIDENTIFIER,
	[Customer] NVARCHAR(100),
	[Date] SMALLDATETIME,
	[Hours] DECIMAL(18, 2),
	[CreatedOn] DATETIME,
)
GO

-- **************************************************
-- INSERTS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

-- **************************************************
INSERT INTO [Permission] ([Level], [Description], [DescriptionLong])
	VALUES (1, 'Normal', 'Users, who can only see their own times'),
	(2, 'Advanced', 'Can approve absences of Normal users'),
	(3, 'High', 'Can make changes to Details and Users')
GO

-- **************************************************
INSERT INTO [USER] ([IdPermission], [Firstname], [Lastname], [Username], [Password], [Department], [Holidays], [Deactivated])
	VALUES ((SELECT [ID] FROM [Permission] WHERE [Level] = 1), 'Florian', 'Dendorfer', 'fdendorfer', CONVERT(VARCHAR(32), HashBytes('MD5', '123'), 1), 'Software', 25, 0),
	((SELECT [ID] FROM [Permission] WHERE [Level] = 2), 'Roman', 'Bleisch', 'rbleisch', CONVERT(VARCHAR(32), HashBytes('MD5', '123'), 1), 'Software', 25, 0),
	((SELECT [ID] FROM [Permission] WHERE [Level] = 3), 'Marco', 'Andreoli', 'mandreoli', CONVERT(VARCHAR(32), HashBytes('MD5', '123'), 1), 'Technik', 25, 0)
GO

-- **************************************************
INSERT INTO [AbsenceDetail] ([Reason])
	VALUES ('Ferien'),
	('Krank'),
	('Militär'),
	('Schule'),
	('Unfall')
GO

-- **************************************************
INSERT INTO [OvertimeDetail] ([Description], [Rate])
	VALUES ('Werktag, VOR 7:00 und NACH 18:00 Uhr ohne Zuschlag', 1.00),
	('SAMSTAG, ganzer Tag mit 25% Zuschlag', 1.25),
	('SONNTAG, ganzer Tag mit 50% Zuschlag', 1.50)
GO