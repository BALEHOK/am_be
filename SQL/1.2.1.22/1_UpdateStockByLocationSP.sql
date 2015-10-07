SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Alexey Nesterov
-- Create date: 15 Aug 2013
-- Update date: 29 Aug 2013
-- Description:	Returns list of available stock items grouped by location
-- =============================================
ALTER PROCEDURE [dbo].[_cust_GetStocksByLocation]
	-- Add the parameters for the stored procedure here
	@assetid bigint, 
	@configid bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET FMTONLY OFF;

        -- Insert statements for procedure here
	with Transactions (LocationId, FromLocationId, TransactionTypeUid, StockCount) as
	(select LocationId, FromLocationId, TransactionTypeUid, StockCount from DynEntityTransaction where DynEntityId = @assetid and DynEntityConfigId = @configid)
	select * into #Trans from Transactions;
	
	with AllTransactions (GivenLocationId, StockIn) as
	(select LocationId as GivenLocationId, SUM(StockCount) as StockIn from #Trans group by LocationId having LocationId is not null)
	select	GivenLocationId as Location,
			ISNULL(ISNULL((select SUM(StockCount) from #Trans where LocationId = GivenLocationId and TransactionTypeUid = 1), 0) + 
			ISNULL((select SUM(StockCount) from #Trans where LocationId = GivenLocationId and TransactionTypeUid = 3), 0) -
			ISNULL((select SUM(StockCount) from #Trans where FromLocationId = GivenLocationId and TransactionTypeUid = 3), 0) -
			ISNULL((select SUM(StockCount) from #Trans where FromLocationId = GivenLocationId and TransactionTypeUid = 2), 0), 0) as RestCount
	from AllTransactions
	drop table #Trans	
END
