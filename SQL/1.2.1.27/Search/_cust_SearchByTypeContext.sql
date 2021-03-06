/****** Object:  StoredProcedure [dbo].[_cust_SearchByTypeContext]    Script Date: 01/08/2013 16:30:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author		: Wouter Steegmans
-- Create date	: 21.08.2012
-- Description	: Performs an initial search by keywords
-- =============================================
ALTER PROCEDURE [dbo].[_cust_SearchByTypeContext] 
        @SearchId bigint,
        -- Id of current user (required)
        @UserId bigint,
        --  Space-separated list of ConfigIds - assettype (optional parameter)
        @ConfigIds varchar(MAX) = NULL,
        --  Space-separated list of Taxonomies (optional parameter)
        @taxonomyItemsIds varchar(MAX) = NULL,
        -- Search area
        @active bit = 1, 
        -- OrderBy field (enumeration in code, optional parameter, defaults will be set)
        -- 0 - by relevance (rank)
        -- 1 - by date
        -- 2 - by location
        -- 3 - by user
        @orderby tinyint = 1,
        -- Pagination support (optional parameter, defaults will be set),
        @PageNumber int = 1, 
        @PageSize int = 20
AS
BEGIN

/* -- As illustration. The selectByTypeContext must always give a result set with DynEntityUid, DynEntityConfigUid
			SELECT DynEntityUid, DynEntityConfigUid 
			  FROM ADynEntityBUBDossier
			 WHERE ADynEntityBUBDossier.Name like '%michel%'
			   AND ActiveVersion = 1
*/

        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;
		SET FMTONLY OFF;

		-- Number of rows returned!
		DECLARE @SearchBufferCount int = 0;
				
		-- Check ConfigIds and TaxonomyItemsIds
        IF @ConfigIds = '' 
			SET @ConfigIds = NULL;
		IF @taxonomyItemsIds = ''
			SET @taxonomyItemsIds = NULL;		

        -- Check if results are already saved in temp table, so search can be skipped ...
		SELECT @SearchBufferCount = COUNT(1) FROM _search_srchres WHERE SearchId = @SearchId AND UserId = @UserId
		-- Check if there are still records in the temp table for the next page
		IF (@SearchBufferCount / @PageSize) >= @PageNumber
		BEGIN			
			EXEC _cust_GetSrchResPage @SearchId, @UserId, @active, @PageNumber, @PageSize
			RETURN
		END

		-- Search active assets
		IF @active = 1
		BEGIN
			EXEC _cust_SearchByTypeContext_Active @SearchId, @UserId, @ConfigIds, @taxonomyItemsIds, @active, @orderby, @SearchBufferCount
		END

		-- Search history
		IF @active = 0
		BEGIN
			EXEC _cust_SearchByTypeContext_History @SearchId, @UserId, @ConfigIds, @taxonomyItemsIds, @active, @orderby, @SearchBufferCount		
		END

		-- Get the results and return ...
        EXEC _cust_GetSrchResPage @SearchId, @UserId, @active, @PageNumber, @PageSize;
        
        SET ROWCOUNT 0;

END