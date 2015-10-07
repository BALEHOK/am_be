/****** Object:  Table [dbo].[_search_srchres]    Script Date: 01/09/2013 08:46:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[_search_srchres](
	[SearchId] [bigint] NULL,
	[UserId] [bigint] NULL,
	[IndexUid] [bigint] NULL,
	[Active] [bit] NULL,
	[DynEntityConfigId] [bigint] NULL,
	[TaxonomyItemsIds] [nvarchar](1000) NULL,
	[rownumber] [int] NULL,
	[SearchDateTimeStamp] [datetime] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[_search_srchres] ADD  DEFAULT (getdate()) FOR [SearchDateTimeStamp]
GO

