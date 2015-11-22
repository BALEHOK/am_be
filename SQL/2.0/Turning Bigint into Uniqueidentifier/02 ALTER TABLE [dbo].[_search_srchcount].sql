GO
begin tran

DROP TABLE _search_srchcount

CREATE TABLE _search_srchcount (
	SearchId uniqueidentifier NULL,
	UserId bigint NULL,
	Type varchar(50) NULL,
	id bigint NULL,
	Count int NULL,
	SearchDateTimeStamp datetime NOT NULL
)

ALTER TABLE [dbo].[_search_srchcount] ADD  DEFAULT (getdate()) FOR [SearchDateTimeStamp]
GO

commit tran

GO