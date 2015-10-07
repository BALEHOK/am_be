UPDATE [dbo].[DataType]
   SET [ValidationMessage]  = 'Barcode must be 7-digit number'
 WHERE ValidationExpr = '@CheckBarcode ' AND Name = 'barcode'
GO


