/****** Object:  UserDefinedFunction [dbo].[f_GetGrantedSearchUserIds]    Script Date: 12/12/2012 10:59:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Ilya Bolkhovsky
-- Create date: 11.08.2012
-- Description:	Returns permissions set for a specific user
-- =============================================
CREATE FUNCTION [dbo].[f_GetGrantedSearchUserIds]
(	
	@UserId bigint
)
RETURNS @UserIds TABLE 
(
	UserId bigint
)
AS
BEGIN

	DECLARE @RelatedUserId bigint;
	DECLARE @AttribConfigIdAttribUsers bigint;
	
	-- Get DynEntityAttribConfigId for attribute 'Users' (User asset type)
	SELECT @AttribConfigIdAttribUsers = DynEntityAttribConfigId 
	  FROM DynEntityAttribConfig ac
			INNER JOIN DynEntityConfig c ON ac.DynEntityConfigUid = ac.DynEntityConfigUid
	 WHERE ac.ActiveVersion = 1
	   AND c.ActiveVersion = 1
	   AND c.DynEntityConfigId = 
				(	SELECT DynEntityConfigId
					  FROM PredefinedAttributes
					 WHERE Name = 'User'
				)
	   AND ac.Name = 'Users'
		
	-- Insert the current UserId in the result table
	INSERT @UserIds
		SELECT DynEntityId FROM ADynEntityUser
		 WHERE DynEntityId = @UserId
		   AND ActiveVersion = 1;

	-- Start a loop to get all related users from the current user
	SELECT DISTINCT @RelatedUserId = MIN(RelatedDynEntityId)
	  FROM MultipleAssetsActive ma
	 WHERE DynEntityId = @UserId
	   AND DynEntityAttribConfigId = @AttribConfigIdAttribUsers
	 ORDER BY 1
		
	WHILE @RelatedUserId IS NOT NULL
	BEGIN
		-- Recursive call to go a level deeper ...
		INSERT @UserIds
		SELECT * FROM f_GetGrantedSearchUserIds(@RelatedUserId)
		
		-- Get next related user from current ...
		SELECT DISTINCT @RelatedUserId = MIN(RelatedDynEntityId)
		  FROM MultipleAssetsActive ma
		 WHERE DynEntityId = @UserId
		   AND DynEntityAttribConfigId = @AttribConfigIdAttribUsers
		   AND RelatedDynEntityId > @RelatedUserId
		 ORDER BY 1
	END

	RETURN 
END

