CREATE TABLE [dbo].[DynEntityAttribScreens](
	[DynEntityAttribScreensId] [bigint] IDENTITY(1,1) NOT NULL,
	[DynEntityAttribUid] [bigint] NOT NULL,
	[ScreenId] [bigint] NOT NULL,
 CONSTRAINT [PK_DynEntityAttribScreens_1] PRIMARY KEY CLUSTERED 
(
	[DynEntityAttribScreensId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DynEntityAttribScreens]  WITH CHECK ADD  CONSTRAINT [FK_DynEntityAttribScreens_DynEntityAttribUid] FOREIGN KEY([DynEntityAttribUid])
REFERENCES [dbo].[DynEntityAttribConfig] ([DynEntityAttribConfigUid])
GO

ALTER TABLE [dbo].[DynEntityAttribScreens] CHECK CONSTRAINT [FK_DynEntityAttribScreens_DynEntityAttribUid]
GO

ALTER TABLE [dbo].[DynEntityAttribScreens]  WITH CHECK ADD  CONSTRAINT [FK_DynEntityAttribScreens_ScreenId] FOREIGN KEY([ScreenId])
REFERENCES [dbo].[AssetTypeScreen] ([ScreenId])
ON DELETE CASCADE;
GO

ALTER TABLE [dbo].[DynEntityAttribScreens] CHECK CONSTRAINT [FK_DynEntityAttribScreens_ScreenId]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DynEntityAttribScreens_DynEntityAttribUid_ScreenId] ON [dbo].[DynEntityAttribScreens] 
(
	[DynEntityAttribUid] ASC,
	[ScreenId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DynEntityAttribScreens_ScreenId_DynEntityAttribUid] ON [dbo].[DynEntityAttribScreens] 
(
	[ScreenId] ASC,
	[DynEntityAttribUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO