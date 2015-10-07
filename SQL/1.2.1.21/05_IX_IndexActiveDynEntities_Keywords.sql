/****** Object:  Index [IX_IndexActiveDynEntities_Keywords]    Script Date: 11.08.2012 21:48:34 ******/
CREATE NONCLUSTERED INDEX [IX_IndexActiveDynEntities_Keywords] ON [dbo].[IndexActiveDynEntities]
(
	[Keywords] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

