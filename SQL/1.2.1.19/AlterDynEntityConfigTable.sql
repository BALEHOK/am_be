ALTER TABLE DynEntityConfig DROP CONSTRAINT DF_DynEntityConfig_AutoGenerateName;

GO

alter table DynEntityConfig
alter column AutoGenerateName int not null

GO

ALTER TABLE [dbo].[DynEntityConfig] ADD  
CONSTRAINT [DF_DynEntityConfig_AutoGenerateName]  DEFAULT ((0)) FOR [AutoGenerateName]