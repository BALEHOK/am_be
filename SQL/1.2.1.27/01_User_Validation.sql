/****** Script for SelectTopNRows command from SSMS  ******/
DECLARE @ValidationUid bigint;
SELECT @ValidationUid=ValidationUid FROM ValidationList WHERE Name = 'IsUnique';

UPDATE [DynEntityAttribConfig] SET ValidationExpr = '@IsUnique' WHERE DynEntityAttribConfigId IN
(   SELECT deac.DynEntityAttribConfigId 
	FROM [dbo].[DynEntityAttribConfig] AS deac	
	WHERE LOWER(ValidationExpr) LIKE '%unique%' AND ActiveVersion=1
);	 

INSERT INTO DynEntityAttribValidation
	SELECT deac.DynEntityAttribConfigId, @ValidationUid  
	FROM [dbo].[DynEntityAttribConfig] AS deac
	LEFT JOIN DynEntityAttribValidation AS deav ON deac.DynEntityAttribConfigId = deav.DynEntityAttribConfigId
	WHERE ValidationExpr = '@IsUnique'	 
	AND ActiveVersion=1 
	AND deav.DynEntityAttribConfigId IS NULL;