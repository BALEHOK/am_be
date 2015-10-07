CREATE TABLE [dbo].[DeletedEntities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DynEntityConfigId] [bigint] NOT NULL,
	[DynEntityId] [bigint] NOT NULL,
	[DynEntityUid] [bigint] NOT NULL,
 CONSTRAINT [PK_DeletedEntities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


