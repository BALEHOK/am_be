/****** Object:  Index [IX_TaxonomyItem_TaxonomyItemIdActiveVersion]    Script Date: 11.08.2012 20:28:51 ******/
CREATE NONCLUSTERED INDEX [IX_TaxonomyItem_TaxonomyItemIdActiveVersion] ON [dbo].[TaxonomyItem]
(
	[TaxonomyItemId] ASC,
	[ActiveVersion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

