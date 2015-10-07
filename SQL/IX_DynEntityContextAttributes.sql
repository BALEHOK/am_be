/****** Object:  Index [IX_DynEntityContextAttributes_ContextId]    Script Date: 07/05/2011 14:59:18 ******/
CREATE NONCLUSTERED INDEX [IX_DynEntityContextAttributes_ContextId] ON [dbo].[DynEntityContextAttributes] 
(
	[ContextId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_DynEntityContextAttributes_ContextId_History]    Script Date: 07/05/2011 15:00:51 ******/
CREATE NONCLUSTERED INDEX [IX_DynEntityContextAttributes_ContextId_History] ON [dbo].[DynEntityContextAttributes] 
(
	[ContextId] ASC,
	[IsHistory] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

