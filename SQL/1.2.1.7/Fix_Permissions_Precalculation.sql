-- =============================================
-- Author:		Bolkhovsky Ilya
-- Create date: 20.03.2012
-- Description:	Retrieves the assets of given type which is permitted for given user
-- =============================================
ALTER PROCEDURE [dbo].[_cust_GetPermittedAssets]
	@assetTypeId bigint,
	@userId bigint	
AS
BEGIN	

	CREATE TABLE #rights (DepartmentId bigint);        
	CREATE TABLE #childUsers (UserId bigint);
	
	INSERT INTO #rights
		SELECT DISTINCT DepartmentId
		FROM Rights
		WHERE DynEntityConfigId = @assetTypeId
		OR DynEntityConfigId = 0				-- if allow all assettypes
		AND Rights1 >= 8							-- all for reading (1000)
		AND UserId = @userId
		AND IsDeny = 'False';		
		
	INSERT INTO #childUsers
		EXEC _cust_GetUsersTree @userId  				
	
	-- permitted assets
	SELECT DynEntityIndex.*
	FROM DynEntityIndex
	INNER JOIN #rights
	ON #rights.DepartmentId = DynEntityIndex.DepartmentId 
	OR #rights.DepartmentId = 0					-- if allow all departments
	WHERE DynEntityIndex.DynEntityConfigId = @assetTypeId	
	-- own assets
	UNION
	SELECT DynEntityIndex.*
	FROM DynEntityIndex
	WHERE DynEntityIndex.DynEntityConfigId = @assetTypeId
	AND (UserId = @userId OR OwnerId = @userId)	
	-- child user's assets
	UNION
	SELECT DynEntityIndex.*
	FROM DynEntityIndex
	WHERE DynEntityIndex.DynEntityConfigId = @assetTypeId
	AND (UserId IN (SELECT * FROM #childUsers) OR OwnerId IN (SELECT * FROM #childUsers))
	-- child users 
	UNION
	SELECT DynEntityIndex.*
	FROM DynEntityIndex
	WHERE DynEntityIndex.DynEntityConfigId = @assetTypeId AND @assetTypeId = (
		SELECT PredefinedAttributes.DynEntityConfigID
		FROM PredefinedAttributes
		WHERE Name = 'User'
	) AND DynEntityIndex.DynEntityId IN (SELECT * FROM #childUsers)
	
	DROP TABLE #childUsers;	
	DROP TABLE #rights;	
		
END

