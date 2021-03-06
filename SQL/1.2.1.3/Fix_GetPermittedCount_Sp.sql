
/****** Object:  StoredProcedure [dbo].[_cust_GetPermittedAssetsCount]    Script Date: 02/24/2012 15:57:04 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Bolkhovsky Ilya
-- Create date: 01.09.2009
-- Description:	Retrieves the count of assets of given type which is permitted for given user
-- =============================================
ALTER PROCEDURE [dbo].[_cust_GetPermittedAssetsCount]
	@assetTypeId bigint,
	@userId bigint	
AS
BEGIN
	DECLARE @RowCount int;
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	CREATE TABLE #rights (DepartmentId bigint);
        
	INSERT INTO #rights
		SELECT DepartmentId
		FROM Rights
		WHERE DynEntityConfigId = @assetTypeId
		OR DynEntityConfigId = 0				-- if allow all assettypes
		AND Rights1 >= 8							-- all for reading (1000)
		AND UserId = @userId
		AND IsDeny = 'False';
		
	SELECT COUNT(*) 
	FROM DynEntityIndex
	INNER JOIN #rights
	ON #rights.DepartmentId = DynEntityIndex.DepartmentId 
	OR #rights.DepartmentId = 0					-- if allow all departments
	WHERE DynEntityIndex.DynEntityConfigId = @assetTypeId;
	
	DROP TABLE #rights;
		
END
