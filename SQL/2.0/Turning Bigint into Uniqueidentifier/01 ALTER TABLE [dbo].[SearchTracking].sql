GO
begin tran

DROP TABLE SearchTracking

CREATE TABLE SearchTracking (
	Id	bigint NOT NULL Primary Key IDENTITY(1,1),
	SearchType smallint NOT NULL,
	Parameters xml NOT NULL,
	UpdateUser bigint NOT NULL,
	UpdateDate datetime NOT NULL,
	VerboseString nvarchar(MAX) NOT NULL,
	SearchId uniqueidentifier NOT NULL
)

CREATE UNIQUE NONCLUSTERED INDEX [IX_SearchTracking_SearchId] ON [dbo].[SearchTracking]
(
	[SearchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

commit tran

GO