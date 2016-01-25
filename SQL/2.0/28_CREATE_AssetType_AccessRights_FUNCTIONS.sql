SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.f_GetGrantedConfigIds') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ) ) 
DROP FUNCTION [dbo].[f_GetGrantedConfigIds]
GO

-- =============================================
-- Author:		Alexandr Shukletsov
-- Create date: 19.01.2015
-- Description:	returns Config ids available for read, write or delete for a specific user
-- =============================================
/*
access level bits
54321
5 - delete (10000)
4 - read normal info (01000)
3 - write normal info (00100)
2 - read financial info (00010)
1 - write financial info (00001)
*/
CREATE FUNCTION [dbo].[f_GetGrantedConfigIds]
(	
	@UserId bigint,
	@AccesLevel bigint = NULL
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
					  AND (Rights1 & @AccesLevel) = @AccesLevel
					)
	 UNION
	-- Granted Item types
	SELECT DynEntityConfigId 
	  FROM Rights 
	 WHERE DynEntityConfigId > 0
	   AND UserId = @UserId
	   AND IsDeny = 0
	   AND (Rights1 & @AccesLevel) = @AccesLevel
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
	   AND (Rights1 & @AccesLevel) = @AccesLevel
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
					  AND (Rights1 & @AccesLevel) = @AccesLevel
				   )
	 UNION
	-- Denied Item Types
	SELECT DynEntityConfigId 
	  FROM Rights 
	 WHERE DynEntityConfigId > 0
	   AND UserId = @UserId
	   AND IsDeny = 1
	   AND (Rights1 & @AccesLevel) = @AccesLevel
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
	   AND (Rights1 & @AccesLevel) = @AccesLevel
	 )

	
	 
	 INSERT @ConfigIds
	 SELECT DISTINCT *
	   FROM Granted
	 EXCEPT
	 SELECT DISTINCT *
	   FROM Denied
				  
	RETURN 
END
GO


IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.f_GetGrantedSearchConfigIds') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ) ) 
DROP FUNCTION [dbo].[f_GetGrantedSearchConfigIds]
GO

-- =============================================
-- Author:		Ilya Bolkhovsky
-- Create date: 11.08.2012
-- Description:	Returns permissions set for a specific user
-- =============================================
-- Modified:	Igor Pistolyaka
-- Create date: 29.04.2013
-- Description:	Modified funtion to work with number of TaxonomyId
-- =============================================
CREATE FUNCTION [dbo].[f_GetGrantedSearchConfigIds]
(	
	@UserId bigint,
	@ConfigId bigint = NULL,
	@TaxonomyIds varchar(MAX) = NULL
)
RETURNS @ConfigIds TABLE 
(
	DynEntityConfigId bigint
)
AS
BEGIN

	INSERT @ConfigIds
	 SELECT *
	   FROM dbo.[f_GetGrantedConfigIds](@UserId, 8) -- all for reading (1000)
	  WHERE (DynEntityConfigId = @ConfigId OR @ConfigId IS NULL)
		AND (DynEntityConfigId IN (SELECT DynEntityConfigId FROM DynEntityConfigTaxonomy WHERE (TaxonomyItemId in (select id from dbo.f_SplitIds(@TaxonomyIds)))) OR @TaxonomyIds IS NULL)
				  
	RETURN 
END
GO