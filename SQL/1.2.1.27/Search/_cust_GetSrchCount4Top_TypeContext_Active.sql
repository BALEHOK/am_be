/****** Object:  StoredProcedure [dbo].[_cust_GetSrchCount4Top_TypeContext_Active]    Script Date: 01/08/2013 16:30:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:              Ilya Bolkhovsky, Wouter Steegmans
-- Create date: 16.08.2012
-- Description: Performs facet count for type/context search if top 400 recores are returned.
-- =============================================
ALTER PROCEDURE [dbo].[_cust_GetSrchCount4Top_TypeContext_Active] 
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
			  FROM IndexActiveDynEntities i
					INNER JOIN _search_srchTypeContext r ON (i.DynEntityUid = r.DynEntityUid AND i.DynEntityConfigUid = r.DynEntityConfigUid AND @SearchId = r.SearchId)
			 WHERE 

				-- In the future, if search performance has to be optimized, an option is to 
				-- cache the user permissions in a (temp) table with lay-out Userid, Active, IndexUid
				-- IndexUid IN (SELECT IndexUid FROM ##test WHERE UserId = @UserId) 
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
		 
		DROP TABLE _search_srchTypeContext;		 
		
END