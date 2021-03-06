
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.JSONEncode') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ) ) 
DROP FUNCTION [dbo].[JSONEncode]
GO

CREATE FUNCTION [dbo].[JSONEncode]
(
	@text varchar(MAX)
)
RETURNS varchar(MAX)
AS
BEGIN

	RETURN REPLACE(@text, '"', '\"')

END
GO


ALTER PROCEDURE [dbo].[_cust_ReIndex_BuildJsonObject] 
	@DynEntityAttribConfigUid bigint, 
	@field nvarchar(max) OUTPUT, 
	@from nvarchar(max) OUTPUT,
	@creatett nvarchar(max) OUTPUT, 
	@droptt nvarchar(max) OUTPUT,
	@culture nvarchar(10),
	@delimiter nvarchar(max) = ', ',
	@PrimaryTable nvarchar(max) = 'prim',
	@AssetLinkedTable nvarchar(max) = '' OUTPUT,
	@AssetDynEntityConfigUid bigint OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DynEntityConfigUid bigint;
	DECLARE @DynEntityAttribConfigId bigint;
	DECLARE @ActiveVersion bit;
	DECLARE @DataType varchar(100);
	DECLARE @RelatedAssetTypeID bigint;
	DECLARE @RelatedAssetTypeAttributeID bigint;
	DECLARE @ttname varchar(100);
	DECLARE @ttname2 varchar(100);
	
	DECLARE @TableFieldname varchar(100);
	DECLARE @Name varchar(100);
	DECLARE @dbRefTableName varchar(100);
	DECLARE @dbRefLnkName varchar(20);
	DECLARE @dbRefTableFieldname varchar(100);
	DECLARE @DynListUid bigint;
	DECLARE @linkTablenameId int;
	DECLARE @DynListUids nvarchar(1000);
	
	-- Select datatype and asset/dynlist related ids
	SELECT	@DynEntityConfigUid = DynEntityConfigUid, @DynEntityAttribConfigId = DynEntityAttribConfigId, @ActiveVersion = ActiveVersion,
			@Name = ac.Name, @TableFieldname = DBTableFieldname, @DataType = dt.Name, 
			@RelatedAssetTypeID = ac.RelatedAssetTypeID, @RelatedAssetTypeAttributeID = ac.RelatedAssetTypeAttributeID,
			@DynListUid = DynListUid
	  FROM DynEntityAttribConfig ac
				INNER JOIN DataType dt ON ac.DataTypeUid = dt.DataTypeUid
	 WHERE ac.DynEntityAttribConfigUid = @DynEntityAttribConfigUid
	
	IF LEN(@field) > 0 
	BEGIN
		SET @delimiter = '''' + @delimiter + ''' + ';
		SET @field = @field + ' + ';
	END
	ELSE
		SET @delimiter = '';
	
	-- Object datatypes
	IF @DataType = 'asset' OR @DataType = 'document'
	BEGIN
		-- Get table name and tablefield name for the property
		IF @DataType = 'asset' BEGIN
			SELECT @AssetDynEntityConfigUid = c.DynEntityConfigUid, @dbRefTableName = c.DBTableName, 
				   @dbRefTableFieldname = ac.DBTableFieldname
			  FROM DynEntityConfig c 
						INNER JOIN DynEntityAttribConfig ac ON c.DynEntityConfigUid = ac.DynEntityConfigUid
			 WHERE c.DynEntityConfigId = @RelatedAssetTypeID
			   AND ac.DynEntityAttribConfigId = @RelatedAssetTypeAttributeID
			   AND c.ActiveVersion = 1 AND ac.ActiveVersion = 1
		END
		ELSE BEGIN
			SET @dbRefTableName = 'ADynEntityDocument'
			SET @dbRefTableFieldname = 'Name'
		END
		 
		-- Search in LinkedTables if used before
		SELECT @linkTablenameId = id FROM #tLinkedTables WHERE TableName = @dbRefTableName AND RefDynEntityAttribConfigUid = @DynEntityAttribConfigUid
		
		-- If not found, add it ...
		IF @linkTablenameId IS NULL
		BEGIN
			INSERT #tLinkedTables (TableName, RefDynEntityAttribConfigUid) VALUES (@dbRefTableName, @DynEntityAttribConfigUid)
			SELECT @linkTablenameId = Scope_Identity();
			SET @dbRefLnkName = 'lnk' + CONVERT(varchar(10), @linkTablenameId);
			IF @ActiveVersion = 1
				SET @from = @from + ' LEFT OUTER JOIN ' + @dbRefTableName + ' ' + @dbRefLnkName + ' ON ' + @dbRefLnkName + '.DynEntityId = ' + @PrimaryTable + '.' + @TableFieldname +
										        ' AND ' + @dbRefLnkName + '.ActiveVersion = 1'
			ELSE
				SET @from = @from + ' LEFT OUTER JOIN ' + @dbRefTableName + ' ' + @dbRefLnkName + ' ON ' + @dbRefLnkName + '.DynEntityUid = ' + @PrimaryTable + '.' + @TableFieldname
		END
		ELSE
			SET @dbRefLnkName = 'lnk' + CONVERT(varchar(10), @linkTablenameId);

		SET @field = @field + 'CASE WHEN ' + @dbRefLnkName + '.' + @dbRefTableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: "'' + CONVERT(nvarchar(3000),' + @dbRefLnkName + '.' + @dbRefTableFieldname + ') + ''"}''  END  '
	END
	ELSE IF @DataType = 'assets'
	BEGIN

		-- Get table name and tablefield name for the property
		SELECT @dbRefTableName = c.DBTableName, @dbRefTableFieldname = ac.DBTableFieldname
		  FROM DynEntityConfig c INNER JOIN DynEntityAttribConfig ac ON c.DynEntityConfigUid = ac.DynEntityConfigUid
		 WHERE c.DynEntityConfigId = @RelatedAssetTypeID
		   AND ac.DynEntityAttribConfigId = @RelatedAssetTypeAttributeID
		   AND c.ActiveVersion = 1 AND ac.ActiveVersion = 1
		 
		-- Search in LinkedTables if used before
		SELECT @linkTablenameId = id FROM #tLinkedTables WHERE TableName = @dbRefTableName AND RefDynEntityAttribConfigUid = @DynEntityAttribConfigUid	
			
		-- If not found, add it ...
		IF @linkTablenameId IS NULL
		BEGIN
			INSERT #tLinkedTables (TableName, RefDynEntityAttribConfigUid) VALUES (@dbRefTableName, @DynEntityAttribConfigUid)
			SELECT @linkTablenameId = Scope_Identity();
			SET @ttname = '#t' + CONVERT(varchar(10), @linkTablenameId);
			
			IF @ActiveVersion = 1 BEGIN
				SET @creatett = @creatett + 
					'CREATE TABLE ' + @ttname + 
					'(' +
					'	DynEntityId bigint,' +
					'	Value nvarchar(3000) collate database_default' +
					'); ' +
					'INSERT ' + @ttname + ' ' +
					'SELECT t1.DynEntityId,'+
					'	STUFF' + 
					'	(' +
					'		(' +
					'			SELECT '' '' + ' + @dbRefTableFieldname + 
					'			  FROM ' + @dbRefTableName + ' t2' +
					'			 WHERE t2.ActiveVersion = 1' +
					'			   AND t2.DynEntityId IN (SELECT RelatedDynEntityId FROM MultipleAssetsActive WHERE DynEntityId = t1.DynEntityId)' +
					'			   FOR XML PATH (''''), TYPE' + 
					'		).value(''.'', ''nvarchar(max)''), 1, 1, '''') AS Value FROM MultipleAssetsActive t1 ' + 
					' WHERE DynEntityAttribConfigId = ' + CONVERT(varchar(10), @DynEntityAttribConfigId) + ' GROUP BY t1.DynEntityId; '
			END
			ELSE BEGIN
				SET @creatett = @creatett + 
					'CREATE TABLE ' + @ttname + 
					'(' +
					'	DynEntityUid bigint,' +
					'	Value nvarchar(3000) collate database_default' +
					'); ' +
					'INSERT ' + @ttname + ' ' +
					'SELECT t1.DynEntityUid,'+
					'	STUFF' + 
					'	(' +
					'		(' +
					'			SELECT '' '' + ' + @dbRefTableFieldname + 
					'			  FROM ' + @dbRefTableName + ' t2' +
					'			 WHERE t2.ActiveVersion = 1' +
					'			   AND t2.DynEntityUid IN (SELECT RelatedDynEntityUid FROM MultipleAssetsHistory WHERE DynEntityUid = t1.DynEntityUid)' +
					'			   FOR XML PATH (''''), TYPE' + 
					'		).value(''.'', ''nvarchar(max)''), 1, 1, '''') AS Value FROM MultipleAssetsHistory t1 ' + 
					' WHERE DynEntityAttribConfigUid = ' + CONVERT(varchar(10), @DynEntityAttribConfigUid) + ' GROUP BY t1.DynEntityUid; '
			END
			
			SET @droptt = @droptt + 'DROP TABLE ' + @ttname + '; '
			
			IF @ActiveVersion = 1 BEGIN
				SET @from = @from + ' LEFT OUTER JOIN ' + @ttname + ' ON ' + @ttname + '.DynEntityId = ' + @PrimaryTable + '.DynEntityId '
			END
			ELSE BEGIN
				SET @from = @from + ' LEFT OUTER JOIN ' + @ttname + ' ON ' + @ttname + '.DynEntityUid = ' + @PrimaryTable + '.DynEntityUid '
			END
		END
		ELSE BEGIN
			SET @ttname = '#t' + CONVERT(varchar(10), @linkTablenameId);
		END
		SET @field = @field + ' CASE WHEN ' + @ttname + '.Value IS NULL THEN ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: "'' + ' +  @ttname + '.Value + ''"}'' END '		
	END
	ELSE IF @DataType = 'dynlist' OR @DataType = 'dynlists'
	BEGIN
		-- Search in LinkedTables if used before
		SELECT @linkTablenameId = id FROM #tLinkedTables WHERE TableName = 'vwDynListValue' AND RefDynEntityAttribConfigUid = @DynEntityAttribConfigUid

		-- If not found, add it ...
		IF @linkTablenameId IS NULL
		BEGIN
			INSERT #tLinkedTables (TableName, RefDynEntityAttribConfigUid) VALUES ('vwDynListValue', @DynEntityAttribConfigUid)
			SELECT @linkTablenameId = Scope_Identity();
			SET @ttname = '#t' + CONVERT(varchar(10), @linkTablenameId);
			
			-- Get DynListUids used with this attribute (more than 1 if parent/child lists are used). Insert all DynListItems (with translations) in temp table
			SET @creatett = @creatett + 
				'CREATE TABLE ' + @ttname + '(DynListItemUid bigint, Value nvarchar(3000) collate database_default); ' + 
				'INSERT ' + @ttname + 
				' SELECT DynListItemUid, Value + '' '' + dbo.GetStringResourceValue(Value) FROM DynListItem WHERE DynListUid IN ' + 
				' (SELECT * FROM dbo.GetParentChildDynListUids(' + CONVERT(nvarchar(10), @DynListUid) + ')); '
			SET @droptt = @droptt + 'DROP TABLE ' + @ttname + '; '
			
			-- Extra temp table, to store all the selected DynListItems, grouped by item (AssetUid)
			SET @ttname2 = '#tt' + CONVERT(varchar(10), @linkTablenameId);
			SET @creatett = @creatett + 
				'CREATE TABLE ' + @ttname2 + '(AssetUid bigint, DynEntityConfigUid bigint, DynEntityAttribConfigUid bigint, Value nvarchar(max) collate database_default); ' +
				'INSERT ' + @ttname2 + ' ' + 
				'SELECT AssetUid, DynEntityConfigUid, DynEntityAttribConfigUid, ' +
				  ' STUFF ' +
				  ' ( ' +
					  ' (	SELECT '' '' + Value ' + 
							' FROM ' + @ttname +
						   ' WHERE ' + @ttname + '.DynListItemUid IN (SELECT DynListItemUid FROM DynListValue WHERE AssetUid = lv.AssetUid AND DynEntityAttribConfigUid = lv.DynEntityAttribConfigUid) ' +
							 ' FOR XML PATH (''''), TYPE  ).value(''.'', ''nvarchar(max)''), 1, 1, '''') AS Value ' +
				' FROM DynListValue lv ' +
			   ' WHERE DynEntityAttribConfigUid = ' + CONVERT(nvarchar(10), @DynEntityAttribConfigUid) + 
			   ' GROUP BY AssetUid, DynEntityConfigUid, DynEntityAttribConfigUid; '
			SET @droptt = @droptt + 'DROP TABLE ' + @ttname2 + '; '
			
			SET @from = @from + ' LEFT OUTER JOIN ' + @ttname2 + ' ON ' + @ttname2 + '.AssetUid = ' + @PrimaryTable + '.DynEntityUid' + 
											' AND ' + @ttname2 + '.DynEntityConfigUid = ' + CONVERT(varchar(10), @DynEntityConfigUid) + 
								            ' AND ' + @ttname2 + '.DynEntityAttribConfigUid = ' + CONVERT(varchar(10), @DynEntityAttribConfigUid)
								            
		END
		ELSE BEGIN
			SET @ttname2 = '#tt' + CONVERT(varchar(10), @linkTablenameId);
		END
				
		SET @field = @field + ' CASE WHEN ' + @ttname2 + '.Value IS NULL THEN ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: "'' + dbo.JSONEncode(' +  @ttname2 + '.Value) + ''"}'' END ';
				
	END
	
	-- Normal datatypes
	ELSE IF @DataType = 'datetime' OR @DataType = 'currentdate'
	BEGIN
		IF @culture = 'nl-BE' OR @culture = 'nl-NL' OR @culture = 'fr-FR'
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: "'' + dbo.JSONEncode(CONVERT(nvarchar(20),' + @PrimaryTable + '.' + @TableFieldname + ', 103)) + ''"}''  END  '
		ELSE 
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: "'' + dbo.JSONEncode(CONVERT(nvarchar(20),' + @PrimaryTable + '.' + @TableFieldname + ')) + ''"}''  END  '
	END
	ELSE IF @DataType = 'float'
	BEGIN
		IF @culture = 'nl-BE' OR @culture = 'nl-NL' OR @culture = 'fr-FR'
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: "'' + dbo.JSONEncode(REPLACE(LTRIM(STR(' + @PrimaryTable + '.' + @TableFieldname + ', 10, 2)), ''.'', '','')) + ''"}''  END  '
		ELSE
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: "'' + dbo.JSONEncode(LTRIM(STR(' + @PrimaryTable + '.' + @TableFieldname + ', 10, 2))) + ''"}''  END  ';
	END
	ELSE IF @DataType = 'money' OR @DataType = 'usd' OR @DataType = 'euro'
	BEGIN
		IF @culture = 'nl-BE' OR @culture = 'nl-NL' OR @culture = 'fr-FR'
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: "'' + dbo.JSONEncode(REPLACE(CONVERT(nvarchar(25), ' + @PrimaryTable + '.' + @TableFieldname + ', 0), ''.'', '','')) + ''"}''  END  '
		ELSE
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: "'' + dbo.JSONEncode(CONVERT(nvarchar(25), ' + @PrimaryTable + '.' + @TableFieldname + ', 0)) + ''"}''  END  '
	END
	ELSE IF @DataType = 'richtext'
	BEGIN
		SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: "'' + dbo.JSONEncode(dbo.StripHTML(' + @PrimaryTable + '.' + @TableFieldname + ')) + ''"}''  END  '
	END
	
	-- No special formatting required, just paste the value
	ELSE BEGIN
		SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + dbo.JSONEncode(REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''')) + '", value: "'' + dbo.JSONEncode(CONVERT(nvarchar(1000), ' + @PrimaryTable + '.' + @TableFieldname + ')) + ''"}''  END  '
	END;
	
	-- Return linked table (only valid if datatype is asset)
	SET @AssetLinkedTable = @dbRefLnkName;
		
END


