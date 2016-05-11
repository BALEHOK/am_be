-- 08.03.2016 Ilya Bolkhovsky
-- This script adds new IsDeleted column to all dynamic tables
-- and also sets its value accordingly to the obsolete DeletedEntites table.

IF OBJECT_ID('dbo.DeletedEntitiesOld', 'U') IS NOT NULL 
  EXEC sp_rename 'dbo.DeletedEntitiesOld', 'DeletedEntities'; 

DECLARE @DynEntityConfigId bigint,
		@DynEntityUid bigint,
		@DynEntityId bigint,
		@TableName nvarchar(MAX),
		@AlterTableSQL nvarchar(MAX),
		@UpdateSQL nvarchar(MAX),
		@i int = 1,
        @rowcount int,
		@IsDeleted bit;

DECLARE @t TABLE (DynEntityConfigId bigint, DBTableName nvarchar(MAX) );

INSERT @t
  SELECT DynEntityConfigId, DBTableName 
  FROM DynEntityConfig 
  WHERE ActiveVersion = 1 AND Active = 1;

SELECT @rowcount = @@ROWCOUNT

WHILE @i <= @rowcount BEGIN
    SELECT TOP 1 @DynEntityConfigId = DynEntityConfigId, @TableName = DBTableName FROM @t
    
	if not exists(select * from sys.columns where Name = N'IsDeleted' and
              Object_ID = Object_ID(@TableName))
	BEGIN
		SET @AlterTableSQL = 'ALTER TABLE ' + QUOTENAME(@TableName) + ' ADD IsDeleted bit NOT NULL DEFAULT 0';
		EXEC (@AlterTableSQL);		
	END
	
	SET @UpdateSQL = '
		UPDATE A
		SET IsDeleted = 1
		FROM ' + QUOTENAME(@TableName) + ' A
		WHERE EXISTS (
			SELECT * 
			FROM DeletedEntities de 
			WHERE A.DynEntityId = de.DynEntityId
			AND A.DynEntityUid = de.DynEntityUid
			AND de.DynEntityConfigId = @DynEntityConfigId
		)';

	EXECUTE sp_executesql @UpdateSQL, N'@DynEntityConfigId bigint', @DynEntityConfigId

    SET @i = @i + 1
    DELETE FROM @t WHERE DynEntityConfigId = @DynEntityConfigId
END

EXEC sp_rename 'dbo.DeletedEntities', 'DeletedEntitiesOld';