GO
begin tran

DROP TABLE _search_srchTypeContext

CREATE TABLE _search_srchTypeContext (
	SearchId uniqueidentifier NOT NULL,
	DynEntityUid bigint NOT NULL,
	DynEntityConfigUid bigint NOT NULL
)

commit tran

GO