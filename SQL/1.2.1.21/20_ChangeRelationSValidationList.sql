IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[ValidationList_DynEntityAttribValidation_FK1]') AND parent_object_id = OBJECT_ID(N'[dbo].[DynEntityAttribValidation]'))
ALTER TABLE [dbo].[DynEntityAttribValidation] DROP CONSTRAINT [ValidationList_DynEntityAttribValidation_FK1]
GO

ALTER TABLE [dbo].[DynEntityAttribValidation]  WITH CHECK ADD  CONSTRAINT [ValidationList_DynEntityAttribValidation_FK1] FOREIGN KEY([ValidationUid])
REFERENCES [dbo].[ValidationList] ([ValidationUid])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[DynEntityAttribValidation] CHECK CONSTRAINT [ValidationList_DynEntityAttribValidation_FK1]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ValidationOperandValue_ValidationList]') AND parent_object_id = OBJECT_ID(N'[dbo].[ValidationOperandValue]'))
ALTER TABLE [dbo].[ValidationOperandValue] DROP CONSTRAINT [FK_ValidationOperandValue_ValidationList]
GO

ALTER TABLE [dbo].[ValidationOperandValue]  WITH CHECK ADD  CONSTRAINT [FK_ValidationOperandValue_ValidationList] FOREIGN KEY([ValidationListUid])
REFERENCES [dbo].[ValidationList] ([ValidationUid])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ValidationOperandValue] CHECK CONSTRAINT [FK_ValidationOperandValue_ValidationList]
GO


