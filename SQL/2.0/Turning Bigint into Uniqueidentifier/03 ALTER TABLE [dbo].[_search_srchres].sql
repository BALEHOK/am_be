GO
begin tran

DROP TABLE _search_srchres

CREATE TABLE _search_srchres (
	SearchId uniqueidentifier NULL,
	UserId bigint NULL,
	IndexUid bigint NULL,
	Active bit NULL,
	DynEntityConfigId bigint NULL,
	TaxonomyItemsIds nvarchar(1000) NULL,
	rownumber int NULL,
	SearchDateTimeStamp datetime NOT NULL
)

ALTER TABLE [dbo].[_search_srchres] ADD  DEFAULT (getdate()) FOR [SearchDateTimeStamp]
GO

commit tran

GO