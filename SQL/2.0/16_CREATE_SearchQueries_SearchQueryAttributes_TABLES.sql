SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.SearchQueryAttributes') AND type IN (N'U')) 
DROP TABLE [dbo].[SearchQueryAttributes]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.SearchQueries') AND type IN (N'U')) 
DROP TABLE [dbo].[SearchQueries]
GO

CREATE TABLE [dbo].[SearchQueries](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[SearchId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[AssetTypeId] [bigint] NOT NULL,
	[Context] [tinyint] NOT NULL,
	[Created] [datetime] NOT NULL CONSTRAINT [DF_SearchQueries_Created]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_SearchQueries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[SearchQueryAttributes](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[SearchQueryId] [bigint] NULL,
	[ParentId] [bigint] NULL,
	[Parenthesis] [tinyint] NULL,
	[LogicalOperator] [tinyint] NOT NULL,
	[OperatorId] [int] NULL,
	[ValueLabel] [nvarchar](max) NULL,
	[Value] [nvarchar](max) NULL,
	[ReferencedAttributeId] [bigint] NOT NULL,
	[Index] [int] NOT NULL,
 CONSTRAINT [PK_SearchQueryAttributes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[SearchQueryAttributes]  WITH CHECK ADD  CONSTRAINT [FK_SearchQueryAttributes_SearchQueries] FOREIGN KEY([SearchQueryId])
REFERENCES [dbo].[SearchQueries] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SearchQueryAttributes] CHECK CONSTRAINT [FK_SearchQueryAttributes_SearchQueries]
GO

ALTER TABLE [dbo].[SearchQueryAttributes]  WITH CHECK ADD  CONSTRAINT [FK_SearchQueryAttributes_SearchQueryAttributes] FOREIGN KEY([ParentId])
REFERENCES [dbo].[SearchQueryAttributes] ([Id])
GO

ALTER TABLE [dbo].[SearchQueryAttributes] CHECK CONSTRAINT [FK_SearchQueryAttributes_SearchQueryAttributes]
GO
