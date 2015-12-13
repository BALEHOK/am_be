SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.GetStringResourceValue2') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ) ) 
DROP FUNCTION [dbo].[GetStringResourceValue2]
GO

CREATE FUNCTION [dbo].[GetStringResourceValue2]
(
	@ResourceKey nvarchar(128),
	@CultureName char(5)
)
RETURNS nvarchar(4000)
AS
BEGIN
	DECLARE @Result nvarchar(3000) = '';
	
	SELECT @result = 
	(
		SELECT ResourceValue + ' '
		  FROM StringResources
		 WHERE CultureName=@CultureName AND ResourceKey = @ResourceKey
		 FOR XML PATH(''), TYPE
	).value('.', 'nvarchar(max)')
 
	-- Return the result of the function
	RETURN RTRIM(ISNULL(@Result, @ResourceKey))

END
GO

-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 31/01/2013
-- Description:	
-- =============================================
ALTER FUNCTION [dbo].[GetStringResourceValue]
(
	@ResourceKey nvarchar(128)
)
RETURNS nvarchar(4000)
AS
BEGIN
	RETURN [dbo].[GetStringResourceValue2](@ResourceKey,'en-US')
END
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo._cust_ReIndex_BuildJsonObject') AND type IN ( N'P', N'PC' ) ) 
DROP PROCEDURE [dbo].[_cust_ReIndex_BuildJsonObject]
GO

CREATE PROCEDURE [dbo].[_cust_ReIndex_BuildJsonObject] 
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
				
		SET @field = @field + ' CASE WHEN ' + @ttname2 + '.Value IS NULL THEN ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: "'' + ' +  @ttname2 + '.Value + ''"}'' END ';
				
	END
	
	-- Normal datatypes
	ELSE IF @DataType = 'datetime' OR @DataType = 'currentdate'
	BEGIN
		IF @culture = 'nl-BE' OR @culture = 'nl-NL' OR @culture = 'fr-FR'
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: "'' + CONVERT(nvarchar(20),' + @PrimaryTable + '.' + @TableFieldname + ', 103) + ''"}''  END  '
		ELSE 
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: "'' + CONVERT(nvarchar(20),' + @PrimaryTable + '.' + @TableFieldname + ') + ''"}''  END  '
	END
	ELSE IF @DataType = 'float'
	BEGIN
		IF @culture = 'nl-BE' OR @culture = 'nl-NL' OR @culture = 'fr-FR'
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: "'' + REPLACE(LTRIM(STR(' + @PrimaryTable + '.' + @TableFieldname + ', 10, 2)), ''.'', '','') + ''"}''  END  '
		ELSE
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: "'' + LTRIM(STR(' + @PrimaryTable + '.' + @TableFieldname + ', 10, 2)) + ''"}''  END  ';
	END
	ELSE IF @DataType = 'money' OR @DataType = 'usd' OR @DataType = 'euro'
	BEGIN
		IF @culture = 'nl-BE' OR @culture = 'nl-NL' OR @culture = 'fr-FR'
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: "'' + REPLACE(CONVERT(nvarchar(25), ' + @PrimaryTable + '.' + @TableFieldname + ', 0), ''.'', '','') + ''"}''  END  '
		ELSE
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: "'' + CONVERT(nvarchar(25), ' + @PrimaryTable + '.' + @TableFieldname + ', 0) + ''"}''  END  '
	END
	ELSE IF @DataType = 'richtext'
	BEGIN
		SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: "'' + dbo.StripHTML(' + @PrimaryTable + '.' + @TableFieldname + ') + ''"}''  END  '
	END
	
	-- No special formatting required, just paste the value
	ELSE BEGIN
		SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: ""}'' ELSE ' + @delimiter + ' ''{ key: "' + REPLACE([dbo].[GetStringResourceValue2](@Name, @culture), '''', '''''') + '", value: "'' + CONVERT(nvarchar(1000), ' + @PrimaryTable + '.' + @TableFieldname + ') + ''"}''  END  '
	END;
	
	-- Return linked table (only valid if datatype is asset)
	SET @AssetLinkedTable = @dbRefLnkName;
		
END

GO
IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo._cust_Reindex_BuildIndexField_ForDisplayValues') AND type IN ( N'P', N'PC' ) ) 
DROP PROCEDURE [dbo].[_cust_Reindex_BuildIndexField_ForDisplayValues]
GO

CREATE PROCEDURE [dbo].[_cust_Reindex_BuildIndexField_ForDisplayValues] 
	-- Add the parameters for the stored procedure here
	@DynEntityConfigUid bigint,
	@FieldType int = 0,
	@field nvarchar(max) OUTPUT, 
	@from nvarchar(max) OUTPUT,
	@creatett nvarchar(max) OUTPUT, 
	@droptt nvarchar(max) OUTPUT,
	@culture nvarchar(10),
	@delimiter nvarchar(max) = ', ',
	@PrimaryTable nvarchar(max) = 'prim'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @DISPLAYVALUES int = 5;
	DECLARE @DISPLAYEXTVALUES int = 6;

	DECLARE @DynEntityAttribConfigUid bigint;	
	DECLARE @DataType nvarchar(60);
	DECLARE @tAttributes TABLE (DynEntityAttribConfigUid bigint, DataType nvarchar(60));
	DECLARE @AssetLinkedTable varchar(100);
	DECLARE @AssetDynEntityConfigUid bigint;
	
		
	-- Build Dynamic SQL
	INSERT @tAttributes
	SELECT ac.DynEntityAttribConfigUid, dt.Name
	  FROM DynEntityAttribConfig ac
		INNER JOIN DataType dt ON dt.DataTypeUid = ac.DataTypeUid
     WHERE ac.DynEntityConfigUid = @DynEntityConfigUid
	   AND (((@FieldType = @DISPLAYVALUES) AND (DisplayOnResultList = 1)) 
	   OR  ((@FieldType = @DISPLAYEXTVALUES) AND (DisplayOnExtResultList = 1)))
	 ORDER BY CASE @FieldType WHEN @DISPLAYVALUES THEN DisplayOrderResultList WHEN @DISPLAYEXTVALUES THEN DisplayOrderExtResultList ELSE 1 END;
	 	   

	WHILE EXISTS (SELECT 1 FROM @tAttributes)
	BEGIN
		SELECT TOP 1 @DynEntityAttribConfigUid = DynEntityAttribConfigUid, @DataType = DataType FROM @tAttributes
				
		EXEC _cust_ReIndex_BuildJsonObject @DynEntityAttribConfigUid, @field OUTPUT, @from OUTPUT, @creatett OUTPUT, @droptt OUTPUT,
									@culture, @delimiter, @PrimaryTable, @AssetLinkedTable OUTPUT, @AssetDynEntityConfigUid OUTPUT;			
			
		DELETE FROM @tAttributes WHERE DynEntityAttribConfigUid = @DynEntityAttribConfigUid
	END;

	set @field = '''['' + ' + @field + ' + '']'''
END

GO

-- =============================================
-- Author:		Steegmans Wouter
-- Create date: 25/01/2013
-- Description:	Reindex items of certain asset/item type
-- =============================================
ALTER PROCEDURE [dbo].[_cust_ReIndex]
	-- Add the parameters for the stored procedure here
	@DynEntityConfigId bigint = NULL, 
	@active bit = 1,
	@buildDynEntityIndex bit = 0,
	@culture nvarchar(10) = 'nl-BE',
	@entities DynEntityIdsTableType READONLY	-- table variable
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @rowcountEntities int = 0;
	DECLARE @rowcount int;
	DECLARE @IsNormalAssetType bit = 0;			-- if not Normal -> Data Asset Type
	DECLARE @TableFieldname varchar(100);
	DECLARE @tConfigUids TABLE (DynEntityConfigUid bigint);

	DECLARE @EntityConfigKeywords nvarchar(max);
	DECLARE @CategoryKeywords nvarchar(max);
	DECLARE @TaxonomyKeywords nvarchar(max);
	DECLARE @DynEntityConfigUid bigint;
	DECLARE @vDynEntityConfigId bigint;
	DECLARE @DynEntityAttribConfigUid bigint;
	DECLARE @CategoryUids nvarchar(max);
	DECLARE @TaxonomyUids nvarchar(max);
	DECLARE @TaxonomyItemsIds nvarchar(max);

	DECLARE @sql nvarchar(max);
	DECLARE @from nvarchar(max);
	DECLARE @creatett nvarchar(max);
	DECLARE @droptt nvarchar(max);
	DECLARE @description nvarchar(max);
	DECLARE @keywords nvarchar(max);
	DECLARE @AllAttrib2IndexValues nvarchar(max);
	DECLARE @AllAttribValues nvarchar(max);
	DECLARE @AllContextAttribValues nvarchar(max);
	DECLARE @DisplayValues nvarchar(max);
	DECLARE @DisplayExtValues nvarchar(max);	
	DECLARE @User nvarchar(max);
	DECLARE @UserId nvarchar(max);
	DECLARE @LocationUid nvarchar(max);
	DECLARE @Location nvarchar(max);
	DECLARE @OwnerId nvarchar(max);
	DECLARE @DepartmentId nvarchar(max);
	DECLARE @Department nvarchar(max);
		
	DECLARE @linkTablenameId nvarchar(max);
	
	CREATE TABLE #tLinkedTables
	(
		Id int identity(1,1),
		Tablename varchar(100),
		RefDynEntityAttribConfigUid bigint
	)

	SELECT @rowcountEntities = COUNT(1) FROM @entities;
		
	-- Check if DynEntityConfigId is set, OR @entities is filled. Both at the same time isn't allowed.
	IF ISNULL(@DynEntityConfigId, 0) > 0 AND @rowcountEntities > 0 BEGIN
		RAISERROR (	N'Parameter DynEntityConfigId and entities can''t be used both at the same time. Only set one parameter.', -- Message text.
					15, -- Severity.
					1-- State.
				   );		
	END
	
	-- Check if history isn't combined with rebuild DynEntityIndex (active)
	IF @active = 0 AND @buildDynEntityIndex = 1 BEGIN
		RAISERROR (	N'DynEntityIndex can''t be rebuild when indexing history items. Change one of both parameters (@active, @buildDynEntityIndex).', -- Message text.
					15, -- Severity.
					1-- State.
				   );			
	END

	-- DELETE entries in index tables
	IF @active = 1
	BEGIN
		IF ISNULL(@DynEntityConfigId, 0) > 0 BEGIN
			DELETE FROM IndexActiveDynEntities 
			 WHERE DynEntityConfigId = @DynEntityConfigId
		END
		
		IF @rowcountEntities > 0 BEGIN
			DELETE IndexActiveDynEntities 
			  FROM @entities e
						INNER JOIN DynEntityConfig c ON c.DynEntityConfigUid = e.DynEntityConfigUid
						INNER JOIN IndexActiveDynEntities ae ON ae.DynEntityUid = e.DynEntityUid
						       AND ae.DynEntityConfigId = c.DynEntityConfigId
		END
	END
	ELSE BEGIN
		IF ISNULL(@DynEntityConfigId, 0) > 0 BEGIN
			DELETE FROM IndexHistoryDynEntities 
			 WHERE DynEntityConfigId = @DynEntityConfigId
		END
		
		IF @rowcountEntities > 0 BEGIN
			DELETE IndexHistoryDynEntities 
			  FROM @entities e
						INNER JOIN DynEntityConfig c ON c.DynEntityConfigUid = e.DynEntityConfigUid
						INNER JOIN IndexHistoryDynEntities ae ON ae.DynEntityUid = e.DynEntityUid
						       AND ae.DynEntityConfigId = c.DynEntityConfigId
		END
	END
	
	IF @buildDynEntityIndex = 1
	BEGIN
		IF ISNULL(@DynEntityConfigId, 0) > 0 BEGIN
			DELETE FROM DynEntityIndex 
			 WHERE DynEntityConfigId = @DynEntityConfigId
		END
		IF @rowcountEntities > 0 BEGIN
			DELETE DynEntityIndex
			  FROM DynEntityIndex ei
						INNER JOIN DynEntityConfig c ON ei.DynEntityConfigId = c.DynEntityConfigId
						INNER JOIN @entities e ON e.DynEntityId = ei.DynEntityId
						       AND e.DynEntityConfigUid = c.DynEntityConfigUid
						
		END
	END

	-- Get ConfigUids (only > 1 if history or different configs in @entities)
	IF ISNULL(@DynEntityConfigId, 0) > 0 BEGIN
		INSERT @tConfigUids
		SELECT DynEntityConfigUid FROM DynEntityConfig
		 WHERE DynEntityConfigId = @DynEntityConfigId
		   AND ((ActiveVersion = @active) OR (@active = 0))
	END
	IF @rowcountEntities > 0 BEGIN
		INSERT @tConfigUids
		SELECT DISTINCT DynEntityConfigUid FROM @entities
	END
	   
	WHILE EXISTS(SELECT 1 FROM @tConfigUids)
	BEGIN		
		-- Initialize variables
		SET @EntityConfigKeywords = '';
		SET @CategoryKeywords = '';				
		SET @TaxonomyKeywords = '';
		SET @CategoryUids = '';
		SET @TaxonomyUids = '';
		SET @TaxonomyItemsIds = '';		
		SET @creatett = '';
		SET @droptt = '';
		SET @description = '';
		SET @keywords = '';	   
		SET @AllAttrib2IndexValues = '';
		SET @AllAttribValues = '';
		SET @AllContextAttribValues = '';
		SET @DisplayValues = ''
		SET @DisplayExtValues = '';	
		SET @User = '''''';
		SET @UserId = '''''';
		SET @LocationUid = '''0''';
		SET @Location = '''''';
		SET @OwnerId = '''''';
		SET @DepartmentId = '''0''';
		SET @Department = '''''';
		
		-- Get constant values, linked to asset/item type
		SELECT TOP 1 @DynEntityConfigUid = tc.DynEntityConfigUid, @vDynEntityConfigId = c.DynEntityConfigId
		  FROM @tConfigUids tc
					INNER JOIN DynEntityConfig c ON tc.DynEntityConfigUid = c.DynEntityConfigUid
		
		-- Check if item/asset type is normal/data
		SET @IsNormalAssetType = dbo.IsNormalAssetType(@vDynEntityConfigId);		
		
		SELECT @TableFieldname = DBTableName FROM DynEntityConfig WHERE DynEntityConfigUid = @DynEntityConfigUid;
		
		-- Check if history and no records exists with this particular configuration, continue ... 
		-- This because sometimes, physical fields from old configurations are removed from the SQL Table, giving errors ...
		IF @active = 0 BEGIN
			SET @sql = 'SELECT @rowcount = COUNT(1) FROM ' + @TableFieldname + ' WHERE DynEntityConfigUid = @DynEntityConfigUid; '
			EXEC sp_executesql @sql, N'@DynEntityConfigUid bigint, @rowcount int OUTPUT', @DynEntityConfigUid, @rowcount OUTPUT
			IF @rowcount = 0 BEGIN
				DELETE FROM @tConfigUids WHERE DynEntityConfigUid = @DynEntityConfigUid;
				CONTINUE
			END
		END;
		
		-- Get Entity Config Keywords 
		WITH t1 (Name) AS
		(	SELECT Name FROM DynEntityConfig WHERE DynEntityConfigUid = @DynEntityConfigUid	)
		SELECT @EntityConfigKeywords = LTRIM(RTRIM(@EntityConfigKeywords + ' ' + Name + ' ' + dbo.GetStringResourceValue(Name))) FROM t1
		
		-- Actual items
		IF @active = 1
		BEGIN
			-- Get Category + Taxonomy Keywords
			WITH t1 (Name, CategoryUid) AS
			(	SELECT ti.Name, ti.TaxonomyItemUid FROM TaxonomyItem ti
								INNER JOIN (SELECT DISTINCT DynEntityConfigUid, TaxonomyItemUid 
								              FROM DynEntityTaxonomyItem 
								             WHERE DynEntityConfigUid = @DynEntityConfigUid) dti
									   ON dti.TaxonomyItemUid = ti.TaxonomyItemUid
								INNER JOIN Taxonomy t ON t.TaxonomyUid = ti.TaxonomyUid
								       AND t.IsCategory = 1
			)
			SELECT @CategoryKeywords = LTRIM(RTRIM(@CategoryKeywords + ' ' + Name + ' ' + dbo.GetStringResourceValue(Name)))FROM t1;

			WITH t1 (Name, TaxonomyItemId) AS
			(	SELECT ti.Name, ti.TaxonomyItemId FROM TaxonomyItem ti
								INNER JOIN (SELECT DISTINCT DynEntityConfigUid, TaxonomyItemUid 
								              FROM DynEntityTaxonomyItem 
								             WHERE DynEntityConfigUid = @DynEntityConfigUid) dti
									   ON dti.TaxonomyItemUid = ti.TaxonomyItemUid
			)
			SELECT @TaxonomyKeywords = LTRIM(RTRIM(@TaxonomyKeywords + ' ' + Name + ' ' + dbo.GetStringResourceValue(Name))), 
			       @TaxonomyItemsIds = LTRIM(RTRIM(@TaxonomyItemsIds + ' ' + CONVERT(varchar(20), TaxonomyItemId))) FROM t1

			SELECT @CategoryUids = @CategoryUids + CASE t.IsCategory WHEN 1 THEN ' ' + CONVERT(nvarchar(10), t.TaxonomyUid) ELSE '' END, 
			       @TaxonomyUids = @TaxonomyUids + ' ' + CONVERT(nvarchar(10), t.TaxonomyUid)
			  FROM Taxonomy t
						INNER JOIN TaxonomyItem ti ON ti.TaxonomyUid = t.TaxonomyUid
						INNER JOIN (SELECT DISTINCT DynEntityConfigUid, TaxonomyItemUid 
						              FROM DynEntityTaxonomyItem 
						             WHERE DynEntityConfigUid = @DynEntityConfigUid) dti
							   ON dti.TaxonomyItemUid = ti.TaxonomyItemUid
		END
		
		-- Archive (History)
		ELSE BEGIN
			WITH t1 (Name, CategoryUid) AS
			(	SELECT ti.Name, ti.TaxonomyItemUid FROM TaxonomyItem ti
								INNER JOIN (SELECT DISTINCT DynEntityConfigUid, TaxonomyItemUid 
								              FROM DynEntityTaxonomyItemHistory
								             WHERE DynEntityConfigUid = @DynEntityConfigUid) dti
									   ON dti.TaxonomyItemUid = ti.TaxonomyItemUid
								INNER JOIN Taxonomy t ON t.TaxonomyUid = ti.TaxonomyUid
								       AND t.IsCategory = 1
			)
			SELECT @CategoryKeywords = LTRIM(RTRIM(@CategoryKeywords + ' ' + Name + ' ' + dbo.GetStringResourceValue(Name)))FROM t1;

			WITH t1 (Name, TaxonomyItemId) AS
			(	SELECT ti.Name, ti.TaxonomyItemId FROM TaxonomyItem ti
								INNER JOIN (SELECT DISTINCT DynEntityConfigUid, TaxonomyItemUid 
								              FROM DynEntityTaxonomyItemHistory
								             WHERE DynEntityConfigUid = @DynEntityConfigUid) dti
									   ON dti.TaxonomyItemUid = ti.TaxonomyItemUid
			)
			SELECT @TaxonomyKeywords = LTRIM(RTRIM(@TaxonomyKeywords + ' ' + Name + ' ' + dbo.GetStringResourceValue(Name))), 
			       @TaxonomyItemsIds = LTRIM(RTRIM(@TaxonomyItemsIds + ' ' + CONVERT(varchar(20), TaxonomyItemId))) FROM t1

			SELECT @CategoryUids = @CategoryUids + CASE t.IsCategory WHEN 1 THEN ' ' + CONVERT(nvarchar(10), t.TaxonomyUid) ELSE '' END, 
			       @TaxonomyUids = @TaxonomyUids + ' ' + CONVERT(nvarchar(10), t.TaxonomyUid)
			  FROM Taxonomy t
						INNER JOIN TaxonomyItem ti ON ti.TaxonomyUid = t.TaxonomyUid
						INNER JOIN (SELECT DISTINCT DynEntityConfigUid, TaxonomyItemUid 
						              FROM DynEntityTaxonomyItemHistory
						             WHERE DynEntityConfigUid = @DynEntityConfigUid) dti
							   ON dti.TaxonomyItemUid = ti.TaxonomyItemUid			
		END

		-- Build Dynamic SQL (init)
		SET @from = @TableFieldname + ' prim';

		-- Build Dynamic SQL (Description field)
		EXEC _cust_Reindex_BuildIndexField  @DynEntityConfigUid, 1, 0, @description OUTPUT, @from OUTPUT, @creatett OUTPUT, @droptt OUTPUT,
											@culture, ' ', 'prim';

		-- Build Dynamic SQL (Keywords field)
		EXEC _cust_Reindex_BuildIndexField  @DynEntityConfigUid, 2, 0, @keywords OUTPUT, @from OUTPUT, @creatett OUTPUT, @droptt OUTPUT,
											@culture, ' ', 'prim';
		
		-- Build Dynamic SQL (IsFullTextInidex field)
		EXEC _cust_Reindex_BuildIndexField  @DynEntityConfigUid, 3, 0, @AllAttrib2IndexValues OUTPUT, @from OUTPUT, @creatett OUTPUT, @droptt OUTPUT,
											@culture, ' ', 'prim';

		-- Build Dynamic SQL (AllAttribValues field)
		EXEC _cust_Reindex_BuildIndexField  @DynEntityConfigUid, 4, 3, @AllAttribValues OUTPUT, @from OUTPUT, @creatett OUTPUT, @droptt OUTPUT,
											@culture, ' ', 'prim';
		
		-- Build Dynamic SQL (DisplayValues field)
		EXEC _cust_Reindex_BuildIndexField_ForDisplayValues  @DynEntityConfigUid, 5, @DisplayValues OUTPUT, @from OUTPUT, @creatett OUTPUT, @droptt OUTPUT,
											@culture, ', ', 'prim';
		
		-- Build Dynamic SQL (DisplayExtValues field)
		EXEC _cust_Reindex_BuildIndexField_ForDisplayValues  @DynEntityConfigUid, 6, @DisplayExtValues OUTPUT, @from OUTPUT, @creatett OUTPUT, @droptt OUTPUT,
											@culture, ', ', 'prim';
											
		-- Add location (if asset type is normal)
		IF @IsNormalAssetType = 1 BEGIN
			-- Search linked Location table
			SELECT @linkTablenameId = 'lnk' + CONVERT(nvarchar(10), id)
			  FROM #tLinkedTables t
						INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityAttribConfigUid = t.RefDynEntityAttribConfigUid
			 WHERE ac.Name = 'Location'
			
			SET @LocationUid = 'ISNULL(' + @linkTablenameId + '.DynEntityUid, 0)'
			SET @Location = @linkTablenameId + '.Name'
		END
		
		-- Add Department (if asset type is normal)
		IF @IsNormalAssetType = 1 BEGIN
			-- Search linked Location table
			SELECT @linkTablenameId = 'lnk' + CONVERT(nvarchar(10), id)
			  FROM #tLinkedTables t
						INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityAttribConfigUid = t.RefDynEntityAttribConfigUid
			 WHERE ac.Name = 'Department'
			
			SET @DepartmentId = 'ISNULL(' + @linkTablenameId + '.DynEntityId, 0)';
			SET @Department = @linkTablenameId + '.Name'
		END
		
		-- Add OwnerId (if asset type is normal)
		IF @IsNormalAssetType = 1 BEGIN
			SELECT @linkTablenameId = 'lnk' + CONVERT(nvarchar(10), id)
			  FROM #tLinkedTables t
						INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityAttribConfigUid = t.RefDynEntityAttribConfigUid
			 WHERE ac.Name = 'Owner'
			
			SET @OwnerId = 'ISNULL(' + @linkTablenameId + '.DynEntityId, 0)';			
		END
		
		-- Add UserId (if asset type is normal)
		IF @IsNormalAssetType = 1 BEGIN
			SELECT @linkTablenameId = 'lnk' + CONVERT(nvarchar(10), id)
			  FROM #tLinkedTables t
						INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityAttribConfigUid = t.RefDynEntityAttribConfigUid
			 WHERE ac.Name = 'User'
			
			SET @UserId = 'ISNULL(' + @linkTablenameId + '.DynEntityId, 0)';			
			SET @User = 'ISNULL(' + @linkTablenameId + '.Name, '''')';			
		END
		
		-- If Entities provided, filter on current DynEntityConfigUid
		IF @rowcountEntities > 0 BEGIN
			SET @from = @from + ' INNER JOIN @entities e ON prim.DynEntityUid = e.DynEntityUid AND prim.DynEntityConfigUid = e.DynEntityConfigUid '
		END

		SET @sql = CONVERT(NVARCHAR(max), @creatett + ' ' + 
				'INSERT ' + CASE @active WHEN 1 THEN 'IndexActiveDynEntities ' ELSE 'IndexHistoryDynEntities ' END +
					'(DynEntityUid, Barcode, Name, Description, Keywords, AllAttrib2IndexValues, AllAttribValues, AllContextAttribValues, EntityConfigKeywords, ' +
					' CategoryKeywords, TaxonomyKeywords, DynEntityConfigUid, [User], UserId, LocationUid, Location, OwnerId, DepartmentId, Department, DynEntityId, TaxonomyItemsIds, UpdateDate, ' +
					' CategoryUids, TaxonomyUids, DynEntityConfigId, DisplayValues, DisplayExtValues) ' +
				'SELECT prim.DynEntityUid, ' + 
						CASE @IsNormalAssetType WHEN 1 THEN 'prim.Barcode' ELSE '''''' END + ' AS Barcode, ' +
						'prim.Name,' + 
						CASE LEN(@description) WHEN 0 THEN '''''' ELSE @description END + ' AS Description, ' + 
						CASE LEN(@keywords) WHEN 0 THEN '''''' ELSE @keywords END + ' AS KEYWORDS, ' + 
						CASE LEN (@AllAttrib2IndexValues) WHEN 0 THEN '''''' ELSE @AllAttrib2IndexValues END + ' AS AllAttrib2IndexValues, ' +
						CASE LEN (@AllAttribValues) WHEN 0 THEN '''''' ELSE @AllAttribValues END + ' AS AllAttribValues, ' +
				        CASE LEN (@AllContextAttribValues) WHEN 0 THEN '''''' ELSE @AllContextAttribValues END + ' AS AllContextAttribValues, ' +
				        '''' + @EntityConfigKeywords + ''' AS EntityConfigKeywords,' +
				        '''' + @CategoryKeywords + ''' AS CategoryKeywords, ' + 
				        '''' + @TaxonomyKeywords + ''' AS TaxonomyKeywords, ' + 
				        'prim.DynEntityConfigUid, ' +
				        @User + ' AS [user], ' +
				        @UserId + ' AS UserId, ' +
				        @LocationUid + ' AS LocationUid, ' +
				        @Location + ' AS Location, ' +
				        @OwnerId + ' AS OwnerId, ' +
				        @DepartmentId + ' AS DepartmentId, ' +
				        @Department + ' AS Department, ' +
				        'prim.DynEntityId, ' + 
				        CASE LEN (@TaxonomyItemsIds) WHEN 0 THEN '''''' ELSE '''' + @TaxonomyItemsIds + '''' END + ' AS TaxonomyItemsIds, ' +
				        'prim.UpdateDate, ' +
				        '''' + @CategoryUids + ''' AS CategoryUids, ' +
				        '''' + @TaxonomyUids + ''' AS TaxonomyUids, ' +
				        CONVERT(nvarchar(max), @vDynEntityConfigId) + ' AS DynEntityConfigId, CONVERT(nvarchar(450),' +
                @DisplayValues + ') AS DisplayValues, ' +
				        @DisplayExtValues + ' AS DisplayExtValues ' +
				' FROM ' + @from + 
				' WHERE prim.ActiveVersion = ' + CONVERT(varchar(1), @active) +
				CASE @active WHEN 1 THEN '; ' ELSE ' AND prim.DynEntityConfigUid = @DynEntityConfigUid; ' END +
				@droptt);
		
		--SELECT @sql
		EXEC sp_executesql @sql, N'@DynEntityConfigUid bigint, @entities DynEntityIdsTableType READONLY', @DynEntityConfigUid, @entities;
		
		-- Fill DynEntityIndex
		IF @buildDynEntityIndex = 1 AND @active = 1 BEGIN
			IF @IsNormalAssetType  = 1 BEGIN
				IF ISNULL(@DynEntityConfigId, 0) > 0 BEGIN
					INSERT DynEntityIndex
					SELECT i.DynEntityId, i.DynEntityConfigId, i.Barcode, l.DynEntityId AS LocationId, i.DepartmentId, i.UserId, i.OwnerId, i.Name
					  FROM IndexActiveDynEntities i
								LEFT OUTER JOIN ADynEntityLocation l ON i.LocationUid = l.DynEntityUid
					 WHERE DynEntityConfigId = @DynEntityConfigId
				END
				IF @rowcountEntities > 0 BEGIN
					INSERT DynEntityIndex
					SELECT i.DynEntityId, i.DynEntityConfigId, i.Barcode, l.DynEntityId AS LocationId, i.DepartmentId, i.UserId, i.OwnerId, i.Name
					  FROM IndexActiveDynEntities i
								LEFT OUTER JOIN ADynEntityLocation l ON i.LocationUid = l.DynEntityUid
								INNER JOIN @entities e ON e.DynEntityUid = i.DynEntityUid AND e.DynEntityConfigUid = i.DynEntityConfigUid
					 WHERE i.DynEntityConfigUid = @DynEntityConfigUid
				END
			END
			ELSE BEGIN
				IF ISNULL(@DynEntityConfigId, 0) > 0 BEGIN
					INSERT DynEntityIndex (DynEntityId, DynEntityConfigId, Name)
					SELECT DynEntityId, DynEntityConfigId, Name
					  FROM IndexActiveDynEntities
					 WHERE DynEntityConfigId = @DynEntityConfigId
				END
				IF @rowcountEntities > 0 BEGIN
					INSERT DynEntityIndex (DynEntityId, DynEntityConfigId, Name)
					SELECT i.DynEntityId, i.DynEntityConfigId, i.Name
					  FROM IndexActiveDynEntities i
								INNER JOIN @entities e ON e.DynEntityUid = i.DynEntityUid AND e.DynEntityConfigUid = i.DynEntityConfigUid
					 WHERE i.DynEntityConfigUid = @DynEntityConfigUid
				END				
			END
		END
				
		-- Clean DisplayValues/DisplayExtValues/Name
		WHILE EXISTS(SELECT 1 FROM IndexActiveDynEntities WHERE DynEntityConfigUid = @DynEntityConfigUid AND DisplayValues LIKE ', %') BEGIN
			UPDATE IndexActiveDynEntities SET DisplayValues = STUFF(DisplayValues, 1, 2, '')
			 WHERE DynEntityConfigUid = @DynEntityConfigUid AND DisplayValues LIKE ', %'
		END
		
		WHILE EXISTS(SELECT 1 FROM IndexActiveDynEntities WHERE DynEntityConfigUid = @DynEntityConfigUid AND DisplayExtValues LIKE ', %') BEGIN
			UPDATE IndexActiveDynEntities SET DisplayExtValues = STUFF(DisplayExtValues, 1, 2, '')
			 WHERE DynEntityConfigUid = @DynEntityConfigUid AND DisplayExtValues LIKE ', %'
		END

		UPDATE IndexActiveDynEntities 
		   SET Name = LTRIM(RTRIM(Name)),
			   DisplayValues = LTRIM(RTRIM(DisplayValues)),
			   DisplayExtValues = LTRIM(RTRIM(DisplayExtValues))
		 WHERE DynEntityConfigUid = @DynEntityConfigUid 
				
		DELETE FROM @tConfigUids WHERE DynEntityConfigUid = @DynEntityConfigUid
		TRUNCATE TABLE #tLinkedTables;
	END
	
	DROP TABLE #tLinkedTables;
	
END