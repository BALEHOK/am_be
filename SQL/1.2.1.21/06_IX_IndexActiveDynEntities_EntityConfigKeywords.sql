/****** Object:  Index [IX_IndexActiveDynEntities_EntityConfigKeywords]    Script Date: 11.08.2012 21:48:45 ******/
CREATE NONCLUSTERED INDEX [IX_IndexActiveDynEntities_EntityConfigKeywords] ON [dbo].[IndexActiveDynEntities]
(
	[EntityConfigKeywords] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

