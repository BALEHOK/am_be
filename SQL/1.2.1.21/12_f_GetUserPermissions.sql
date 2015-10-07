/****** Object:  UserDefinedFunction [dbo].[f_GetUserPermissions]    Script Date: 12.08.2012 1:22:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Ilya Bolkhovsky
-- Create date: 11.08.2012
-- Description:	Returns permissions set for a specific user
-- =============================================
CREATE FUNCTION [dbo].[f_GetUserPermissions]
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
	INSERT INTO @permissions
		SELECT r.DynEntityConfigId, r.DepartmentId, r.CategoryId, r.IsDeny
		FROM Rights AS r
		WHERE Rights1 >= 8							-- all for reading (1000)
			  AND UserId = @userId		
	RETURN 
END

GO

