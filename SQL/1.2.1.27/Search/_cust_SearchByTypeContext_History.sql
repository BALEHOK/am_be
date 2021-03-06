/****** Object:  StoredProcedure [dbo].[_cust_SearchByTypeContext_History]    Script Date: 01/08/2013 16:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
        @SearchBufferCount int = 0
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;

		-- CTE, First (Search) full text filtering on the keywords
		WITH Search AS
		(	
				SELECT *, ROW_NUMBER() OVER(ORDER BY	CASE WHEN @orderby = 0 THEN UpdateDate END DESC,        
														CASE WHEN @orderby = 1 THEN UpdateDate END DESC,
														CASE WHEN @orderby = 2 THEN Location END ASC,
														CASE WHEN @orderby = 3 THEN [User] END ASC, Name) AS RowNumber                			
				FROM 
				(
					SELECT IndexUid, UserId, OwnerId, DynEntityConfigId, DepartmentId, TaxonomyItemsIds, UpdateDate, Location, [User], 0 AS Active, Name
						FROM IndexHistoryDynEntities i
								INNER JOIN _search_srchTypeContext r ON (i.DynEntityUid = r.DynEntityUid AND i.DynEntityConfigUid = r.DynEntityConfigUid AND @SearchId = r.SearchId)
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