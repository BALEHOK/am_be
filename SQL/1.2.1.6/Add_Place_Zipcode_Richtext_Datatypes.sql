INSERT INTO DataType ([Name] ,[NameTranslationId] ,[DBDataType] ,[FrameworkDataType] ,[Comment] ,[UpdateUserId] ,[UpdateDate] ,[StringSize] ,[DefaultValueID] ,[ValidationExpr] ,[IsInternal] ,[IsEditable] ,[ValidationMessage])
VALUES 
('richtext', 'richtext', 'text', 'string', NULL , 1, GETDATE(), NULL, NULL, NULL, 0, 1, NULL),
('place', 'place', 'nvarchar', 'string', NULL , 1, GETDATE(), 255, NULL, NULL, 0, 1, NULL),
('zipcode', 'pipcode', 'bigint', 'Int64', NULL , 1, GETDATE(), NULL, NULL, NULL, 0, 1, NULL);
