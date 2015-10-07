/****** Object:  Index [IX_UQ_IndexHistoryDynEntities_DynEntityId_DynEntityConfigId]    Script Date: 10/15/2012 16:47:37 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IndexHistoryDynEntities]') AND name = N'IX_UQ_IndexHistoryDynEntities_DynEntityId_DynEntityConfigId')
DROP INDEX [IX_UQ_IndexHistoryDynEntities_DynEntityId_DynEntityConfigId] ON [dbo].[IndexHistoryDynEntities] WITH ( ONLINE = OFF )
GO


