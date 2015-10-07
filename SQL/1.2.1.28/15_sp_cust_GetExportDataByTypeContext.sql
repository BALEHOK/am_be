USE [Assetmanager]
GO

/****** Object:  StoredProcedure [dbo].[_cust_GetExportDataByTypeContext]    Script Date: 06/26/2013 09:35:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[_cust_GetExportDataByTypeContext]
	@SearchId bigint,
	@TableName varchar(MAX),
	@UserId bigint
AS
BEGIN
	SET NOCOUNT ON;

	EXEC ('SELECT tn.* from ' + @TableName + ' tn
	INNER JOIN _search_srchTypeContext ss
	ON tn.DynEntityUid = ss.DynEntityUid and tn.DynEntityConfigUid = ss.DynEntityConfigUid and ss.SearchId = ' + @SearchId)
END	

GO
