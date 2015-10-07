CREATE TRIGGER [dbo].[tr_Insert%%TableName%%RevisionNumber] 
	ON [dbo].[%%TableName%%] INSTEAD OF INSERT 
	AS 
	BEGIN 
		-- v.28
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

-- SEPARATOR --

CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_Id_Revision_%%TABLENAME%%] ON [dbo].[%%TABLENAME%%]
( [DynEntityId] ASC, [Revision] ASC );

-- SEPARATOR --

CREATE NONCLUSTERED INDEX [IX_Id_ActiveVersion_%%TABLENAME%%] ON [dbo].[%%TABLENAME%%]
( [DynEntityId] ASC, [ActiveVersion] ASC ); 

-- SEPARATOR --