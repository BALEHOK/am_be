ALTER TABLE [SearchQueryAttributes]
ADD [ChildAssets] [bit] NULL
GO

UPDATE [SearchQueryAttributes]
SET [ChildAssets] = 0
GO

ALTER TABLE [SearchQueryAttributes]
ALTER COLUMN [ChildAssets] [bit] NOT NULL
GO