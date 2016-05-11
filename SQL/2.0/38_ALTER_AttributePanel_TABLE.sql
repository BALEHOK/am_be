SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ALTER [AttributePanel]
ALTER TABLE [dbo].[AttributePanel]
ADD [IsChildAssets] [bit] NULL CONSTRAINT [DF_AttributePanel_IsChildAssets]  DEFAULT ((0))
GO

UPDATE [AttributePanel]
SET [IsChildAssets] = 0
WHERE [IsChildAssets] IS NULL
GO

ALTER TABLE [dbo].[AttributePanel]
ALTER COLUMN [IsChildAssets] [bit] NOT NULL
GO

ALTER TABLE [dbo].[AttributePanel]
ADD [ChildAssetAttrId] [bigint] NULL
GO

