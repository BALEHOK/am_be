SELECT [SearchOperatorUid] FROM [SearchOperators]
WHERE [ServiceMethod] IN (N'AssetInOperator', N'AssetNotInOperator')

IF @@ROWCOUNT = 0
BEGIN
	INSERT INTO [SearchOperators]
	([Operator], [ServiceMethod], [Description])
	VALUES
	(N'IN', N'AssetInOperator', N'In (for asset only)'),
	(N'NOT IN', N'AssetNotInOperator', N'Not In (for asset only)')

	declare @assetType bigint;
	SELECT @assetType = [DataTypeUid] FROM [DataType] WHERE name = 'asset'

	DELETE [DataTypeSearchOperators]
	WHERE [DataTypeUid] = @assetType

	INSERT INTO [DataTypeSearchOperators]
	(DataTypeUid, SearchOperatorUid)
	SELECT @assetType, [SearchOperatorUid] FROM [SearchOperators]
	WHERE [ServiceMethod] IN (N'AssetInOperator', N'AssetNotInOperator')

END