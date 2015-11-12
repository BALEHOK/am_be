
IF OBJECT_ID (N'dbo.ADynEntityUserPassBackup', N'U') IS NULL
  CREATE TABLE [dbo].[ADynEntityUserPassBackup](
    [DynEntityUid] [bigint] NOT NULL,
    [Name] [nvarchar](255) NOT NULL,
    [Password] [nvarchar](255) NOT NULL,
    [BackupDate] [datetime] NOT NULL DEFAULT (CURRENT_TIMESTAMP),

  ) ON [PRIMARY]
GO

IF OBJECT_ID (N'dbo.tempEncryptPassword', N'FN') IS NOT NULL
  DROP FUNCTION tempEncryptPassword;
GO

CREATE FUNCTION dbo.tempEncryptPassword(@password nvarchar(255))
RETURNS varchar(255) 
AS
BEGIN
  DECLARE @Hashed varbinary(20);
    SET @Hashed = HASHBYTES('SHA1', @password);

    RETURN cast('' as xml).value('xs:base64Binary(sql:variable("@Hashed"))', 'nvarchar(255)');
END;
GO

insert [dbo].[ADynEntityUserPassBackup]
  ([DynEntityUid], [Name], [Password])
  SELECT [DynEntityUid], [Name], [Password]
    FROM [dbo].[ADynEntityUser]
  WHERE RIGHT([Password], 1) != '=' AND [ActiveVersion] = 1

update [dbo].[ADynEntityUser]
  set [Password] = dbo.tempEncryptPassword([Password])
  WHERE RIGHT([Password], 1) != '=' AND [ActiveVersion] = 1
GO

DROP FUNCTION tempEncryptPassword;
GO