alter table DynEntityAttribConfig
add GUIDisplayOrder int null

GO

update DynEntityAttribConfig 
set GUIDisplayOrder=0

GO

alter table DynEntityAttribConfig
alter column GUIDisplayOrder int not null
