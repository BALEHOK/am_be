USE [ACV.SobBub]
GO
/****** Object:  UserDefinedFunction [dbo].[f_GetUserPermissions]    Script Date: 11/03/2012 11:53:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Ilya Bolkhovsky
-- Create date: 11.08.2012
-- Description:	Returns permissions set for a specific user
-- =============================================
ALTER FUNCTION [dbo].[f_GetUserPermissions]
(	
	@UserId bigint
)
RETURNS @permissions TABLE 
(
	DynEntityConfigId bigint,
	DepartmentId bigint,
	TaxonomyItemId bigint,	
	IsDeny bit
)
AS
BEGIN
	-- Add the SELECT statement with parameter references here
	INSERT @permissions

		SELECT CASE r.DynEntityConfigId WHEN 0 THEN -2 ELSE r.DynEntityConfigId END, 
				CASE r.DepartmentId WHEN 0 THEN -2 ELSE r.DepartmentId END, 
				CASE r.CategoryId WHEN 0 THEN -2 ELSE r.CategoryId END, 
				MAX(CONVERT(tinyint, r.IsDeny)) -- Deny is always dominant
		  FROM Rights AS r
		 WHERE Rights1 >= 8		-- all for reading (1000)
		   AND UserId = @userId	
		   AND (r.DynEntityConfigId <> 0 OR r.DepartmentId <> 0 OR r.CategoryId <> 0)
		   -- By grouping them, there is a first selection if the same for ex. asset type is allowed and denied.
		 GROUP BY r.DynEntityConfigId, r.DepartmentId, r.CategoryId
		UNION 
		SELECT	-1,-1,-1, 
				r.IsDeny
		  FROM Rights AS r
		 WHERE Rights1 >= 8		-- all for reading (1000)
		   AND UserId = @userId	
		   AND (r.DynEntityConfigId = 0 AND r.DepartmentId = 0 AND r.CategoryId = 0)	
			  
	RETURN 
END

