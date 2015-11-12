SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo._cust_GetReports') AND type IN (N'P', N'PC')) 
DROP PROCEDURE [dbo].[_cust_GetReports]
GO

CREATE PROCEDURE [dbo].[_cust_GetReports]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [ReportUid]
      ,[r].[Name] as [Name]
      ,[IsFinancial]
      ,[ReportFile]
      ,[DynConfigId] as [DynEntityConfigId]
      ,[c].[Name] as [DynEntityConfigName]
	FROM [dbo].[Report] r join [dbo].[DynEntityConfig] c on r.DynConfigId = c.DynEntityConfigId
	where [r].[Type] = 10 and [c].[Active] = 1 and [c].[ActiveVersion] = 1
END
GO
