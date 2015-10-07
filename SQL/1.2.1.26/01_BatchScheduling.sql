ALTER TABLE [dbo].[BatchJob] DROP CONSTRAINT [DF_BatchJob_ScheduleId];
ALTER TABLE [dbo].[BatchJob] DROP CONSTRAINT [FK_BatchJob_BatchSchedule];
ALTER TABLE BatchJob DROP COLUMN ScheduleId;
ALTER TABLE BatchJob DROP COLUMN ScheduleDate;

TRUNCATE TABLE BatchSchedule;
ALTER TABLE BatchSchedule DROP COLUMN ScheduleType;
ALTER TABLE BatchSchedule DROP COLUMN ExecutionInterval;
ALTER TABLE BatchSchedule ADD IsEnabled bit NOT NULL DEFAULT 1;
ALTER TABLE BatchSchedule ADD ExecuteAt datetime NOT NULL DEFAULT getdate();
ALTER TABLE BatchSchedule ADD RepeatInHours int NULL;
ALTER TABLE BatchSchedule ADD Notes ntext NULL;

ALTER TABLE BatchJob  ADD BatchScheduleId bigint NULL;
ALTER TABLE BatchJob  WITH CHECK ADD CONSTRAINT FK_BatchJob_BatchSchedule FOREIGN KEY(BatchScheduleId)
REFERENCES BatchSchedule (ScheduleId);