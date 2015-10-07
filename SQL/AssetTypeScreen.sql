USE [assetmanager_1.2.0]
GO

/****** Object:  Table [dbo].[AssetTypeScreen]    Script Date: 07/05/2011 11:04:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AssetTypeScreen](
	[ScreenId] [bigint] IDENTITY(1,1) NOT NULL,
	[DynEntityConfigId] [bigint] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Status] [int] NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Subtitle] [nvarchar](max) NULL,
	[PageText] [nvarchar](max) NULL,
	[Comment] [nvarchar](max) NULL,
	[UpdateUserId] [bigint] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[LayoutId] [int] NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[CheckedDynEntities] [nvarchar](max) NULL,
 CONSTRAINT [PK_AssetTypeScreen] PRIMARY KEY CLUSTERED 
(
	[ScreenId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[AssetTypeScreen]  WITH CHECK ADD  CONSTRAINT [FK_AssetTypeScreen_ScreenLayout] FOREIGN KEY([LayoutId])
REFERENCES [dbo].[ScreenLayout] ([ScreenLayoutId])
GO

ALTER TABLE [dbo].[AssetTypeScreen] CHECK CONSTRAINT [FK_AssetTypeScreen_ScreenLayout]
GO

ALTER TABLE [dbo].[AssetTypeScreen] ADD  CONSTRAINT [DF_AssetTypeScreen_UpdateDate]  DEFAULT (getdate()) FOR [UpdateDate]
GO

ALTER TABLE [dbo].[AssetTypeScreen] ADD  CONSTRAINT [DF_AssetTypeScreen_IsDefault]  DEFAULT ((0)) FOR [IsDefault]
GO


