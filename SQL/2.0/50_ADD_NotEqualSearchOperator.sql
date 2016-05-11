IF (NOT EXISTS (SELECT 1 FROM [SearchOperators] WHERE [ServiceMethod] = N'NotEqualOperator'))
	INSERT [SearchOperators] (Operator, ServiceMethod, Description)
	VALUES (N'<>', N'NotEqualOperator', N'Not equal')