CREATE TABLE [dbo].[TaskRights](
	[TaskRightsId] [bigint] IDENTITY(1,1) NOT NULL,
	[ViewId] [bigint] NOT NULL,
	[TaxonomyItemId] [bigint] NULL,
	[DynEntityConfigId] [bigint] NULL,
	[UserId] [bigint] NOT NULL,
	[IsDeny] [bit] NOT NULL,
 CONSTRAINT [PK_TaskRights] PRIMARY KEY CLUSTERED 
(
	[TaskRightsId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE PROCEDURE [dbo].[_cust_GetPermittedTask]
	@DynEntityConfigId bigint,
	@userId bigint,
	@TaxonomyItemId bigint
AS
BEGIN

DECLARE
@IsAllow bit
set @IsAllow=0

IF (SELECT COUNT(*) FROM [dbo].[TaskRights] WHERE IsDeny=1 AND UserId=@userId AND
(DynEntityConfigId=@DynEntityConfigId OR TaxonomyItemId=@TaxonomyItemId OR 
(DynEntityConfigId=0 AND TaxonomyItemId=0)))<>0
BEGIN
	set @IsAllow=0
END
ELSE IF (SELECT COUNT(*) FROM [dbo].[TaskRights] WHERE IsDeny=0 AND UserId=@userId AND  
(DynEntityConfigId=@DynEntityConfigId OR TaxonomyItemId=@TaxonomyItemId OR 
(DynEntityConfigId=0 AND TaxonomyItemId=0)))<>0
BEGIN
	set @IsAllow=1
END

SELECT @IsAllow

END
