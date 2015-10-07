USE [assetmanager]
GO

/****** Object:  Trigger [dbo].[UpdateTaxonomyItemId]    Script Date: 06/24/2011 17:16:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Ilya Bolkhovsky
-- Create date: 24.06.2011
-- Description:	
-- =============================================
CREATE TRIGGER [dbo].[UpdateDynListItemId]
   ON  [dbo].[DynListItem] 
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here    
    update [DynListItem] set [DynListItemId] = [DynListItemUid] where [DynListItemId] = 0 and [DynListItemUid] in (select [DynListItemUid] from inserted);

END

GO


