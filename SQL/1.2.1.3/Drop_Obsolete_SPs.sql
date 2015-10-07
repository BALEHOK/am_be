/****** Object:  StoredProcedure [dbo].[GetPermittedAssetsCount]    Script Date: 02/24/2012 15:53:46 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPermittedAssetsCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPermittedAssetsCount]
GO

/****** Object:  StoredProcedure [dbo].[GetPermittedAssetsCount]    Script Date: 02/24/2012 15:53:46 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPermittedAssetsCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[_DynEntityIndex_GetPermitted]
GO



