  UPDATE [dbo].[DataType]
  SET ValidationExpr = 'ISEMAIL([@value])'
  WHERE Name = 'email'