alter table DynEntityConfig
add ParentChildRelations bit null 

GO

update DynEntityConfig set ParentChildRelations=0

GO

alter table DynEntityConfig
alter column ParentChildRelations bit not null

GO

CREATE PROCEDURE [dbo].[_cust_GetChildAssets]
 @assetTypeId bigint
AS
 select dynconf.DynEntityConfigUid as AssetTypeUID, dynconf.DynEntityConfigId as AssetTypeID,dynconf.Name as AssetTypeName,
 dynatr.DynEntityAttribConfigUid as AssetUID, dynatr.Name as AttributeName
  from DynEntityAttribConfig dynatr
 inner join DynEntityConfig dynconf on 
 dynatr.DynEntityConfigUid=dynconf.DynEntityConfigUid
 where dynatr.ActiveVersion=1 and dynatr.IsShownOnPanel=1 and
 dynatr.RelatedAssetTypeID=@assetTypeId
 order by dynconf.Name