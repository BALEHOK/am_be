SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steegmans Wouter
-- Create date: 19/02/2013
-- Description:	Reindex items of all asset/item types
-- =============================================
CREATE PROCEDURE _cust_ReIndex_All 
	@active bit = 1,
	@buildDynEntityIndex bit = 0,
	@culture nvarchar(10) = 'nl-BE'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @t TABLE (DynEntityConfigId bigint, Name nvarchar(60) );

	DECLARE      @DynEntityConfigId bigint,

				 @Name nvarchar(60),

				 @entities [dbo].[DynEntityIdsTableType],

				 @i int = 1,

				 @rowcount int;
 

	TRUNCATE TABLE IndexActiveDynEntities;

	TRUNCATE TABLE DynEntityIndex; 

	INSERT @t

	SELECT DynEntityConfigId, Name FROM DynEntityConfig WHERE ActiveVersion = @active;	

	SELECT @rowcount = @@ROWCOUNT

 	BEGIN TRY

		   WHILE @i <= @rowcount BEGIN

				 SELECT TOP 1 @DynEntityConfigId = DynEntityConfigId, @Name = Name FROM @t            

				 EXEC _cust_ReIndex @DynEntityConfigId, @active, @buildDynEntityIndex, @culture, @entities = @entities            

				 SET @i = @i + 1

				 DELETE FROM @t WHERE DynEntityConfigId = @DynEntityConfigId

		   END

	END TRY

	BEGIN CATCH

		   DECLARE @ErrorMessage NVARCHAR(4000);

		   DECLARE @ErrorSeverity INT;

		   DECLARE @ErrorState INT;

 		   SELECT

				 @ErrorMessage = ERROR_MESSAGE(),

				 @ErrorSeverity = ERROR_SEVERITY(),

				 @ErrorState = ERROR_STATE();
				             
		   SET @ErrorMessage = @ErrorMessage + ' (Asset/Item type: ' + @Name + ')';

 		   -- Use RAISERROR inside the CATCH block to return error

		   -- information about the original error that caused

		   -- execution to jump to the CATCH block.

		   RAISERROR (@ErrorMessage, -- Message text.

						   @ErrorSeverity, -- Severity.

						   @ErrorState -- State.

						   );       

		   RETURN      

	END CATCH
END
GO
