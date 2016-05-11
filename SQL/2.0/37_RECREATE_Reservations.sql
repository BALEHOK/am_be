/****** Object:  Table [dbo].[Reservation]    Script Date: 10.02.2016 16:08:04 ******/
IF OBJECT_ID('dbo.ReservedAssets', 'U') IS NOT NULL 
  DROP TABLE dbo.ReservedAssets; 

IF OBJECT_ID('dbo.Reservations', 'U') IS NOT NULL 
  DROP TABLE dbo.Reservations; 

IF OBJECT_ID('dbo.Reservation', 'U') IS NOT NULL 
  DROP TABLE dbo.Reservation; 

/****** Object:  Table [dbo].[Reservation]    Script Date: 10.02.2016 16:08:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Reservations](
	[ReservationId] [bigint] IDENTITY(1,1) NOT NULL,
	[UpdateUserId] [bigint] NOT NULL,
	[UpdateDate] [datetime2] NOT NULL CONSTRAINT [DF_Reservation_UpdateDate]  DEFAULT (getdate()),
	[BorrowerId] [bigint] NOT NULL,
	[StartDate] [datetime2] NOT NULL,
	[EndDate] [datetime2] NOT NULL,
	[Comment] [nvarchar](MAX) NULL,
	[State] [smallint] NOT NULL CONSTRAINT [DF_Reservation_State]  DEFAULT ((0)),
 CONSTRAINT [PK_Reservation] PRIMARY KEY CLUSTERED 
(
	[ReservationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[ReservedAssets]
(
	[ReservedAssetId] [bigint] IDENTITY(1,1) NOT NULL,
	[ReservationId] [bigint] NOT NULL,
	[DynEntityConfigId] [bigint] NOT NULL,
	[DynEntityId] [bigint] NOT NULL,
	[UpdateUserId] [bigint] NOT NULL,
	[UpdateDate] [datetime2] NOT NULL CONSTRAINT [DF_ReservedAsset_UpdateDate]  DEFAULT (getdate())
 CONSTRAINT [PK_ReservedAsset] PRIMARY KEY CLUSTERED 
(
	[ReservedAssetId] ASC
)
CONSTRAINT FK_ReservedAsset_Reservation FOREIGN KEY (ReservationId) 
    REFERENCES Reservations (ReservationId) 
    ON DELETE CASCADE
)

CREATE UNIQUE NONCLUSTERED INDEX [IDX_ReservedAssets] ON [dbo].[ReservedAssets]
(
	[ReservationId] ASC,
	[DynEntityConfigId] ASC,
	[DynEntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO




