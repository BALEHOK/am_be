/****** Object:  StoredProcedure [dbo].[_cust_CreateNewRevision]    Script Date: 03/05/2013 12:10:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[_cust_UpdateAssetsReferences] 
	@entities DynEntityIdsTableType READONLY	-- table variable
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Temp tables
	CREATE TABLE #t
	(
		DynEntityUid bigint, 
		DynEntityConfigUid bigint, 
		NewDynEntityUid bigint
	)

	-- Variable declarations
	DECLARE @DynEntityUid bigint = 0;
	DECLARE @DynEntityConfigUid bigint = 0;
	DECLARE @PrevDynEntityConfigUid bigint = 0;

	DECLARE @tempConfigUids TABLE (DynEntityConfigUid bigint)
	DECLARE @tempMultipleAssetAtts TABLE (DynEntityAttribConfigId bigint, DBTableName varchar(100))
	
	DECLARE @NewEntityUid bigint;
	DECLARE @DynEntityConfigId BIGINT;
	DECLARE @DynEntityDBTableName VARCHAR(50);
	DECLARE @AttributeNames VARCHAR(5000) = '';
	
	DECLARE @DynEntityAttribConfigId bigint;
	DECLARE @DynRelatedMaaEntityDBTableName nvarchar(128);
	
	DECLARE @i int = 1;
	DECLARE @rows int;
	
	DECLARE @sqlCommand NVARCHAR(max);
	
	-- Get all different Config's
	INSERT @tempConfigUids
	SELECT DISTINCT DynEntityConfigUid FROM @entities;

	SET @rows = @@ROWCOUNT
				
END

GO


