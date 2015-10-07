-- =============================================
-- Author		: Ilya Bolkhovsky, Wouter Steegmans
-- Create date	: 16.08.2012
-- Description	: Performs an initial search by keywords
-- =============================================
CREATE PROCEDURE [dbo].[_cust_SearchByKeywords_History] 
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
        @SearchBufferCount int = 0
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;

/*
        DROP TABLE ##permissions
        DROP TABLE ##users
        DROP TABLE ##srchres
        DROP TABLE ##srchcount
*/


		-- CTE, First (Search) full text filtering on the keywords
		WITH Search AS
		(	
				SELECT *, ROW_NUMBER() OVER(ORDER BY	CASE WHEN @orderby = 0 THEN [Rank] END DESC,        
														CASE WHEN @orderby = 1 THEN UpdateDate END DESC,
														CASE WHEN @orderby = 2 THEN Location END ASC,
														CASE WHEN @orderby = 3 THEN [User] END ASC) AS RowNumber                			
				FROM 
				(
					SELECT IndexUid, UserId, OwnerId, DynEntityConfigId, DepartmentId, TaxonomyItemsIds, UpdateDate, Location, [User], 0 AS Active,
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
							(            
								-- Deny permissions
								(
									EXISTS (SELECT 1 FROM ##permissions WHERE CurUserId=@UserId AND DynEntityConfigId > 0 AND IsDeny=1)
									AND
										(	DynEntityConfigId NOT IN (SELECT DynEntityConfigId 
																		FROM ##permissions 
																	   WHERE CurUserId=@UserId AND IsDeny=1)	-- Check Asset Type 
										)
								)
								AND 
								(
									EXISTS (SELECT 1 FROM ##permissions WHERE CurUserId=@UserId AND DepartmentId > 0 AND IsDeny=1)
									AND
										(
											DepartmentId   NOT IN (SELECT DepartmentId 
																		FROM ##permissions 
																	   WHERE CurUserId=@UserId AND IsDeny=1)		-- Check Department 
										)
								)
								AND 
								(
									EXISTS (SELECT 1 FROM ##permissions WHERE CurUserId=@UserId AND TaxonomyItemId > 0 AND IsDeny=1)
									AND
										(
											NOT EXISTS (SELECT TOP 1 1											-- Check Category
														 FROM ##permissions 
														WHERE CHARINDEX(' ' + CONVERT(NVARCHAR(10), TaxonomyItemId) + ' ', ' ' + TaxonomyItemsIds + ' ') > 0
														  AND CurUserId = @UserId
														  AND IsDeny = 1)
										)
								)
							)
							OR
							(    
								-- Allow Permissions      
								(
									-- Granted for all types, if so, skip all other checks in allow ...
									EXISTS (SELECT 1												
											  FROM ##permissions 
											 WHERE CurUserId=@UserId AND IsDeny=0 AND DynEntityConfigId = -1)
								)
								OR
								(	-- if not  Granted for all types, check further ...
									NOT EXISTS (SELECT 1												
												  FROM ##permissions 
												 WHERE CurUserId=@UserId AND IsDeny=0 AND DynEntityConfigId = -1)

									AND
									(
										-- Check user and subusers in user/owner
										(
											EXISTS (SELECT 1 FROM ##users WHERE CurUserId=@UserId) 
											AND
												(
													OwnerId IN (SELECT UserId FROM ##users WHERE CurUserId=@UserId)		/* Check Owner */
													OR UserId IN (SELECT UserId FROM ##users WHERE CurUserId=@UserId)	/* Check User */        
												)
										)
										OR 
										-- Check ConfigId (Asset Type)
										(
											EXISTS (SELECT 1 FROM ##permissions WHERE CurUserId=@UserId AND DynEntityConfigId > 0 AND IsDeny=0)
											AND
												(
													DynEntityConfigId IN (
														SELECT DynEntityConfigId 
														  FROM ##permissions 
														 WHERE CurUserId=@UserId AND IsDeny=0 )				/* Check Asset Type */  
												)
										)
										OR 
										(
											EXISTS (SELECT 1 FROM ##permissions WHERE CurUserId=@UserId AND DepartmentId > 0 AND IsDeny=0)
											AND
												(
													DepartmentId IN (SELECT DepartmentId 
																	   FROM ##permissions 
																	  WHERE CurUserId=@UserId AND IsDeny=0)			/* Check Department */  
												)
										)
										OR 
										(
											EXISTS (SELECT 1 FROM ##permissions WHERE CurUserId=@UserId AND TaxonomyItemId > 0 AND IsDeny=0)
											AND
											(
												EXISTS (SELECT TOP 1 1											/* Check Category */
															 FROM ##permissions 
															WHERE CHARINDEX(' ' + CONVERT(NVARCHAR(10), TaxonomyItemId) + ' ', ' ' + TaxonomyItemsIds + ' ') > 0
															  AND CurUserId = @UserId
															  AND IsDeny = 0)
											)
										)
									)
								)                                                           
							)
						)
						AND
						-- Filtering on Taxonomies (space-separated list)- SP Parameter
						(   @taxonomyItemsIds IS NULL OR
							(NOT @taxonomyItemsIds IS NULL AND EXISTS 
								(
									SELECT TOP 1 1 FROM f_SplitIds2Str(@taxonomyItemsIds)
									 WHERE CHARINDEX(' ' + id + ' ', ' ' + taxonomyItemsIds + ' ') > 0
								)
							)
						)
						AND
						-- Filtering on ConfigIds (space-separated list) - SP Parameter
						(	@ConfigIds IS NULL OR 
							(NOT @ConfigIds IS NULL AND EXISTS
							(
									SELECT TOP 1 1 FROM f_SplitIds(@ConfigIds)
									 WHERE id = DynEntityConfigId
								)
							)
						)
										
				) AS SearchResults
		)
        
		-- Insert results in temp table ##srchres, so for the next pages (2, 3, ...) this temp table can be queried.
		INSERT ##srchres
		SELECT TOP 400 @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
		  FROM Search
		 WHERE RowNumber > @SearchBufferCount

END
GO

