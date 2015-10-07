/****** Object:  Index [IX_APA_DEAC_AP]    Script Date: 03/17/2012 17:27:20 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AttributePanelAttribute]') AND name = N'IX_APA_DEAC_AP')
DROP INDEX [IX_APA_DEAC_AP] ON [dbo].[AttributePanelAttribute] WITH ( ONLINE = OFF )
GO
/****** Object:  Index [IX_APA_DEAC_AP]    Script Date: 03/17/2012 17:27:20 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_APA_DEAC_AP] ON [dbo].[AttributePanelAttribute] 
(
	[AttributePanelUid] ASC,
	[DynEntityAttribConfigUId] ASC,
	[ReferencingDynEntityAttribConfigId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

ALTER INDEX [IX_APA_DEAC_AP] ON [dbo].[AttributePanelAttribute] DISABLE
GO


