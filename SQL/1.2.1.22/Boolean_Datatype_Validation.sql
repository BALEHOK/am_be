UPDATE DataType SET ValidationExpr = '@IsBoolean', ValidationMessage = '{0} is not valid boolean value.' WHERE Name = 'bool';
INSERT INTO ValidationList (Name, ValidationOperatorUid) VALUES ('IsBoolean', 1);
DECLARE @ValidationListUid bigint;
SELECT @ValidationListUid = @@IDENTITY;
INSERT INTO ValidationOperandValue ([ValidationListUid], [ValidationOperandUid], [Value]) VALUES (@ValidationListUid, 1, '[01]|true|false|True|False');