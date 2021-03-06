ALTER PROCEDURE [dbo].[_cust_GetPermittedAssets]
	@assetTypeId bigint,
	@userId bigint,
	@rowStart int,
	@rowsNumber	int
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
	
	IF @rowStart IS NULL AND @rowsNumber IS NULL
	BEGIN
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
	END
	ELSE
	BEGIN
		-- permitted assets
		SELECT r.* FROM (SELECT temp.* ,
		ROW_NUMBER() OVER (ORDER BY temp.DynEntityId) AS RowNumber  
		FROM (SELECT DynEntityIndex.*		
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
		) AND DynEntityIndex.DynEntityId IN (SELECT * FROM #childUsers)) temp) r
		WHERE r.RowNumber>@rowStart AND r.RowNumber<=(@rowStart+@rowsNumber)
	END
		
	DROP TABLE #childUsers;	
	DROP TABLE #rights;	
		
END

