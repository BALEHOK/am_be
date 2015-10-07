SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wouter Steegmans, Ilya Bolkhovsky	
-- Create date: 22/02/2013
-- Description:	Rebuilds triggers of entities table
-- =============================================
CREATE PROCEDURE _cust_RebuildTriggers
	@dynEntityConfigUid bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET FMTONLY OFF;
	
    DECLARE @triggers TABLE (id int identity(1,1), schemaname nvarchar(50), triggername nvarchar(255), tablename nvarchar(255))
	DECLARE @i int = 1,
			@rowcount int,
			@sqlCommand nvarchar(max) = '',
			@schemaname nvarchar(50), 
			@triggername nvarchar(255),
			@tablename nvarchar(144),
			@Fields nvarchar(max);

	DECLARE @CREATETRIGGER_DEFINITION nvarchar(max) = '
	CREATE TRIGGER [dbo].[tr_Insert%%TableName%%RevisionNumber] 
	ON [dbo].[%%TableName%%] INSTEAD OF INSERT 
	AS 
	BEGIN 

		DECLARE @rowcount int = @@ROWCOUNT;
		DECLARE @GetIdentity bit;
		
		-- If just one row, explicitly return the new identity (needed when AFTER INSERT/UPDATE trigger creates extra records)
		IF @rowcount = 1
			SET @GetIdentity = 1
		ELSE
			SET @GetIdentity = 0
			
		SET NOCOUNT ON;

		DECLARE @uid bigint;
		DECLARE @minId bigint;

		-- Select Min ID. For new records (DynEntityId = 0), temporary a negative ID will generated to not break 
		-- the unique index IX_%%TableName%%_Id_Revision
		SELECT @minId = ISNULL(MIN(DynEntityId), 0) FROM %%TableName%% (READUNCOMMITTED)
		SET @minId = (ABS(@minId) + 1000000) * (-1);

		-- In a temp table, retrieve the latest revision for each item
		WITH temp (Id, LastRev) AS
		(
			SELECT DynEntityId, MAX(Revision) 
			  FROM %%TableName%% (ROWLOCK)
			 WHERE DynEntityId IN (SELECT DynEntityId FROM Inserted)
			 GROUP BY DynEntityId
		)
		-- Insert items. For new items, create a temporary negative DynEntityId to remain unique on DynEntityId|ActiveVersion
		INSERT %%TableName%% (DynEntityId,ActiveVersion,DynEntityConfigUid,Name,Revision,%%Fields%%)
		SELECT CASE DynEntityId WHEN 0 THEN @minId-ROW_NUMBER() OVER (ORDER BY DynEntityId) ELSE DynEntityId END,ActiveVersion,DynEntityConfigUid,Name,
			   ISNULL(t.LastRev, 0) +1,%%Fields%%
		  FROM INSERTED i
					LEFT OUTER JOIN temp t ON i.DynEntityId = t.Id

		-- Get the identity ID (only when one record is inserted - from the framework)
		IF @GetIdentity = 1
			SET @uid = Scope_Identity();	

		-- Update DynEntityId for new items
		UPDATE %%TableName%% SET DynEntityId = DynEntityUid 
		 WHERE DynEntityId BETWEEN (@minId - @rowcount) AND @minId 
		 

		-- The following statements MUST be last in this trigger. It resets @@Identity
		-- to be the same as the earlier Scope_Identity() value.
		-- http://stackoverflow.com/questions/908257/instead-of-trigger-in-sql-server-loses-scope-identity	
		IF @GetIdentity = 1
		BEGIN	
			SELECT DynEntityUid INTO #Trash FROM [%%TableName%%] WHERE DynEntityUid=@uid;
			DROP TABLE #Trash;	
		END

	END;
	'
	
	-- Save triggers in temp table ...
	INSERT @triggers
	SELECT tbl.name AS [schemaName], trg.name AS triggerName, tbl.tblname AS [tablename]
	 FROM sys.triggers trg
	 INNER JOIN DynEntityConfig dc ON dc.DynEntityConfigUid = @dynEntityConfigUid
				LEFT OUTER JOIN 
					(	SELECT tparent.object_id, ts.name, tparent.name AS tblname 
						  FROM sys.tables tparent 
									INNER JOIN sys.schemas ts ON TS.schema_id = tparent.SCHEMA_ID
					) AS tbl ON tbl.OBJECT_ID = trg.parent_id
     
	 WHERE tbl.tblname = dc.DBTableName AND trg.name NOT IN ('tr_SetADynEntityBUBDossierID','tr_SetADynEntityPersonID','tr_SetDynEntityId_ADynEntityBUB_Certificates_B','tr_SetDynEntityId_ADynEntitySOB_Certificates_B')

	SELECT @rowcount = @@ROWCOUNT

	-- Loop all triggers and build dynamic sql ...
	WHILE @i <= @rowcount BEGIN
		SELECT TOP 1 @schemaname = schemaname, @triggername = triggername FROM @triggers WHERE id = @i
		SET @sqlCommand = @sqlCommand + 'DROP TRIGGER [' + @schemaname + '].[' + @triggername + ']; ' 
		
		SET @i = @i + 1
	END

	-- Delete all triggers ...
	EXEC sp_executesql @sqlCommand

	-- Add new triggers ...
	SET @i = 1;
	WHILE @i <= @rowcount BEGIN
		SELECT TOP 1 @tablename = tablename FROM @triggers WHERE id = @i
		
		SET @sqlCommand = '
		SELECT @Fields = 
			STUFF
			(
				(	SELECT '',['' + c.name + '']''
					  FROM sys.tables t
							INNER JOIN sys.columns c ON t.OBJECT_ID = c.OBJECT_ID
					 WHERE t.Name = ''' + @tablename + '''
					   AND c.Name NOT IN (''DynEntityUid'',''DynEntityId'',''ActiveVersion'',''DynEntityConfigUid'',''Name'',''Revision'')
					   FOR XML PATH (''''), TYPE
				).value(''.'', ''nvarchar(max)'')
			,1,1,'''') '
		EXEC sp_executesql @sqlCommand, N'@Fields nvarchar(max) OUTPUT', @Fields = @Fields OUTPUT
		
		SET @sqlCommand = REPLACE(REPLACE(@CREATETRIGGER_DEFINITION, '%%TableName%%', @tablename), '%%Fields%%', @Fields)
		EXEC sp_executesql @sqlCommand	
		SET @i = @i + 1
	END 

END
GO
