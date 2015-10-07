/****** Object:  Index [IX_TaxonomyItem_TaxonomyItemUid]    Script Date: 11.08.2012 20:28:12 ******/
CREATE NONCLUSTERED INDEX [IX_TaxonomyItem_TaxonomyItemUid] ON [dbo].[TaxonomyItem]
(
	[TaxonomyItemUid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

