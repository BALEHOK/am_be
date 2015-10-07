SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 2012-10-08
-- Description:	Truncate (delete) index tables - done every night
-- =============================================
CREATE PROCEDURE _cust_TruncateIndexTables
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @currDateTime DATETIME = getdate();

    -- Insert statements for procedure here
	DELETE FROM _search_srchres WHERE SearchDateTimeStamp < DATEADD(hour, 2, @currDateTime)
	DELETE FROM _search_srchcount WHERE SearchDateTimeStamp < DATEADD(hour, 2, @currDateTime)
END
GO
