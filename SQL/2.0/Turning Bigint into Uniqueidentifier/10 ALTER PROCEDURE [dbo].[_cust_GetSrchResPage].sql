USE [facilitymanager_facilityflexware_com]
GO
/****** Object:  StoredProcedure [dbo].[_cust_GetSrchResPage]    Script Date: 22.11.2015 17:21:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans
-- Create date: 2012/08/13
-- Description:	Get all results from temp table
-- =============================================
ALTER PROCEDURE [dbo].[_cust_GetSrchResPage]
	-- Add the parameters for the stored procedure here
	@SearchId uniqueidentifier,
	@UserId bigint,
	@Active bit,
	@PageNumber int,
	@PageSize int	
AS
BEGIN
	SET NOCOUNT ON;

-- Get first and last row identifier
	DECLARE @FirstRow int, @LastRow int;
	SELECT  @FirstRow   = ((@PageNumber - 1) * @PageSize) + 1,
			@LastRow    = ((@PageNumber - 1) * @PageSize) + @PageSize;
	
	
	IF @Active = 1
	BEGIN
		SELECT i.*, r.rownumber FROM _search_srchres r INNER JOIN IndexActiveDynEntities i ON r.IndexUid = i.IndexUid
		 WHERE r.active = 1 AND r.rownumber BETWEEN @FirstRow AND @LastRow
		   AND r.SearchId = @SearchId AND r.UserId=@UserId
		 ORDER BY r.rownumber
	END
	ELSE BEGIN
		SELECT i.*, r.rownumber FROM _search_srchres r INNER JOIN IndexHistoryDynEntities i ON r.IndexUid = i.IndexUid
		 WHERE r.active = 0 AND r.rownumber BETWEEN @FirstRow AND @LastRow
		   AND r.SearchId = @SearchId AND r.UserId=@UserId
  		 ORDER BY r.rownumber
	END
	
END
