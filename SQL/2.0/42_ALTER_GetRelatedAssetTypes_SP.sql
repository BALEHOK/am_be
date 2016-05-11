SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[_cust_GetRelatedAssetTypes]
	@userId bigint,
	@assetTypeId bigint
AS
 SELECT 
	  dynconf.DynEntityConfigUid
	 ,dynconf.DynEntityConfigId 
	 ,dynconf.Name as AssetTypeName
	 ,dynatr.DynEntityAttribConfigUid
	 ,dynatr.DynEntityAttribConfigId
	 ,dynatr.Name as AttributeName
 FROM DynEntityAttribConfig dynatr
 INNER JOIN DynEntityConfig dynconf 
	ON dynatr.DynEntityConfigUid=dynconf.DynEntityConfigUid
 INNER JOIN (SELECT DynEntityConfigId FROM [f_GetGrantedConfigIds](@userId, 8)) granted -- all for reading 1000
	ON dynconf.DynEntityConfigId = granted.DynEntityConfigId
 WHERE 
	 dynatr.RelatedAssetTypeID=@assetTypeId and
	 dynatr.ActiveVersion=1 and 
	 dynatr.IsShownOnPanel=1 and	  
	 dynconf.Active=1 and
	 dynatr.DataTypeUid in
	 (
		SELECT DataTypeUid FROM DataType WHERE Name in ('asset', 'assets')
	 )
 ORDER BY dynconf.Name
