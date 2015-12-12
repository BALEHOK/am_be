SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Create date: 11/12/2015
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[_cust_Reindex_BuildIndexField_ForDisplayValues] 
	-- Add the parameters for the stored procedure here
	@DynEntityConfigUid bigint,
	@FieldType int = 0, 
	@RecursiveIndexFieldType int = 0,
	@field nvarchar(max) OUTPUT, 
	@from nvarchar(max) OUTPUT,
	@creatett nvarchar(max) OUTPUT, 
	@droptt nvarchar(max) OUTPUT,
	@culture nvarchar(10),
	@delimiter nvarchar(max) = ' ',
	@PrimaryTable nvarchar(max) = 'prim'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @DESCRIPTION int = 1;
	DECLARE @KEYWORD int = 2;
	DECLARE @FULLTEXTINDEX int = 3;
	DECLARE @ALLATTRIBUTEVALUES int = 4;
	DECLARE @DISPLAYVALUES int = 5;
	DECLARE @DISPLAYEXTVALUES int = 6;

	DECLARE @DynEntityAttribConfigUid bigint;	
	DECLARE @DataType nvarchar(60);
	DECLARE @tAttributes TABLE (DynEntityAttribConfigUid bigint, DataType nvarchar(60));
	DECLARE @AssetLinkedTable varchar(100);
	DECLARE @AssetDynEntityConfigUid bigint;
	
	DECLARE @IsDescription bit = 0;
	DECLARE @IsKeyword bit = 0;
	DECLARE @IsFullTextInidex bit = 0;

	IF @FieldType = @DESCRIPTION 
		SET @IsDescription = 1;
	ELSE IF @FieldType = @KEYWORD
		SET @IsKeyword = 1;
	ELSE IF @FieldType = @FULLTEXTINDEX 
		SET @IsFullTextInidex = 1;
	
	-- Build Dynamic SQL
	INSERT @tAttributes
	SELECT ac.DynEntityAttribConfigUid, dt.Name
	  FROM DynEntityAttribConfig ac
		INNER JOIN DataType dt ON dt.DataTypeUid = ac.DataTypeUid
     WHERE ac.DynEntityConfigUid = @DynEntityConfigUid
	   AND (((@FieldType = @DISPLAYVALUES) AND (DisplayOnResultList = 1)) 
	   OR  ((@FieldType = @DISPLAYEXTVALUES) AND (DisplayOnExtResultList = 1)))
	 ORDER BY CASE @FieldType WHEN @DISPLAYVALUES THEN DisplayOrderResultList WHEN @DISPLAYEXTVALUES THEN DisplayOrderExtResultList ELSE 1 END;
	 	   
	WHILE EXISTS (SELECT 1 FROM @tAttributes)
	BEGIN
		SELECT TOP 1 @DynEntityAttribConfigUid = DynEntityAttribConfigUid, @DataType = DataType FROM @tAttributes
		
		EXEC _cust_ReIndex_BuildSQL @DynEntityAttribConfigUid, @field OUTPUT, @from OUTPUT, @creatett OUTPUT, @droptt OUTPUT,
									@culture, @delimiter, @PrimaryTable, @AssetLinkedTable OUTPUT, @AssetDynEntityConfigUid OUTPUT;
		
		IF @DataType = 'asset' AND @RecursiveIndexFieldType > 0 BEGIN	
			EXEC _cust_Reindex_BuildIndexField	@AssetDynEntityConfigUid, @RecursiveIndexFieldType, 0, @field OUTPUT, @from OUTPUT, @creatett OUTPUT, @droptt OUTPUT,
												@culture, @delimiter, @AssetLinkedTable;
			
		END
		
		DELETE FROM @tAttributes WHERE DynEntityAttribConfigUid = @DynEntityAttribConfigUid
	END;

END
