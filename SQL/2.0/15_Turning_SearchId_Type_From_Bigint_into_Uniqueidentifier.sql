GO
begin tran

DROP TABLE SearchTracking

CREATE TABLE SearchTracking (
	Id	bigint NOT NULL Primary Key IDENTITY(1,1),
	SearchType smallint NOT NULL,
	Parameters xml NOT NULL,
	UpdateUser bigint NOT NULL,
	UpdateDate datetime NOT NULL,
	VerboseString nvarchar(MAX) NOT NULL,
	SearchId uniqueidentifier NOT NULL
)

CREATE UNIQUE NONCLUSTERED INDEX [IX_SearchTracking_SearchId] ON [dbo].[SearchTracking]
(
	[SearchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


DROP TABLE _search_srchcount

CREATE TABLE _search_srchcount (
	SearchId uniqueidentifier NULL,
	UserId bigint NULL,
	Type varchar(50) NULL,
	id bigint NULL,
	Count int NULL,
	SearchDateTimeStamp datetime NOT NULL
)

ALTER TABLE [dbo].[_search_srchcount] ADD  DEFAULT (getdate()) FOR [SearchDateTimeStamp]
GO


DROP TABLE _search_srchTypeContext

CREATE TABLE _search_srchTypeContext (
	SearchId uniqueidentifier NOT NULL,
	DynEntityUid bigint NOT NULL,
	DynEntityConfigUid bigint NOT NULL
)

GO

/****** Object:  StoredProcedure [dbo].[_cust_GetSrchCount]    Script Date: 22.11.2015 17:16:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 2012/08/14
-- Description:	Return facet counts + total count
-- =============================================
ALTER PROCEDURE [dbo].[_cust_GetSrchCount]
	-- Add the parameters for the stored procedure here
        @SearchId uniqueidentifier,
        -- Id of current user (required)
        @UserId bigint,
        -- Space-separated list of keywords (optional parameter). Ex.: '"item*" AND "111*"'     
        @keywords nvarchar(1000) = NULL,
        --  Space-separated list of ConfigIds - assettype (optional parameter)
        @ConfigIds varchar(MAX) = NULL,
        --  Space-separated list of Taxonomies (optional parameter)
        @taxonomyItemsIds varchar(MAX) = NULL,
        -- Search area
        @active bit = 1,
		-- Search type: Keywords or Type/Context
        @type bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET FMTONLY OFF;
	DECLARE @count int = 0;

	IF NOT EXISTS (SELECT 1 FROM _search_srchcount WHERE SearchId=@SearchId AND UserId=@UserId)
	BEGIN
		INSERT _search_srchcount (SearchId, UserId, [Type], id, [Count])
		SELECT @SearchId, @UserId, 'TotalCount', 0 As Id, COUNT(1) AS Count 
		  FROM _search_srchres
		 WHERE SearchId=@SearchId AND UserId=@UserId;
		SELECT @count = COUNT FROM _search_srchcount WHERE SearchId = @SearchId AND UserId = @UserId AND [Type] = 'TotalCount';
		
		-- Check if 400 rows are returned as count, means that the cache recordset was truncated ...
		IF @count = 400 
		BEGIN
			IF ISNULL(@keywords, '') <> '' AND @active = 1
			BEGIN
				EXEC _cust_GetSrchCount4Top_Keywords_Active @SearchId, @UserId, @keywords, @ConfigIds, @taxonomyItemsIds
			END
			IF ISNULL(@keywords, '') <> '' AND @active = 0
			BEGIN
				EXEC _cust_GetSrchCount4Top_Keywords_History @SearchId, @UserId, @keywords, @ConfigIds, @taxonomyItemsIds
			END
			IF @type = 1 AND @active = 1
			BEGIN
				EXEC _cust_GetSrchCount4Top_TypeContext_Active @SearchId, @UserId, @ConfigIds, @taxonomyItemsIds
			END
			IF @type = 1 AND @active = 0
			BEGIN
				EXEC _cust_GetSrchCount4Top_TypeContext_History @SearchId, @UserId, @ConfigIds, @taxonomyItemsIds
			END
		END
		ELSE BEGIN
			INSERT _search_srchcount (SearchId, UserId, [Type], id, [Count])
			SELECT @SearchId, @UserId, 'AssetType', DynEntityConfigId, COUNT(1) AS Count
			  FROM _search_srchres
			 WHERE SearchId=@SearchId AND UserId=@UserId
			 GROUP BY DynEntityConfigId
			UNION ALL
			SELECT @SearchId, @UserId, 'Taxonomy', id as TaxonomyId, SUM(IdsCount) AS Count 
			  FROM (
						SELECT TaxonomyItemsIds, COUNT(1) AS IdsCount
						  FROM _search_srchres
						 WHERE SearchId=@SearchId AND UserId=@UserId
						 GROUP BY TaxonomyItemsIds			   
				   ) AS CountTax CROSS APPLY f_SplitIds(TaxonomyItemsIds)
			 GROUP BY id;	 
		END;
	END;	
	
-- Return results

	-- WAITFOR DELAY '000:00:05';

	-- Sort the list, AssetType will be sorted by it's (EN) name.
	WITH tSearchCount AS
	(
		SELECT *, 1 as GroupId, '' AS Name FROM _search_srchcount 
		 WHERE SearchId=@SearchId AND UserId=@UserId
		   AND Type = 'TotalCount'
		 UNION
		SELECT cnt.*, 2 as GroupdId, config.name AS Name
		  FROM _search_srchcount cnt LEFT JOIN DynEntityConfig config ON cnt.id = config.DynEntityConfigId
		 WHERE SearchId=@SearchId AND UserId=@UserId
		   AND config.ActiveVersion = 1
		   AND Type = 'AssetType'
		 UNION
		SELECT *, 3 as GroupId, '' AS Name FROM _search_srchcount 
		 WHERE SearchId=@SearchId AND UserId=@UserId
		   AND Type = 'Taxonomy'
	)
	SELECT SearchId, UserId, Type, id, Count FROM tSearchCount
	 ORDER BY GroupId, Name;

END

/****** Object:  StoredProcedure [dbo].[_cust_GetSrchCount4Top_Keywords_Active]    Script Date: 22.11.2015 17:18:27 ******/
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
        @SearchId uniqueidentifier,
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

/****** Object:  StoredProcedure [dbo].[_cust_GetSrchCount4Top_Keywords_History]    Script Date: 22.11.2015 17:19:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:              Ilya Bolkhovsky, Wouter Steegmans
-- Create date: 16.08.2012
-- Description: Performs facet count for keywords search if top 2500 recores are returned.
-- =============================================
ALTER PROCEDURE [dbo].[_cust_GetSrchCount4Top_Keywords_History] 
        @SearchId uniqueidentifier,
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
			  FROM IndexHistoryDynEntities 
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

/****** Object:  StoredProcedure [dbo].[_cust_GetSrchCount4Top_TypeContext_Active]    Script Date: 22.11.2015 17:20:39 ******/
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
        @SearchId uniqueidentifier,
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
					INNER JOIN _search_srchTypeContext r ON (i.DynEntityUid = r.DynEntityUid AND i.DynEntityConfigUid = r.DynEntityConfigUid)
					       AND r.SearchId = @SearchId
			 WHERE 

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

/****** Object:  StoredProcedure [dbo].[_cust_GetSrchCount4Top_TypeContext_History]    Script Date: 22.11.2015 17:21:23 ******/
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
        @SearchId uniqueidentifier,
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
					INNER JOIN _search_srchTypeContext r ON (i.DynEntityUid = r.DynEntityUid AND i.DynEntityConfigUid = r.DynEntityConfigUid)
						   AND r.SearchId = @SearchId
			 WHERE 

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

/****** Object:  StoredProcedure [dbo].[_cust_GetSrchResPage]    Script Date: 22.11.2015 17:21:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 2012/08/13
-- Description:	Get all results from temp table
-- =============================================
ALTER PROCEDURE [dbo].[_cust_GetSrchResPage]
	-- Add the parameters for the stored procedure here
	@SearchId uniqueidentifier,
	@UserId bigint,
	@Active bit,
	@PageNumber int,
	@PageSize int	
AS
BEGIN
	SET NOCOUNT ON;

-- Get first and last row identifier
	DECLARE @FirstRow int, @LastRow int;
	SELECT  @FirstRow   = ((@PageNumber - 1) * @PageSize) + 1,
			@LastRow    = ((@PageNumber - 1) * @PageSize) + @PageSize;
	
	
	IF @Active = 1
	BEGIN
		SELECT i.*, r.rownumber FROM _search_srchres r INNER JOIN IndexActiveDynEntities i ON r.IndexUid = i.IndexUid
		 WHERE r.active = 1 AND r.rownumber BETWEEN @FirstRow AND @LastRow
		   AND r.SearchId = @SearchId AND r.UserId=@UserId
		 ORDER BY r.rownumber
	END
	ELSE BEGIN
		SELECT i.*, r.rownumber FROM _search_srchres r INNER JOIN IndexHistoryDynEntities i ON r.IndexUid = i.IndexUid
		 WHERE r.active = 0 AND r.rownumber BETWEEN @FirstRow AND @LastRow
		   AND r.SearchId = @SearchId AND r.UserId=@UserId
  		 ORDER BY r.rownumber
	END
	
END

/****** Object:  StoredProcedure [dbo].[_cust_SearchByKeywords]    Script Date: 22.11.2015 17:22:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author		: Ilya Bolkhovsky, Wouter Steegmans
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
        @PageSize int = 20
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
		-- Check if there are still records in the temp table for the next page
		IF (@SearchBufferCount / @PageSize) >= @PageNumber
		BEGIN							
			EXEC _cust_GetSrchResPage @SearchId, @UserId, @active, @PageNumber, @PageSize
			RETURN
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
				EXEC _cust_SearchByKeywords_Active @SearchId, @UserId, @keywords, @ConfigIds, @taxonomyItemsIds, @active, @orderby, @SearchBufferCount, @SearchBufferSize
			END

			-- Search history
			IF @active = 0
			BEGIN
				EXEC _cust_SearchByKeywords_History @SearchId, @UserId, @keywords, @ConfigIds, @taxonomyItemsIds, @active, @orderby, @SearchBufferCount, @SearchBufferSize
			END
		END
		
		-- Get the results and return ...
        EXEC _cust_GetSrchResPage @SearchId, @UserId, @active, @PageNumber, @PageSize;
   
        SET ROWCOUNT 0;

END

/****** Object:  StoredProcedure [dbo].[_cust_SearchByKeywords_Active]    Script Date: 22.11.2015 17:22:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author		: Ilya Bolkhovsky, Wouter Steegmans
-- Create date	: 16.08.2012
-- Description	: Performs an initial search by keywords
-- =============================================
ALTER PROCEDURE [dbo].[_cust_SearchByKeywords_Active] 
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
        @orderby tinyint = 1,
        -- Pagination support (optional parameter, defaults will be set),
        @SearchBufferCount int = 0,
        -- Search buffer, number of records to load at once
        @SearchBufferSize int = 400        
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
					SELECT IndexUid, UserId, OwnerId, DynEntityConfigId, DepartmentId, TaxonomyItemsIds, UpdateDate, Location, [User], 1 AS Active, Name,
						(       
							KEY_TBL1.RANK +						/* AllAttribValuesRank */
							(ISNULL(KEY_TBL2.RANK, 0) * 10) +   /* AllAttrib2IndexValuesRank */
							(ISNULL(KEY_TBL3.RANK, 0) * 100) +  /* AllContextAttribValues */
							(CASE WHEN ISNULL(KEY_TBL4.RANK, 0) > 0 THEN KEY_TBL4.RANK + 10000 ELSE 0 END) +	/* EntityConfigKeywordsRank */
							(CASE WHEN ISNULL(KEY_TBL5.RANK, 0) > 0 THEN KEY_TBL5.RANK + 100000 ELSE 0 END) +   /* KeywordsRank */
							(CASE WHEN ISNULL(KEY_TBL6.RANK, 0) > 0 THEN KEY_TBL6.RANK + 1000000 ELSE 0 END)    /* NameRank */          
						) AS [Rank]
						FROM IndexActiveDynEntities AS FT_TBL 
						
								 JOIN CONTAINSTABLE(IndexActiveDynEntities, (AllAttribValues), @keywords) AS KEY_TBL1 ON FT_TBL.IndexUid = KEY_TBL1.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexActiveDynEntities, (AllContextAttribValues), @keywords) AS KEY_TBL2 ON FT_TBL.IndexUid = KEY_TBL2.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexActiveDynEntities, (AllAttrib2IndexValues), @keywords) AS KEY_TBL3 ON FT_TBL.IndexUid = KEY_TBL3.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexActiveDynEntities, (EntityConfigKeywords), @keywords) AS KEY_TBL4 ON FT_TBL.IndexUid = KEY_TBL4.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexActiveDynEntities, (Keywords), @keywords) AS KEY_TBL5 ON FT_TBL.IndexUid = KEY_TBL5.[KEY]
							LEFT JOIN CONTAINSTABLE(IndexActiveDynEntities, (Name), @keywords) AS KEY_TBL6 ON FT_TBL.IndexUid = KEY_TBL6.[KEY]
						WHERE   
/*
						-- In the future, if search performance has to be optimized, an option is to 
						-- cache the user permissions in a (temp) table with lay-out Userid, Active, IndexUid
						IndexUid IN (SELECT IndexUid FROM ##test WHERE UserId = @UserId) 
*/ 
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
																
				) AS SearchResults
		)
        
		-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
		INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
		SELECT TOP(@SearchBufferSize) @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
		  FROM Search
		 WHERE RowNumber > @SearchBufferCount

END

/****** Object:  StoredProcedure [dbo].[_cust_SearchByKeywords_History]    Script Date: 22.11.2015 17:22:55 ******/
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
        @orderby tinyint = 1,
        -- Pagination support (optional parameter, defaults will be set),
        @SearchBufferCount int = 0,
        -- Search buffer, number of records to load at once
        @SearchBufferSize int = 400
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
																						
				) AS SearchResults
		)
        
		-- Insert results in temp table _search_srchres, so for the next pages (2, 3, ...) this temp table can be queried.
		INSERT _search_srchres (SearchId, UserId, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, rownumber)
		SELECT TOP(@SearchBufferSize) @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
		  FROM Search
		 WHERE RowNumber > @SearchBufferCount

END

/****** Object:  StoredProcedure [dbo].[_cust_SearchByTypeContext]    Script Date: 22.11.2015 17:23:13 ******/
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
		-- Check if there are still records in the temp table for the next page
		IF (@SearchBufferCount / @PageSize) >= @PageNumber
		BEGIN			
			EXEC _cust_GetSrchResPage @SearchId, @UserId, @active, @PageNumber, @PageSize
			RETURN
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

/****** Object:  StoredProcedure [dbo].[_cust_SearchByTypeContext_Active]    Script Date: 22.11.2015 17:23:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author		: Ilya Bolkhovsky, Wouter Steegmans
-- Create date	: 16.08.2012
-- Description	: Performs an initial search by keywords
-- =============================================
ALTER PROCEDURE [dbo].[_cust_SearchByTypeContext_Active] 
        @SearchId uniqueidentifier,
        -- Id of current user (required)
        @UserId bigint,
        -- Extra Select for Search By Type/Context          
        @ConfigIds varchar(MAX) = NULL,
        --  Space-separated list of Taxonomies (optional parameter)
        @taxonomyItemsIds varchar(MAX) = NULL,
        -- Search area
        @active bit = 1, 
        @orderby tinyint = 1,
        -- Pagination support (optional parameter, defaults will be set),
        @SearchBufferCount int = 0,
        -- Search buffer, number of records to load at once
        @SearchBufferSize int = 400        
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;

		-- CTE, First (Search) full text filtering on the keywords
		WITH Search AS
		(	
				SELECT *, ROW_NUMBER() OVER(ORDER BY	CASE WHEN @orderby = 0 THEN Name END,        
														CASE WHEN @orderby = 1 THEN UpdateDate END DESC,
														CASE WHEN @orderby = 2 THEN Location END ASC,
														CASE WHEN @orderby = 3 THEN [User] END ASC/*, Name*/) AS RowNumber                			
				FROM 
				(
					SELECT IndexUid, UserId, OwnerId, DynEntityConfigId, DepartmentId, TaxonomyItemsIds, UpdateDate, Location, [User], 1 AS Active, Name
						FROM IndexActiveDynEntities i
								INNER JOIN _search_srchTypeContext r ON (i.DynEntityUid = r.DynEntityUid AND i.DynEntityConfigUid = r.DynEntityConfigUid)
								       AND r.SearchId = @SearchId
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
		SELECT TOP (@SearchBufferSize) @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
		  FROM Search
		 WHERE RowNumber > @SearchBufferCount	 
		

END

/****** Object:  StoredProcedure [dbo].[_cust_SearchByTypeContext_History]    Script Date: 22.11.2015 17:23:36 ******/
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
        @SearchId uniqueidentifier,
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
        @SearchBufferCount int = 0,
        -- Search buffer, number of records to load at once
        @SearchBufferSize int = 400        
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;

		-- CTE, First (Search) full text filtering on the keywords
		WITH Search AS
		(	
				SELECT *, ROW_NUMBER() OVER(ORDER BY	CASE WHEN @orderby = 0 THEN Name END,        
														CASE WHEN @orderby = 1 THEN UpdateDate END DESC,
														CASE WHEN @orderby = 2 THEN Location END ASC,
														CASE WHEN @orderby = 3 THEN [User] END ASC/*, Name*/) AS RowNumber                			
				FROM 
				(
					SELECT IndexUid, UserId, OwnerId, DynEntityConfigId, DepartmentId, TaxonomyItemsIds, UpdateDate, Location, [User], 0 AS Active, Name
						FROM IndexHistoryDynEntities i
								INNER JOIN _search_srchTypeContext r ON (i.DynEntityUid = r.DynEntityUid AND i.DynEntityConfigUid = r.DynEntityConfigUid)
									   AND r.SearchId = @SearchId
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
		SELECT TOP(@SearchBufferSize) @searchid, @userid, IndexUid, Active, DynEntityConfigId, TaxonomyItemsIds, RowNumber
		  FROM Search
		 WHERE RowNumber > @SearchBufferCount
		
		
END

/****** Object:  StoredProcedure [dbo].[_cust_SearchCustomQuery]    Script Date: 22.11.2015 17:23:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author		: Wouter Steegmans
-- Create date	: 05/01/2013
-- Description	: Checks for custom queries
-- =============================================
-- Overview custom queries
--	* ...
-- =============================================
ALTER PROCEDURE [dbo].[_cust_SearchCustomQuery]
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
        @SearchBufferCount int = 0,
        @IsCustomQuery int OUTPUT
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;
        
		IF CHARINDEX('[', @keywords) = 0 RETURN

/*
		IF @keywords = '[]'
		BEGIN
			SET @IsCustomQuery = 1;
			RETURN;
		END
*/
END

commit tran

GO