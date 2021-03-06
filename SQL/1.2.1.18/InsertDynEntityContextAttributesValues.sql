CREATE TABLE [dbo].[DynEntityContextAttributesValues](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DynEntityContextAttributesId] [bigint] NOT NULL,
	[StringValue] [nvarchar](max) NULL,
	[DateTimeValue] [datetime] NULL,
	[NumericValue] [float] NULL,
	[DynamicListItemUid] [bigint] NULL,
 CONSTRAINT [PK_DynEntityContextAttributesValues] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DynEntityContextAttributesValues]  WITH CHECK ADD  CONSTRAINT [FK_DynEntityContextAttributesValues_DynEntityContextAttributes] FOREIGN KEY([DynEntityContextAttributesId])
REFERENCES [dbo].[DynEntityContextAttributes] ([DynEntityContextAttributesId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[DynEntityContextAttributesValues] CHECK CONSTRAINT [FK_DynEntityContextAttributesValues_DynEntityContextAttributes]
GO

INSERT INTO 
	[dbo].[DynEntityContextAttributesValues]
           ([DynEntityContextAttributesId]           
           ,[DateTimeValue])  
SELECT attr.DynEntityContextAttributesId,attr.[Value]     
  FROM DynEntityContextAttributes attr
  left join DynEntityContextAttributesValues attrv on 
  attrv.DynEntityContextAttributesId=attr.DynEntityContextAttributesId
  where attrv.Id is null and ISDATE(value)=1
GO

INSERT INTO 
	[dbo].[DynEntityContextAttributesValues]
           ([DynEntityContextAttributesId]           
           ,[NumericValue])  
SELECT attr.DynEntityContextAttributesId,attr.[Value]     
  FROM DynEntityContextAttributes attr
  left join DynEntityContextAttributesValues attrv on 
  attrv.DynEntityContextAttributesId=attr.DynEntityContextAttributesId
  where attrv.Id is null and ISNUMERIC(value)=1 and LEN(attr.[Value])<>7

GO


INSERT INTO 
			[dbo].[DynEntityContextAttributesValues]
           ([DynEntityContextAttributesId]           
           ,[DynamicListItemUid])
SELECT attr.DynEntityContextAttributesId,
  (SELECT TOP(1)dynlist.DynListItemId FROM DynListItem dynlist 
  where dynlist.[Value]=attr.[Value]) as DynamicListItemUid     
  FROM DynEntityContextAttributes attr
  left join DynEntityContextAttributesValues attrv on 
  attrv.DynEntityContextAttributesId=attr.DynEntityContextAttributesId
  where attrv.Id is null AND ((SELECT Count(*) FROM DynListItem dynlist 
  where dynlist.[Value]=attr.[Value])<>0 )


GO


INSERT INTO 
			[dbo].[DynEntityContextAttributesValues]
           ([DynEntityContextAttributesId]           
           ,[DynamicListItemUid])
SELECT attr.DynEntityContextAttributesId,
  (SELECT TOP(1)dynlist.DynListItemId FROM DynListItem dynlist 
  where dynlist.[Value]=(select Top(1) ResourceKey from StringResources 
  where ResourceValue=attr.[Value])) as DynamicListItemUid     
  FROM DynEntityContextAttributes attr
  left join DynEntityContextAttributesValues attrv on 
  attrv.DynEntityContextAttributesId=attr.DynEntityContextAttributesId
  where attrv.Id is null AND ((SELECT Count(*) FROM DynListItem dynlist 
  where dynlist.[Value]=(select Top(1) ResourceKey from StringResources 
  where ResourceValue=attr.[Value]))<>0 )

GO

SET LANGUAGE Norwegian

INSERT INTO 
	[dbo].[DynEntityContextAttributesValues]
           ([DynEntityContextAttributesId]           
           ,[DateTimeValue])  
SELECT attr.DynEntityContextAttributesId,attr.[Value]     
  FROM DynEntityContextAttributes attr
  left join DynEntityContextAttributesValues attrv on 
  attrv.DynEntityContextAttributesId=attr.DynEntityContextAttributesId
  where attrv.Id is null and ISDATE(value)=1
  
GO

SET LANGUAGE Norwegian


INSERT INTO 
	[dbo].[DynEntityContextAttributesValues]
           ([DynEntityContextAttributesId]           
           ,[NumericValue])  
SELECT attr.DynEntityContextAttributesId,attr.[Value]     
  FROM DynEntityContextAttributes attr
  left join DynEntityContextAttributesValues attrv on 
  attrv.DynEntityContextAttributesId=attr.DynEntityContextAttributesId
  where attrv.Id is null and ISNUMERIC(value)=1 and LEN(attr.[Value])<>7

GO

SET LANGUAGE French

INSERT INTO 
	[dbo].[DynEntityContextAttributesValues]
           ([DynEntityContextAttributesId]           
           ,[DateTimeValue])  
SELECT attr.DynEntityContextAttributesId,attr.[Value]     
  FROM DynEntityContextAttributes attr
  left join DynEntityContextAttributesValues attrv on 
  attrv.DynEntityContextAttributesId=attr.DynEntityContextAttributesId
  where attrv.Id is null and ISDATE(value)=1
  
GO

SET LANGUAGE French


INSERT INTO 
	[dbo].[DynEntityContextAttributesValues]
           ([DynEntityContextAttributesId]           
           ,[NumericValue])  
SELECT attr.DynEntityContextAttributesId,attr.[Value]     
  FROM DynEntityContextAttributes attr
  left join DynEntityContextAttributesValues attrv on 
  attrv.DynEntityContextAttributesId=attr.DynEntityContextAttributesId
  where attrv.Id is null and ISNUMERIC(value)=1 and LEN(attr.[Value])<>7

GO

INSERT INTO 
	[dbo].[DynEntityContextAttributesValues]
           ([DynEntityContextAttributesId]           
           ,[StringValue])  
SELECT attr.DynEntityContextAttributesId,attr.[Value]     
  FROM DynEntityContextAttributes attr
  left join DynEntityContextAttributesValues attrv on 
  attrv.DynEntityContextAttributesId=attr.DynEntityContextAttributesId
  where attrv.Id is null 


