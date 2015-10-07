-- =============================================
-- Author		: Wouter Steegmans
-- Create date	: 21.08.2012
-- Description	: Performs an initial search by keywords
-- =============================================
CREATE PROCEDURE [dbo].[_cust_SearchByTypeContext] 
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
/*
        DROP TABLE ##permissions
        DROP TABLE ##users
        DROP TABLE ##srchres
        DROP TABLE ##srchcount
*/

		-- Number of rows returned!
		DECLARE @SearchBufferCount int = 0;
				
		-- Check ConfigIds and TaxonomyItemsIds
        IF @ConfigIds = '' 
			SET @ConfigIds = NULL;
		IF @taxonomyItemsIds = ''
			SET @taxonomyItemsIds = NULL;		

        -- Check if results are already saved in temp table, so search can be skipped ...
        IF NOT OBJECT_ID('tempdb..##srchres') IS NULL
        BEGIN
			SELECT @SearchBufferCount = COUNT(1) FROM ##srchres WHERE SearchId = @SearchId AND UserId = @UserId
			-- Check if there are still records in the temp table for the next page
			IF (@SearchBufferCount / @PageSize) >= @PageNumber
			BEGIN			
				EXEC _cust_GetSrchResPage @SearchId, @UserId, @active, @PageNumber, @PageSize
				RETURN
			END
        END

        -- Create global temp tables
        IF OBJECT_ID('tempdb..##permissions') IS NULL
        BEGIN
                CREATE TABLE ##permissions 
                (
                        CurUserId bigint,
                        DynEntityConfigId bigint,
                        DepartmentId bigint,
                        TaxonomyItemId bigint,  
                        IsDeny bit
                );
                CREATE INDEX IX_1 ON ##permissions (CurUserId)
        END

        IF OBJECT_ID('tempdb..##users') IS NULL
        BEGIN
                CREATE TABLE ##users 
                (
                        CurUserId bigint, 
                        UserId bigint
                );
                CREATE INDEX IX_1 ON ##users (CurUserId, UserId)
        END

        IF OBJECT_ID('tempdb..##srchres') IS NULL
        BEGIN
                CREATE TABLE ##srchres 
                (       SearchId bigint, 
                        UserId bigint, 
                        IndexUid bigint, 
                        Active bit, 
                        DynEntityConfigId bigint,
                        TaxonomyItemsIds nvarchar(1000),
                        rownumber int,
                ); 
                CREATE INDEX IX_1 ON ##srchres (SearchId, UserId, IndexUid, Active);
                CREATE INDEX IX_2 ON ##srchres (SearchId, UserId, rownumber);
        END

        IF OBJECT_ID('tempdb..##srchcount') IS NULL
        BEGIN
                CREATE TABLE ##srchcount
                (       SearchId bigint, 
                        UserId bigint, 
                        [Type] varchar(50), 
                        id bigint,
                        Count int
                ); 
                CREATE INDEX IX_1 ON ##srchcount (SearchId, UserId, [Type], id);
        END

        -- Fill permissions temp table
        IF NOT EXISTS(SELECT 1 FROM ##permissions WHERE CurUserId = @UserId)
        BEGIN
                INSERT INTO ##permissions
                        SELECT @UserId, * FROM f_GetUserPermissions(@UserId);
        END

        -- Fill users hierarchy temp table
        IF NOT EXISTS(SELECT 1 FROM ##users WHERE CurUserId = @UserId)
        BEGIN
                INSERT INTO ##users
                        SELECT @UserId, @UserId;
                CREATE TABLE #users (UserId bigint);
                INSERT INTO #users      
                        EXEC _cust_GetUsersTree @UserID;
                INSERT INTO ##users
                        SELECT @UserId, * FROM #users;
                DROP TABLE #users
        END

        -- Fill taxonomy items (SP parameter) ids table
        DECLARE @taxonomies TABLE (TaxonomyItemId int);
        INSERT INTO @taxonomies 
                SELECT * FROM f_SplitIds(@taxonomyItemsIds);

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
GO

