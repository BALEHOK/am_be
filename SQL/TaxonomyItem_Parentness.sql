ALTER Table TaxonomyItem ALTER COLUMN ParentTaxonomyItemUid bigint NULL;
UPDATE TaxonomyItem SET ParentTaxonomyItemUid = NULL WHERE ParentTaxonomyItemUid = 0;
ALTER TABLE [dbo].[TaxonomyItem]  WITH CHECK ADD  CONSTRAINT [FK_TaxonomyItem_TaxonomyItem_Parentness] FOREIGN KEY([ParentTaxonomyItemUid])
REFERENCES [dbo].[TaxonomyItem] ([TaxonomyItemUid]);