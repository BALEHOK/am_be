USE [Anubis_Assetmanager]
GO

/****** Object:  StoredProcedure [dbo].[CreateView]    Script Date: 01/27/2014 14:32:24 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreateView]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CreateView]
GO

USE [Anubis_Assetmanager]
GO

/****** Object:  StoredProcedure [dbo].[CreateView]    Script Date: 01/27/2014 14:32:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Igor 
-- Description:	Creates view by table config id and table name that return actual data for assets
-- =============================================
CREATE PROCEDURE [dbo].[CreateView]
	@DynEntityConfigUid int,
	@TableName NVARCHAR(MAX)
AS	
BEGIN

DECLARE @dynamicSql VARCHAR(MAX)
DECLARE @cols VARCHAR(MAX)
DECLARE @colNames VARCHAR(MAX)
DECLARE @ViewTableName VARCHAR(MAX)

select @cols = (
select ',src.[' + DBTableFieldName + '] as ' + '''' + Label + '''' 
from DynEntityAttribConfig
where DynEntityConfigUid=@DynEntityConfigUid and Active=1 and ActiveVersion = 1
	and Label is not null and Label != ''
for xml path('')
)

SET @cols = SUBSTRING(@cols, 2, LEN(@cols));

SET @ViewTableName = REPLACE(@TableName, 'ADynEntity', '');

SET @dynamicSql = 'IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N''[dbo].[Report_'+@ViewTableName+']''))
DROP VIEW [dbo].[Report_'+@ViewTableName+']'

EXEC (@dynamicSql)

SET @dynamicSql = 'CREATE VIEW Report_' + @ViewTableName + ' AS select src.DynEntityUId as uid, src.DynEntityId as id, ' + @cols + ' from ' + @TableName + ' src {joinResultString} where src.ActiveVersion = 1 '

--------------------------------------------------------------------
--dyn lists start
--------------------------------------------------------------------
DECLARE @uid int;
DECLARE @fieldName NVARCHAR(MAX);
DECLARE @joinString NVARCHAR(MAX);
DECLARE @joinInterimString NVARCHAR(MAX);
DECLARE @joinResultString NVARCHAR(MAX);
SET @joinResultString = '';

SET @joinString = ' LEFT OUTER JOIN dynlistvalue DLV{fieldName} ON
DLV{fieldName}.assetuid = src.DynEntityUId and 
DLV{fieldName}.DynEntityConfigUid = {configUid} and 
DLV{fieldName}.DynListUid = {dynListUid} ';

DECLARE dynlist_cursor CURSOR FOR 

SELECT DynListUid, DBTableFieldName FROM DynEntityAttribConfig
WHERE DynEntityConfigUid = @DynEntityConfigUid AND IsDynListValue = 1

OPEN dynlist_cursor

FETCH NEXT FROM dynlist_cursor
INTO @uid, @fieldName

WHILE @@FETCH_STATUS = 0
BEGIN
	SET @joinInterimString = @joinString;
	
	SET @joinInterimString = REPLACE(@joinInterimString, '{configUid}', @DynEntityConfigUid);
	SET @joinInterimString = REPLACE(@joinInterimString, '{dynListUid}', @uid);
	SET @joinInterimString = REPLACE(@joinInterimString, '{fieldName}', @fieldName);
	
	SET @joinResultString = @joinResultString + @joinInterimString;

	SET @dynamicSql = REPLACE(@dynamicSql, 'src.[' + @fieldName + ']', 'DLV' + @fieldName + '.Value');
	SET @dynamicSql = REPLACE(@dynamicSql, 'src.' + @fieldName, 'DLV' + @fieldName + '.Value');

	FETCH NEXT FROM dynlist_cursor into @uid, @fieldName
END

CLOSE dynlist_cursor;
DEALLOCATE dynlist_cursor;

--------------------------------------------------------------------
--dyn lists end
--------------------------------------------------------------------

SET @dynamicSql = REPLACE(@dynamicSql, '{joinResultString}', @joinResultString);

EXEC (@dynamicSql)

END



GO

