-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[_cust_GetValueByAT_ATA_A] 
	-- Add the parameters for the stored procedure here
	@AssetTypeID bigint,
	@AssetTypeAttributeID bigint,
	@AssetID bigint,
	@isUniqueId bit = 0,
	@result nvarchar(MAX) OUTPUT
AS
BEGIN
	DECLARE 
	@tableName nvarchar(255),
	@fieldName nvarchar(255),
	@sqlQuery nvarchar(1000);	
	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    -- Insert statements for procedure here
	SELECT @tableName = DynEntityConfig.DBTableName 
	FROM DynEntityConfig
	WHERE DynEntityConfigId = @AssetTypeID
	AND ActiveVersion = 'true';
	
	SELECT @fieldName = DynEntityAttribConfig.DBTableFieldname
	FROM DynEntityAttribConfig
	WHERE DynEntityAttribConfigId = @AssetTypeAttributeID
	AND ActiveVersion = 'true';
	
	CREATE TABLE #Result (col1 nvarchar(MAX));
	
	IF @isUniqueId = 0
	BEGIN
		SET @sqlQuery = 'INSERT INTO #Result 
					SELECT CAST([' + @fieldName + '] AS nvarchar(MAX)) FROM [' 
					+  @tableName + '] WHERE ActiveVersion=1 AND DynEntityId=' 
					+ CAST(@AssetID AS nvarchar(10));
	END
	ELSE	
	BEGIN
		SET @sqlQuery = 'INSERT INTO #Result 
					SELECT CAST([' + @fieldName + '] AS nvarchar(MAX)) FROM [' 
					+  @tableName + '] WHERE DynEntityUid=' 
					+ CAST(@AssetID AS nvarchar(10));
	END
	EXEC(@sqlQuery);				
	SELECT @result = col1 FROM #Result;
	DROP TABLE #Result;	
END
