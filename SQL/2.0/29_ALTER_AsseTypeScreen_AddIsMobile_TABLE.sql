SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [AssetTypeScreen]
ADD [IsMobile] bit NULL
GO

ALTER TABLE [AssetTypeScreen] ADD  DEFAULT (0) FOR [IsMobile]
GO

UPDATE [AssetTypeScreen]
SET [IsMobile] = 0
GO

ALTER TABLE [AssetTypeScreen]
ALTER COLUMN [IsMobile] [bit] NOT NULL
GO
