/****** Object:  StoredProcedure [dbo].[_cust_GetSrchCount4Top_TypeContext_History]    Script Date: 10/04/2012 12:27:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:              Ilya Bolkhovsky, Wouter Steegmans
-- Create date: 16.08.2012
-- Description: Performs facet count for type/context search if top 400 recores are returned.
-- =============================================
ALTER PROCEDURE [dbo].[_cust_GetSrchCount4Top_TypeContext_History] 
        @SearchId bigint,
        -- Id of current user (required)
        @UserId bigint,
        -- Extra Select for Search By Type/Context           
        @ConfigIds varchar(MAX) = NULL,
        --  Space-separated list of Taxonomies (optional parameter)
        @taxonomyItemsIds varchar(MAX) = NULL
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;
		
		-- Check ConfigIds and TaxonomyItemsIds
        IF @ConfigIds = '' 
			SET @ConfigIds = NULL;
		IF @taxonomyItemsIds = ''
			SET @taxonomyItemsIds = NULL;	

		-- Delete all facet counts in _search_srchcount
		DELETE FROM _search_srchcount WHERE SearchId = @SearchId AND UserId = @UserId;
		
        -- CTE, First (Search) full text filtering on the keywords
        
		WITH Search AS
		(       
			-- Ranked Fulltext search via Active index      
			SELECT UserId, OwnerId, DynEntityConfigId, DepartmentId, TaxonomyItemsIds
			  FROM IndexHistoryDynEntities i
					INNER JOIN ##ResultsCntbyTypeContext r ON (i.DynEntityUid = r.DynEntityUid AND i.DynEntityConfigUid = r.DynEntityConfigUid)
			 WHERE 

				-- In the future, if search performance has to be optimized, an option is to 
				-- cache the user permissions in a (temp) table with lay-out Userid, Active, IndexUid
				-- IndexUid IN (SELECT IndexUid FROM ##test WHERE UserId = @UserId) 

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
		)
        
		-- Insert facet coutn results in temp table _search_srchcount
		INSERT _search_srchcount (SearchId, UserId, [Type], id, [Count])
		SELECT @SearchId, @UserId, 'TotalCount', 0 As Id, COUNT(1) AS Count 
		  FROM Search
		UNION ALL
		SELECT @SearchId, @UserId, 'AssetType', DynEntityConfigId, COUNT(1) AS Count
		  FROM Search
		 GROUP BY DynEntityConfigId
		UNION ALL
		SELECT @SearchId, @UserId, 'Taxonomy', id as TaxonomyId, SUM(IdsCount) AS Count 
		  FROM (
					SELECT TaxonomyItemsIds, COUNT(1) AS IdsCount
					  FROM Search
					 GROUP BY TaxonomyItemsIds			   
			   ) AS CountTax CROSS APPLY f_SplitIds(TaxonomyItemsIds)
		 GROUP BY id;
		 
		DROP TABLE ##ResultsCntbyTypeContext;
		
END