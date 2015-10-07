-- Recreate DynEntityIdsTableType TABLE User Type (removing id identity field)
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_cust_ReIndex]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[_cust_ReIndex]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_cust_CreateNewRevision]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[_cust_CreateNewRevision]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'P', N'PC') AND name = '_cust_UpdateDynEntityTaxonomyItem')
DROP PROCEDURE [dbo].[_cust_UpdateDynEntityTaxonomyItem]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'P', N'PC') AND name = '_cust_BUB_UpdateRemainingAmount')
DROP PROCEDURE [dbo].[_cust_BUB_UpdateRemainingAmount]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'P', N'PC') AND name = '_cust_UpdateAssetsReferences')
DROP PROCEDURE [dbo].[_cust_UpdateAssetsReferences]
GO

IF  EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'DynEntityIdsTableType' AND ss.name = N'dbo')
DROP TYPE [dbo].[DynEntityIdsTableType]
GO

CREATE TYPE [dbo].[DynEntityIdsTableType] AS TABLE(
	--[id] [bigint] IDENTITY(1,1) NOT NULL,
	[DynEntityUid] [bigint] NOT NULL,
	[DynEntityId] [bigint] NOT NULL,
	[DynEntityConfigUid] [bigint] NOT NULL
)
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'P', N'PC') AND name = '_cust_SSIS_BUB_SetBatchOnCerts')
DROP PROCEDURE _cust_SSIS_BUB_SetBatchOnCerts
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'P', N'PC') AND name = '_cust_SSIS_SOB_SetBatchOnCerts')
DROP PROCEDURE _cust_SSIS_SOB_SetBatchOnCerts
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'P', N'PC') AND name = '_cust_GetSOBDossiers2PrintCert')
DROP PROCEDURE _cust_GetSOBDossiers2PrintCert
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 28/02/2013
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[_cust_UpdateDynEntityTaxonomyItem]
	@active bit = 1,
	@entities AS DynEntityIdsTableType READONLY
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @rowcount int,
			@i int = 1,
			@DynEntityConfigUid bigint;
	DECLARE @tmp AS TABLE (DynEntityConfigUid bigint);
	
	INSERT @tmp
	SELECT DISTINCT DynEntityConfigUid FROM @entities
	SET @rowcount = @@ROWCOUNT
	
	-- Loop all differenct DynEntityConfigUids to delete ...
	WHILE @i <= @rowcount BEGIN
		SELECT TOP 1 @DynEntityConfigUid = DynEntityConfigUid FROM @tmp;
			
		-- Update DynEntityTaxonomyItem (first delete eventually already existing entries)
		IF @active = 1 BEGIN
			DELETE FROM DynEntityTaxonomyItem
			 WHERE DynEntityUid IN (SELECT DISTINCT DynEntityUid FROM @entities WHERE DynEntityConfigUid = @DynEntityConfigUid)
			   AND DynEntityConfigUid = @DynEntityConfigUid
		END
		ELSE BEGIN
			DELETE FROM DynEntityTaxonomyItemHistory
			 WHERE DynEntityUid IN (SELECT DISTINCT DynEntityUid FROM @entities WHERE DynEntityConfigUid = @DynEntityConfigUid)
			   AND DynEntityConfigUid = @DynEntityConfigUid
		END
		
		DELETE FROM @tmp WHERE DynEntityConfigUid = @DynEntityConfigUid
		SET @i = @i + 1
	END		   

	-- Insert records, based on @entities table ...
	IF @active = 1 BEGIN
		INSERT DynEntityTaxonomyItem
		SELECT ent.DynEntityConfigUid, ent.DynEntityUid, tax.TaxonomyItemUid
		  FROM @entities ent
					INNER JOIN DynEntityConfig config ON ent.DynEntityConfigUid = config.DynEntityConfigUid
					INNER JOIN DynEntityConfigTaxonomy ctax ON ctax.DynEntityConfigId = config.DynEntityConfigId
					INNER JOIN TaxonomyItem tax ON tax.TaxonomyItemId = ctax.TaxonomyItemId
		WHERE tax.ActiveVersion = 1
	END
	ELSE BEGIN
		INSERT DynEntityTaxonomyItemHistory
		SELECT ent.DynEntityConfigUid, ent.DynEntityUid, tax.TaxonomyItemUid
		  FROM @entities ent
					INNER JOIN DynEntityConfig config ON ent.DynEntityConfigUid = config.DynEntityConfigUid
					INNER JOIN DynEntityConfigTaxonomy ctax ON ctax.DynEntityConfigId = config.DynEntityConfigId
					INNER JOIN TaxonomyItem tax ON tax.TaxonomyItemId = ctax.TaxonomyItemId
		WHERE tax.ActiveVersion = 1
	END		
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'P', N'PC') AND name = '_cust_CreateNewRevision')
DROP PROCEDURE _cust_CreateNewRevision
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 7/01/2013
-- Description:	Create a new revision for any 
-- asset/item type by first copying the current to 
-- history and afterwards adding 1 to revision for the current
-- =============================================
CREATE PROCEDURE [dbo].[_cust_CreateNewRevision] 
	@entities DynEntityIdsTableType READONLY	-- table variable
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Variable declarations
	DECLARE @DynEntityUid bigint = 0;
	DECLARE @DynEntityConfigUid bigint = 0;
	DECLARE @PrevDynEntityConfigUid bigint = 0;

	DECLARE @tempConfigUids TABLE (DynEntityConfigUid bigint)
	DECLARE @tempMultipleAssetAtts TABLE (DynEntityAttribConfigId bigint, DBTableName varchar(100))
	DECLARE @assetAtts TABLE (DynEntityAttribConfigUid bigint not null, DBTableFieldname nvarchar(50) not null, RelatedDBTableName nvarchar(50))
	
	DECLARE @NewEntityUid bigint;
	DECLARE @DynEntityConfigId BIGINT;
	DECLARE @DynEntityDBTableName VARCHAR(50);
	DECLARE @AttributeNames VARCHAR(5000) = '';
	
	DECLARE @DynEntityAttribConfigId bigint;
	DECLARE @DynRelatedMaaEntityDBTableName nvarchar(128);
	
	DECLARE @DynEntityAttribConfigUid bigint,
			@DBTableFieldname nvarchar(50),
			@RelatedDBTableName nvarchar(50)

	
	DECLARE @i int = 1;
	DECLARE @j int;
	DECLARE @rows int;
	DECLARE @rowsassetatts int;
	
	DECLARE @sqlCommand NVARCHAR(max);
	
	-- Get all different Config's
	INSERT @tempConfigUids
	SELECT DISTINCT DynEntityConfigUid FROM @entities;

	SET @rows = @@ROWCOUNT
	
	-- Loop different Configs ...
	WHILE @i <= @rows BEGIN	
		-- Temp tables
		IF OBJECT_ID('tempdb..#t') IS NOT NULL DROP TABLE #t;
		CREATE TABLE #t
		(
			DynEntityUid bigint, 
			DynEntityConfigUid bigint, 
			NewDynEntityUid bigint
		)

		-- Get config information
		SELECT TOP 1 @DynEntityConfigUid = c.DynEntityConfigUid, @DynEntityConfigId = c.DynEntityConfigId, @DynEntityDBTableName = c.DBTableName 
		  FROM @tempConfigUids tc
					INNER JOIN DynEntityConfig c ON tc.DynEntityConfigUid = c.DynEntityConfigUid
					
		INSERT @assetAtts (DynEntityAttribConfigUid, DBTableFieldname, RelatedDBTableName)
		SELECT ac.DynEntityAttribConfigUid, ac.DBTableFieldName, refc.DBTableName
		  FROM DynEntityAttribConfig ac
					INNER JOIN DynEntityConfig refc ON refc.DynEntityConfigId = ac.RelatedAssetTypeId
						   AND refc.ActiveVersion = 1
		 WHERE ac.DynEntityConfigUid = @DynEntityConfigUid
		   AND DataTypeUid IN (SELECT DataTypeUid FROM DataType WHERE Name = 'asset')
		   
		SET @rowsassetatts = @@ROWCOUNT
		
		-- Get all attributes
		SELECT @AttributeNames = 
			STUFF
			(
				(	SELECT ',[' + DBTableFieldName + ']'
					  FROM DynEntityAttribConfig
					 WHERE DynEntityConfigUid = @DynEntityConfigUid
					   AND Name NOT IN ('DynEntityUid', 'ActiveVersion')
					   FOR XML PATH(''), TYPE
				).value('.', 'nvarchar(max)'),
			1,1,'')
		
		-- Set ActiveVersion = 0 for current Active items
		SET @sqlCommand =	'UPDATE upd' + 
							  ' SET ActiveVersion = 0 ' +
							 ' FROM ' + @DynEntityDBTableName + ' upd INNER JOIN @entities e ON upd.DynEntityUid = e.DynEntityUid ' +
							                                               ' AND upd.DynEntityConfigUid = e.DynEntityConfigUid ' +
							' WHERE e.DynEntityConfigUid = @DynEntityConfigUid '
		EXECUTE sp_executesql @sqlCommand, N'@DynEntityConfigUid bigint, @entities DynEntityIdsTableType READONLY', @DynEntityConfigUid, @entities
		
		-- Create new revision
		SET @sqlCommand =	'INSERT ' + @DynEntityDBTableName + ' (ActiveVersion,' + @AttributeNames + ') ' +
							'SELECT ' + '1,' + REPLACE(REPLACE(@AttributeNames, '[DynEntityId]', 'prim.DynEntityId'), '[DynEntityConfigUid]', 'prim.DynEntityConfigUid') + 
							 ' FROM ' + @DynEntityDBTableName + ' prim INNER JOIN @entities e ON prim.DynEntityUid = e.DynEntityUid ' +
							                                               ' AND prim.DynEntityConfigUid = e.DynEntityConfigUid ' +
							' WHERE e.DynEntityConfigUid = @DynEntityConfigUid '
		EXECUTE sp_executesql @sqlCommand, N'@DynEntityConfigUid bigint, @entities DynEntityIdsTableType READONLY', @DynEntityConfigUid, @entities
		
		-- Update references asset attributes for created archive/history version. Replace DynEntityId -> DynEntityUid
		SET @j = 1;
		
		-- Loop asset attributes and replace reference with Uid	
		WHILE @j <= @rowsassetatts BEGIN
			SELECT TOP 1 @DynEntityAttribConfigUid = DynEntityAttribConfigUid, @DBTableFieldname = DBTableFieldname, 
						 @RelatedDBTableName = RelatedDBTableName
			  FROM @assetAtts
			  
			SET @sqlCommand = 'UPDATE prim ' + 
								' SET ' + @DBTableFieldname + ' = ref.DynEntityUid ' + 
							   ' FROM ' + @DynEntityDBTableName + ' prim ' + 
											' INNER JOIN ' + @RelatedDBTableName + ' ref ON ref.DynEntityId = prim.' + @DBTableFieldname + ' AND ref.ActiveVersion = 1 ' +
											' INNER JOIN @entities e ON prim.DynEntityUid = e.DynEntityUid ' + 
											       ' AND prim.DynEntityConfigUid = e.DynEntityConfigUid ' +
							  ' WHERE e.DynEntityConfigUid = @DynEntityConfigUid '
			EXECUTE sp_executesql @sqlCommand, N'@DynEntityConfigUid bigint, @entities DynEntityIdsTableType READONLY', @DynEntityConfigUid, @entities
			  
			DELETE FROM @assetatts WHERE DynEntityAttribConfigUid = @DynEntityAttribConfigUid
			SET @j = @j + 1;
		END
		
		
		-- Get new DynEntityUids (link to previous one)
		SET @sqlCommand =	'INSERT #t ' + 
							'SELECT t.DynEntityUid, t.DynEntityConfigUid, tbl.DynEntityUid AS NewDynEntityUid ' +
							 ' FROM @entities t INNER JOIN ' + @DynEntityDBTableName + ' tbl ON tbl.DynEntityId = t.DynEntityId AND tbl.ActiveVersion = 1 ' + 
							' WHERE t.DynEntityConfigUid = @DynEntityConfigUid; '
		EXECUTE sp_executesql @sqlCommand, N'@DynEntityConfigUid bigint, @entities DynEntityIdsTableType READONLY', @DynEntityConfigUid, @entities
			
		-- Update DynEntityTaxonomyItem/DynEntityTaxonomyItemHistory
		INSERT DynEntityTaxonomyItemHistory
		SELECT eti.* FROM DynEntityTaxonomyItem eti INNER JOIN #t t ON eti.DynEntityConfigUid = t.DynEntityConfigUid AND eti.DynEntityUid = t.DynEntityUid
		UPDATE eti SET DynEntityUid = t.NewDynEntityUid
		  FROM DynEntityTaxonomyItem eti INNER JOIN #t t ON eti.DynEntityConfigUid = t.DynEntityConfigUid AND eti.DynEntityUid = t.DynEntityUid
					       		
		-- Update DynEntityContextAttributesValues
		INSERT DynEntityContextAttributesValues
		SELECT StringValue, DateTimeValue, NumericValue, DynamicListItemUid, ContextId, NewDynEntityUid, ecav.DynEntityConfigUid, 1 
		  FROM DynEntityContextAttributesValues ecav INNER JOIN #t t ON ecav.DynEntityConfigUid = t.DynEntityConfigUid AND ecav.DynEntityUid = t.DynEntityUid
		UPDATE ecav SET IsActive = 0 
		  FROM DynEntityContextAttributesValues ecav INNER JOIN #t t ON ecav.DynEntityConfigUid = t.DynEntityConfigUid AND ecav.DynEntityUid = t.DynEntityUid
	
		-- Update DynListValue
		INSERT DynListValue (DynListUid,DynListItemUid,ParentListId,Value,AssetUid,DynEntityConfigUid,DynEntityAttribConfigUid)
		SELECT DynListUid, DynListItemUid, ParentListId, Value, NewDynEntityUid, lv.DynEntityConfigUid, DynEntityAttribConfigUid 
		 FROM DynListValue lv INNER JOIN #t t ON lv.DynEntityConfigUid = t.DynEntityConfigUid AND lv.AssetUid = t.DynEntityUid
		
		-- Update MultipleAssetsHistory
		INSERT @tempMultipleAssetAtts
		SELECT DISTINCT ac.DynEntityAttribConfigId, cr.DBTableName
		  FROM MultipleAssetsActive maa
					INNER JOIN DynEntityConfig c ON c.DynEntityConfigUid = @DynEntityConfigUid
					INNER JOIN DynEntityAttribConfig ac ON c.DynEntityConfigUid = ac.DynEntityConfigUid
					       AND ac.DynEntityAttribConfigId = maa.DynEntityAttribConfigId
					INNER JOIN DynEntityConfig cr ON ac.RelatedAssetTypeID = cr.DynEntityConfigId
					       AND cr.ActiveVersion = 1
					       
		WHILE EXISTS (SELECT 1 FROM @tempMultipleAssetAtts) BEGIN
			SELECT TOP 1 @DynEntityAttribConfigId = DynEntityAttribConfigId, @DynRelatedMaaEntityDBTableName = DBTableName FROM @tempMultipleAssetAtts
						
			SET @sqlCommand =	' INSERT MultipleAssetsHistory   ' +
								' SELECT e.DynEntityUid, ac.DynEntityAttribConfigUid, ref.DynEntityUid ' +
								  ' FROM MultipleAssetsActive maa ' +
											' INNER JOIN @entities e ON maa.DynEntityId = e.DynEntityId ' +
											' INNER JOIN DynEntityAttribConfig ac ON e.DynEntityConfigUid = ac.DynEntityConfigUid ' +
											       ' AND ac.DynEntityAttribConfigId = maa.DynEntityAttribConfigId ' +
											' INNER JOIN ' + @DynRelatedMaaEntityDBTableName + ' ref ON ref.DynEntityId = maa.RelatedDynEntityId ' +
								  'WHERE maa.DynEntityAttribConfigId = @DynEntityAttribConfigId; '
			EXEC sp_executesql @sqlCommand, N'@DynEntityAttribConfigId bigint, @entities DynEntityIdsTableType READONLY', @DynEntityAttribConfigId, @entities
			
			DELETE FROM @tempMultipleAssetAtts WHERE DynEntityAttribConfigId = @DynEntityAttribConfigId
		END
				
		-- Update IndexActiveDynEntities/IndexHistoryDynEntities
		INSERT IndexHistoryDynEntities (DynEntityUId,BarCode,Name,Description,Keywords,EntityConfigKeywords,AllAttrib2IndexValues,AllContextAttribValues,AllAttribValues,
										CategoryKeywords,TaxonomyKeywords,[User],LocationUid,Location,Department,DynEntityConfigUId,UpdateDate,CategoryUids,TaxonomyUids,
										OwnerId,DepartmentId,DynEntityId,TaxonomyItemsIds,DynEntityConfigId,DisplayValues,DisplayExtValues,UserId)
		SELECT iad.DynEntityUId,BarCode,Name,Description,Keywords,EntityConfigKeywords,AllAttrib2IndexValues,AllContextAttribValues,AllAttribValues,
			   CategoryKeywords,TaxonomyKeywords,[User],LocationUid,Location,Department,iad.DynEntityConfigUId,UpdateDate,CategoryUids,TaxonomyUids,
			   OwnerId,DepartmentId,iad.DynEntityId,TaxonomyItemsIds,DynEntityConfigId,DisplayValues,DisplayExtValues,UserId
		  FROM IndexActiveDynEntities iad INNER JOIN #t t ON iad.DynEntityConfigUid = t.DynEntityConfigUid AND iad.DynEntityUid = t.DynEntityUid
		UPDATE iad SET DynEntityUid = t.NewDynEntityUid 
		  FROM IndexActiveDynEntities iad INNER JOIN #t t ON iad.DynEntityConfigUid = t.DynEntityConfigUid AND iad.DynEntityUid = t.DynEntityUid
		
		-- Delete DynEntityConfigUid from temp table and add counter ...
		DROP TABLE #t
		DELETE FROM @tempConfigUids WHERE DynEntityConfigUid = @DynEntityConfigUid
		SET @i = @i + 1
	END
			
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'P', N'PC') AND name = '_cust_ReIndex_BuildSQL')
DROP PROCEDURE _cust_ReIndex_BuildSQL
GO

/****** Object:  StoredProcedure [dbo].[_cust_ReIndex_BuildSQL]    Script Date: 02/13/2013 21:59:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 16/01/2013
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[_cust_ReIndex_BuildSQL] 
	-- Add the parameters for the stored procedure here
	@DynEntityAttribConfigUid bigint, 
	@field nvarchar(max) OUTPUT, 
	@from nvarchar(max) OUTPUT,
	@creatett nvarchar(max) OUTPUT, 
	@droptt nvarchar(max) OUTPUT,
	@culture nvarchar(10),
	@delimiter nvarchar(max) = ' ',
	@PrimaryTable nvarchar(max) = 'prim',
	@AssetLinkedTable nvarchar(max) = '' OUTPUT,
	@AssetDynEntityConfigUid bigint OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
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
	DECLARE @dbRefTableName varchar(100);
	DECLARE @dbRefLnkName varchar(20);
	DECLARE @dbRefTableFieldname varchar(100);
	DECLARE @DynListUid bigint;
	DECLARE @linkTablenameId int;
	DECLARE @DynListUids nvarchar(1000);
	
	-- Select datatype and asset/dynlist related ids
	SELECT	@DynEntityConfigUid = DynEntityConfigUid, @DynEntityAttribConfigId = DynEntityAttribConfigId, @ActiveVersion = ActiveVersion,
			@TableFieldname = DBTableFieldname, @DataType = dt.Name, 
			@RelatedAssetTypeID = ac.RelatedAssetTypeID, @RelatedAssetTypeAttributeID = ac.RelatedAssetTypeAttributeID,
			@DynListUid = DynListUid
	  FROM DynEntityAttribConfig ac
				INNER JOIN DataType dt ON ac.DataTypeUid = dt.DataTypeUid
	 WHERE ac.DynEntityAttribConfigUid = @DynEntityAttribConfigUid
	
	IF LEN(@field) > 0 
	BEGIN
		SET @delimiter = /* ' + */ '''' + @delimiter + ''' + ';
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
		
		SET @field = @field + 'CASE WHEN ' + @dbRefLnkName + '.' + @dbRefTableFieldname + ' IS NULL THEN '''' ELSE ' + @delimiter + @dbRefLnkName + '.' + @dbRefTableFieldname + ' END';
		
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
		
		SET @field = @field + ' CASE WHEN ' + @ttname + '.Value IS NULL THEN '''' ELSE ' + @delimiter + @ttname + '.Value END ';		
		
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
		
		-- SET @field = @field + 'ISNULL(' + @ttname + '.Value, '''')' ;
		SET @field = @field + ' CASE WHEN ' + @ttname2 + '.Value IS NULL THEN '''' ELSE ' + @delimiter + @ttname2 + '.Value END';
				
	END
	
	-- Normal datatypes
	ELSE IF @DataType = 'datetime' OR @DataType = 'currentdate'
	BEGIN
		IF @culture = 'nl-BE' OR @culture = 'nl-NL' OR @culture = 'fr-FR'
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN '''' ELSE ' + @delimiter + ' CONVERT(nvarchar(20), ' + @PrimaryTable + '.' + @TableFieldname + ', 103) END';
		ELSE 
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN '''' ELSE ' + @delimiter + ' CONVERT(nvarchar(20), ' + @PrimaryTable + '.' + @TableFieldname + ') END';
	END
	ELSE IF @DataType = 'float'
	BEGIN
/*
		IF @culture = 'nl-BE' OR @culture = 'nl-NL' OR @culture = 'fr-FR'
			SET @field = @field + ' CASE WHEN prim.' + @TableFieldname + ' IS NULL THEN '''' ELSE ' + @delimiter + 'REPLACE(CONVERT(nvarchar(25), prim.' + @TableFieldname + ', 1), ''.'', '','') END '
		ELSE
			SET @field = @field + ' CASE WHEN prim.' + @TableFieldname + ' IS NULL THEN '''' ELSE ' + @delimiter + 'CONVERT(nvarchar(25), prim.' + @TableFieldname + ') END '
*/
		IF @culture = 'nl-BE' OR @culture = 'nl-NL' OR @culture = 'fr-FR'
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN '''' ELSE ' + @delimiter + 'REPLACE(LTRIM(STR(' + @PrimaryTable + '.' + @TableFieldname + ', 10, 2)), ''.'', '','') END '
		ELSE
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN '''' ELSE ' + @delimiter + 'LTRIM(STR(' + @PrimaryTable + '.' + @TableFieldname + ', 10, 2)) END '
	END
	ELSE IF @DataType = 'money' OR @DataType = 'usd' OR @DataType = 'euro'
	BEGIN
		IF @culture = 'nl-BE' OR @culture = 'nl-NL' OR @culture = 'fr-FR'
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN '''' ELSE ' + @delimiter + 'REPLACE(CONVERT(nvarchar(25), ' + @PrimaryTable + '.' + @TableFieldname + ', 0), ''.'', '','') END '
		ELSE
			SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN '''' ELSE ' + @delimiter + 'CONVERT(nvarchar(25), ' + @PrimaryTable + '.' + @TableFieldname + ', 0) END '
	END
	ELSE IF @DataType = 'richtext'
	BEGIN
		SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN '''' ELSE ' + @delimiter + 'dbo.StripHTML(' + @PrimaryTable + '.' + @TableFieldname + ') END '
	END
	
	-- No special formatting required, just paste the value
	ELSE BEGIN
		SET @field = @field + ' CASE WHEN ' + @PrimaryTable + '.' + @TableFieldname + ' IS NULL THEN '''' ELSE ' + @delimiter + ' CONVERT(nvarchar(1000), ' + @PrimaryTable + '.' + @TableFieldname + ') END'
	END;
	
	-- Return linked table (only valid if datatype is asset)
	SET @AssetLinkedTable = @dbRefLnkName;
		
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'P', N'PC') AND name = '_cust_Reindex_BuildIndexField')
DROP PROCEDURE _cust_Reindex_BuildIndexField
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 03/02/2013
-- Description:	
-- =============================================
CREATE PROCEDURE _cust_Reindex_BuildIndexField 
	-- Add the parameters for the stored procedure here
	@DynEntityConfigUid bigint,
	@FieldType int = 0, 
	@RecursiveIndexFieldType int = 0,
	@field nvarchar(max) OUTPUT, 
	@from nvarchar(max) OUTPUT,
	@creatett nvarchar(max) OUTPUT, 
	@droptt nvarchar(max) OUTPUT,
	@culture nvarchar(10),
	@delimiter nvarchar(max) = ' ',
	@PrimaryTable nvarchar(max) = 'prim'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @DESCRIPTION int = 1;
	DECLARE @KEYWORD int = 2;
	DECLARE @FULLTEXTINDEX int = 3;
	DECLARE @ALLATTRIBUTEVALUES int = 4;
	DECLARE @DISPLAYVALUES int = 5;
	DECLARE @DISPLAYEXTVALUES int = 6;

	DECLARE @DynEntityAttribConfigUid bigint;	
	DECLARE @DataType nvarchar(60);
	DECLARE @tAttributes TABLE (DynEntityAttribConfigUid bigint, DataType nvarchar(60));
	DECLARE @AssetLinkedTable varchar(100);
	DECLARE @AssetDynEntityConfigUid bigint;
	
	DECLARE @IsDescription bit = 0;
	DECLARE @IsKeyword bit = 0;
	DECLARE @IsFullTextInidex bit = 0;

	IF @FieldType = @DESCRIPTION 
		SET @IsDescription = 1;
	ELSE IF @FieldType = @KEYWORD
		SET @IsKeyword = 1;
	ELSE IF @FieldType = @FULLTEXTINDEX 
		SET @IsFullTextInidex = 1;
	
	-- Build Dynamic SQL
	INSERT @tAttributes
	SELECT ac.DynEntityAttribConfigUid, dt.Name
	  FROM DynEntityAttribConfig ac
				INNER JOIN DataType dt ON dt.DataTypeUid = ac.DataTypeUid
     WHERE ac.DynEntityConfigUid = @DynEntityConfigUid
	   AND (ac.IsDescription = @IsDescription OR @fieldtype <> @DESCRIPTION)
	   AND (ac.IsKeyword = @IsKeyword   OR @fieldtype <> @KEYWORD)
	   AND (ac.IsFullTextInidex = @IsFullTextInidex OR @fieldtype <> @FULLTEXTINDEX)
	   AND ((	(@FieldType = @ALLATTRIBUTEVALUES) 
			AND (ac.Name NOT IN ('DynEntityUid', 'DynEntityId', 'ActiveVersion', 'DynEntityConfigUid', 'Revision', 'Update User', 'Update Date'))
		    /* AND (ac.IsFullTextInidex = 0 AND ac.IsKeyword = 0 AND ac.IsDescription = 0)*/ )
		     OR (@FieldType <> @ALLATTRIBUTEVALUES))
	   AND ((	(@FieldType = @DISPLAYVALUES) AND (DisplayOnResultList = 1)) OR (@FieldType <> @DISPLAYVALUES))
	   AND ((	(@FieldType = @DISPLAYEXTVALUES) AND (DisplayOnExtResultList = 1)) OR (@FieldType <> @DISPLAYEXTVALUES))
	 ORDER BY CASE @FieldType WHEN @DISPLAYVALUES THEN DisplayOrderResultList WHEN @DISPLAYEXTVALUES THEN DisplayOrderExtResultList ELSE 1 END;
	 	   
	WHILE EXISTS (SELECT 1 FROM @tAttributes)
	BEGIN
		SELECT TOP 1 @DynEntityAttribConfigUid = DynEntityAttribConfigUid, @DataType = DataType FROM @tAttributes
		
		EXEC _cust_ReIndex_BuildSQL @DynEntityAttribConfigUid, @field OUTPUT, @from OUTPUT, @creatett OUTPUT, @droptt OUTPUT,
									@culture, @delimiter, @PrimaryTable, @AssetLinkedTable OUTPUT, @AssetDynEntityConfigUid OUTPUT;
		
		IF @DataType = 'asset' AND @RecursiveIndexFieldType > 0 BEGIN	
			EXEC _cust_Reindex_BuildIndexField	@AssetDynEntityConfigUid, @RecursiveIndexFieldType, 0, @field OUTPUT, @from OUTPUT, @creatett OUTPUT, @droptt OUTPUT,
												@culture, @delimiter, @AssetLinkedTable;
			
		END
		
		DELETE FROM @tAttributes WHERE DynEntityAttribConfigUid = @DynEntityAttribConfigUid
	END;

END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'P', N'PC') AND name = '_cust_ReIndex')
DROP PROCEDURE _cust_ReIndex
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steegmans Wouter
-- Create date: 25/01/2013
-- Description:	Reindex items of certain asset/item type
-- =============================================
CREATE PROCEDURE _cust_ReIndex
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
		   AND ActiveVersion = @active
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
		EXEC _cust_Reindex_BuildIndexField  @DynEntityConfigUid, 5, 0, @DisplayValues OUTPUT, @from OUTPUT, @creatett OUTPUT, @droptt OUTPUT,
											@culture, ', ', 'prim';
		
		-- Build Dynamic SQL (DisplayExtValues field)
		EXEC _cust_Reindex_BuildIndexField  @DynEntityConfigUid, 6, 0, @DisplayExtValues OUTPUT, @from OUTPUT, @creatett OUTPUT, @droptt OUTPUT,
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
				        CONVERT(nvarchar(max), @vDynEntityConfigId) + ' AS DynEntityConfigId, ' +
				        @DisplayValues + ' AS DisplayValues, ' +
				        @DisplayExtValues + ' AS DisplayExtValues ' +
				' FROM ' + @from + 
				' WHERE prim.ActiveVersion = ' + CONVERT(varchar(1), @active) +
				CASE @active WHEN 1 THEN '; ' ELSE ' AND prim.DynEntityConfigUid = @DynEntityConfigUid; ' END +
				@droptt);
		
		-- SELECT @sql
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
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'P', N'PC') AND name = '_cust_ReIndex_All')
DROP PROCEDURE _cust_ReIndex_All
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steegmans Wouter
-- Create date: 19/02/2013
-- Description:	Reindex items of all asset/item types
-- =============================================
CREATE PROCEDURE _cust_ReIndex_All 
	@active bit = 1,
	@buildDynEntityIndex bit = 0,
	@culture nvarchar(10) = 'nl-BE'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	DECLARE @t TABLE (DynEntityConfigId bigint, Name nvarchar(60) );
	DECLARE	@DynEntityConfigId bigint,
			@Name nvarchar(60),
			@entities DynEntityIdsTableType,
			@i int = 1,
			@rowcount int;


	IF @active = 1 BEGIN
		TRUNCATE TABLE IndexActiveDynEntities;
		DBCC CHECKIDENT (IndexActiveDynEntities, RESEED, 0) WITH NO_INFOMSGS
	END
	ELSE BEGIN
		TRUNCATE TABLE IndexHistoryDynEntities;
		DBCC CHECKIDENT (IndexHistoryDynEntities, RESEED, 0) WITH NO_INFOMSGS
	END
	
	IF @buildDynEntityIndex = 1 BEGIN
		TRUNCATE TABLE DynEntityIndex;
	END
	
	INSERT @t
	SELECT DynEntityConfigId, Name FROM DynEntityConfig WHERE ActiveVersion = 1

	SELECT @rowcount = @@ROWCOUNT

	BEGIN TRY
		WHILE @i <= @rowcount BEGIN
			SELECT TOP 1 @DynEntityConfigId = DynEntityConfigId, @Name = Name FROM @t
			EXEC _cust_ReIndex @DynEntityConfigId, @active, @buildDynEntityIndex, @culture, @entities = @entities
			
			SET @i = @i + 1
			DELETE FROM @t WHERE DynEntityConfigId = @DynEntityConfigId
		END
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();
			
		SET @ErrorMessage = @ErrorMessage + ' (Asset/Item type: ' + @Name + ')';

		-- Use RAISERROR inside the CATCH block to return error
		-- information about the original error that caused
		-- execution to jump to the CATCH block.
		RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );		
		RETURN		
	END CATCH
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_cust_UpdateAssetsReferences]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[_cust_UpdateAssetsReferences]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 05/03/2013
-- Description:	Update reference to configuration Uid (DynEntityConfigUid
--				of all active entities, passed by the parameter.
--				Only active items must be changed. However, there is no check
--				in this SP for ActiveVersion = 1.
-- =============================================
CREATE PROCEDURE _cust_UpdateAssetsReferences
	-- Add the parameters for the stored procedure here
	@entities AS DynEntityIdsTableType READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Variable declarations
	DECLARE @tempConfigIds TABLE (DynEntityConfigId bigint)
	DECLARE @tmp AS DynEntityIdsTableType 

	DECLARE @DynEntityConfigUid bigint;
	DECLARE @DynEntityConfigId bigint;
	DECLARE @DynEntityDBTableName VARCHAR(50);

	DECLARE @i int = 1;
	DECLARE @rows int;	
	DECLARE @sqlCommand NVARCHAR(max);

	-- Get all different Config's
	INSERT @tempConfigIds
	SELECT DISTINCT DynEntityConfigId 
	  FROM DynEntityConfig c
				INNER JOIN @entities e ON e.DynEntityConfigUid = c.DynEntityConfigUid

	SET @rows = @@ROWCOUNT
	
	-- Loop different Configs ...
	WHILE @i <= @rows BEGIN	
		-- Get config information
		SELECT TOP 1 @DynEntityConfigUid = c.DynEntityConfigUid, @DynEntityConfigId = c.DynEntityConfigId, @DynEntityDBTableName = c.DBTableName 
		  FROM @tempConfigIds tc
					INNER JOIN DynEntityConfig c ON tc.DynEntityConfigId = c.DynEntityConfigId
						   AND c.ActiveVersion = 1

		-- Get entities with wrong DynEntityConfigUid
		DELETE FROM @tmp
		INSERT @tmp (DynEntityUid, DynEntityId, DynEntityConfigUid)
		SELECT e.DynEntityUid, e.DynEntityId, e.DynEntityConfigUid
		  FROM @entities e
		 WHERE e.DynEntityConfigUid IN (SELECT DynEntityConfigUid FROM DynEntityConfig WHERE DynEntityConfigId = @DynEntityConfigId)
		  -- AND e.DynEntityConfigUid <> @DynEntityConfigUid

		-- UpdateDynEntityConfigUid in entity table (ADynEntity*)
		SET @sqlCommand =	'UPDATE upd' + 
							  ' SET DynEntityConfigUid = @DynEntityConfigUid ' +
							 ' FROM ' + @DynEntityDBTableName + ' upd ' +
										' INNER JOIN @tmp t ON upd.DynEntityUid = t.DynEntityUid ' +
							                   ' AND upd.DynEntityConfigUid = t.DynEntityConfigUid ' 
							            
		EXECUTE sp_executesql @sqlCommand, N'@DynEntityConfigUid bigint, @tmp DynEntityIdsTableType READONLY', @DynEntityConfigUid, @tmp
		
		-- Update DynEntityTaxonomyItem
		UPDATE eti
		   SET DynEntityConfigUid = @DynEntityConfigUid
		  FROM @tmp t 
					INNER JOIN DynEntityConfig c ON c.DynEntityConfigUid = t.DynEntityConfigUid
					INNER JOIN DynEntityConfig c2 ON c.DynEntityConfigId = c2.DynEntityConfigId
					INNER JOIN DynEntityTaxonomyItem eti ON eti.DynEntityUid = t.DynEntityUid
					       AND eti.DynEntityConfigUid = c2.DynEntityConfigUid

		-- Update DynEntityContextAttributesValues
		UPDATE ecav
		   SET DynEntityConfigUid = @DynEntityConfigUid
		  FROM @tmp t 
					INNER JOIN DynEntityConfig c ON c.DynEntityConfigUid = t.DynEntityConfigUid
					INNER JOIN DynEntityConfig c2 ON c2.DynEntityConfigId = c2.DynEntityConfigId
					INNER JOIN DynEntityContextAttributesValues ecav ON ecav.DynEntityUid = t.DynEntityUid
					       AND ecav.DynEntityConfigUid  = c2.DynEntityConfigUid
					
		-- Update DynListValue (DynEntityConfigUid)
		UPDATE lv
		   SET DynEntityConfigUid = @DynEntityConfigUid,
			   DynEntityAttribConfigUid = acnew.DynEntityAttribConfigUid
		  FROM @tmp t
					INNER JOIN DynEntityConfig c ON c.DynEntityConfigUid = t.DynEntityConfigUid
					INNER JOIN DynEntityConfig c2 ON c.DynEntityConfigId = c2.DynEntityConfigId
					INNER JOIN DynListValue lv ON lv.DynEntityConfigUid = c2.DynEntityConfigUid
						   AND lv.AssetUid = t.DynEntityUid
					INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityAttribConfigUid = lv.DynEntityAttribConfigUid
					INNER JOIN DynEntityAttribConfig acnew ON acnew.DynEntityAttribConfigId = ac.DynEntityAttribConfigId
						   AND acnew.ActiveVersion = 1 
		
		-- Update MultipleAssets (not necessary, using Id fields/links)
				
		-- Update IndexActiveDynEntities
		UPDATE iad
		   SET DynEntityConfigUid = @DynEntityConfigUid
		  FROM @tmp t
					INNER JOIN DynEntityConfig c ON c.DynEntityConfigUid = t.DynEntityConfigUid
					INNER JOIN DynEntityConfig c2 ON c2.DynEntityConfigId = c.DynEntityConfigId
					INNER JOIN IndexActiveDynEntities iad ON t.DynEntityUid = iad.DynEntityUid
					        AND c2.DynEntityConfigUid = iad.DynEntityConfigUid
		
		-- Delete DynEntityConfigUid from temp table and add counter ...
		DELETE FROM @tempConfigIds WHERE DynEntityConfigId = @DynEntityConfigId
		SET @i = @i + 1
	END    
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'P', N'PC') AND name = 'RestoreDeletedItem')
DROP PROCEDURE RestoreDeletedItem
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 04/02/2013
-- Description:	Restore deleted item
-- =============================================
CREATE PROCEDURE RestoreDeletedItem 
	-- Add the parameters for the stored procedure here
	@DynEntityId bigint = 0, 
	@DynEntityConfigId bigint = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @DynEntityConfigUid bigint;
	DECLARE @DynEntityUid bigint = NULL;
	DECLARE @RelatedDynEntityId bigint = NULL;
	DECLARE @NewDynEntityUid bigint;
	DECLARE @IsNormalAssetType bit;
	DECLARE @Tablename varchar(255);
	DECLARE @sql nvarchar(max);
	
	DECLARE @rowcount int ;
	
	DECLARE @tempMultipleAssetAtts TABLE (DynEntityAttribConfigUid bigint, DBTableName varchar(100));
	DECLARE @tempDeletedUids TABLE (DynEntityUid bigint);
	DECLARE @DynEntityAttribConfigUid bigint;
	DECLARE @DynRelatedMaaEntityDBTableName nvarchar(128);
	
	BEGIN TRY

		-- Get config data
		SELECT @TableName = DBTableName 
		  FROM DynEntityConfig
		 WHERE DynEntityConfigId = @DynEntityConfigId
		   AND ActiveVersion = 1

		-- Check if there is no active revision anymore ...
		SET @sql = 'SELECT TOP 1 @DynEntityUId = DynEntityUid FROM ' + @TableName	+ ' WHERE DynEntityId = @DynEntityId AND ActiveVersion = 1 ORDER BY Revision DESC';
		EXEC sp_executesql @sql, N'@DynEntityId bigint, @DynEntityUid bigint OUTPUT', @DynEntityId, @DynEntityUid = @DynEntityUid OUTPUT
		
		IF NOT @DynEntityUid IS NULL BEGIN
			DECLARE @intpar int;
			SET @intpar = CONVERT(int, @DynEntityUid);
			RAISERROR ('Error while recovering asset/item. There already exists an active version <<DynEntityUid=%d>> so no need to restore.', 16,1, @intpar);
		END
		 
		-- Remove deleted revision in DeletedEntities, the entity table (and IndexHistoryDynEntities, if present)
		DELETE FROM DeletedEntities 
		OUTPUT DELETED.DynEntityUid INTO @tempDeletedUids
		 WHERE DynEntityConfigId = @DynEntityConfigId
		   AND DynEntityId = @DynEntityId
		
		SET @rowcount = @@ROWCOUNT
		
		IF @rowcount > 0 BEGIN
			-- Get latest revision (the deleted revision)
			SELECT @DynEntityUid = DynEntityUid
			  FROM @tempDeletedUids

			-- Deleted latest revision (because created as DELETED recored)
			SET @sql = 'DELETE FROM ' + @TableName + 
					   ' WHERE DynEntityUid = ' + CONVERT(nvarchar(max), @DynEntityUid)
			EXEC sp_executesql @sql, N'@DynEntityUid bigint OUTPUT', @DynEntityUid
			   
			DELETE FROM IndexHistoryDynEntities
			 WHERE DynEntityUid = @DynEntityUid
			   AND DynEntityConfigId = @DynEntityConfigId		   
		END

		-- Get latest (active) revision, previous to the deleted one
		SET @sql = 'SELECT TOP 1 @DynEntityUid = DynEntityUid, @DynEntityConfigUid = DynEntityConfigUid ' + 
					' FROM ' + @TableName + 
				   ' WHERE DynEntityId = ' + CONVERT(nvarchar(max), @DynEntityId) + 
				   ' ORDER BY Revision DESC; '
		EXEC sp_executesql @sql, N'@DynEntityUid bigint OUTPUT, @DynEntityConfigUid bigint OUTPUT', 
								 @DynEntityUid = @DynEntityUid OUTPUT, @DynEntityConfigUid = @DynEntityConfigUid OUTPUT	
		
		-- Copy history -> active --
		-- Copy to DynEntityTaxonomyItem
		IF NOT EXISTS(SELECT * FROM DynEntityTaxonomyItem WHERE DynEntityConfigUid = @DynEntityConfigUid AND DynEntityUid = @DynEntityUid) BEGIN
			INSERT DynEntityTaxonomyItem
			SELECT * FROM DynEntityTaxonomyItemHistory WHERE DynEntityConfigUid = @DynEntityConfigUid AND DynEntityUid = @DynEntityUid
		END
			
		DELETE FROM DynEntityTaxonomyItemHistory WHERE DynEntityConfigUid = @DynEntityConfigUid AND DynEntityUid = @DynEntityUid
		
		-- Copy to Multiple Assets table (TO DO)
		INSERT @tempMultipleAssetAtts
		SELECT DISTINCT ac.DynEntityAttribConfigUid, cr.DBTableName
		  FROM MultipleAssetsHistory mah
					INNER JOIN DynEntityConfig c ON c.DynEntityConfigUid = @DynEntityConfigUid
					INNER JOIN DynEntityAttribConfig ac ON c.DynEntityConfigUid = ac.DynEntityConfigUid
					       AND ac.DynEntityAttribConfigUid = mah.DynEntityAttribConfigUid
					INNER JOIN DynEntityConfig cr ON ac.RelatedAssetTypeID = cr.DynEntityConfigId
					       AND cr.ActiveVersion = 1
		 WHERE mah.DynEntityUid = @DynEntityUid
					       
		WHILE EXISTS (SELECT 1 FROM @tempMultipleAssetAtts) BEGIN
			SELECT TOP 1 @DynEntityAttribConfigUid = DynEntityAttribConfigUid, @DynRelatedMaaEntityDBTableName = DBTableName FROM @tempMultipleAssetAtts
						
			SET @sql =	' INSERT MultipleAssetsActive ' +
						' SELECT @DynEntityId, ac.DynEntityAttribConfigId, ref.DynEntityId ' +
						  ' FROM MultipleAssetsHistory mah ' +
									' INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityAttribConfigUid = mah.DynEntityAttribConfigUid ' +
									' INNER JOIN ' + @DynRelatedMaaEntityDBTableName + ' ref ON ref.DynEntityUid = mah.RelatedDynEntityUid ' +
						 ' WHERE mah.DynEntityUid = @DynEntityUid ' +
							'AND mah.DynEntityAttribConfigUid = @DynEntityAttribConfigUid; '
			EXEC sp_executesql @sql, N'@DynEntityUid bigint, @DynEntityId bigint, @DynEntityAttribConfigUid bigint', @DynEntityUid, @DynEntityId, @DynEntityAttribConfigUid
			
			DELETE FROM MultipleAssetsHistory 
			 WHERE DynEntityUid = @DynEntityUid
			   AND DynEntityAttribConfigUid = @DynEntityAttribConfigUid
			
			DELETE FROM @tempMultipleAssetAtts WHERE DynEntityAttribConfigUid = @DynEntityAttribConfigUid
		END
		
		-- Copy to IndexActiveDynEntities
		INSERT IndexActiveDynEntities
			   (DynEntityUId,BarCode,Name,Description,Keywords,EntityConfigKeywords,AllAttrib2IndexValues,AllContextAttribValues,AllAttribValues,CategoryKeywords,
				TaxonomyKeywords,[User],LocationUid,Location,Department,DynEntityConfigUId,UpdateDate,CategoryUids,TaxonomyUids,OwnerId,DepartmentId,DynEntityId,
				TaxonomyItemsIds,DynEntityConfigId,DisplayValues,DisplayExtValues,UserId)
		SELECT DynEntityUId,BarCode,Name,Description,Keywords,EntityConfigKeywords,AllAttrib2IndexValues,AllContextAttribValues,AllAttribValues,CategoryKeywords,
			   TaxonomyKeywords,[User],LocationUid,Location,Department,DynEntityConfigUId,UpdateDate,CategoryUids,TaxonomyUids,OwnerId,DepartmentId,DynEntityId,
			   TaxonomyItemsIds,DynEntityConfigId,DisplayValues,DisplayExtValues,UserId
		  FROM IndexHistoryDynEntities 
		 WHERE DynEntityUid = @DynEntityUid
		   AND DynEntityConfigUid = @DynEntityConfigUid
		   
		DELETE IndexHistoryDynEntities 
		 WHERE DynEntityUid = @DynEntityUid
		   AND DynEntityConfigUid = @DynEntityConfigUid

		-- Set active most recent revision
		SET @sql = 'UPDATE ' + @TableName + 
					 ' SET ActiveVersion = 1 ' +
				   ' WHERE DynEntityUid = ' + CONVERT(nvarchar(max), @DynEntityUid) 
		EXEC sp_executesql @sql, N'@DynEntityUid bigint OUTPUT', @DynEntityUid = @DynEntityUid
		
		-- Insert in DynEntityIndex
		IF NOT EXISTS(SELECT 1 FROM DynEntityIndex WHERE DynEntityId = @DynEntityId AND DynEntityConfigId = @DynEntityConfigId) BEGIN
		
			SET @IsNormalAssetType = dbo.IsNormalAssetType(@DynEntityConfigId);
			
			IF @IsNormalAssetType = 1 BEGIN
				SET @sql =	'INSERT DynEntityIndex ' + 
							'SELECT DynEntityId, @DynEntityConfigId, Barcode, LocationId, DepartmentId, UserId, OwnerId, Name ' +
							 ' FROM ' + @Tablename + 
							' WHERE DynEntityId = @DynEntityId AND DynEntityConfigUid = @DynEntityConfigUid AND ActiveVersion = 1 '
				EXEC sp_executesql @sql, N'@DynEntityId bigint, @DynEntityConfigId bigint, @DynEntityConfigUid bigint', @DynEntityId, @DynEntityConfigId, @DynEntityConfigUid
			END
			ELSE BEGIN
				SET @sql =	'INSERT DynEntityIndex (DynEntityId, DynEntityConfigId, Name)' + 
							'SELECT DynEntityId, @DynEntityConfigId, Name ' +
							 ' FROM ' + @Tablename + 
							' WHERE DynEntityId = @DynEntityId AND DynEntityConfigUid = @DynEntityConfigUid AND ActiveVersion = 1 '
				EXEC sp_executesql @sql, N'@DynEntityId bigint, @DynEntityConfigId bigint, @DynEntityConfigUid bigint', @DynEntityId, @DynEntityConfigId, @DynEntityConfigUid
			END
		END
		
		-- Update DynEntityConfigUid to latest revision, if necessary for restored item(s).
		DECLARE @entities AS DynEntityIdsTableType 
		INSERT @entities
		SELECT DynEntityUid, DynEntityId, DynEntityConfigUid
		  FROM IndexActiveDynEntities
		 WHERE DynEntityId = @DynEntityId
		   AND DynEntityConfigId = @DynEntityConfigId
		EXEC _cust_UpdateAssetsReferences @entities = @entities
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();

		-- Use RAISERROR inside the CATCH block to return error
		-- information about the original error that caused
		-- execution to jump to the CATCH block.
		RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );		
	END CATCH
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'TF', N'FN') AND name = 'GetStringResourceValue')
DROP FUNCTION GetStringResourceValue
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 31/01/2013
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[GetStringResourceValue]
(
	-- Add the parameters for the function here
	@ResourceKey nvarchar(128)
)
RETURNS nvarchar(4000)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result nvarchar(3000) = '';

	-- Add the T-SQL statements to compute the return value here
	
	SELECT @result = 
	(
		SELECT ResourceValue + ' '
		  FROM StringResources
		 WHERE ResourceKey = @ResourceKey
		 FOR XML PATH(''), TYPE
	).value('.', 'nvarchar(max)')
 
	-- Return the result of the function
	RETURN RTRIM(ISNULL(@Result, ''))

END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'TF', N'FN') AND name = 'StripHTML')
DROP FUNCTION StripHTML
GO

CREATE FUNCTION [dbo].[StripHTML]
(
	@HTMLText varchar(MAX)
)
RETURNS varchar(MAX)
AS
BEGIN

	DECLARE @Start INT
	DECLARE @End INT
	DECLARE @Length INT
	
	SET @Start = CHARINDEX('<',@HTMLText)
	SET @End = CHARINDEX('>',@HTMLText,CHARINDEX('<',@HTMLText))
	SET @Length = (@End - @Start) + 1
	
	WHILE @Start > 0 AND @End > 0 AND @Length > 0 BEGIN
		SET @HTMLText = STUFF(@HTMLText,@Start,@Length,'')
		SET @Start = CHARINDEX('<',@HTMLText)
		SET @End = CHARINDEX('>',@HTMLText,CHARINDEX('<',@HTMLText))
		SET @Length = (@End - @Start) + 1
	END
	
	WHILE CHARINDEX('  ',@HTMLText) > 0 BEGIN
		SET @HTMLText = REPLACE(@HTMLText, '  ', ' ');
	END
	
	RETURN LTRIM(RTRIM(@HTMLText))

END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'TF', N'FN') AND name = 'IsNormalAssetType')
DROP FUNCTION IsNormalAssetType
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 11/02/2013
-- Description:	
-- =============================================
CREATE FUNCTION IsNormalAssetType 
(
	-- Add the parameters for the function here
	@DynEntityConfigId bigint
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @IsNormalAssetType bit = 0;

	-- Check if item/asset type is normal/data
	SELECT @IsNormalAssetType = CASE COUNT(1) WHEN 0 THEN 0 ELSE 1 END
	  FROM DynEntityAttribConfig ac
				INNER JOIN DynEntityConfig c ON c.DynEntityConfigUid = ac.DynEntityConfigUid
				       AND c.ActiveVersion = 1
	 WHERE c.DynEntityConfigId = @DynEntityConfigId
	   AND ac.ActiveVersion = 1
	   AND ac.Name = 'Base Location'
	
	-- Return the result of the function
	RETURN @IsNormalAssetType

END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type in (N'TF', N'FN') AND name = 'GetParentChildDynListUids')
DROP FUNCTION GetParentChildDynListUids
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 03/02/2013
-- Description:	
-- =============================================
CREATE FUNCTION GetParentChildDynListUids
(	
	-- Add the parameters for the function here
	@ParentDynListUid bigint
)
RETURNS @DynListUids TABLE 
(
	DynListUid bigint
)
AS
BEGIN
	DECLARE @DynListUid bigint;
	DECLARE @t TABLE (DynListUid bigint);
	DECLARE @rowcount int;
	DECLARE @i int = 1;

	INSERT @DynListUids VALUES (@ParentDynListUid)
	INSERT @t SELECT DISTINCT AssociatedDynListUid FROM DynListItem WHERE DynListUid = @ParentDynListUid AND NOT AssociatedDynListUid IS NULL
	
	SELECT @rowcount = @@ROWCOUNT
	
	WHILE @i <= @rowcount BEGIN
		SELECT TOP 1 @DynListUid = DynListUid FROM @t
		INSERT @DynListUids SELECT * FROM dbo.GetParentChildDynListUids(@DynListUid)
		
		DELETE FROM @t WHERE DynListUid = @DynListUid 
		SET @i = @i + 1
	END
	
	RETURN
END
GO

IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'DynEntityIdsTableType')
CREATE TYPE DynEntityIdsTableType AS TABLE
(
	id bigint identity(1,1),
	DynEntityUid bigint not null,
	DynEntityId bigint not null,
	DynEntityConfigUid bigint not null
)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name='vwDynListValue' and xtype='V')
DROP VIEW vwDynListValue
GO
CREATE VIEW vwDynListValue
AS
SELECT lv.DynEntityConfigUid, lv.DynEntityAttribConfigUid, lv.AssetUid, lv.DynListItemUid, li.Value, 'en-GB' as CultureName
  FROM DynListValue lv 
			INNER JOIN DynListItem li ON lv.DynListItemUid = li.DynListItemUid
UNION
SELECT lv.DynEntityConfigUid, lv.DynEntityAttribConfigUid, lv.AssetUid, lv.DynListItemUid, sr.ResourceValue, sr.CultureName
  FROM DynListValue lv 
			INNER JOIN DynListItem li ON lv.DynListItemUid = li.DynListItemUid
			INNER JOIN StringResources sr ON sr.ResourceKey = li.Value
GO

-- Update Ilya (revision 1.2.1.28)
if NOT EXISTS(SELECT * FROM sys.columns 
            where Name = N'IsMandatory' and Object_ID = Object_ID(N'BatchAction')) 
ALTER TABLE BatchAction ADD IsMandatory bit NOT NULL DEFAULT 1;
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_cust_RebuildTriggers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[_cust_RebuildTriggers]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans, Ilya Bolkhovsky	
-- Create date: 22/02/2013
-- Description:	Rebuilds triggers of entities table
-- =============================================
CREATE PROCEDURE _cust_RebuildTriggers
	@dynEntityConfigUid bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET FMTONLY OFF;
	
    DECLARE @triggers TABLE (id int identity(1,1), schemaname nvarchar(50), triggername nvarchar(255), tablename nvarchar(255))
	DECLARE @i int = 1,
			@rowcount int,
			@sqlCommand nvarchar(max) = '',
			@schemaname nvarchar(50), 
			@triggername nvarchar(255),
			@tablename nvarchar(144),
			@Fields nvarchar(max);

	DECLARE @CREATETRIGGER_DEFINITION nvarchar(max) = '
	CREATE TRIGGER [dbo].[tr_Insert%%TableName%%RevisionNumber] 
	ON [dbo].[%%TableName%%] INSTEAD OF INSERT 
	AS 
	BEGIN 

		DECLARE @rowcount int = @@ROWCOUNT;
		DECLARE @GetIdentity bit;
		
		-- If just one row, explicitly return the new identity (needed when AFTER INSERT/UPDATE trigger creates extra records)
		IF @rowcount = 1
			SET @GetIdentity = 1
		ELSE
			SET @GetIdentity = 0
			
		SET NOCOUNT ON;

		DECLARE @uid bigint;
		DECLARE @minId bigint;

		-- Select Min ID. For new records (DynEntityId = 0), temporary a negative ID will generated to not break 
		-- the unique index IX_%%TableName%%_Id_Revision
		SELECT @minId = ISNULL(MIN(DynEntityId), 0) FROM %%TableName%% (READUNCOMMITTED)
		SET @minId = (ABS(@minId) + 1000000) * (-1);

		-- In a temp table, retrieve the latest revision for each item
		WITH temp (Id, LastRev) AS
		(
			SELECT DynEntityId, MAX(Revision) 
			  FROM %%TableName%% (ROWLOCK)
			 WHERE DynEntityId IN (SELECT DynEntityId FROM Inserted)
			 GROUP BY DynEntityId
		)
		-- Insert items. For new items, create a temporary negative DynEntityId to remain unique on DynEntityId|ActiveVersion
		INSERT %%TableName%% (DynEntityId,ActiveVersion,DynEntityConfigUid,Name,Revision,%%Fields%%)
		SELECT CASE DynEntityId WHEN 0 THEN @minId-ROW_NUMBER() OVER (ORDER BY DynEntityId) ELSE DynEntityId END,ActiveVersion,DynEntityConfigUid,Name,
			   ISNULL(t.LastRev, 0) +1,%%Fields%%
		  FROM INSERTED i
					LEFT OUTER JOIN temp t ON i.DynEntityId = t.Id

		-- Get the identity ID (only when one record is inserted - from the framework)
		IF @GetIdentity = 1
			SET @uid = Scope_Identity();	

		-- Update DynEntityId for new items
		UPDATE %%TableName%% SET DynEntityId = DynEntityUid 
		 WHERE DynEntityId BETWEEN (@minId - @rowcount) AND @minId 
		 

		-- The following statements MUST be last in this trigger. It resets @@Identity
		-- to be the same as the earlier Scope_Identity() value.
		-- http://stackoverflow.com/questions/908257/instead-of-trigger-in-sql-server-loses-scope-identity	
		IF @GetIdentity = 1
		BEGIN	
			SELECT DynEntityUid INTO #Trash FROM [%%TableName%%] WHERE DynEntityUid=@uid;
			DROP TABLE #Trash;	
		END

	END;
	'
	
	-- Save triggers in temp table ...
	INSERT @triggers
	SELECT tbl.name AS [schemaName], trg.name AS triggerName, tbl.tblname AS [tablename]
	 FROM sys.triggers trg
	 INNER JOIN DynEntityConfig dc ON dc.DynEntityConfigUid = @dynEntityConfigUid
				LEFT OUTER JOIN 
					(	SELECT tparent.object_id, ts.name, tparent.name AS tblname 
						  FROM sys.tables tparent 
									INNER JOIN sys.schemas ts ON TS.schema_id = tparent.SCHEMA_ID
					) AS tbl ON tbl.OBJECT_ID = trg.parent_id
     
	 WHERE tbl.tblname = dc.DBTableName AND trg.name NOT IN ('tr_SetADynEntityBUBDossierID','tr_SetADynEntityPersonID','tr_SetDynEntityId_ADynEntityBUB_Certificates_B','tr_SetDynEntityId_ADynEntitySOB_Certificates_B')

	SELECT @rowcount = @@ROWCOUNT

	-- Loop all triggers and build dynamic sql ...
	WHILE @i <= @rowcount BEGIN
		SELECT TOP 1 @schemaname = schemaname, @triggername = triggername FROM @triggers WHERE id = @i
		SET @sqlCommand = @sqlCommand + 'DROP TRIGGER [' + @schemaname + '].[' + @triggername + ']; ' 
		
		SET @i = @i + 1
	END

	-- Delete all triggers ...
	EXEC sp_executesql @sqlCommand

	-- Add new triggers ...
	SET @i = 1;
	WHILE @i <= @rowcount BEGIN
		SELECT TOP 1 @tablename = tablename FROM @triggers WHERE id = @i
		
		SET @sqlCommand = '
		SELECT @Fields = 
			STUFF
			(
				(	SELECT '',['' + c.name + '']''
					  FROM sys.tables t
							INNER JOIN sys.columns c ON t.OBJECT_ID = c.OBJECT_ID
					 WHERE t.Name = ''' + @tablename + '''
					   AND c.Name NOT IN (''DynEntityUid'',''DynEntityId'',''ActiveVersion'',''DynEntityConfigUid'',''Name'',''Revision'')
					   FOR XML PATH (''''), TYPE
				).value(''.'', ''nvarchar(max)'')
			,1,1,'''') '
		EXEC sp_executesql @sqlCommand, N'@Fields nvarchar(max) OUTPUT', @Fields = @Fields OUTPUT
		
		SET @sqlCommand = REPLACE(REPLACE(@CREATETRIGGER_DEFINITION, '%%TableName%%', @tablename), '%%Fields%%', @Fields)
		EXEC sp_executesql @sqlCommand	
		SET @i = @i + 1
	END 

END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_cust_AlterTable]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[_cust_AlterTable]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[_cust_AlterTable]
@dynEntityConfigUid bigint,
@column_name nvarchar (50),
@column_datatype nvarchar (100),
@column_isnull bit,
@column_default nvarchar (50)
AS
BEGIN
	DECLARE @sqlScriptAdd nvarchar(2000),
			@sqlScriptAlter nvarchar(2000),
			@table_name nvarchar (50);

	SELECT @table_name = DBTableName FROM DynEntityConfig WHERE DynEntityConfigUid = @dynEntityConfigUid;
	
	IF (EXISTS (SELECT * FROM  information_schema.TABLES 
				WHERE TABLE_NAME = @table_name))
	BEGIN	
		
		-- rename original column
		IF (EXISTS (SELECT * FROM  information_schema.COLUMNS 
					WHERE table_name = @table_name 
					AND column_name = @column_name))
		BEGIN 	
			DECLARE @full_table_name varchar(200);
			SET @full_table_name = @table_name + '.' + @column_name;
				
			DECLARE @new_column_name varchar(50);
			DECLARE @columns_count int;
				
			SELECT @columns_count = COUNT(*) FROM  information_schema.COLUMNS 
			WHERE table_name = @table_name 
			AND (column_name = @column_name OR column_name LIKE  @column_name + '_OLD%');
				
			SET @new_column_name = @column_name +'_OLD' + CAST(@columns_count AS varchar(3));
			EXEC sp_rename @full_table_name, @new_column_name, 'COLUMN';

			SET @sqlScriptAlter = ' ALTER TABLE ' + @table_name +  ' ALTER COLUMN ' + @new_column_name + ' ' + @column_datatype + ' NULL ';					  
			EXEC (@sqlScriptAlter);
			
			-- update the config when OLD DB-fields are created to support index and versioning
			UPDATE DynEntityAttribConfig
			SET DBTableFieldname = @new_column_name
			WHERE DBTableFieldname = @column_name			
			AND DynEntityConfigUid IN (
				SELECT DynEntityConfigUid 
				FROM DynEntityConfig 
				WHERE DBTableName = @table_name
				AND DynEntityConfigUid != @dynEntityConfigUid
			);
		END			
				
		-- add new column
		DECLARE @nulldef varchar (20);
		IF @column_isnull = 1
		BEGIN
			SET @nulldef = ' NULL ';
		END
		ELSE			
		BEGIN
			SET @nulldef = ' NOT NULL ';
		END
		IF (LEN(@column_default) > 0)
		BEGIN			
			SET @sqlScriptAlter = ' ALTER TABLE ' + @table_name +  
								  ' ADD "' + @column_name + '" ' + @column_datatype + @nulldef +								   
								  ' DEFAULT ' + @column_default;
		END	
		ELSE
		BEGIN
			SET @sqlScriptAlter = ' ALTER TABLE ' + @table_name +  
								  '	ADD "' + @column_name + '" ' + @column_datatype  + @nulldef;
		END								  
		EXEC (@sqlScriptAlter)

	END
END
GO

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = '_search_srchres'))
DROP TABLE _search_srchres
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[_search_srchres](
	[SearchId] [bigint] NULL,
	[UserId] [bigint] NULL,
	[IndexUid] [bigint] NULL,
	[Active] [bit] NULL,
	[DynEntityConfigId] [bigint] NULL,
	[TaxonomyItemsIds] [nvarchar](1000) NULL,
	[rownumber] [int] NULL,
	[SearchDateTimeStamp] [datetime] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[_search_srchres] ADD  DEFAULT (getdate()) FOR [SearchDateTimeStamp]
GO

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = '_search_srchcount'))
DROP TABLE _search_srchcount
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[_search_srchcount](
	[SearchId] [bigint] NULL,
	[UserId] [bigint] NULL,
	[Type] [varchar](50) NULL,
	[id] [bigint] NULL,
	[Count] [int] NULL,
	[SearchDateTimeStamp] [datetime] NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[_search_srchcount] ADD  DEFAULT (getdate()) FOR [SearchDateTimeStamp]
GO

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = '_search_srchTypeContext'))
DROP TABLE _search_srchTypeContext
GO

CREATE TABLE _search_srchTypeContext (
	[SearchId] [bigint] NOT NULL,
	[DynEntityUid]  [bigint] NOT NULL,
	[DynEntityConfigUid]  [bigint] NOT NULL
);
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author		: Ilya Bolkhovsky, Wouter Steegmans
-- Create date	: 16.08.2012
-- Description	: Performs an initial search by keywords
-- =============================================
ALTER PROCEDURE [dbo].[_cust_SearchByTypeContext_Active] 
        @SearchId bigint,
        -- Id of current user (required)
        @UserId bigint,
        -- Extra Select for Search By Type/Context          
        @ConfigIds varchar(MAX) = NULL,
        --  Space-separated list of Taxonomies (optional parameter)
        @taxonomyItemsIds varchar(MAX) = NULL,
        -- Search area
        @active bit = 1, 
        @orderby tinyint = 1,
        -- Pagination support (optional parameter, defaults will be set),
        @SearchBufferCount int = 0,
        -- Search buffer, number of records to load at once
        @SearchBufferSize int = 400        
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;

		-- CTE, First (Search) full text filtering on the keywords
		WITH Search AS
		(	
				SELECT *, ROW_NUMBER() OVER(ORDER BY	CASE WHEN @orderby = 0 THEN Name END,        
														CASE WHEN @orderby = 1 THEN UpdateDate END DESC,
														CASE WHEN @orderby = 2 THEN Location END ASC,
														CASE WHEN @orderby = 3 THEN [User] END ASC/*, Name*/) AS RowNumber                			
				FROM 
				(
					SELECT IndexUid, UserId, OwnerId, DynEntityConfigId, DepartmentId, TaxonomyItemsIds, UpdateDate, Location, [User], 1 AS Active, Name
						FROM IndexActiveDynEntities i
								INNER JOIN _search_srchTypeContext r ON (i.DynEntityUid = r.DynEntityUid AND i.DynEntityConfigUid = r.DynEntityConfigUid)
								       AND r.SearchId = @SearchId
						WHERE   
/*
						-- In the future, if search performance has to be optimized, an option is to 
						-- cache the user permissions in a (temp) table with lay-out Userid, Active, IndexUid
						IndexUid IN (SELECT IndexUid FROM ##test WHERE UserId = @UserId) 
*/ 
					   (	
								DynEntityConfigId IN (SELECT * FROM f_GetGrantedSearchConfigIds(@UserId, 
																	CONVERT(bigint, @ConfigIds), CONVERT(bigint, @taxonomyItemsIds)))
								OR	
								(
									(
											DepartmentId IN (SELECT * FROM f_GetGrantedSearchDepartmentIds(@UserId, NULL))
										OR	OwnerId IN (SELECT * FROM f_GetGrantedSearchUserIds(@UserId)) 
										OR  UserId IN (SELECT * FROM f_GetGrantedSearchUserIds(@UserId)) 
									)
									AND
									(	@ConfigIds IS NULL AND @taxonomyItemsIds IS NULL)
								)										
						)										
				) AS SearchResults
		)
        
		-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
		INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
		SELECT TOP (@SearchBufferSize) @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
		  FROM Search
		 WHERE RowNumber > @SearchBufferCount	 
		

END
GO

-- =============================================
-- Author		: Wouter Steegmans
-- Create date	: 21.08.2012
-- Description	: Performs an initial search by keywords
-- =============================================
ALTER PROCEDURE [dbo].[_cust_SearchByTypeContext] 
        @SearchId bigint,
        -- Id of current user (required)
        @UserId bigint,
        --  Space-separated list of ConfigIds - assettype (optional parameter)
        @ConfigIds varchar(MAX) = NULL,
        --  Space-separated list of Taxonomies (optional parameter)
        @taxonomyItemsIds varchar(MAX) = NULL,
        -- Search area
        @active bit = 1, 
        -- OrderBy field (enumeration in code, optional parameter, defaults will be set)
        -- 0 - by relevance (rank)
        -- 1 - by date
        -- 2 - by location
        -- 3 - by user
        @orderby tinyint = 1,
        -- Pagination support (optional parameter, defaults will be set),
        @PageNumber int = 1, 
        @PageSize int = 20
AS
BEGIN

/* -- As illustration. The selectByTypeContext must always give a result set with DynEntityUid, DynEntityConfigUid
			SELECT DynEntityUid, DynEntityConfigUid 
			  FROM ADynEntityBUBDossier
			 WHERE ADynEntityBUBDossier.Name like '%michel%'
			   AND ActiveVersion = 1
*/

        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;
		SET FMTONLY OFF;

		-- Number of rows returned!
		DECLARE @SearchBufferCount int = 0;
		DECLARE @SearchBufferSize int = 400;
		
		IF @PageSize > 400 BEGIN
			SET @SearchBufferSize = @PageSize
		END		
				
		-- Check ConfigIds and TaxonomyItemsIds
        IF @ConfigIds = '' 
			SET @ConfigIds = NULL;
		IF @taxonomyItemsIds = ''
			SET @taxonomyItemsIds = NULL;		

        -- Check if results are already saved in temp table, so search can be skipped ...
		SELECT @SearchBufferCount = COUNT(1) FROM _search_srchres WHERE SearchId = @SearchId AND UserId = @UserId
		-- Check if there are still records in the temp table for the next page
		IF (@SearchBufferCount / @PageSize) >= @PageNumber
		BEGIN			
			EXEC _cust_GetSrchResPage @SearchId, @UserId, @active, @PageNumber, @PageSize
			RETURN
		END

		-- Search active assets
		IF @active = 1
		BEGIN
			EXEC _cust_SearchByTypeContext_Active @SearchId, @UserId, @ConfigIds, @taxonomyItemsIds, @active, @orderby, @SearchBufferCount, @SearchBufferSize
		END

		-- Search history
		IF @active = 0
		BEGIN
			EXEC _cust_SearchByTypeContext_History @SearchId, @UserId, @ConfigIds, @taxonomyItemsIds, @active, @orderby, @SearchBufferCount, @SearchBufferSize
		END

		-- Get the results and return ...
        EXEC _cust_GetSrchResPage @SearchId, @UserId, @active, @PageNumber, @PageSize;
        
        SET ROWCOUNT 0;

END
GO

-- =============================================
-- Author		: Ilya Bolkhovsky, Wouter Steegmans
-- Create date	: 16.08.2012
-- Description	: Performs an initial search by keywords
-- =============================================
ALTER PROCEDURE [dbo].[_cust_SearchByKeywords_History] 
        @SearchId bigint,
        -- Id of current user (required)
        @UserId bigint,
        -- Space-separated list of keywords (required parameter). Ex.: '"item*" AND "111*"'     
        @keywords nvarchar(1000),
        --  Space-separated list of ConfigIds - assettype (optional parameter)
        @ConfigIds varchar(MAX) = NULL,
        --  Space-separated list of Taxonomies (optional parameter)
        @taxonomyItemsIds varchar(MAX) = NULL,
        -- Search area
        @active bit = 1, 
        @orderby tinyint = 1,
        -- Pagination support (optional parameter, defaults will be set),
        @SearchBufferCount int = 0,
        -- Search buffer, number of records to load at once
        @SearchBufferSize int = 400
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;

		-- CTE, First (Search) full text filtering on the keywords
		WITH Search AS
		(	
				SELECT *, ROW_NUMBER() OVER(ORDER BY	CASE WHEN @orderby = 0 THEN [Rank] END DESC,        
														CASE WHEN @orderby = 1 THEN UpdateDate END DESC,
														CASE WHEN @orderby = 2 THEN Location END ASC,
														CASE WHEN @orderby = 3 THEN [User] END ASC, Name) AS RowNumber                			
				FROM 
				(
					SELECT IndexUid, UserId, OwnerId, DynEntityConfigId, DepartmentId, TaxonomyItemsIds, UpdateDate, Location, [User], 0 AS Active, Name,
						(       
							KEY_TBL1.RANK +						/* AllAttribValuesRank */
							(ISNULL(KEY_TBL2.RANK, 0) * 10) +   /* AllAttrib2IndexValuesRank */
							(ISNULL(KEY_TBL3.RANK, 0) * 100) +  /* AllContextAttribValues */
							(CASE WHEN ISNULL(KEY_TBL4.RANK, 0) > 0 THEN KEY_TBL4.RANK + 10000 ELSE 0 END) +	/* EntityConfigKeywordsRank */
							(CASE WHEN ISNULL(KEY_TBL5.RANK, 0) > 0 THEN KEY_TBL5.RANK + 100000 ELSE 0 END) +   /* KeywordsRank */
							(CASE WHEN ISNULL(KEY_TBL6.RANK, 0) > 0 THEN KEY_TBL6.RANK + 1000000 ELSE 0 END)    /* NameRank */          
						) AS [Rank]
						FROM IndexHistoryDynEntities AS FT_TBL 
								 JOIN CONTAINSTABLE(IndexHistoryDynEntities, (AllAttribValues), @keywords) AS KEY_TBL1 ON FT_TBL.IndexUid = KEY_TBL1.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexHistoryDynEntities, (AllContextAttribValues), @keywords) AS KEY_TBL2 ON FT_TBL.IndexUid = KEY_TBL2.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexHistoryDynEntities, (AllAttrib2IndexValues), @keywords) AS KEY_TBL3 ON FT_TBL.IndexUid = KEY_TBL3.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexHistoryDynEntities, (EntityConfigKeywords), @keywords) AS KEY_TBL4 ON FT_TBL.IndexUid = KEY_TBL4.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexHistoryDynEntities, (Keywords), @keywords) AS KEY_TBL5 ON FT_TBL.IndexUid = KEY_TBL5.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexHistoryDynEntities, (Name), @keywords) AS KEY_TBL6 ON FT_TBL.IndexUid = KEY_TBL6.[KEY]
						WHERE   
/*
						-- In the future, if search performance has to be optimized, an option is to 
						-- cache the user permissions in a (temp) table with lay-out Userid, Active, IndexUid
						IndexUid IN (SELECT IndexUid FROM ##test WHERE UserId = @UserId) 
*/ 
					   (	
								DynEntityConfigId IN (SELECT * FROM f_GetGrantedSearchConfigIds(@UserId, 
																	CONVERT(bigint, @ConfigIds), CONVERT(bigint, @taxonomyItemsIds)))
								OR	
								(
									(
											DepartmentId IN (SELECT * FROM f_GetGrantedSearchDepartmentIds(@UserId, NULL))
										OR	OwnerId IN (SELECT * FROM f_GetGrantedSearchUserIds(@UserId)) 
										OR  UserId IN (SELECT * FROM f_GetGrantedSearchUserIds(@UserId)) 
									)
									AND
									(	@ConfigIds IS NULL AND @taxonomyItemsIds IS NULL)
								)										
						)
																						
				) AS SearchResults
		)
        
		-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
		INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
		SELECT TOP(@SearchBufferSize) @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
		  FROM Search
		 WHERE RowNumber > @SearchBufferCount

END
GO

-- =============================================
-- Author		: Ilya Bolkhovsky, Wouter Steegmans
-- Create date	: 16.08.2012
-- Description	: Performs an initial search by keywords
-- =============================================
ALTER PROCEDURE [dbo].[_cust_SearchByKeywords_Active] 
        @SearchId bigint,
        -- Id of current user (required)
        @UserId bigint,
        -- Space-separated list of keywords (required parameter). Ex.: '"item*" AND "111*"'     
        @keywords nvarchar(1000),
        --  Space-separated list of ConfigIds - assettype (optional parameter)
        @ConfigIds varchar(MAX) = NULL,
        --  Space-separated list of Taxonomies (optional parameter)
        @taxonomyItemsIds varchar(MAX) = NULL,
        -- Search area
        @active bit = 1, 
        @orderby tinyint = 1,
        -- Pagination support (optional parameter, defaults will be set),
        @SearchBufferCount int = 0,
        -- Search buffer, number of records to load at once
        @SearchBufferSize int = 400        
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;

		-- CTE, First (Search) full text filtering on the keywords
		WITH Search AS
		(	
				SELECT *, ROW_NUMBER() OVER(ORDER BY	CASE WHEN @orderby = 0 THEN [Rank] END DESC,        
														CASE WHEN @orderby = 1 THEN UpdateDate END DESC,
														CASE WHEN @orderby = 2 THEN Location END ASC,
														CASE WHEN @orderby = 3 THEN [User] END ASC, Name) AS RowNumber                			
				FROM 
				(
					SELECT IndexUid, UserId, OwnerId, DynEntityConfigId, DepartmentId, TaxonomyItemsIds, UpdateDate, Location, [User], 1 AS Active, Name,
						(       
							KEY_TBL1.RANK +						/* AllAttribValuesRank */
							(ISNULL(KEY_TBL2.RANK, 0) * 10) +   /* AllAttrib2IndexValuesRank */
							(ISNULL(KEY_TBL3.RANK, 0) * 100) +  /* AllContextAttribValues */
							(CASE WHEN ISNULL(KEY_TBL4.RANK, 0) > 0 THEN KEY_TBL4.RANK + 10000 ELSE 0 END) +	/* EntityConfigKeywordsRank */
							(CASE WHEN ISNULL(KEY_TBL5.RANK, 0) > 0 THEN KEY_TBL5.RANK + 100000 ELSE 0 END) +   /* KeywordsRank */
							(CASE WHEN ISNULL(KEY_TBL6.RANK, 0) > 0 THEN KEY_TBL6.RANK + 1000000 ELSE 0 END)    /* NameRank */          
						) AS [Rank]
						FROM IndexActiveDynEntities AS FT_TBL 
						
								 JOIN CONTAINSTABLE(IndexActiveDynEntities, (AllAttribValues), @keywords) AS KEY_TBL1 ON FT_TBL.IndexUid = KEY_TBL1.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexActiveDynEntities, (AllContextAttribValues), @keywords) AS KEY_TBL2 ON FT_TBL.IndexUid = KEY_TBL2.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexActiveDynEntities, (AllAttrib2IndexValues), @keywords) AS KEY_TBL3 ON FT_TBL.IndexUid = KEY_TBL3.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexActiveDynEntities, (EntityConfigKeywords), @keywords) AS KEY_TBL4 ON FT_TBL.IndexUid = KEY_TBL4.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexActiveDynEntities, (Keywords), @keywords) AS KEY_TBL5 ON FT_TBL.IndexUid = KEY_TBL5.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexActiveDynEntities, (Name), @keywords) AS KEY_TBL6 ON FT_TBL.IndexUid = KEY_TBL6.[KEY]
						WHERE   
/*
						-- In the future, if search performance has to be optimized, an option is to 
						-- cache the user permissions in a (temp) table with lay-out Userid, Active, IndexUid
						IndexUid IN (SELECT IndexUid FROM ##test WHERE UserId = @UserId) 
*/ 
					   (	
								DynEntityConfigId IN (SELECT * FROM f_GetGrantedSearchConfigIds(@UserId, 
																	CONVERT(bigint, @ConfigIds), CONVERT(bigint, @taxonomyItemsIds)))
								OR	
								(
									(
											DepartmentId IN (SELECT * FROM f_GetGrantedSearchDepartmentIds(@UserId, NULL))
										OR	OwnerId IN (SELECT * FROM f_GetGrantedSearchUserIds(@UserId)) 
										OR  UserId IN (SELECT * FROM f_GetGrantedSearchUserIds(@UserId)) 
									)
									AND
									(	@ConfigIds IS NULL AND @taxonomyItemsIds IS NULL)
								)										
						)
																
				) AS SearchResults
		)
        
		-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
		INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
		SELECT TOP(@SearchBufferSize) @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
		  FROM Search
		 WHERE RowNumber > @SearchBufferCount

END
GO

-- =============================================
-- Author		: Ilya Bolkhovsky, Wouter Steegmans
-- Create date	: 16.08.2012
-- Description	: Performs an initial search by keywords
-- =============================================
ALTER PROCEDURE [dbo].[_cust_SearchByKeywords] 
        @SearchId bigint,
        -- Id of current user (required)
        @UserId bigint,
        -- Space-separated list of keywords (required parameter). Ex.: '"item*" AND "111*"'     
        @keywords nvarchar(1000),
        --  Space-separated list of ConfigIds - assettype (optional parameter)
        @ConfigIds varchar(MAX) = NULL,
        --  Space-separated list of Taxonomies (optional parameter)
        @taxonomyItemsIds varchar(MAX) = NULL,
        -- Search area
        @active bit = 1, 
        -- OrderBy field (enumeration in code, optional parameter, defaults will be set)
        -- 0 - by relevance (rank)
        -- 1 - by date
        -- 2 - by location
        -- 3 - by user
        @orderby tinyint = 1,
        -- Pagination support (optional parameter, defaults will be set),
        @PageNumber int = 1, 
        @PageSize int = 20
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;

		-- Number of rows returned!
		DECLARE @SearchBufferCount int = 0;
		DECLARE @SearchBufferSize int = 400;
		
		IF @PageSize > 400 BEGIN
			SET @SearchBufferSize = @PageSize
		END
		
		-- Check ConfigIds and TaxonomyItemsIds
        IF @ConfigIds = '' 
			SET @ConfigIds = NULL;
		IF @taxonomyItemsIds = ''
			SET @taxonomyItemsIds = NULL;		

        -- Check if results are already saved in temp table, so search can be skipped ...
		SELECT @SearchBufferCount = COUNT(1) FROM _search_srchres WHERE SearchId = @SearchId AND UserId = @UserId
		-- Check if there are still records in the temp table for the next page
		IF (@SearchBufferCount / @PageSize) >= @PageNumber
		BEGIN							
			EXEC _cust_GetSrchResPage @SearchId, @UserId, @active, @PageNumber, @PageSize
			RETURN
		END

		-- Check for customized queries
		DECLARE @IsCustomQuery bit = 0;
		EXEC _cust_SearchCustomQuery @SearchId, @UserId, @keywords, @ConfigIds, @taxonomyItemsIds, @SearchBufferCount, @IsCustomQuery OUTPUT

		IF NOT @IsCustomQuery = 1
		BEGIN
		
			-- Analyse keywords
			IF CHARINDEX('*', @keywords) = 0
			BEGIN
				SET @keywords = '"' + REPLACE(@keywords, ' ', '*" AND "') + '*"';
			END		
			
			-- Search active assets
			IF @active = 1
			BEGIN
				EXEC _cust_SearchByKeywords_Active @SearchId, @UserId, @keywords, @ConfigIds, @taxonomyItemsIds, @active, @orderby, @SearchBufferCount, @SearchBufferSize
			END

			-- Search history
			IF @active = 0
			BEGIN
				EXEC _cust_SearchByKeywords_History @SearchId, @UserId, @keywords, @ConfigIds, @taxonomyItemsIds, @active, @orderby, @SearchBufferCount, @SearchBufferSize
			END
		END
		
		-- Get the results and return ...
        EXEC _cust_GetSrchResPage @SearchId, @UserId, @active, @PageNumber, @PageSize;
   
        SET ROWCOUNT 0;

END
GO

-- =============================================
-- Author		: Ilya Bolkhovsky, Wouter Steegmans
-- Create date	: 16.08.2012
-- Description	: Performs an initial search by type/context
-- =============================================
ALTER PROCEDURE [dbo].[_cust_SearchByTypeContext_History] 
        @SearchId bigint,
        -- Id of current user (required)
        @UserId bigint,
        --  Space-separated list of ConfigIds - assettype (optional parameter)
        @ConfigIds varchar(MAX) = NULL,
        --  Space-separated list of Taxonomies (optional parameter)
        @taxonomyItemsIds varchar(MAX) = NULL,
        -- Search area
        @active bit = 0, 
        @orderby tinyint = 1,
        -- Pagination support (optional parameter, defaults will be set),
        @SearchBufferCount int = 0,
        -- Search buffer, number of records to load at once
        @SearchBufferSize int = 400        
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;

		-- CTE, First (Search) full text filtering on the keywords
		WITH Search AS
		(	
				SELECT *, ROW_NUMBER() OVER(ORDER BY	CASE WHEN @orderby = 0 THEN Name END,        
														CASE WHEN @orderby = 1 THEN UpdateDate END DESC,
														CASE WHEN @orderby = 2 THEN Location END ASC,
														CASE WHEN @orderby = 3 THEN [User] END ASC/*, Name*/) AS RowNumber                			
				FROM 
				(
					SELECT IndexUid, UserId, OwnerId, DynEntityConfigId, DepartmentId, TaxonomyItemsIds, UpdateDate, Location, [User], 0 AS Active, Name
						FROM IndexHistoryDynEntities i
								INNER JOIN _search_srchTypeContext r ON (i.DynEntityUid = r.DynEntityUid AND i.DynEntityConfigUid = r.DynEntityConfigUid)
									   AND r.SearchId = @SearchId
						WHERE   
/*
						-- In the future, if search performance has to be optimized, an option is to 
						-- cache the user permissions in a (temp) table with lay-out Userid, Active, IndexUid
						IndexUid IN (SELECT IndexUid FROM ##test WHERE UserId = @UserId) 
*/ 
					   (	
								DynEntityConfigId IN (SELECT * FROM f_GetGrantedSearchConfigIds(@UserId, 
																	CONVERT(bigint, @ConfigIds), CONVERT(bigint, @taxonomyItemsIds)))
								OR	
								(
									(
											DepartmentId IN (SELECT * FROM f_GetGrantedSearchDepartmentIds(@UserId, NULL))
										OR	OwnerId IN (SELECT * FROM f_GetGrantedSearchUserIds(@UserId)) 
										OR  UserId IN (SELECT * FROM f_GetGrantedSearchUserIds(@UserId)) 
									)
									AND
									(	@ConfigIds IS NULL AND @taxonomyItemsIds IS NULL)
								)										
						)										
				) AS SearchResults
		)
        
		-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
		INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
		SELECT TOP(@SearchBufferSize) @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
		  FROM Search
		 WHERE RowNumber > @SearchBufferCount
		
		
END
GO

/**********************************************************************************************************************
*********************************************** Updates specific for S&B **********************************************
**********************************************************************************************************************/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_cust_UpdateBUBPayment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[_cust_UpdateBUBPayment]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 03/03/2013
-- Description:	
-- =============================================
CREATE PROCEDURE _cust_UpdateBUBPayment 
	@PayDynEntityId bigint,
	@Amount money,
	@Date_from datetime,
	@Date_until datetime,
	@PaymentDate datetime	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @tmp AS DynEntityIdsTableType

	INSERT @tmp (DynEntityUid, DynEntityId, DynEntityConfigUid)
	SELECT DynEntityUid, DynEntityId, DynEntityConfigUid 
	  FROM ADynEntityBUBPayment 
	 WHERE DynEntityId = @PayDynEntityId
	   AND ActiveVersion = 1

	EXEC _cust_CreateNewRevision @entities = @tmp

	UPDATE ADynEntityBUBPayment
	   SET Amount = @Amount,
		   Date_from = @Date_from,
		   Date_until = @Date_until,
		   Payment_date = @PaymentDate
	 WHERE DynEntityId  = @PayDynEntityId
	AND ActiveVersion = 1
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_cust_UpdateSOBPayment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[_cust_UpdateSOBPayment]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 03/03/2013
-- Description:	
-- =============================================
CREATE PROCEDURE _cust_UpdateSOBPayment 
	@PayDynEntityId bigint,
	@Amount money,
	@Days int,
	@PaymentDate datetime	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @tmp AS DynEntityIdsTableType;

	INSERT @tmp (DynEntityUid, DynEntityId, DynEntityConfigUid)
	SELECT DynEntityUid, DynEntityId, DynEntityConfigUid 
	  FROM ADynEntitySOBPayment 
	 WHERE DynEntityId = @PayDynEntityId
	   AND ActiveVersion = 1

	EXEC _cust_CreateNewRevision @entities = @tmp

	UPDATE ADynEntitySOBPayment
	   SET Amount = @Amount,
		   Days = @Days,
		   Payment_date = @PaymentDate
	 WHERE DynEntityId  = @PayDynEntityId
	AND ActiveVersion = 1
	
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_cust_BUB_CreateCertificates]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[_cust_BUB_CreateCertificates]
GO

-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 29/09/2011
-- Description:	Remove ADynEntity table
-- =============================================

CREATE PROCEDURE [dbo].[_cust_BUB_CreateCertificates]
	
	(
	@DynEntityUid bigint,
	@DynEntityConfigUid bigint,
	@DynEntityConfigId bigint
	)
	
AS
	SET NOCOUNT ON 

	DECLARE @claimToAmount money;
	DECLARE @expectedAmount money;
	DECLARE @personId bigint;
	DECLARE @ill bit;
	DECLARE @bub5254 bit;
	DECLARE @dossierName nvarchar(MAX);
	DECLARE @dossierNumber bigint;
	DECLARE @dossierEntityId bigint;
	DECLARE @unemployedSince date;
	DECLARE @Q int;
	DECLARE @S int;
	DECLARE @comments nvarchar(MAX);
	DECLARE @State nvarchar(255) = NULL;
	DECLARE @TemporaryCert bit = 0;

	DECLARE @retirementAge int;
	DECLARE @birthdate date;
	DECLARE @retirementdate date;
	DECLARE @gender char;
	DECLARE @age int;
	
	DECLARE @SkipRecords int = 0;

	-- Get Dossier record
	SELECT @claimToAmount=Claim_to, @expectedAmount = Expected, @personId=Person, @ill=IllO, @bub5254=BUB_52_54, 
		   @dossierNumber=Dossier_Number, @dossierName=[Name], @dossierEntityId=DynEntityId, @unemployedSince=Employed_since, 
		   @comments=Comments, @Q=CASE Q_hours_a_week WHEN 0 THEN 40 ELSE Q_hours_a_week END,
		   @S=CASE S_Fulltime_regime WHEN 0 THEN 40 ELSE S_Fulltime_regime END
	  FROM ADynEntityBUBDossier
	 WHERE DynEntityUid = @DynEntityUid
	   AND ActiveVersion = 1;
	   
	IF @@ROWCOUNT = 0
		RETURN;	-- Could mean already an update is done for the BUBDossier, so UID is not the active version anymore.

	-- Get the State for the BUBDossier. If not available (yet), this means that the S&B/AM system hasn't saved it yet ...
	WHILE @State IS NULL
	BEGIN
		WAITFOR DELAY '0:00:00.500';

		SELECT @State = dli.Value 
		  FROM DynListValue dlv
					INNER JOIN DynListItem dli ON dlv.DynListItemUid = dli.DynListItemUid
					INNER JOIN DynEntityConfig c ON c.DynEntityConfigUid = dlv.DynEntityConfigUid
					INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityAttribConfigUid = dlv.DynEntityAttribConfigUid
		 WHERE AssetUid = @DynEntityUid
		   AND c.DynEntityConfigUid = @DynEntityConfigUid
		   AND ac.Name = 'State'		   
	END

	-- Validate state of dossier, if Membership OK ...
	IF @State = 'Membership OK' 
	BEGIN
		-- If at least 1 certificate exists -> exit
		IF EXISTS (SELECT 1 FROM ADynEntityBUBCertificate WHERE Dossier = @dossierEntityId)
			RETURN;
		ELSE
		-- No certificates created yet, so create one temporary certificate
		BEGIN
			SET @claimToAmount = @expectedAmount;
			SET @TemporaryCert = 1;
		END
	END
		
	-- If running, create final certificates (if not already done)
	IF @State = 'Running' 
	BEGIN
		IF ISNULL(@dossierNumber, 0) = 0 OR ISNULL(@claimToAmount, 0) = 0
			RETURN;
			
		-- If temporary certificate is found, create the final versions ...
		IF EXISTS (SELECT 1 FROM AdynEntityBUBCertificate WHERE Dossier = @dossierEntityId AND Temporary = 1)
		BEGIN
			-- Get the BUBDossier data when the temporary certifite was created. Use the history version for it, 
			-- so the Uid to the BUBDossier can be used. Important for BUB52/54, so monthly/quarter certificates
			DECLARE @hBUB5254 bit = NULL;
			SELECT @hBUB5254=d.BUB_52_54 FROM ADynEntityBUBDossier d
						INNER JOIN ADynEntityBUBCertificate c ON c.Dossier = d.DynEntityUid
			 WHERE c.Revision = 1 AND c.ActiveVersion = 0
			   AND d.DynEntityId = @dossierEntityId
			
			-- If BUB52/54 option is different between temporary certificate and actual
			IF @hBUB5254 <> @bub5254 
			BEGIN
				-- If BUB52/54 was selected when temp cert was created, just ignore this certificate.
				
				-- If for current BUB52/54 = true, skip first 3 certificates (months)
				IF @bub5254 = 1
					SET @SkipRecords = 3;
			END
			ELSE
			BEGIN
				-- Just skip the first certificate (record)
				SET @SkipRecords = 1;
			END
			
			-- Update the temporary certificate and make it final
			UPDATE ADynEntityBUBCertificate 
			   SET Temporary = 0
			 WHERE Dossier = @dossierEntityId
			   AND Temporary = 1
			   AND ActiveVersion = 1;
		END
		ELSE 
			-- If certificates already exist, or final dossier data is missing, exit, otherwise, 
			-- create certificates (no temporary exist).
			IF EXISTS (SELECT 1 FROM AdynEntityBUBCertificate WHERE Dossier = @dossierEntityId AND ISNULL(Temporary, 0) = 0) OR
				NOT ((@claimToAmount > 0 OR @bub5254 = 1) AND @dossierNumber > 0)
			RETURN
	END

	-- Check if Certificates aren't created yet and the Amount to claim is bigger then zero
	IF (@claimToAmount > 0) AND (@State = 'Membership OK' OR @State = 'Running')
	BEGIN
				
		-- Get gender and birthdate (and so the age) from the person linked to the inserted dossier
		SELECT @birthdate=birth_date, @gender=UPPER(gender) FROM ADynEntityPerson
			WHERE DynEntityId = @personId
			AND ActiveVersion = 1;

		-- Check if the birthdate is filled in ...
		IF NOT ((NOT @birthdate IS NULL) AND (NOT @unemployedSince IS NULL))
		BEGIN
			-- If no birthdate or employed since date, a warning is generated in the Comments text of the dossier ...
			   SET @comments = @comments + ' -- AANDACHT!! Attesten aanmaken mislukt. Controleer geboortedatum en datum werkloosheid. Nadat deze aanpassingen gebeurd zijn, verwijderd dan deze melding en sla het dossier opnieuw op. Dit zorgt ervoor dat de attesten opnieuw aangemaakt worden.'
		END
		ELSE BEGIN
			-- Get System variables BUB (retirement age)
			IF @gender='M'
			BEGIN			
				SELECT @retirementAge=Age_retirement 
				  FROM ADynEntityBUBSysVariables
				 WHERE DynEntityId=1 AND ActiveVersion=1;
			END
			ELSE
			BEGIN
				SELECT @retirementAge=Age_retirement_Woman
				  FROM ADynEntityBUBSysVariables
				 WHERE DynEntityId=1 AND ActiveVersion=1;
			END
			-- Compute the age and retirement date 
			-- Retirement date is always first day of next month some has reached the retirment age ...
			SET @age = dbo.GetAgeInYears(@birthdate, @unemployedSince)
			SET @retirementdate=DATEADD(month,1,DATEADD(yy,@retirementAge,@birthdate));
			SET @retirementdate=DATEADD(day,day(@retirementdate),@retirementdate);

			-- Create a temporary table to hold the selected records
			DECLARE @uniqueId bigint
			DECLARE @TEMP TABLE (uniqueId bigint)
 
			-- Insert into the temporary table a list of the records to be updated
			INSERT INTO @TEMP (uniqueId)
			SELECT DynEntityId FROM BUBInstallmentAmount
			 WHERE Age_from <= @age AND Age_until > @age
			   AND IllA=@ill 
			   AND BUB_52_54=@bub5254
			   AND Regulation_from < @unemployedSince
			   AND ISNULL(Regulation_until, @unemployedSince) >= @unemployedSince
			 ORDER BY Seniority_years DESC, From_installment ASC;

			DECLARE @amountProcessed money;
			DECLARE @amount4Certificate money;
			DECLARE @dateFrom date;
			DECLARE @dateUntil date;
			DECLARE @fromInstallment int;
			DECLARE @untilInstallment int;
			DECLARE @interval int;
			DECLARE @maxEachTime money;
			DECLARE @i int;
			DECLARE @DynCertificateEntityConfigUid bigint;
			DECLARE @DynEntityId bigint;
			DECLARE @UpdateDate datetime;
			
			DECLARE @WriteRow bit;

			-- Init variables ...
			SET @amountProcessed = 0;
			SET @dateFrom = @unemployedSince;
			SET @dateUntil = DATEADD(day,-1,DATEADD(month, @interval, @dateFrom));
			SET @UpdateDate = getdate();
			
			SET @WriteRow = 1;

			SELECT @DynCertificateEntityConfigUid=DynEntityConfigUid 
			  FROM DynEntityConfig
			 WHERE DynEntityConfigId = 267;
			 
			-- Select max amount to be paid each time (in case of last payment before a retirement)
			DECLARE @MaxAmountEachTime money;
			SELECT @MaxAmountEachTime = MAX(Max_each_time) FROM ADynEntityBUBAmount;

			-- Start looping through the records from BUBInstallmentAmount
			WHILE (EXISTS (SELECT 1 FROM @TEMP)) AND (@claimToAmount - @amountProcessed > 1) AND (@dateFrom < @retirementdate)
			BEGIN
				-- Grab the first record out of the temp table
				SELECT Top 1 @uniqueId = uniqueId FROM @TEMP;
			
				-- Get the record with Installment and Amount information
				SELECT @fromInstallment=From_installment, @untilInstallment=Until_installment, 
					   @interval=Interval, @maxEachTime=Max_each_time
				  FROM BUBInstallmentAmount
				 WHERE DynEntityId = @uniqueId;
     
				-- Loop from From_Installment until Until_Installment
				SET @i=@fromInstallment;
				WHILE (@i <= @untilInstallment) AND (@claimToAmount - @amountProcessed > 1) AND (@dateFrom < @retirementdate)
				BEGIN
				
					-- Compute the amount for the attest
					SET @amount4Certificate = CONVERT(DECIMAL(6,2), ROUND((@maxEachTime / @S) * @Q, 2));
					IF (@amount4Certificate + @amountProcessed > @claimToAmount)
					BEGIN
						SET @amount4Certificate = @claimToAmount - @amountProcessed;
					END

					SET @amountProcessed = @amountProcessed + @amount4Certificate;
					SET @dateUntil = DATEADD(day,-1,DATEADD(month, @interval, @dateFrom));

					-- Check dateUntil, when after retirementDate, adapt it to a day before the retirement date
					IF (@retirementdate < @dateUntil)
					BEGIN
						-- SET @amount4Certificate = CONVERT(DECIMAL(6,2), ROUND((@amount4Certificate / DATEDIFF(dd, @dateFrom, @dateUntil) + 1) * DATEDIFF(dd, @dateFrom, @retirementdate), 2));
						
						IF (@bub5254 = 1)
						BEGIN
							SET @WriteRow = 0;
						END
						ELSE BEGIN
							SET @amount4Certificate = @claimToAmount - @amountProcessed;
							IF (@amount4Certificate > @MaxAmountEachTime)
							BEGIN
								SET @amount4Certificate = @MaxAmountEachTime;
							END
							SET @dateUntil = DATEADD(day, -1, @retirementdate);
						END
						
					END

					-- Insert certificate
					IF (@amount4Certificate) > 1 AND (@WriteRow = 1) AND (@i > @SkipRecords)
					BEGIN
						IF @TemporaryCert = 1 
						BEGIN
							DECLARE @uid bigint;
							DECLARE @id bigint;
													
							INSERT ADynEntityBUBCertificate (ActiveVersion,DynEntityConfigUid,[Name],UpdateUserId,UpdateDate,Dossier,Date_from,Date_until,Comments, Temporary)
													 VALUES (0, @DynCertificateEntityConfigUid,
															 'Cert. ' + RIGHT('000' + CONVERT(VARCHAR, @i), 3) + ' -- ' + CONVERT(VARCHAR, @dateUntil, 103) + ', ' + @dossierName, 
															 1, @UpdateDate, @DynEntityUid, @dateFrom, @dateUntil, 
															 'Amount: ' + CONVERT(VARCHAR, @amount4Certificate), @TemporaryCert);
							-- Scope_Identity() isn't usable here because of the INSTEAD OF INSERT trigger causing Scope_Identity() returning NULL							
							SET @uid = @@IDENTITY
							
							SELECT @id = DynEntityId FROM ADynEntityBUBCertificate WHERE DynEntityUid = @uid;
							INSERT ADynEntityBUBCertificate (DynEntityId, ActiveVersion,DynEntityConfigUid,[Name],UpdateUserId,UpdateDate,Dossier,Date_from,Date_until,Comments, Temporary)
													 VALUES (@id, 1, @DynCertificateEntityConfigUid,
															 'Cert. ' + RIGHT('000' + CONVERT(VARCHAR, @i), 3) + ' -- ' + CONVERT(VARCHAR, @dateUntil, 103) + ', ' + @dossierName, 
															 1, @UpdateDate, @dossierEntityId, @dateFrom, @dateUntil, 
															 'Amount: ' + CONVERT(VARCHAR, @amount4Certificate), @TemporaryCert);							
						END
						ELSE BEGIN
							INSERT ADynEntityBUBCertificate (ActiveVersion,DynEntityConfigUid,[Name],UpdateUserId,UpdateDate,Dossier,Date_from,Date_until,Comments, Temporary)
													 VALUES (1, @DynCertificateEntityConfigUid,
															 'Cert. ' + RIGHT('000' + CONVERT(VARCHAR, @i), 3) + ' -- ' + CONVERT(VARCHAR, @dateUntil, 103) + ', ' + @dossierName, 
															 1, @UpdateDate, @dossierEntityId, @dateFrom, @dateUntil, 
															 'Amount: ' + CONVERT(VARCHAR, @amount4Certificate), @TemporaryCert);
						END
					END

					-- Only create one certificate when dossier is created but not yet approved by fund ...
					IF @State = 'Membership OK'
						BREAK;
						
					-- Compute next dateFrom
					SET @dateFrom = DATEADD(month, @interval, @dateFrom);

					-- Add counter for next iteration
					SET @i = @i + 1;
				END

				-- Only create one certificate when dossier is created but not yet approved by fund ...
				IF @State = 'Membership OK'
					BREAK;
     
				-- Drop the record so we can move onto the next one
				DELETE FROM @TEMP WHERE uniqueId = @uniqueId;
			END

			-- Update Index and Taxonomy tables --
			DECLARE @entities AS DynEntityIdsTableType;
			
			-- Update DynEntityTaxonomyItem
			INSERT @entities (DynEntityUid, DynEntityId, DynEntityConfigUid)
			SELECT DynEntityUid, DynEntityId, DynEntityConfigUid
			  FROM ADynEntityBUBCertificate c
			 WHERE c.ActiveVersion = 1
			   AND c.UpdateDate = @UpdateDate
			EXEC _cust_UpdateDynEntityTaxonomyItem 1, @entities = @entities

			 -- Insert record in DynEntitIndex and IndexActiveDynEntities table
			EXEC _cust_ReIndex NULL, 1, 1, 'nl-BE', @entities = @entities

		END

	END	
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_cust_BUB_UpdateRemainingAmount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[_cust_BUB_UpdateRemainingAmount]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 04/03/2013
-- Description:	
-- =============================================
CREATE PROCEDURE _cust_BUB_UpdateRemainingAmount 
	@CreateNewRevision bit = 0,
	@UpdateDate datetime = NULL,
	@UpdateUserId bigint = 1,
	@BUBDossiers AS DynEntityIdsTableType READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @entities AS DynEntityIdsTableType;
	DECLARE @t TABLE (DosDynEntityUid bigint, DosDynEntityId bigint, Remaining money);

	IF @UpdateDate IS NULL BEGIN
		SET @UpdateDate = getdate();
	END

	-- Count total sum of payments for every dossier (provided by parameter)
	;WITH SumPayments (dossDynEntityId, TotalAmountPayed) AS
	(		 
		SELECT d.DynEntityId AS Dossier, ISNULL(SUM(p.Amount), 0)
		  FROM @BUBDossiers d 
					LEFT OUTER JOIN ADynEntityBUBPayment p ON d.DynEntityId = p.Dossier
								AND p.ActiveVersion = 1
		 GROUP BY d.DynEntityId
	)
	-- Insert dossiers to update in temp table
	INSERT @t
	SELECT d.DynEntityUid, d.DynEntityId, d.Claim_to - sp.TotalAmountPayed
	-- SELECT d.DynEntityUid, d.Claim_to - sp.TotalAmountPayed AS 'Remaining_computed', d.Claim_to, sp.TotalAmountPayed, d.Remaining, d.name
	  FROM ADynEntityBUBDossier d
				INNER JOIN SumPayments sp ON sp.dossDynEntityId = d.DynEntityId
	 WHERE d.ActiveVersion = 1
	   -- AND d.Employed_since >= '2010-01-01'
	   AND ISNULL(d.Remaining, 0) <> ISNULL(d.Claim_to, 0) - ISNULL(TotalAmountPayed, 0)
	  
	-- Create new revision (copy current to archive/history   
	IF @CreateNewRevision = 1 BEGIN
		INSERT @entities (DynEntityUid, DynEntityId, DynEntityConfigUid)
		SELECT DynEntityUid, DynEntityId, DynEntityConfigUid
		  FROM ADynEntityBUBDossier
		 WHERE ActiveVersion = 1
		   AND DynEntityUid IN (SELECT DosDynEntityUid FROM @t)
		EXEC _cust_CreateNewRevision @entities = @entities
	END
	
	-- Update Remaining amount
	UPDATE d
	   SET Remaining = t.Remaining,
		   UpdateDate = CASE @CreateNewRevision WHEN 1 THEN @UpdateDate ELSE UpdateDate END,
		   UpdateUserId = CASE @CreateNewRevision WHEN 1 THEN @UpdateUserId ELSE UpdateUserId END
	  FROM ADynEntityBUBDossier d
				INNER JOIN @t t ON t.DosDynEntityId = d.DynEntityId
	 WHERE d.ActiveVersion = 1
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_cust_BUB_GetDistrictUsers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[_cust_BUB_GetDistrictUsers]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 04/03/2013
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[_cust_BUB_GetDistrictUsers]
	(
	@destrictId nvarchar(max)
	)
AS
	SET NOCOUNT ON

	DECLARE @tt TABLE (UserName nvarchar(50), email nvarchar(150));

	INSERT INTO @tt
	SELECT ADynEntityUser.Name AS UserName, ADynEntityUser.Email
	  FROM ADynEntityUser INNER JOIN MultipleAssetsActive ON ADynEntityUser.DynEntityId = MultipleAssetsActive.DynEntityId 
		   INNER JOIN ADynEntityDistrict ON MultipleAssetsActive.RelatedDynEntityId = ADynEntityDistrict.DynEntityId
	 WHERE (MultipleAssetsActive.DynEntityAttribConfigId = 7042) -- Attrib District
	   AND (ADynEntityDistrict.ActiveVersion = 1) 
	   AND (ADynEntityUser.ActiveVersion = 1) 
	   AND (ADynEntityDistrict.Number = @destrictId)
	   AND (ADynEntityUser.DistrictUserType > 0)		-- Only if Type > 0!

	IF NOT EXISTS (SELECT 1 FROM @tt)
	BEGIN
		INSERT INTO @tt
		SELECT ADynEntityUser.Name AS UserName, ADynEntityUser.Email
		  FROM ADynEntityUser 
		 WHERE (ADynEntityUser.ActiveVersion = 1) 
           AND (ADynEntityUser.DistrictUserType & 2 = 2) -- BUB Super user -> 2
	END

	SELECT * FROM @tt;

	RETURN
GO

IF  EXISTS (select * from sys.triggers where name = 'tr_SetADynEntityBUBDossierID')
DROP TRIGGER [dbo].[tr_SetADynEntityBUBDossierID]
GO

-- ==========================================================================================
-- Author:		Wouter Steegmans
-- Create date: 26/05/2012
-- Description:	After update trigger for dossier. Will compute the expected amount and 
--				inserts certificates if the claim to amount is filled in
-- Remarks:		INSTEAD OF INSERT Trigger must be removed from ADynEntityBUBCertificate.
--				More, the Unique DynEntityId Revision index must be altered to UNIQUE=FALSE.
-- ==========================================================================================
CREATE TRIGGER [dbo].[tr_SetADynEntityBUBDossierID]
ON [dbo].[ADynEntityBUBDossier]  AFTER INSERT, UPDATE
AS 
BEGIN

	-- Only fire trigger if one record is updated/inserted. This one isn't fired when multiple records are inserted/updated
	IF @@ROWCOUNT > 1
	BEGIN
		DECLARE @BUBDossiers AS DynEntityIdsTableType 
		INSERT @BUBDossiers SELECT DynEntityUid, DynEntityId, DynEntityConfigUid FROM Inserted
		EXEC _cust_BUB_UpdateRemainingAmount NULL, NULL, NULL, @BUBDossiers = @BUBDossiers
		RETURN
	END

	DECLARE @claimToAmount money;
	DECLARE @expectedAmount money;
	DECLARE @personId bigint;
	DECLARE @ill bit;
	DECLARE @bub5254 bit;
	DECLARE @dossierEntityUid bigint;
	DECLARE @dossierEntityId bigint;
	DECLARE @dossierDynEntityConfigUid bigint;
	DECLARE @employedSince date;
	DECLARE @Q int;
	DECLARE @S int;
	DECLARE @seniority int;

	DECLARE @DoUpdate bit;

	SET NOCOUNT ON;

	SET @DoUpdate=0;

	SELECT @claimToAmount=Claim_to, @personId=Person, @ill=IllO, @bub5254=BUB_52_54, 
		   @dossierEntityId=DynEntityId,@dossierEntityUid=DynEntityUid, @employedSince=Employed_since, 
		   @dossierDynEntityConfigUid = DynEntityConfigUid,
		   @Q=CASE Q_hours_a_week WHEN 0 THEN 40 ELSE Q_hours_a_week END,
		   @S=CASE S_Fulltime_regime WHEN 0 THEN 40 ELSE S_Fulltime_regime END, @seniority=Seniority,
		   @expectedAmount=Expected 
	  FROM Inserted;

	-- Check if DynEntityId is 0
	IF (@dossierEntityId = 0) 
	BEGIN
		SET @dossierEntityId = @dossierEntityUid;
		SET @DoUpdate=1;
	END

	-- Try to get the expected amount 
	IF NOT EXISTS (SELECT 1 FROM deleted) AND ISNULL(@expectedAmount, 0) = 0
	BEGIN
	EXEC _cust_BUB_SetExpectedDossierAmount
		@personId, 
		@employedsince, 
		@seniority,
		@Q, 
		@S,
		@ill, 
		@bub5254,
		@expectedAmount OUTPUT;

		IF (@expectedAmount <> 0) SET @DoUpdate=1;
	END
	
	-- Calculate remaining amount
	INSERT @BUBDossiers SELECT DynEntityUid, DynEntityId, DynEntityConfigUid FROM Inserted
	EXEC _cust_BUB_UpdateRemainingAmount NULL, NULL, NULL, @BUBDossiers = @BUBDossiers

	-- Insert Certificates
	DECLARE @body XML (DOCUMENT EntityXmlSchema);
	SELECT @body =  (
		SELECT (
			SELECT
					 @dossierEntityUid as "@DynEntityUid",
					 i.DynEntityConfigUid as "@DynEntityConfigUid"
				FROM inserted i
				FOR XML PATH ('DynEntity'), TYPE
		   ) FOR XML PATH ('Root'), TYPE
	 ); EXEC _ssb_EntityAfterUpdateSend @body;
	 
	IF @DoUpdate=1
	BEGIN
		UPDATE [ADynEntityBUBDossier] 
		SET DynEntityId = @dossierEntityId,
			Expected = @expectedAmount
	  WHERE DynEntityUid IN (SELECT DynEntityUid FROM Inserted); 

	END
END
GO

IF  EXISTS (select * from sys.triggers where name = 'tr_BUBPayment_AfterUpdate')
DROP TRIGGER [dbo].[tr_BUBPayment_AfterUpdate]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 05/03/2013
-- Description:	
-- =============================================
CREATE TRIGGER tr_BUBPayment_AfterUpdate
   ON  ADynEntityBUBPayment 
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Calculate remaining amount
	DECLARE @BUBDossiers AS DynEntityIdsTableType 
	INSERT @BUBDossiers 
	SELECT d.DynEntityUid, d.DynEntityId, d.DynEntityConfigUid 
	  FROM ADynEntityBUBDossier d
				INNER JOIN Inserted i ON i.Dossier = d.DynEntityId
	 WHERE d.ActiveVersion = 1
	EXEC _cust_BUB_UpdateRemainingAmount NULL, NULL, NULL, @BUBDossiers = @BUBDossiers

END
GO

