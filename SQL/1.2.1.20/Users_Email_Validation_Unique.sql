DECLARE @validationOperatorUid bigint;
SELECT @validationOperatorUid = [ValidationOperatorUid] FROM [ValidationOperator] WHERE Name = 'Unique';
IF @validationOperatorUid IS NULL
BEGIN
	INSERT INTO [ValidationOperator] (Name, ClassName, Unary)
	VALUES ('Unique', 'Operators.ValidationOperatorUnique', 1);
	SET @validationOperatorUid = @@IDENTITY;
END;
IF NOT EXISTS(SELECT * FROM ValidationList WHERE Name='Unique' AND ValidationOperatorUid=@validationOperatorUid)
BEGIN
	INSERT INTO ValidationList (Name, ValidationOperatorUid)
	VALUES ('Unique', @validationOperatorUid);
END;
UPDATE [DynEntityAttribConfig] SET ValidationExpr='@Unique'
  WHERE [Name]='Email' AND
  [DynEntityConfigUid] IN (
  SELECT [DynEntityConfigUid]      
  FROM [DynEntityConfig]
  WHERE Name='User'
);