begin transaction

alter table dbo.DynEntityTransaction
add LocationId bigint

alter table dbo.DynEntityTransaction
add EndUserId bigint

alter table dbo.DynEntityTransaction
add FromLocationId bigint

commit transaction