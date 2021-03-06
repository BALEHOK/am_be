SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[_cust_GetExportDataByTypeContext]
	@SearchId uniqueidentifier,
	@TableName varchar(MAX),
	@UserId bigint
AS
BEGIN
	SET NOCOUNT ON;

	EXEC ('SELECT tn.* from ' + @TableName + ' tn
	INNER JOIN _search_srchTypeContext ss
	ON tn.DynEntityUid = ss.DynEntityUid and tn.DynEntityConfigUid = ss.DynEntityConfigUid and ss.SearchId = ''' + @SearchId + '''')
END
