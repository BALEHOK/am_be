ALTER PROCEDURE [dbo].[_cust_GetChildAssets]
 @assetTypeId bigint
AS
 select dynconf.DynEntityConfigUid as AssetTypeUID, dynconf.DynEntityConfigId as AssetTypeID,dynconf.Name as AssetTypeName,
 dynatr.DynEntityAttribConfigUid as AssetUID, dynatr.Name as AttributeName
  from DynEntityAttribConfig dynatr
 inner join DynEntityConfig dynconf on 
 dynatr.DynEntityConfigUid=dynconf.DynEntityConfigUid
 where dynatr.ActiveVersion=1 and dynatr.IsShownOnPanel=1 and
 dynatr.RelatedAssetTypeID=@assetTypeId and dynconf.Active=1
 order by dynconf.Name