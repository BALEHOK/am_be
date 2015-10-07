TRUNCATE TABLE SearchTracking;
ALTER TABLE SearchTracking ADD VerboseString nvarchar(MAX) NOT NULL;
ALTER TABLE SearchTracking ADD SearchId bigint NOT NULL;
ALTER TABLE SearchTracking DROP CONSTRAINT [DF_SearchTracking_ResultCount];
ALTER TABLE SearchTracking DROP COLUMN ResultCount;