UPDATE Rights SET DynEntityConfigId = 0 WHERE DynEntityConfigId = -1;

/****** Object:  UserDefinedFunction [dbo].[f_GetUserPermissions]    Script Date: 10/12/2012 15:33:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:              Ilya Bolkhovsky
-- Create date: 11.08.2012
-- Description: Returns permissions set for a specific user
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
        INSERT INTO @permissions
                SELECT CASE r.DynEntityConfigId WHEN 0 THEN -1 ELSE r.DynEntityConfigId END, r.DepartmentId, r.CategoryId, r.IsDeny
                FROM Rights AS r
                WHERE Rights1 >= 8                                                      -- all for reading (1000)
                          AND UserId = @userId          
        RETURN 
END
