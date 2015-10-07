IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Report_Report]') AND parent_object_id = OBJECT_ID(N'[dbo].[Report]'))
ALTER TABLE [dbo].[Report] DROP CONSTRAINT [FK_Report_Report]
GO


