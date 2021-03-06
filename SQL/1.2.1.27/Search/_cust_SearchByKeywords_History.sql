/****** Object:  StoredProcedure [dbo].[_cust_SearchByKeywords_History]    Script Date: 01/08/2013 16:30:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
        @SearchBufferCount int = 0
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
		SELECT TOP 400 @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
		  FROM Search
		 WHERE RowNumber > @SearchBufferCount

END