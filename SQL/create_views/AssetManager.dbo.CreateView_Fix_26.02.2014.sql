/****** Object:  StoredProcedure [dbo].[CreateView]    Script Date: 26.02.2014 12:10:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Igor 
-- Description:	Creates view by table config id and table name that return actual data for assets
-- =============================================
ALTER PROCEDURE [dbo].[CreateView]
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

DECLARE @prematureDelete nvarchar(MAX);
SET @prematureDelete = 'IF EXISTS(SELECT name FROM sys.objects WHERE name = N''Report_' + @ViewTableName + ''') DROP VIEW dbo.Report_' + @ViewTableName + ';';
EXEC (@prematureDelete)
EXEC (@dynamicSql)

END

