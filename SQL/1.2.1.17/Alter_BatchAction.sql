ALTER TABLE BatchAction DROP CONSTRAINT [DF_BatchAction_ErrorMessage];
ALTER TABLE BatchAction ALTER COLUMN ErrorMessage nvarchar(MAX) NULL;