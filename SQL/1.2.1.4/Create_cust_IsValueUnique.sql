-- ================================================
-- =============================================
-- Author:		Ilya Bolkhovsky
-- Create date: 02.03.2012
-- Description:	Checks if provided value is unique within others
-- =============================================
CREATE PROCEDURE _cust_IsValueUnique 
	-- Add the parameters for the stored procedure here
	@DynEntityTableName varchar(50), 
	@columnName varchar(100),
	@value nvarchar(MAX),
	@excludeDynEntityId bigint,
	@result bit OUTPUT
AS
BEGIN
	DECLARE @sqlQuery nvarchar(1000),
			@sqlExclude nvarchar(50);
	SET @sqlExclude = ''
	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		
	IF @excludeDynEntityId IS NOT NULL
	BEGIN
		SET @sqlExclude = ' AND DynEntityId != ' + CAST (@excludeDynEntityId AS nvarchar(10))
	END	
    
    SET @sqlQuery = 'INSERT INTO #Result 
						SELECT DynEntityId FROM [' + @DynEntityTableName + '] WHERE ActiveVersion = 1 AND [' + 
						@columnName + '] = ''' + CAST(@value  AS nvarchar(10)) + ''''  + @sqlExclude + ' ;'
	
	CREATE TABLE #Result (col1 bigint);
	EXEC(@sqlQuery)
	IF EXISTS(SELECT col1 FROM #Result)
	BEGIN
		SET @result = 0
	END
	ELSE
	BEGIN
		SET @result = 1
	END	
	DROP TABLE #Result;		
END
GO
