SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[Taxonomy]
ALTER COLUMN [Name] [nvarchar](255) NOT NULL
GO

ALTER TABLE [dbo].[Taxonomy]
ALTER COLUMN [NameTranslationId] [nvarchar](255) NULL
GO


