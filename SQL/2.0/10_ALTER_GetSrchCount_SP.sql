SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:    Wouter Steegmans
-- Create date: 2012/08/14
-- Description: Return facet counts + total count
-- =============================================
ALTER PROCEDURE [dbo].[_cust_GetSrchCount]
  -- Add the parameters for the stored procedure here
        @SearchId bigint,
        -- Id of current user (required)
        @UserId bigint,
        -- Space-separated list of keywords (optional parameter). Ex.: '"item*" AND "111*"'     
        @keywords nvarchar(1000) = NULL,
        --  Space-separated list of ConfigIds - assettype (optional parameter)
        @ConfigIds varchar(MAX) = NULL,
        --  Space-separated list of Taxonomies (optional parameter)
        @taxonomyItemsIds varchar(MAX) = NULL,
        -- Search area
        @active bit = 1,
    -- Search type: Keywords or Type/Context
        @type bit = 0
AS
BEGIN
  -- SET NOCOUNT ON added to prevent extra result sets from
  -- interfering with SELECT statements.
  SET NOCOUNT ON;
  SET FMTONLY OFF;
  DECLARE @count int = 0;

  IF NOT EXISTS (SELECT 1 FROM _search_srchcount WHERE SearchId=@SearchId AND UserId=@UserId)
  BEGIN
    INSERT _search_srchcount (SearchId, UserId, [Type], id, [Count])
    SELECT @SearchId, @UserId, 'TotalCount', 0 As Id, COUNT(1) AS Count 
      FROM _search_srchres
     WHERE SearchId=@SearchId AND UserId=@UserId;
    SELECT @count = COUNT FROM _search_srchcount WHERE SearchId = @SearchId AND UserId = @UserId AND [Type] = 'TotalCount';
    
    -- Check if 400 rows are returned as count, means that the cache recordset was truncated ...
    IF @count = 400 
    BEGIN
      IF ISNULL(@keywords, '') <> '' AND @active = 1
      BEGIN
        EXEC _cust_GetSrchCount4Top_Keywords_Active @SearchId, @UserId, @keywords, @ConfigIds, @taxonomyItemsIds
      END
      IF ISNULL(@keywords, '') <> '' AND @active = 0
      BEGIN
        EXEC _cust_GetSrchCount4Top_Keywords_History @SearchId, @UserId, @keywords, @ConfigIds, @taxonomyItemsIds
      END
      IF @type = 1 AND @active = 1
      BEGIN
        EXEC _cust_GetSrchCount4Top_TypeContext_Active @SearchId, @UserId, @ConfigIds, @taxonomyItemsIds
      END
      IF @type = 1 AND @active = 0
      BEGIN
        EXEC _cust_GetSrchCount4Top_TypeContext_History @SearchId, @UserId, @ConfigIds, @taxonomyItemsIds
      END
    END
    ELSE BEGIN
      INSERT _search_srchcount (SearchId, UserId, [Type], id, [Count])
      SELECT @SearchId, @UserId, 'AssetType', DynEntityConfigId, COUNT(1) AS Count
        FROM _search_srchres
       WHERE SearchId=@SearchId AND UserId=@UserId
       GROUP BY DynEntityConfigId
      UNION ALL
      SELECT @SearchId, @UserId, 'Taxonomy', id as TaxonomyId, SUM(IdsCount) AS Count 
        FROM (
            SELECT TaxonomyItemsIds, COUNT(1) AS IdsCount
              FROM _search_srchres
             WHERE SearchId=@SearchId AND UserId=@UserId
             GROUP BY TaxonomyItemsIds         
           ) AS CountTax CROSS APPLY f_SplitIds(TaxonomyItemsIds)
       GROUP BY id;  
    END;
  END;  
  
-- Return results

  -- WAITFOR DELAY '000:00:05';

  -- Sort the list, AssetType will be sorted by it's (EN) name.
  WITH tSearchCount AS
  (
    SELECT *, 1 as GroupId, '' AS Name FROM _search_srchcount 
     WHERE SearchId=@SearchId AND UserId=@UserId
       AND Type = 'TotalCount'
     UNION
    SELECT cnt.*, 2 as GroupdId, config.name AS Name
      FROM _search_srchcount cnt LEFT JOIN DynEntityConfig config ON cnt.id = config.DynEntityConfigId
     WHERE SearchId=@SearchId AND UserId=@UserId
       AND config.ActiveVersion = 1
       AND Type = 'AssetType'
     UNION
    SELECT *, 3 as GroupId, '' AS Name FROM _search_srchcount 
     WHERE SearchId=@SearchId AND UserId=@UserId
       AND Type = 'Taxonomy'
  )
  SELECT SearchId, UserId, Type, id, Count FROM tSearchCount
   ORDER BY GroupId, Name;

END
