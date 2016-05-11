SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:    Steegmans Wouter
-- Create date: 25/01/2013
-- Changed: 15/04/2016 by Alexander Shukletsov
-- Description: Reindex items of certain asset/item type
-- =============================================
ALTER PROCEDURE [dbo].[_cust_ReIndex]
  -- Add the parameters for the stored procedure here
  @DynEntityConfigId bigint = NULL, 
  @active bit = 1,
  @buildDynEntityIndex bit = 0,
  @culture nvarchar(10) = 'nl-BE',
  @entities DynEntityIdsTableType READONLY  -- table variable
AS
BEGIN
  -- SET NOCOUNT ON added to prevent extra result sets from
  -- interfering with SELECT statements.
  SET NOCOUNT ON;
  
  DECLARE @rowcountEntities int = 0;
  DECLARE @rowcount int;
  DECLARE @IsNormalAssetType bit = 0;     -- if not Normal -> Data Asset Type
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
    RAISERROR ( N'Parameter DynEntityConfigId and entities can''t be used both at the same time. Only set one parameter.', -- Message text.
          15, -- Severity.
          1-- State.
           );   
  END
  
  -- Check if history isn't combined with rebuild DynEntityIndex (active)
  IF @active = 0 AND @buildDynEntityIndex = 1 BEGIN
    RAISERROR ( N'DynEntityIndex can''t be rebuild when indexing history items. Change one of both parameters (@active, @buildDynEntityIndex).', -- Message text.
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
    ( SELECT Name FROM DynEntityConfig WHERE DynEntityConfigUid = @DynEntityConfigUid )
    SELECT @EntityConfigKeywords = LTRIM(RTRIM(@EntityConfigKeywords + ' ' + dbo.GetStringResourceValue(Name))) FROM t1
    
    -- Actual items
    IF @active = 1
    BEGIN
      -- Get Category + Taxonomy Keywords
      -- from now on taxonomy is linked to Asset type rather than Asset.
      -- therefore taxonomy is fetched by Entity config relations
      WITH t1 (Name, CategoryUid) AS
      ( SELECT ti.Name, ti.TaxonomyItemUid FROM TaxonomyItem ti
                INNER JOIN DynEntityConfigTaxonomy ct ON ct.TaxonomyItemId = ti.TaxonomyItemId
                INNER JOIN Taxonomy t ON t.TaxonomyUid = ti.TaxonomyUid
          WHERE t.IsCategory = 1 AND ct.DynEntityConfigId = @vDynEntityConfigId AND ti.ActiveVersion = 1
      )
      SELECT @CategoryKeywords = LTRIM(RTRIM(@CategoryKeywords + ' ' + dbo.GetStringResourceValue(Name)))FROM t1;

      WITH t1 (Name, TaxonomyItemId) AS
      ( SELECT ti.Name, ti.TaxonomyItemId FROM TaxonomyItem ti
                INNER JOIN DynEntityConfigTaxonomy ct ON ct.TaxonomyItemId = ti.TaxonomyItemId
          WHERE ct.DynEntityConfigId = @vDynEntityConfigId AND ti.ActiveVersion = 1
      )
      SELECT @TaxonomyKeywords = LTRIM(RTRIM(@TaxonomyKeywords + ' ' + dbo.GetStringResourceValue(Name))), 
             @TaxonomyItemsIds = LTRIM(RTRIM(@TaxonomyItemsIds + ' ' + CONVERT(varchar(20), TaxonomyItemId))) FROM t1

      SELECT @CategoryUids = @CategoryUids + CASE t.IsCategory WHEN 1 THEN ' ' + CONVERT(nvarchar(10), t.TaxonomyUid) ELSE '' END, 
             @TaxonomyUids = @TaxonomyUids + ' ' + CONVERT(nvarchar(10), t.TaxonomyUid)
        FROM Taxonomy t
            INNER JOIN TaxonomyItem ti ON ti.TaxonomyUid = t.TaxonomyUid
            INNER JOIN DynEntityConfigTaxonomy ct ON ct.TaxonomyItemId = ti.TaxonomyItemId
          WHERE ct.DynEntityConfigId = @vDynEntityConfigId AND ti.ActiveVersion = 1
    END
    
    -- Archive (History)
    ELSE BEGIN
      WITH t1 (Name, CategoryUid) AS
      ( SELECT ti.Name, ti.TaxonomyItemUid FROM TaxonomyItem ti
                INNER JOIN DynEntityConfigTaxonomy ct ON ct.TaxonomyItemId = ti.TaxonomyItemId
                INNER JOIN Taxonomy t ON t.TaxonomyUid = ti.TaxonomyUid
          WHERE t.IsCategory = 1 AND ct.DynEntityConfigId = @vDynEntityConfigId AND ti.ActiveVersion = 1
      )
      SELECT @CategoryKeywords = LTRIM(RTRIM(@CategoryKeywords + ' ' + dbo.GetStringResourceValue(Name)))FROM t1;

      WITH t1 (Name, TaxonomyItemId) AS
      ( SELECT ti.Name, ti.TaxonomyItemId FROM TaxonomyItem ti
                INNER JOIN DynEntityConfigTaxonomy ct ON ct.TaxonomyItemId = ti.TaxonomyItemId
          WHERE ct.DynEntityConfigId = @vDynEntityConfigId AND ti.ActiveVersion = 1
      )
      SELECT @TaxonomyKeywords = LTRIM(RTRIM(@TaxonomyKeywords + ' ' + dbo.GetStringResourceValue(Name))), 
             @TaxonomyItemsIds = LTRIM(RTRIM(@TaxonomyItemsIds + ' ' + CONVERT(varchar(20), TaxonomyItemId))) FROM t1

      SELECT @CategoryUids = @CategoryUids + CASE t.IsCategory WHEN 1 THEN ' ' + CONVERT(nvarchar(10), t.TaxonomyUid) ELSE '' END, 
             @TaxonomyUids = @TaxonomyUids + ' ' + CONVERT(nvarchar(10), t.TaxonomyUid)
        FROM Taxonomy t
            INNER JOIN TaxonomyItem ti ON ti.TaxonomyUid = t.TaxonomyUid
            INNER JOIN DynEntityConfigTaxonomy ct ON ct.TaxonomyItemId = ti.TaxonomyItemId
          WHERE ct.DynEntityConfigId = @vDynEntityConfigId AND ti.ActiveVersion = 1   
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
        ' WHERE ' +
        CASE @active WHEN 1 THEN 'prim.ActiveVersion = 1 AND prim.IsDeleted=0; ' ELSE '(prim.ActiveVersion = 0 OR prim.IsDeleted=1) AND prim.DynEntityConfigUid = @DynEntityConfigUid; ' END +
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
