/****** Object:  Table [dbo].[UploadedFile]    Script Date: 29.04.2015 17:39:04 ******/
DROP TABLE [dbo].[UploadedFile]
GO

/****** Object:  Table [dbo].[UploadedFile]    Script Date: 29.04.2015 17:39:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UploadedFile](
	[FileId] [uniqueidentifier] NOT NULL,
	[DynEntityConfigId] [bigint] NOT NULL,
	[DynEntityAttributeConfigId] [bigint] NOT NULL,
	[AssetId] [bigint] NULL,
	[Filename] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime2] NOT NULL DEFAULT GETDATE(), 
 CONSTRAINT [PK_UploadedFile] PRIMARY KEY CLUSTERED 
(
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


