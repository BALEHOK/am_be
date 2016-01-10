INSERT INTO [AssetTypeScreen]      
	SELECT DISTINCT DynEntityConfigUId, 	
	'default_auto' AS [Name],
	0 AS [Status],
	'Default' AS [Title],
	'Default' AS [Subtitle],
	NULL AS [PageText],
	NULL AS [Comment],
	0 AS [UpdateUserId],
	GETDATE() AS [UpdateDate],	
	(SELECT TOP 1 ScreenLayoutId FROM [DynEntityConfig] WHERE [DynEntityConfigUid] = AttributePanel.DynEntityConfigUId) AS [LayoutId],
	1 AS [IsDefault],
	NEWID() AS [ScreenUid]
	FROM [AttributePanel] AS AttributePanel WHERE ScreenId is NULL GROUP BY DynEntityConfigUId

UPDATE [AttributePanel]
SET ScreenId = (SELECT ScreenId FROM [AssetTypeScreen] 
				WHERE DynEntityConfigUid = [AttributePanel].DynEntityConfigUId AND Name = 'default_auto')
WHERE ScreenId is NULL

UPDATE [AssetTypeScreen]
SET Name = 'Default'
WHERE Name = 'default_auto'