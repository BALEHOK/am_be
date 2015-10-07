

/****** Object:  StoredProcedure [dbo].[_DynEntityIndex_GetPermitted]    Script Date: 02/24/2012 15:59:05 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Bolkhovsky Ilya
-- Create date: 31.08.2009
-- Description:	Retrieves the assets of given type which is permitted for given user
-- =============================================
CREATE PROCEDURE [dbo].[_cust_GetPermittedAssets]
	@assetTypeId bigint,
	@userId bigint	
AS
BEGIN	
	DECLARE @tableName varchar(255);
	DECLARE @sqlQuery varchar(1000);
	
	CREATE TABLE #rights (DepartmentId bigint);
        
	INSERT INTO #rights
		SELECT DISTINCT DepartmentId
		FROM Rights
		WHERE DynEntityConfigId = @assetTypeId
		OR DynEntityConfigId = 0				-- if allow all assettypes
		AND Rights1 >= 8							-- all for reading (1000)
		AND UserId = @userId
		AND IsDeny = 'False';		
		
	SELECT DynEntityIndex.*
	FROM DynEntityIndex
	INNER JOIN #rights
	ON #rights.DepartmentId = DynEntityIndex.DepartmentId 
	OR #rights.DepartmentId = 0					-- if allow all departments
	WHERE DynEntityIndex.DynEntityConfigId = @assetTypeId
		
	DROP TABLE #rights;	
		
END

GO


