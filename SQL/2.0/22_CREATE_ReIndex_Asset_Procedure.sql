
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo._cust_ReIndex_Asset') AND type IN (N'P', N'PC')) 
DROP PROCEDURE [dbo].[_cust_ReIndex_Asset]
GO

-- =============================================
-- Author:		Alexandr Shukletsov
-- Create date: 10/01/2015
-- Description:	reindex active version of asset
-- =============================================
CREATE PROCEDURE _cust_ReIndex_Asset
	@DynEntityUidNew bigint,
	@DynEntityId bigint,
	@DynEntityConfigUidNew bigint,
	@DynEntityConfigId bigint
AS
BEGIN
	DECLARE @DynEntityUidOld bigint, @DynEntityConfigUidOld bigint
	SELECT @DynEntityUidOld = [DynEntityUid], @DynEntityConfigUidOld = [DynEntityConfigUid]
	FROM  [IndexActiveDynEntities]
	WHERE [DynEntityId] = @DynEntityId AND [DynEntityConfigId] = @DynEntityConfigId
	
	BEGIN TRAN

		IF (@DynEntityUidOld IS NOT NULL AND @DynEntityConfigUidOld IS NOT NULL)
		BEGIN
			DECLARE	@oldEntities DynEntityIdsTableType
			INSERT INTO @oldEntities
			([DynEntityUid], [DynEntityId], [DynEntityConfigUid])
			VALUES
			(@DynEntityUidOld, @DynEntityId, @DynEntityConfigUidOld)

			EXEC [dbo].[_cust_ReIndex]
				@active = 0,
				@buildDynEntityIndex = 0,
				@entities = @oldEntities
		END

		DELETE [IndexActiveDynEntities]
		WHERE [DynEntityId] = @DynEntityId AND [DynEntityConfigId] = @DynEntityConfigId

		DECLARE	@newEntities DynEntityIdsTableType
		INSERT INTO @newEntities
		([DynEntityUid], [DynEntityId], [DynEntityConfigUid])
		VALUES
		(@DynEntityUidNew, @DynEntityId, @DynEntityConfigUidNew)

		EXEC [dbo].[_cust_ReIndex]
			@active = 1,
			@buildDynEntityIndex = 1,
			@entities = @newEntities

	COMMIT TRAN
END
GO
