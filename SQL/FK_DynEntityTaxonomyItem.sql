ALTER TABLE [dbo].[DynEntityTaxonomyItemHistory]  WITH CHECK 
ADD  CONSTRAINT [FK_DynEntityTaxonomyItemHistory_DynEntityConfig] 
FOREIGN KEY([DynEntityConfigUID])
REFERENCES [dbo].[DynEntityConfig] ([DynEntityConfigUid]);

ALTER TABLE [dbo].[DynEntityTaxonomyItem]  WITH CHECK 
ADD  CONSTRAINT [FK_DDynEntityTaxonomyItem_DynEntityConfig] 
FOREIGN KEY([DynEntityConfigUid])
REFERENCES [dbo].[DynEntityConfig] ([DynEntityConfigUid]);

ALTER TABLE [dbo].[DynEntityTaxonomyItem]  WITH CHECK 
ADD  CONSTRAINT [FK_DDynEntityTaxonomyItem_TaxonomyItem] 
FOREIGN KEY([TaxonomyItemUid])
REFERENCES [dbo].[TaxonomyItem] ([TaxonomyItemUid]);