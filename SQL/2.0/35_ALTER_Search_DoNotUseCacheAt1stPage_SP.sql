SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author		: Alexander Shukletsov, Ilya Bolkhovsky, Wouter Steegmans
-- Create date	: 16.08.2012
-- Description	: Performs an initial search by keywords
-- =============================================
ALTER PROCEDURE [dbo].[_cust_SearchByKeywords] 
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
        -- OrderBy field (enumeration in code, optional parameter, defaults will be set)
        -- 0 - by relevance (rank)
        -- 1 - by date
        -- 2 - by location
        -- 3 - by user
        @orderby tinyint = 1,
        -- Pagination support (optional parameter, defaults will be set),
        @PageNumber int = 1, 
        @PageSize int = 20,
		-- Related Asset attribute
        @attributeId bigint = 0,
		-- Related Asset id
        @assetId bigint = 0
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;

		-- Number of rows returned!
		DECLARE @SearchBufferCount int = 0;
		DECLARE @SearchBufferSize int = 400;
		
		IF @PageSize > 400 BEGIN
			SET @SearchBufferSize = @PageSize
		END
		
		-- Check ConfigIds and TaxonomyItemsIds
        IF @ConfigIds = '' 
			SET @ConfigIds = NULL;
		IF @taxonomyItemsIds = ''
			SET @taxonomyItemsIds = NULL;		

        -- Check if results are already saved in temp table, so search can be skipped ...
		SELECT @SearchBufferCount = COUNT(1) FROM _search_srchres WHERE SearchId = @SearchId AND UserId = @UserId

		IF (@SearchBufferCount > 0)
		BEGIN
			IF (@PageNumber < 2)
			BEGIN
				DELETE _search_srchres WHERE SearchId = @SearchId
				DELETE _search_srchcount WHERE SearchId = @SearchId
				SET @SearchBufferCount = 0
			END
			ELSE	
				-- Check if there are still records in the temp table for the next page
				IF (@SearchBufferCount / @PageSize) >= @PageNumber
				BEGIN							
					EXEC _cust_GetSrchResPage @SearchId, @UserId, @active, @PageNumber, @PageSize
					RETURN
				END
		END

		-- Check for customized queries
		DECLARE @IsCustomQuery bit = 0;
		EXEC _cust_SearchCustomQuery @SearchId, @UserId, @keywords, @ConfigIds, @taxonomyItemsIds, @SearchBufferCount, @IsCustomQuery OUTPUT

		IF NOT @IsCustomQuery = 1
		BEGIN
		
			-- Analyse keywords
			IF CHARINDEX('*', @keywords) = 0
			BEGIN
				SET @keywords = '"' + REPLACE(@keywords, ' ', '*" AND "') + '*"';
			END		
			
			-- Search active assets
			IF @active = 1
			BEGIN
				EXEC _cust_SearchByKeywords_Active @SearchId, @UserId, @keywords, @ConfigIds, @taxonomyItemsIds, @active, @orderby, @SearchBufferCount, @SearchBufferSize, @attributeId, @assetId
			END

			-- Search history
			IF @active = 0
			BEGIN
				EXEC _cust_SearchByKeywords_History @SearchId, @UserId, @keywords, @ConfigIds, @taxonomyItemsIds, @active, @orderby, @SearchBufferCount, @SearchBufferSize, @attributeId, @assetId
			END
		END
		
		-- Get the results and return ...
        EXEC _cust_GetSrchResPage @SearchId, @UserId, @active, @PageNumber, @PageSize;
   
        SET ROWCOUNT 0;

END
GO


-- =============================================
-- Author		: Wouter Steegmans
-- Create date	: 21.08.2012
-- Description	: Performs an initial search by keywords
-- =============================================
ALTER PROCEDURE [dbo].[_cust_SearchByTypeContext] 
        @SearchId uniqueidentifier,
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
		DECLARE @SearchBufferSize int = 400;
		
		IF @PageSize > 400 BEGIN
			SET @SearchBufferSize = @PageSize
		END		
				
		-- Check ConfigIds and TaxonomyItemsIds
        IF @ConfigIds = '' 
			SET @ConfigIds = NULL;
		IF @taxonomyItemsIds = ''
			SET @taxonomyItemsIds = NULL;		

        -- Check if results are already saved in temp table, so search can be skipped ...
		SELECT @SearchBufferCount = COUNT(1) FROM _search_srchres WHERE SearchId = @SearchId AND UserId = @UserId

		IF (@SearchBufferCount > 0)
		BEGIN
			-- do not use cache for the page #1
			IF (@PageNumber < 2)BEGIN
				DELETE _search_srchres WHERE SearchId = @SearchId
				DELETE _search_srchcount WHERE SearchId = @SearchId
				SET @SearchBufferCount = 0
			END
			ELSE	
				-- Check if there are still records in the temp table for the next page
				IF (@SearchBufferCount / @PageSize) >= @PageNumber
				BEGIN							
					EXEC _cust_GetSrchResPage @SearchId, @UserId, @active, @PageNumber, @PageSize
					RETURN
				END
		END

		-- Search active assets
		IF @active = 1
		BEGIN
			EXEC _cust_SearchByTypeContext_Active @SearchId, @UserId, @ConfigIds, @taxonomyItemsIds, @active, @orderby, @SearchBufferCount, @SearchBufferSize
		END

		-- Search history
		IF @active = 0
		BEGIN
			EXEC _cust_SearchByTypeContext_History @SearchId, @UserId, @ConfigIds, @taxonomyItemsIds, @active, @orderby, @SearchBufferCount, @SearchBufferSize
		END

		-- Get the results and return ...
        EXEC _cust_GetSrchResPage @SearchId, @UserId, @active, @PageNumber, @PageSize;
        
        SET ROWCOUNT 0;

END
GO
