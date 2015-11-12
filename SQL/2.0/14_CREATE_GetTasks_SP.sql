SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo._cust_GetTasks') AND type IN (N'P', N'PC')) 
DROP PROCEDURE [dbo].[_cust_GetTasks]
GO

CREATE PROCEDURE [dbo].[_cust_GetTasks]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [TaskId]
      ,[t].[Name]
      ,[Description]
      ,[t].[DynEntityConfigId]
	  ,[c].[Name] as [DynEntityConfigName]
	FROM [dbo].[Task] t  join [dbo].[DynEntityConfig] c on [t].[DynEntityConfigId] = [c].[DynEntityConfigId]
	WHERE [t].[IsActive] = 1 and [c].[Active] = 1 and [c].[ActiveVersion] = 1
END
GO
