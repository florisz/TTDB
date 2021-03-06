use ITSData
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Object_ObjectReference]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Object] DROP CONSTRAINT FK_Object_ObjectReference
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Object_ObjectRelation1]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Object] DROP CONSTRAINT FK_Object_ObjectRelation1
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Object_ObjectRelation2]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Object] DROP CONSTRAINT FK_Object_ObjectRelation2
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ObjectValue_Object]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ObjectValue] DROP CONSTRAINT FK_ObjectValue_Object
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ObjectValue_ObjectValueReference]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ObjectValue] DROP CONSTRAINT FK_ObjectValue_ObjectValueReference
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Object_ObjectType]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Object] DROP CONSTRAINT FK_Object_ObjectType
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ObjectJournal_ObjectValueAfter]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ObjectJournal] DROP CONSTRAINT FK_ObjectJournal_ObjectValueAfter
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ObjectJournal_ObjectValueBefore]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ObjectJournal] DROP CONSTRAINT FK_ObjectJournal_ObjectValueBefore
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ObjectJournal_Object]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ObjectJournal] DROP CONSTRAINT FK_ObjectJournal_Object
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ObjectType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ObjectType]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Object]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Object]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ObjectValue]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ObjectValue]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ObjectJournal]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ObjectJournal]
GO

CREATE TABLE [dbo].[ObjectType] (
	[Id] [int] IDENTITY (1, 1) NOT NULL ,
	[Name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[RelativeUri] [varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Object] (
	[Id] [uniqueidentifier] NOT NULL ,
	[ExtId] [nvarchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[ObjectTypeId] [int] NOT NULL ,
	[Reference] [uniqueidentifier] NULL ,
	[Relation1] [uniqueidentifier] NULL ,
	[Relation2] [uniqueidentifier] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ObjectValue] (
	[Id] [uniqueidentifier] ROWGUIDCOL NOT NULL ,
	[ObjectId] [uniqueidentifier] NOT NULL ,
	[StartDate] [datetime] NOT NULL ,
	[EndDate] [datetime] NOT NULL ,
	[Reference] [uniqueidentifier] NULL ,
	[ContentType] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Content] [image] NULL ,
	[Version] [int] NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ObjectJournal] (
	[Id] [int] IDENTITY (1, 1) NOT NULL ,
	[ObjectId] [uniqueidentifier] NOT NULL ,
	[Timestamp] [datetime] NOT NULL ,
	[Username] [varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[ObjectValueBeforeId] [uniqueidentifier] NULL ,
	[ObjectValueAfterId] [uniqueidentifier] NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Object] WITH NOCHECK ADD 
	CONSTRAINT [PK_Object] PRIMARY KEY  CLUSTERED 
	(
		[Id]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ObjectType] WITH NOCHECK ADD 
	CONSTRAINT [PK_ObjectType] PRIMARY KEY  CLUSTERED 
	(
		[Id]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ObjectValue] WITH NOCHECK ADD 
	CONSTRAINT [PK_ObjectValue] PRIMARY KEY  CLUSTERED 
	(
		[Id]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ObjectJournal] WITH NOCHECK ADD 
	CONSTRAINT [PK_ObjectJournal] PRIMARY KEY  CLUSTERED 
	(
		[Id]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ObjectValue] WITH NOCHECK ADD 
	CONSTRAINT [DF_ObjectValue_Id] DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[Object] ADD 
	CONSTRAINT [FK_Object_ObjectReference] FOREIGN KEY 
	(
		[Reference]
	) REFERENCES [dbo].[Object] (
		[Id]
	),
	CONSTRAINT [FK_Object_ObjectRelation1] FOREIGN KEY 
	(
		[Relation1]
	) REFERENCES [dbo].[Object] (
		[Id]
	),
	CONSTRAINT [FK_Object_ObjectRelation2] FOREIGN KEY 
	(
		[Relation2]
	) REFERENCES [dbo].[Object] (
		[Id]
	),
	CONSTRAINT [FK_Object_ObjectType] FOREIGN KEY 
	(
		[ObjectTypeId]
	) REFERENCES [dbo].[ObjectType] (
		[Id]
	)
GO

ALTER TABLE [dbo].[ObjectValue] ADD 
	CONSTRAINT [FK_ObjectValue_Object] FOREIGN KEY 
	(
		[ObjectId]
	) REFERENCES [dbo].[Object] (
		[Id]
	),
	CONSTRAINT [FK_ObjectValue_ObjectValueReference] FOREIGN KEY 
	(
		[Reference]
	) REFERENCES [dbo].[ObjectValue] (
		[Id]
	)
GO

ALTER TABLE [dbo].[ObjectJournal] ADD 
	CONSTRAINT [FK_ObjectJournal_Object] FOREIGN KEY 
	(
		[ObjectId]
	) REFERENCES [dbo].[Object] (
		[Id]
	),
	CONSTRAINT [FK_ObjectJournal_ObjectValueAfter] FOREIGN KEY 
	(
		[ObjectValueAfterId]
	) REFERENCES [dbo].[ObjectValue] (
		[Id]
	),
	CONSTRAINT [FK_ObjectJournal_ObjectValueBefore] FOREIGN KEY 
	(
		[ObjectValueBeforeId]
	) REFERENCES [dbo].[ObjectValue] (
		[Id]
	)
GO

DELETE FROM ObjectType
GO

INSERT INTO ObjectType (Name, RelativeUri)
VALUES('casefilespecification', '/specifications/casefiles/')
GO

insert into ObjectType (Name, RelativeUri)
VALUES('casefile', '/casefiles/')
GO

INSERT INTO ObjectType (Name)
VALUES('entity')
GO

INSERT INTO ObjectType (Name, RelativeUri)
VALUES('objectmodel', '/specifications/objectmodels/')
GO

INSERT INTO ObjectType (Name)
VALUES('relation')
GO

INSERT INTO ObjectType (Name, RelativeUri)
VALUES('representation', '/representations/')
GO

INSERT INTO ObjectType (Name, RelativeUri)
VALUES('resource', '/resources/')
GO

INSERT INTO ObjectType (Name, RelativeUri)
VALUES('ruleset', '/rules/')
GO

