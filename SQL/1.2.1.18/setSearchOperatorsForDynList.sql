	delete from DataTypeSearchOperators where DataTypeUid in (select DataTypeUid from [DataType]
	where Name='DynList')
	
	GO
	
	INSERT INTO [DataTypeSearchOperators]
           ([DataTypeUid]
           ,[SearchOperatorUid])
     VALUES
           ((select DataTypeUid from [DataType]
			where Name='DynList')
           ,(SELECT Top(1) [SearchOperatorUid]     
			FROM [SearchOperators] where [Operator]='=='))


