-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ilya Bolkhovsky
-- Create date: 12.03.2012
-- Description:	Performs the fulltext search and ranks a result
-- =============================================
CREATE PROCEDURE _cust_FullTextSearch_IndexHistoryDynEntities
	-- Add the parameters for the stored procedure here
	@keywords nvarchar(1000)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT KEY_TBL1.RANK AS AllAttribValuesRank, ISNULL(KEY_TBL2.RANK, 0) AS AllContextAttribValuesRank, ISNULL(KEY_TBL3.RANK, 0) AS AllAttrib2IndexValuesRank, FT_TBL.*
	FROM IndexHistoryDynEntities AS FT_TBL 
	INNER JOIN FREETEXTTABLE(IndexHistoryDynEntities, (AllAttribValues), @keywords) AS KEY_TBL1 ON FT_TBL.IndexUid = KEY_TBL1.[KEY]
	LEFT JOIN FREETEXTTABLE(IndexHistoryDynEntities, (AllContextAttribValues), @keywords) AS KEY_TBL2 ON FT_TBL.IndexUid = KEY_TBL2.[KEY]
	LEFT JOIN FREETEXTTABLE(IndexHistoryDynEntities, (AllAttrib2IndexValues), @keywords) AS KEY_TBL3 ON FT_TBL.IndexUid = KEY_TBL3.[KEY]
END
GO
