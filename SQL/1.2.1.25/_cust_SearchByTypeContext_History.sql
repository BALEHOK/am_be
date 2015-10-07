USE [ACV.SobBub]
GO
/****** Object:  StoredProcedure [dbo].[_cust_SearchByTypeContext_History]    Script Date: 11/03/2012 10:23:08 ******/
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
														CASE WHEN @orderby = 3 THEN [User] END ASC, DisplayValues) AS RowNumber                			
				FROM 
				(
					SELECT IndexUid, UserId, OwnerId, DynEntityConfigId, DepartmentId, TaxonomyItemsIds, UpdateDate, Location, [User], 0 AS Active, DisplayValues
						FROM IndexHistoryDynEntities i
								INNER JOIN ##ResultsCntbyTypeContext r ON (i.DynEntityUid = r.DynEntityUid AND i.DynEntityConfigUid = r.DynEntityConfigUid)
						WHERE   
/*
						-- In the future, if search performance has to be optimized, an option is to 
						-- cache the user permissions in a (temp) table with lay-out Userid, Active, IndexUid
						IndexUid IN (SELECT IndexUid FROM ##test WHERE UserId = @UserId) 
*/ 


						   (            
								-- Deny permissions
								(
										(	DynEntityConfigId NOT IN (SELECT DynEntityConfigId 
																		FROM ##permissions 
																	   WHERE CurUserId=@UserId AND IsDeny=1)	-- Check Asset Type 
										)
								)
								AND 
								(
										(
											DepartmentId   NOT IN (SELECT DepartmentId 
																		FROM ##permissions 
																	   WHERE CurUserId=@UserId AND IsDeny=1)		-- Check Department 
										)
								)
								AND 
								(
										(
											NOT EXISTS (SELECT TOP 1 1											-- Check Category
														 FROM ##permissions 
														WHERE CHARINDEX(' ' + CONVERT(NVARCHAR(10), TaxonomyItemId) + ' ', ' ' + TaxonomyItemsIds + ' ') > 0
														  AND CurUserId = @UserId
														  AND IsDeny = 1)
										)
								)
							)
							AND
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
        
		-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
		INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
		SELECT TOP 400 @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
		  FROM Search
		 WHERE RowNumber > @SearchBufferCount
		
		
END