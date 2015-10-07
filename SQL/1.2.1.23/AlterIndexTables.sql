-- Drop Constraints 
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_IndexActiveDynEntities_DisplayValues]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[IndexActiveDynEntities] DROP CONSTRAINT [DF_IndexActiveDynEntities_DisplayValues]
END
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_IndexActiveDynEntities_DisplayExtValues]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[IndexActiveDynEntities] DROP CONSTRAINT [DF_IndexActiveDynEntities_DisplayExtValues]
END
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_IndexHistoryDynEntities_DisplayValues]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[IndexHistoryDynEntities] DROP CONSTRAINT [DF_IndexHistoryDynEntities_DisplayValues]
END
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_IndexHistoryDynEntities_DisplayExtValues]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[IndexHistoryDynEntities] DROP CONSTRAINT [DF_IndexHistoryDynEntities_DisplayExtValues]
END
GO

-- Alter table IndexActiveDynEntities (DisplayValues)
ALTER TABLE IndexActiveDynEntities
ALTER COLUMN DisplayValues NVARCHAR(3000) NOT NULL
GO

ALTER TABLE IndexActiveDynEntities
ALTER COLUMN DisplayExtValues NVARCHAR(3000) NOT NULL

-- Alter table IndexHistoryDynEntities (DisplayValues)
ALTER TABLE IndexHistoryDynEntities
ALTER COLUMN DisplayValues NVARCHAR(3000) NOT NULL
GO

ALTER TABLE IndexHistoryDynEntities
ALTER COLUMN DisplayExtValues NVARCHAR(3000) NOT NULL

-- Recreate Constraints
ALTER TABLE [dbo].[IndexActiveDynEntities] ADD  CONSTRAINT [DF_IndexActiveDynEntities_DisplayValues]  DEFAULT ('') FOR [DisplayValues]
GO
ALTER TABLE [dbo].[IndexActiveDynEntities] ADD  CONSTRAINT [DF_IndexActiveDynEntities_DisplayExtValues]  DEFAULT ('') FOR [DisplayExtValues]
GO
ALTER TABLE [dbo].[IndexHistoryDynEntities] ADD  CONSTRAINT [DF_IndexHistoryDynEntities_DisplayValues]  DEFAULT ('') FOR [DisplayValues]
GO
ALTER TABLE [dbo].[IndexHistoryDynEntities] ADD  CONSTRAINT [DF_IndexHistoryDynEntities_DisplayExtValues]  DEFAULT ('') FOR [DisplayExtValues]
GO

-- Create Indexes IndexActiveDynEntities
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IndexActiveDynEntities]') AND name = N'IX_IndexActiveDynEntities_DynEntityConfigUid')
DROP INDEX [IX_IndexActiveDynEntities_DynEntityConfigUid] ON [dbo].[IndexActiveDynEntities] WITH ( ONLINE = OFF )
GO
CREATE NONCLUSTERED INDEX [IX_IndexActiveDynEntities_DynEntityConfigUid] ON [dbo].[IndexActiveDynEntities] 
(
	[DynEntityConfigUId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

--

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IndexActiveDynEntities]') AND name = N'IX_IndexActiveDynEntitiesLocation')
DROP INDEX [IX_IndexActiveDynEntitiesLocation] ON [dbo].[IndexActiveDynEntities] WITH ( ONLINE = OFF )
GO
CREATE NONCLUSTERED INDEX [IX_IndexActiveDynEntitiesLocation] ON [dbo].[IndexActiveDynEntities] 
(
	[Location] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

-- 

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IndexActiveDynEntities]') AND name = N'IX_IndexActiveDynEntitiesSearc')
DROP INDEX [IX_IndexActiveDynEntitiesSearc] ON [dbo].[IndexActiveDynEntities] WITH ( ONLINE = OFF )
GO
CREATE NONCLUSTERED INDEX [IX_IndexActiveDynEntitiesSearc] ON [dbo].[IndexActiveDynEntities] 
(
	[DynEntityConfigId] ASC,
	[DepartmentId] ASC,
	[OwnerId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

--

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IndexActiveDynEntities]') AND name = N'IX_IndexActiveDynEntitiesUpdateDate')
DROP INDEX [IX_IndexActiveDynEntitiesUpdateDate] ON [dbo].[IndexActiveDynEntities] WITH ( ONLINE = OFF )
GO
CREATE NONCLUSTERED INDEX [IX_IndexActiveDynEntitiesUpdateDate] ON [dbo].[IndexActiveDynEntities] 
(
	[UpdateDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

--

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IndexActiveDynEntities]') AND name = N'IX_UQ_IndexActiveDynEntities_DynEntityId_DynEntityConfigId')
DROP INDEX [IX_UQ_IndexActiveDynEntities_DynEntityId_DynEntityConfigId] ON [dbo].[IndexActiveDynEntities] WITH ( ONLINE = OFF )
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_IndexActiveDynEntities_DynEntityId_DynEntityConfigId] ON [dbo].[IndexActiveDynEntities] 
(
	[DynEntityId] ASC,
	[DynEntityConfigId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

--

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IndexActiveDynEntities]') AND name = N'IX_IndexActiveDynEntitiesDispValues')
DROP INDEX [IX_IndexActiveDynEntitiesDispValues] ON [dbo].[IndexActiveDynEntities] WITH ( ONLINE = OFF )
GO
CREATE NONCLUSTERED INDEX [IX_IndexActiveDynEntitiesDispValues] ON [dbo].[IndexActiveDynEntities] 
(
	[DisplayValues] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

-- Create Indexes IndexHistoryDynEntities
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IndexHistoryDynEntities]') AND name = N'IX_IndexHistoryDynEntities_DynEntityConfigUid')
DROP INDEX [IX_IndexHistoryDynEntities_DynEntityConfigUid] ON [dbo].[IndexHistoryDynEntities] WITH ( ONLINE = OFF )
GO
CREATE NONCLUSTERED INDEX [IX_IndexHistoryDynEntities_DynEntityConfigUid] ON [dbo].[IndexHistoryDynEntities] 
(
	[DynEntityConfigUId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

--

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IndexHistoryDynEntities]') AND name = N'IX_IndexHistoryDynEntitiesLocation')
DROP INDEX [IX_IndexHistoryDynEntitiesLocation] ON [dbo].[IndexHistoryDynEntities] WITH ( ONLINE = OFF )
GO
CREATE NONCLUSTERED INDEX [IX_IndexHistoryDynEntitiesLocation] ON [dbo].[IndexHistoryDynEntities] 
(
	[Location] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

-- 

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IndexHistoryDynEntities]') AND name = N'IX_IndexHistoryDynEntitiesSearc')
DROP INDEX [IX_IndexHistoryDynEntitiesSearc] ON [dbo].[IndexHistoryDynEntities] WITH ( ONLINE = OFF )
GO
CREATE NONCLUSTERED INDEX [IX_IndexHistoryDynEntitiesSearc] ON [dbo].[IndexHistoryDynEntities] 
(
	[DynEntityConfigId] ASC,
	[DepartmentId] ASC,
	[OwnerId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

--

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IndexHistoryDynEntities]') AND name = N'IX_IndexHistoryDynEntitiesUpdateDate')
DROP INDEX [IX_IndexHistoryDynEntitiesUpdateDate] ON [dbo].[IndexHistoryDynEntities] WITH ( ONLINE = OFF )
GO
CREATE NONCLUSTERED INDEX [IX_IndexHistoryDynEntitiesUpdateDate] ON [dbo].[IndexHistoryDynEntities] 
(
	[UpdateDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

--

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IndexHistoryDynEntities]') AND name = N'IX_UQ_IndexHistoryDynEntities_DynEntityId_DynEntityConfigId')
DROP INDEX [IX_UQ_IndexHistoryDynEntities_DynEntityId_DynEntityConfigId] ON [dbo].[IndexHistoryDynEntities] WITH ( ONLINE = OFF )
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_IndexHistoryDynEntities_DynEntityId_DynEntityConfigId] ON [dbo].[IndexHistoryDynEntities] 
(
	[DynEntityId] ASC,
	[DynEntityConfigId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

-- 

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IndexHistoryDynEntities]') AND name = N'IX_IndexHistoryDynEntitiesDispValues')
DROP INDEX [IX_IndexHistoryDynEntitiesDispValues] ON [dbo].[IndexHistoryDynEntities] WITH ( ONLINE = OFF )
GO
CREATE NONCLUSTERED INDEX [IX_IndexHistoryDynEntitiesDispValues] ON [dbo].[IndexHistoryDynEntities] 
(
	[DisplayValues] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

-- Add Column SearchDateTimeStamp to index tables (to clear them more safely)
IF NOT EXISTS(SELECT 1 FROM sys.columns 
           WHERE Name = N'SearchDateTimeStamp' and Object_ID = Object_ID(N'_search_srchres'))
ALTER TABLE _search_srchres ADD SearchDateTimeStamp datetime DEFAULT getdate() NOT NULL
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
           WHERE Name = N'SearchDateTimeStamp' and Object_ID = Object_ID(N'_search_srchcount'))
ALTER TABLE _search_srchcount ADD SearchDateTimeStamp datetime DEFAULT getdate() NOT NULL
GO

-- Start propagating tracked changes (if not already done) to the full-text index as they occur
EXEC sp_fulltext_table '[dbo].[IndexActiveDynEntities]', 'Start_change_tracking';
EXEC sp_fulltext_table '[dbo].[IndexActiveDynEntities]', 'Start_background_updateindex';
EXEC sp_fulltext_table '[dbo].[IndexHistoryDynEntities]', 'Start_change_tracking';
EXEC sp_fulltext_table '[dbo].[IndexHistoryDynEntities]', 'Start_background_updateindex';