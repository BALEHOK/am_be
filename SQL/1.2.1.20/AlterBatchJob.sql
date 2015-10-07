alter table BatchJob
add ExecuteOn smallint null

GO

update BatchJob set ExecuteOn=0

GO

alter table BatchJob
alter column ExecuteOn smallint not null  