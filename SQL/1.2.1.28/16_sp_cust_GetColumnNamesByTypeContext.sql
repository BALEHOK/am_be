/****** Object:  StoredProcedure [dbo].[_cust_GetColumnNamesByTypeContext]    Script Date: 06/21/2013 18:12:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[_cust_GetColumnNamesByTypeContext]
@TableName varchar(MAX)
AS
BEGIN

	select deac.name, deac.dbtablefieldname, deac.IsShownOnPanel from DynEntityAttribConfig deac
	inner join DynEntityConfig dec on deac.DynEntityConfigUid = dec.DynEntityConfigUid and dec.DBTableName = @TableName and dec.ActiveVersion = 1

	SET NOCOUNT ON;
END

GO
