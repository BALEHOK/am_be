  UPDATE [dbo].[DataType]
  SET ValidationExpr = 'IsUrl([@value])'
  WHERE ValidationExpr = '@IsUrl'
  UPDATE [dbo].[DataType]
  SET ValidationExpr = 'IsBarcode([@value])'
  WHERE ValidationExpr = '@CheckBarcode'
  UPDATE [dbo].[DataType]
  SET ValidationExpr = NULL
  WHERE ValidationExpr like '@%'