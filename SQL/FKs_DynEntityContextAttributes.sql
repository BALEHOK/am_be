ALTER TABLE [dbo].[DynEntityContextAttributes]  WITH CHECK 
ADD  CONSTRAINT [FK_DynEntityContextAttributes_DynEntityConfigUid] 
FOREIGN KEY([DynEntityConfigUid])
REFERENCES [dbo].[DynEntityConfig] ([DynEntityConfigUid])
ON DELETE CASCADE;

ALTER TABLE [dbo].[DynEntityContextAttributes]  WITH CHECK 
ADD  CONSTRAINT [FK_DynEntityContextAttributes_DynEntityAttribUid] 
FOREIGN KEY([DynEntityAttribUid])
REFERENCES [dbo].[DynEntityAttribConfig] ([DynEntityAttribConfigUid]);

ALTER TABLE [dbo].[DynEntityContextAttributes]  WITH CHECK 
ADD  CONSTRAINT [FK_DynEntityContextAttributes_ContextId] 
FOREIGN KEY([ContextId])
REFERENCES [dbo].[Context] ([ContextId])
ON DELETE CASCADE;