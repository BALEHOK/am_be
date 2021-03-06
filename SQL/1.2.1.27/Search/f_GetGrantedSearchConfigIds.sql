/****** Object:  UserDefinedFunction [dbo].[f_GetGrantedSearchConfigIds]    Script Date: 12/12/2012 10:59:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Ilya Bolkhovsky
-- Create date: 11.08.2012
-- Description:	Returns permissions set for a specific user
-- =============================================
CREATE FUNCTION [dbo].[f_GetGrantedSearchConfigIds]
(	
	@UserId bigint,
	@ConfigId bigint = NULL,
	@TaxonomyId bigint = NULL
	
	
)
RETURNS @ConfigIds TABLE 
(
	DynEntityConfigId bigint
)
AS
BEGIN

	WITH Granted AS
	(
	-- Granted ALL
	SELECT DynEntityConfigId 
	  FROM DynEntityConfig
	 WHERE ActiveVersion = 1
	   AND EXISTS (SELECT 1 
					 FROM Rights 
					WHERE UserId = @UserId 
					  AND DynEntityConfigId = 0 
					  AND CategoryId = 0 
					  AND DepartmentId = 0 
					  AND IsDeny = 0
					  AND Rights1 >= 8		-- all for reading (1000)
					)
	 UNION
	-- Granted Item types
	SELECT DynEntityConfigId 
	  FROM Rights 
	 WHERE DynEntityConfigId > 0
	   AND UserId = @UserId
	   AND IsDeny = 0
	   AND Rights1 >= 8		-- all for reading (1000)
	UNION
	-- Granted for Taxonomies
	SELECT ec.DynEntityConfigId
	  FROM TaxonomyItem ti
			INNER JOIN DynEntityConfigTaxonomy ect ON ect.TaxonomyItemId = ti.TaxonomyItemId
			INNER JOIN DynEntityConfig ec ON ect.DynEntityConfigId = ec.DynEntityConfigId
			INNER JOIN Rights r ON ti.TaxonomyItemId = r.CategoryId
	 WHERE ec.ActiveVersion = 1
	   AND ti.ActiveVersion = 1
	   AND ti.TaxonomyItemId > 0
	   AND r.UserId = @UserId
	   AND r.IsDeny = 0
	   AND r.Rights1 >= 8		-- all for reading (1000)
	 ),
	 Denied AS
	 (
	 -- Denied ALL
	 SELECT DynEntityConfigId 
	  FROM DynEntityConfig
	 WHERE ActiveVersion = 1
	   AND EXISTS (SELECT 1 
					 FROM Rights 
					WHERE UserId = @UserId 
					  AND DynEntityConfigId = 0 
					  AND CategoryId = 0 
					  AND DepartmentId = 0 
					  AND IsDeny = 1
					  AND Rights1 >= 8		-- all for reading (1000)
				   )
	 UNION
	-- Denied Item Types
	SELECT DynEntityConfigId 
	  FROM Rights 
	 WHERE DynEntityConfigId > 0
	   AND UserId = @UserId
	   AND IsDeny = 1
	   AND Rights1 >= 8		-- all for reading (1000)
	UNION
	-- Denied Taxonomies
	SELECT ec.DynEntityConfigId
	  FROM TaxonomyItem ti
			INNER JOIN DynEntityConfigTaxonomy ect ON ect.TaxonomyItemId = ti.TaxonomyItemId
			INNER JOIN DynEntityConfig ec ON ect.DynEntityConfigId = ec.DynEntityConfigId
			INNER JOIN Rights r ON ti.TaxonomyItemId = r.CategoryId
	 WHERE ec.ActiveVersion = 1
	   AND ti.ActiveVersion = 1
	   AND ti.TaxonomyItemId > 0
	   AND r.UserId = @UserId
	   AND r.IsDeny = 1
	   AND r.Rights1 >= 8		-- all for reading (1000)
	 )
	 INSERT @ConfigIds
	 SELECT DISTINCT *
	   FROM Granted
	  WHERE (DynEntityConfigId = @ConfigId OR @ConfigId IS NULL)
		AND (DynEntityConfigId IN (SELECT DynEntityConfigId FROM DynEntityConfigTaxonomy WHERE TaxonomyItemId = @TaxonomyId) OR @TaxonomyId IS NULL)
	 EXCEPT
	 SELECT DISTINCT *
	   FROM Denied
				  
	RETURN 
END

