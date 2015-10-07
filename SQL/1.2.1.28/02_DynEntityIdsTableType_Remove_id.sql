IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_cust_ReIndex]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[_cust_ReIndex]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_cust_CreateNewRevision]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[_cust_CreateNewRevision]
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

-- =============================================
-- Author:		Steegmans Wouter
-- Create date: 25/01/2013
-- Description:	Reindex items of certain asset/item type
-- =============================================
CREATE PROCEDURE [dbo].[_cust_ReIndex]
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
			  FROM IndexActiveDynEntities ae
						INNER JOIN @entities e ON e.DynEntityUid = ae.DynEntityUId
						       AND e.DynEntityConfigUid = ae.DynEntityConfigUid
		END
	END
	ELSE BEGIN
		IF ISNULL(@DynEntityConfigId, 0) > 0 BEGIN
			DELETE FROM IndexHistoryDynEntities 
			 WHERE DynEntityConfigId = @DynEntityConfigId
		END
		
		IF @rowcountEntities > 0 BEGIN
			DELETE IndexHistoryDynEntities 
			  FROM IndexHistoryDynEntities ae
						INNER JOIN @entities e ON e.DynEntityUid = ae.DynEntityUId
						       AND e.DynEntityConfigUid = ae.DynEntityConfigUid
		END
	END
	
	IF @buildDynEntityIndex = 1
	BEGIN
		IF ISNULL(@DynEntityConfigId, 0) > 0 BEGIN
			DELETE FROM DynEntityIndex 
			 WHERE DynEntityConfigId = @DynEntityConfigId
			   AND DynEntityId IN (SELECT DISTINCT DynEntityId FROM @entities)
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
					DELETE FROM DynEntityIndex WHERE DynEntityConfigId = @DynEntityConfigId;
					INSERT DynEntityIndex
					SELECT i.DynEntityId, i.DynEntityConfigId, i.Barcode, l.DynEntityId AS LocationId, i.DepartmentId, i.UserId, i.OwnerId, i.Name
					  FROM IndexActiveDynEntities i
								INNER JOIN ADynEntityLocation l ON i.LocationUid = l.DynEntityUid
					 WHERE DynEntityConfigId = @DynEntityConfigId
				END
				IF @rowcountEntities > 0 BEGIN
					DELETE FROM DynEntityIndex WHERE DynEntityConfigId IN (SELECT DISTINCT DynEntityConfigId 
																		   FROM DynEntityConfig dc
																		   INNER JOIN @entities e ON e.DynEntityConfigUid = dc.DynEntityConfigUid)																		   
					INSERT DynEntityIndex
					SELECT i.DynEntityId, i.DynEntityConfigId, i.Barcode, l.DynEntityId AS LocationId, i.DepartmentId, i.UserId, i.OwnerId, i.Name
					  FROM IndexActiveDynEntities i
								INNER JOIN ADynEntityLocation l ON i.LocationUid = l.DynEntityUid
								INNER JOIN @entities e ON e.DynEntityUid = i.DynEntityUid AND e.DynEntityConfigUid = i.DynEntityConfigUid
					 WHERE i.DynEntityConfigUid = @DynEntityConfigUid
				END
			END
			ELSE BEGIN
				IF ISNULL(@DynEntityConfigId, 0) > 0 BEGIN
					DELETE FROM DynEntityIndex WHERE DynEntityConfigId = @DynEntityConfigId;
					INSERT DynEntityIndex (DynEntityId, DynEntityConfigId, Name)
					SELECT DynEntityId, DynEntityConfigId, Name
					  FROM IndexActiveDynEntities
					 WHERE DynEntityConfigId = @DynEntityConfigId
				END
				IF @rowcountEntities > 0 BEGIN
					DELETE FROM DynEntityIndex WHERE DynEntityConfigId IN (SELECT DISTINCT DynEntityConfigId 
																		   FROM DynEntityConfig dc
																		   INNER JOIN @entities e ON e.DynEntityConfigUid = dc.DynEntityConfigUid)
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

	-- Temp tables
	CREATE TABLE #t
	(
		DynEntityUid bigint, 
		DynEntityConfigUid bigint, 
		NewDynEntityUid bigint
	)

	-- Variable declarations
	DECLARE @DynEntityUid bigint = 0;
	DECLARE @DynEntityConfigUid bigint = 0;
	DECLARE @PrevDynEntityConfigUid bigint = 0;

	DECLARE @tempConfigUids TABLE (DynEntityConfigUid bigint)
	DECLARE @tempMultipleAssetAtts TABLE (DynEntityAttribConfigId bigint, DBTableName varchar(100))
	
	DECLARE @NewEntityUid bigint;
	DECLARE @DynEntityConfigId BIGINT;
	DECLARE @DynEntityDBTableName VARCHAR(50);
	DECLARE @AttributeNames VARCHAR(5000) = '';
	
	DECLARE @DynEntityAttribConfigId bigint;
	DECLARE @DynRelatedMaaEntityDBTableName nvarchar(128);
	
	DECLARE @i int = 1;
	DECLARE @rows int;
	
	DECLARE @sqlCommand NVARCHAR(max);
	
	-- Get all different Config's
	INSERT @tempConfigUids
	SELECT DISTINCT DynEntityConfigUid FROM @entities;

	SET @rows = @@ROWCOUNT
	
	-- Loop different Configs ...
	WHILE @i <= @rows BEGIN
	
		-- Get config information
		SELECT TOP 1 @DynEntityConfigUid = c.DynEntityConfigUid, @DynEntityConfigId = c.DynEntityConfigId, @DynEntityDBTableName = c.DBTableName 
		  FROM @tempConfigUids tc
					INNER JOIN DynEntityConfig c ON tc.DynEntityConfigUid = c.DynEntityConfigUid
		
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

