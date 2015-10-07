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
	[Count] [int] NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


CREATE NONCLUSTERED INDEX [IX_1] ON [dbo].[_search_srchcount] 
(
	[SearchId] ASC,
	[UserId] ASC,
	[Type] ASC,
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/**/

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
	[rownumber] [int] NULL
) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_1] ON [dbo].[_search_srchres] 
(
	[SearchId] ASC,
	[UserId] ASC,
	[IndexUid] ASC,
	[Active] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_2] ON [dbo].[_search_srchres] 
(
	[SearchId] ASC,
	[UserId] ASC,
	[rownumber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO