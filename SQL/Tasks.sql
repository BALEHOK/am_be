USE [assetmanager_1.2.0]
GO

/****** Object:  Table [dbo].[Task]    Script Date: 07/05/2011 11:04:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Task](
	[TaskId] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](350) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[FunctionData] [nvarchar](max) NOT NULL,
	[UpdateUserId] [bigint] NOT NULL,
	[UpdateDate] [date] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[FunctionType] [int] NOT NULL,
	[ExecutableType] [int] NOT NULL,
	[ExecutablePath] [nvarchar](max) NULL,
	[DynEntityConfigId] [bigint] NOT NULL,
 CONSTRAINT [PK_Task] PRIMARY KEY CLUSTERED 
(
	[TaskId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Task] ADD  CONSTRAINT [DF_Task_UpdateDate]  DEFAULT (getdate()) FOR [UpdateDate]
GO

ALTER TABLE [dbo].[Task] ADD  CONSTRAINT [DF_Task_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO


