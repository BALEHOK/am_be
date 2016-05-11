SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author		: Alexander Shukletsov, Ilya Bolkhovsky, Wouter Steegmans
-- Create date	: 16.08.2012
-- Last change  : 24.03.2016
-- Description	: Performs an initial search by keywords
-- =============================================
ALTER PROCEDURE [dbo].[_cust_SearchByKeywords_History] 
        @SearchId uniqueidentifier,
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
        @SearchBufferSize int = 400,
		-- Related Asset attribute
        @attributeId bigint = 0,
		-- Related Asset id
        @assetId bigint = 0  
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;

		DECLARE @configId bigint = CONVERT(bigint, @ConfigIds);

		-- if we are searching for child assets, then fetch child uids into table var
		DECLARE @childAssets TABLE (DynEntityUid bigint);

		IF @attributeId != 0 AND @assetId != 0
		BEGIN
			DECLARE @assetTable varchar(50);
			SELECT @assetTable = DBTableName FROM DynEntityConfig
			WHERE DynEntityConfigId = @configId AND Active = 1 AND ActiveVersion = 1

			DECLARE @assetColumn varchar(50);
			SELECT @assetColumn = DBTableFieldname FROM DynEntityAttribConfig
			WHERE DynEntityAttribConfigId = @attributeId AND Active = 1 AND ActiveVersion = 1

			INSERT INTO @childAssets
				exec (N'SELECT DynEntityUid FROM [' + @assetTable + ']
					WHERE ActiveVersion = 1 AND [' + @assetColumn + '] = ' + @assetId)
		END;

		-- begin access rights
		DECLARE @AccesLevel bigint = 8;
		WITH AllowedConfigs AS
		(
			SELECT DynEntityConfigId 
			FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 0)
		),
		AllowedDepartmetns AS
		(
			SELECT DepartmentId 
			FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 0)
		),
		GrantedUserIds AS
		(
			SELECT UserId FROM f_GetGrantedSearchUserIds(@UserId)
		),
		DeniedConfigs AS
		(
			SELECT [DynEntityConfigId]
			FROM [dbo].[f_GetConfigIdsByRules](@UserId, @AccesLevel, 1)
		),
		DeniedDepartments AS
		(
			SELECT DepartmentId 
			FROM [dbo].[f_GetDepartmentIdsByRules](@UserId, @AccesLevel, 1)
		),
		-- end access rights
		-- CTE, First (Search) full text filtering on the keywords
		Search AS
		(	
				SELECT *, ROW_NUMBER() OVER(ORDER BY	CASE WHEN @orderby = 0 THEN [Rank] END DESC,        
														CASE WHEN @orderby = 1 THEN UpdateDate END DESC,
														CASE WHEN @orderby = 2 THEN Location END ASC,
														CASE WHEN @orderby = 3 THEN [User] END ASC, Name) AS RowNumber                			
				FROM 
				(
					SELECT IndexUid, UserId, OwnerId, DynEntityConfigId, DepartmentId, TaxonomyItemsIds, UpdateDate, Location, [User], 0 AS Active, Name, [Rank]
						FROM 
						(
							SELECT IndexUid, UserId, OwnerId, DynEntityConfigId, DynEntityUid, DepartmentId, TaxonomyItemsIds, UpdateDate, Location, [User], Name,
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
							WHERE @keywords != '"*"'

							UNION ALL

							SELECT IndexUid, UserId, OwnerId, DynEntityConfigId, DynEntityUid, DepartmentId, TaxonomyItemsIds, UpdateDate, Location, [User], Name, 1 AS [Rank]
							FROM IndexHistoryDynEntities
							WHERE @keywords = '"*"'

						) as historyAssets
						WHERE   
/*
						-- In the future, if search performance has to be optimized, an option is to 
						-- cache the user permissions in a (temp) table with lay-out Userid, Active, IndexUid
						IndexUid IN (SELECT IndexUid FROM ##test WHERE UserId = @UserId) 
*/ 
					   (	
							DynEntityConfigId IN
								(
									SELECT @ConfigIds
									UNION
									SELECT DynEntityConfigId FROM DynEntityConfigTaxonomy
									WHERE TaxonomyItemId IN (select id from dbo.f_SplitIds(@taxonomyItemsIds))
								)
							OR	
								(@ConfigIds IS NULL AND @taxonomyItemsIds IS NULL)									
						)
						AND
						( 
							(@attributeId = 0 AND @assetId = 0)
							OR
							DynEntityUid IN (SELECT DynEntityUid FROM @childAssets)
						)
						AND
						(
							DynEntityConfigId IN (SELECT * FROM AllowedConfigs)
							OR
							DepartmentId IN (SELECT * FROM AllowedDepartmetns)
							OR
							UserId IN (SELECT * FROM GrantedUserIds)
							OR
							OwnerId in (SELECT * FROM GrantedUserIds)
						)
						AND
						(
							DynEntityConfigId NOT IN (SELECT * FROM DeniedConfigs)
							OR
							DepartmentId NOT IN (SELECT * FROM DeniedDepartments)
						)
																						
				) AS SearchResults
		)
        
		-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
		INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
		SELECT TOP(@SearchBufferSize) @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
		  FROM Search
		 WHERE RowNumber > @SearchBufferCount
END
