SELECT [Name] FROM [DataType]
WHERE [Name] = N'childAssets'

IF @@ROWCOUNT = 0
BEGIN
	INSERT [DataType]
		(
			[Name]
			,[NameTranslationId]
			,[DBDataType]
			,[FrameworkDataType]
			,[Comment]
			,[UpdateUserId]
			,[UpdateDate]
			,[StringSize]
			,[DefaultValueID]
			,[ValidationExpr]
			,[IsInternal]
			,[IsEditable]
			,[ValidationMessage]
		)
	VALUES
		(
			'childAssets', 'childAssets', 'na',	'na', NULL, 1, GETDATE(), NULL, NULL, NULL, 1, 0, NULL
		)

	INSERT [DataTypeSearchOperators]
	SELECT [DataTypeUid], [SearchOperatorUid] FROM
		(SELECT DataTypeUid FROM [DataType] WHERE [Name] = 'childAssets') as t cross join
		(SELECT [SearchOperatorUid] FROM [SearchOperators] WHERE [ServiceMethod] IN (N'AssetInOperator', N'AssetNotInOperator')) as op
END