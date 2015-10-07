USE [assetmanager]
GO

ALTER TABLE [dbo].[DynListItem]  WITH CHECK ADD  CONSTRAINT [FK_AssociatedDynList_DynListItem] FOREIGN KEY([AssociatedDynListUid])
REFERENCES [dbo].[DynList] ([DynListUid]);

ALTER TABLE [dbo].[DynListItem] CHECK CONSTRAINT [DynList_DynListItem_FK1]
GO


