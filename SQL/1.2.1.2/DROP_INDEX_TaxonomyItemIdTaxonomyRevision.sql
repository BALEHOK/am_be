
/****** Object:  Index [TaxonomyItemIdTaxonomyRevision]    Script Date: 01/29/2012 14:08:48 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[TaxonomyItem]') AND name = N'TaxonomyItemIdTaxonomyRevision')
DROP INDEX [TaxonomyItemIdTaxonomyRevision] ON [dbo].[TaxonomyItem] WITH ( ONLINE = OFF )
GO


