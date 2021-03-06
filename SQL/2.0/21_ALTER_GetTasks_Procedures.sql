SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.f_GetGrantedTaskIds') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ) ) 
DROP FUNCTION [dbo].[f_GetGrantedTaskIds]
GO

CREATE FUNCTION [dbo].[f_GetGrantedTaskIds]
(	
	@UserId bigint
)
RETURNS @permissions TABLE 
(
	TaskId bigint
)
AS
BEGIN
	-- Add the SELECT statement with parameter references here
	INSERT @permissions

		SELECT t.TaskId FROM [Task] t LEFT JOIN [DynEntityConfigTaxonomy] tax ON t.DynEntityConfigId = tax.DynEntityConfigId
		WHERE (
			(SELECT COUNT(*) FROM [dbo].[TaskRights]
				WHERE IsDeny=1 AND UserId=@userId AND
					(DynEntityConfigId=t.DynEntityConfigId OR TaxonomyItemId=tax.TaxonomyItemId OR (DynEntityConfigId=0 AND TaxonomyItemId=0)))=0
			AND
			 (SELECT COUNT(*) FROM [dbo].[TaskRights] WHERE IsDeny=0 AND UserId=@userId AND  
				(DynEntityConfigId=t.DynEntityConfigId OR TaxonomyItemId=tax.TaxonomyItemId OR 
				(DynEntityConfigId=0 AND TaxonomyItemId=0)))<>0
		)	
			  
	RETURN 
END
GO

ALTER PROCEDURE [dbo].[_cust_GetTasks]
	@UserId bigint
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
	WHERE [t].[IsActive] = 1 and [c].[Active] = 1 and [c].[ActiveVersion] = 1 and [TaskId] in (SELECT * FROM [dbo].[f_GetGrantedTaskIds](@UserId))
END
