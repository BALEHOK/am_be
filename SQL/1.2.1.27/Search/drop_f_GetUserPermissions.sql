/****** Object:  UserDefinedFunction [dbo].[f_GetUserPermissions]    Script Date: 12/12/2012 10:59:19 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[f_GetUserPermissions]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[f_GetUserPermissions]
GO


