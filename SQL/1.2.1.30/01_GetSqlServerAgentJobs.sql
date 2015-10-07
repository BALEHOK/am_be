-- =============================================
-- Author:		Ilya Bolkhovsky
-- Create date: 25.04.2014
-- Description:	Returns the list of SQL Server AgentJobs
-- =============================================
CREATE PROCEDURE _cust_GetSqlServerAgentJobs	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;    
	SELECT job_id, [name] FROM msdb.dbo.sysjobs;
END
GO
