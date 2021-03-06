USE [Assetmanager]
GO
/****** Object:  StoredProcedure [dbo].[_cust_GetSrchCount4Top_Keywords_Active]    Script Date: 05/07/2013 14:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:              Ilya Bolkhovsky, Wouter Steegmans
-- Create date: 16.08.2012
-- Description: Performs facet count for keywords search if top 2500 recores are returned.
-- =============================================
ALTER PROCEDURE [dbo].[_cust_GetSrchCount4Top_Keywords_Active] 
        @SearchId bigint,
        -- Id of current user (required)
        @UserId bigint,
        -- Space-separated list of keywords (required parameter). Ex.: '"item*" AND "111*"'     
        @keywords nvarchar(1000),
        --  Space-separated list of ConfigIds - assettype (optional parameter)
        @ConfigIds varchar(MAX) = NULL,
        --  Space-separated list of Taxonomies (optional parameter)
        @taxonomyItemsIds varchar(MAX) = NULL
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;
		
		IF NOT @keywords IS NULL
		BEGIN
			IF CHARINDEX('*', @keywords) = 0
			BEGIN
				SET @keywords = '"' + REPLACE(@keywords, ' ', '*" AND "') + '*"';
			END
		END
				
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
			  FROM IndexActiveDynEntities 
			 WHERE (CONTAINS (AllAttribValues, @keywords))
			   AND

				-- In the future, if search performance has to be optimized, an option is to 
				-- cache the user permissions in a (temp) table with lay-out Userid, Active, IndexUid
				-- IndexUid IN (SELECT IndexUid FROM ##test WHERE UserId = @UserId) 

					   (	
								DynEntityConfigId IN (SELECT * FROM f_GetGrantedSearchConfigIds(@UserId, 
																	CONVERT(bigint, @ConfigIds), @taxonomyItemsIds))
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
		
END