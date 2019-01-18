-- --------------------------------------------------
-- **************************************************
-- Author:		Florian Dendorfer
-- Date:		09.10.2018
-- Description:	Test DB for IPA
-- **************************************************

-- **************************************************
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
	[DescriptionLong] NVARCHAR(MAX),
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
  [Department] NVARCHAR(20),
  [Holidays] DECIMAL(18, 2),
  [Deactivated] bit,
)
GO
-- **************************************************

-- **************************************************
-- Table AbsenceDetail
-- **************************************************
CREATE TABLE [AbsenceDetail] (	
	[ID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[Reason] NVARCHAR(100),
)
GO
-- **************************************************

-- **************************************************
-- Table OvertimeDetail
-- **************************************************
CREATE TABLE OvertimeDetail (
	[ID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[Description] NVARCHAR(100),
	[Rate] DECIMAL(4, 2),
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
  [TotalDays] DECIMAL(18, 2),
	[Reason] NVARCHAR(100),
	[Approved] BIT,
	[CreatedOn] DATETIME,
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
	[Hours] DECIMAL(18, 2),
	[CreatedOn] DATETIME,
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
INSERT INTO [USER] ([IdPermission], [Firstname], [Lastname], [Username], [Password], [Department], [Holidays], [Deactivated])
	VALUES ((SELECT [ID] FROM [Permission] WHERE [Level] = 1), 'Florian', 'Dendorfer', 'fdendorfer', CONVERT(VARCHAR(32), HashBytes('MD5', '123'), 1), 'Software', 25, 0),
	((SELECT [ID] FROM [Permission] WHERE [Level] = 2), 'Roman', 'Bleisch', 'rbleisch', CONVERT(VARCHAR(32), HashBytes('MD5', '123'), 1), 'Software', 25, 0),
	((SELECT [ID] FROM [Permission] WHERE [Level] = 3), 'Marco', 'Andreoli', 'mandreoli', CONVERT(VARCHAR(32), HashBytes('MD5', '123'), 1), 'Technik', 25, 0)
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
INSERT INTO [OvertimeDetail] ([ID], [Description], [Rate])
	VALUES ('84B41D45-AF72-405F-9C7D-AFFC05A0E5DC', 'Werktag, VOR 7:00 und NACH 18:00 Uhr ohne Zuschlag', 1.00),
	('63AD4F14-589E-4C77-8601-A80EEBB20FB7','SAMSTAG, ganzer Tag mit 25% Zuschlag', 1.25),
	('8923C12A-2F13-400F-ABD9-4B529A17A011','SONNTAG, ganzer Tag mit 50% Zuschlag', 1.50)
GO
-- **************************************************

-- --------------------------------------------------