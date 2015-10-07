TRUNCATE TABLE DynEntityContextAttributesValues;
ALTER TABLE DynEntityContextAttributesValues DROP CONSTRAINT [FK_DynEntityContextAttributesValues_DynEntityContextAttributes];
ALTER TABLE DynEntityContextAttributesValues DROP COLUMN DynEntityContextAttributesId;
ALTER TABLE DynEntityContextAttributesValues ADD ContextId bigint NOT NULL;
DROP TABLE DynEntityContextAttributes;
ALTER TABLE [dbo].[DynEntityContextAttributesValues]  WITH CHECK ADD  CONSTRAINT [FK_DynEntityContextAttributesValues_Context] FOREIGN KEY([ContextId])
REFERENCES [dbo].[Context] ([ContextId]);
ALTER TABLE [dbo].[DynEntityContextAttributesValues] CHECK CONSTRAINT [FK_DynEntityContextAttributesValues_Context];
ALTER TABLE DynEntityContextAttributesValues ADD DynEntityUid bigint NOT NULL;
ALTER TABLE DynEntityContextAttributesValues ADD DynEntityConfigUid bigint NOT NULL;
ALTER TABLE DynEntityContextAttributesValues ADD IsActive bit NOT NULL;
ALTER TABLE [dbo].[DynEntityContextAttributesValues]  WITH CHECK ADD  CONSTRAINT [FK_DynEntityContextAttributesValues_DynEntityConfig] FOREIGN KEY([DynEntityConfigUid])
REFERENCES [dbo].[DynEntityConfig] ([DynEntityConfigUid]);
ALTER TABLE [dbo].[DynEntityContextAttributesValues] CHECK CONSTRAINT [FK_DynEntityContextAttributesValues_DynEntityConfig];
ALTER TABLE [dbo].[DynEntityContextAttributesValues]  WITH CHECK ADD  CONSTRAINT [FK_DynEntityContextAttributesValues_DynListItem] FOREIGN KEY([DynamicListItemUid])
REFERENCES [dbo].[DynListItem] ([DynListItemUid]);
ALTER TABLE [dbo].[DynEntityContextAttributesValues] CHECK CONSTRAINT [FK_DynEntityContextAttributesValues_DynListItem];