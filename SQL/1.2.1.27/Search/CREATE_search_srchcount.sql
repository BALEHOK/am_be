/****** Object:  Table [dbo].[_search_srchcount]    Script Date: 01/09/2013 08:45:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[_search_srchcount](
	[SearchId] [bigint] NULL,
	[UserId] [bigint] NULL,
	[Type] [varchar](50) NULL,
	[id] [bigint] NULL,
	[Count] [int] NULL,
	[SearchDateTimeStamp] [datetime] NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[_search_srchcount] ADD  DEFAULT (getdate()) FOR [SearchDateTimeStamp]
GO

