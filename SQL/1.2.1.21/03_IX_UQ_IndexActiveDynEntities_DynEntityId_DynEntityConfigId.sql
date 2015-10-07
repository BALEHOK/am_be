/****** Object:  Index [IX_UQ_IndexActiveDynEntities_DynEntityId_DynEntityConfigId]    Script Date: 11.08.2012 21:48:14 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_IndexActiveDynEntities_DynEntityId_DynEntityConfigId] ON [dbo].[IndexActiveDynEntities]
(
	[DynEntityId] ASC,
	[DynEntityConfigId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

