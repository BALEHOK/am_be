/****** Object:  StoredProcedure [dbo].[_cust_GetMaxSearchId]    Script Date: 12/16/2012 00:33:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Ilya Bolkhovsky
-- Create date: 15.12.2012
-- Description: Returns Max SearchId retrieved from temp table or 1 if none.
-- =============================================
CREATE PROCEDURE [dbo].[_cust_GetMaxSearchId]    
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here

    DECLARE @NextSearchID int = NULL;

    BEGIN TRY
        SELECT @NextSearchID = ISNULL(MAX(SearchId), 0) 
          FROM _search_srchcount
        SET @NextSearchID = @NextSearchID + 1;    
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() = 8115
        BEGIN
            SET @NextSearchID = 1;
        END
    END CATCH;    
    SELECT @NextSearchID;
END