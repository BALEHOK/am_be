/****** Object:  Index [IX_Taxonomy_TaxonomyId_ActiveVersion]    Script Date: 01/08/2012 01:22:04 ******/
CREATE NONCLUSTERED INDEX [IX_Taxonomy_TaxonomyId_ActiveVersion] ON [dbo].[Taxonomy] 
(
	[TaxonomyId] ASC,
	[ActiveVersion] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


