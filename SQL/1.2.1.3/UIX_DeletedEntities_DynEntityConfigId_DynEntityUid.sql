/****** Object:  Index [UIX_DeletedEntities_DynEntityConfigId_DynEntityUid]    Script Date: 02/23/2012 00:39:57 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UIX_DeletedEntities_DynEntityConfigId_DynEntityUid] ON [dbo].[DeletedEntities] 
(
	[DynEntityConfigId] ASC,
	[DynEntityUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


