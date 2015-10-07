/****** Object:  Index [TaxonomyItemIdTaxonomyRevision]    Script Date: 01/08/2012 02:27:28 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[TaxonomyItem]') AND name = N'TaxonomyItemIdTaxonomyRevision')
DROP INDEX [TaxonomyItemIdTaxonomyRevision] ON [dbo].[TaxonomyItem] WITH ( ONLINE = OFF )
GO

UPDATE [dbo].[Taxonomy]
   SET [ActiveVersion] = 0
   WHERE [TaxonomyUid] = 53 OR [TaxonomyUid] = 76
GO



