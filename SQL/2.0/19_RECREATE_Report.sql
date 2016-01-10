DROP TABLE [dbo].[ReportField]
GO

DROP TABLE [dbo].[Report]
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Report](
	[ReportUid] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[IsFinancial] [bit] NOT NULL,
	[DynEntityConfigId] [bigint] NULL,
	[Type] [int] NOT NULL,
	[LayoutData] varbinary(MAX) NULL,	
 CONSTRAINT [PK_Report] PRIMARY KEY CLUSTERED 
(
	[ReportUid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Report] ADD  DEFAULT ((0)) FOR [IsFinancial]
GO

ALTER TABLE [dbo].[Report] ADD  DEFAULT ((0)) FOR [Type]
GO
