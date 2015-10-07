ALTER TABLE [dbo].[AssetTypeScreen]  WITH CHECK 
ADD  CONSTRAINT [FK_AssetTypeScreen_DynEntityConfigUid] 
FOREIGN KEY([DynEntityConfigUid])
REFERENCES [dbo].[DynEntityConfig] ([DynEntityConfigUid]);

CREATE NONCLUSTERED INDEX [IX_AssetTypeScreen_DynEntityConfigUid] ON [dbo].[AssetTypeScreen] 
(
	[DynEntityConfigUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
