USE [facilitymanager_facilityflexware_com]
GO

ALTER TABLE [dbo].[AssetTypeScreen] DROP FK_AssetTypeScreen_ScreenLayout
GO
DROP TABLE [dbo].[ScreenLayout]
GO

/****** Object:  Table [dbo].[ScreenLayout]    Script Date: 23.02.2015 18:36:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ScreenLayout](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Type] [int] NOT NULL,
 CONSTRAINT [PK_ScreenLayout] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[ScreenLayout]
  VALUES ('Tabs', 1)
  INSERT INTO [dbo].[ScreenLayout]
  VALUES ('List', 2)
  INSERT INTO [dbo].[ScreenLayout]
  VALUES ('Tiles', 3)
  
UPDATE [dbo].[AssetTypeScreen]
  SET LayoutId = (SELECT TOP 1 Layout.Id FROM [dbo].[ScreenLayout] as Layout)  
  
ALTER TABLE [dbo].[AssetTypeScreen]  WITH CHECK ADD  CONSTRAINT [FK_AssetTypeScreen_ScreenLayout] FOREIGN KEY([LayoutId])
REFERENCES [dbo].[ScreenLayout] ([Id])
GO
