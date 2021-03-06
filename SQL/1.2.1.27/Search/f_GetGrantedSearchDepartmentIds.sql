/****** Object:  UserDefinedFunction [dbo].[f_GetGrantedSearchDepartmentIds]    Script Date: 12/12/2012 10:59:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Ilya Bolkhovsky
-- Create date: 11.08.2012
-- Description:	Returns permissions set for a specific user
-- =============================================
CREATE FUNCTION [dbo].[f_GetGrantedSearchDepartmentIds]
(	
	@UserId bigint,
	@DepartmentId bigint = NULL	
)
RETURNS @DepartmentIds TABLE 
(
	DepartmentId bigint
)
AS
BEGIN
	WITH GrantedDepartmentIds AS
	(
	SELECT DISTINCT DepartmentId
	  FROM Rights 
	 WHERE UserId = @UserId 
	   AND IsDeny = 0
	   AND Rights1 >= 8		-- all for reading (1000)
	   AND DepartmentId > 0
	 EXCEPT
	SELECT DISTINCT DepartmentId
	  FROM Rights 
	 WHERE UserId = @UserId 
	   AND IsDeny = 1
	   AND Rights1 >= 8		-- all for reading (1000)
	   AND DepartmentId > 0
	)
	INSERT @DepartmentIds
	SELECT * FROM GrantedDepartmentIds			  
	 WHERE DepartmentId = @DepartmentId OR @DepartmentId IS NULL
	
	RETURN 
END

