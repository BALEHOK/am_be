BEGIN TRANSACTION

ALTER TABLE Report ADD SyncWithOpenItem BIT NOT NULL DEFAULT 0
GO

select * from Report
GO

COMMIT TRANSACTION