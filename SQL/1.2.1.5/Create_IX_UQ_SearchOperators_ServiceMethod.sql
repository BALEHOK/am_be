
/****** Object:  Index [IX_UQ_SearchOperators_ServiceMethod]    Script Date: 03/10/2012 15:17:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_SearchOperators_ServiceMethod] ON [dbo].[SearchOperators] 
(
	[ServiceMethod] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


