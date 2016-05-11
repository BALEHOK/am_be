-- create activity status dynlist
declare @updateDate datetime2(7) = GETDATE();
declare @statusListName nvarchar(30) = N'Activity status';
declare @statusListUid bigint;

declare @longTypeId bigint;
declare @boolTypeId bigint;
declare @stringTypeId bigint;
declare @datetimeTypeId bigint;
declare @currentdateTypeId bigint;
declare @revisionTypeId bigint;
declare @assetTypeId bigint;
declare @dynlistTypeId bigint;

select @longTypeId = [DataTypeUid]
from [DataType]
where [Name] = 'long'
select @boolTypeId = [DataTypeUid]
from [DataType]
where [Name] = 'bool'
select @stringTypeId = [DataTypeUid]
from [DataType]
where [Name] = 'string'
select @datetimeTypeId = [DataTypeUid]
from [DataType]
where [Name] = 'datetime'
select @currentdateTypeId = [DataTypeUid]
from [DataType]
where [Name] = 'currentdate'
select @revisionTypeId = [DataTypeUid]
from [DataType]
where [Name] = 'revision'
select @assetTypeId = [DataTypeUid]
from [DataType]
where [Name] = 'asset'
select @dynlistTypeId = [DataTypeUid]
from [DataType]
where [Name] = 'dynlist'

insert [dbo].[DynList]([Name], [NameTranslationId], [DataTypeId], [Label], [LabelTranslationId], [Comment], [UpdateUserId], [UpdateDate], [Active])
values (@statusListName, null, @stringTypeId, @statusListName, null, null, 1, @updateDate, 1)

select @statusListUid = [DynListUid]
from [dbo].[DynList]
where @@ROWCOUNT > 0 and [DynListUid] = scope_identity()

insert [dbo].[DynListItem]([DynListItemId], [Revision], [DynListUid], [ActiveVersion], [DisplayOrder], [Value], [ValueTranslationId], [AssociatedDynListUid])
values (0, 1, @statusListUid, 1, 1, N'Not started', null, null)
insert [dbo].[DynListItem]([DynListItemId], [Revision], [DynListUid], [ActiveVersion], [DisplayOrder], [Value], [ValueTranslationId], [AssociatedDynListUid])
values (0, 1, @statusListUid, 1, 1, N'In Progress', null, null)
insert [dbo].[DynListItem]([DynListItemId], [Revision], [DynListUid], [ActiveVersion], [DisplayOrder], [Value], [ValueTranslationId], [AssociatedDynListUid])
values (0, 1, @statusListUid, 1, 1, N'Completed', null, null)
insert [dbo].[DynListItem]([DynListItemId], [Revision], [DynListUid], [ActiveVersion], [DisplayOrder], [Value], [ValueTranslationId], [AssociatedDynListUid])
values (0, 1, @statusListUid, 1, 1, N'Waiting on someone else', null, null)
insert [dbo].[DynListItem]([DynListItemId], [Revision], [DynListUid], [ActiveVersion], [DisplayOrder], [Value], [ValueTranslationId], [AssociatedDynListUid])
values (0, 1, @statusListUid, 1, 1, N'Deferred', null, null)

--create Activity Asset type
declare @typeName nvarchar(50) = N'Activity';
declare @tableName nvarchar(50) = N'ADynEntityActivity';
declare @typeUid bigint;
declare @typeId bigint;

insert [dbo].[DynEntityConfig]([DynEntityConfigId], [Revision], [ActiveVersion], [Name], [NameTranslationId], [DBTableName], [TypeId], [ContextId], [BaseDynEntityConfigId], [LinkedDynEntityId],
	[Active], [IsSearchable], [IsIndexed], [IsContextIndexed], [Comment], [UpdateUserId], [UpdateDate], [MeasureUnitId], [IsInStock], [IsUnpublished], [AllowBorrow], [LayoutId], [AutoGenerateName], [ScreenLayoutId], [ParentChildRelations])
values (0, 1, 1, @typeName, null, @tableName, 1, null, null, null,
	1, 1, 1, 0, N'', 1, @updateDate, 0, 0, 0, 0, 0, 0, 1, 1)
select @typeUid = [DynEntityConfigUid]
from [dbo].[DynEntityConfig]
where @@ROWCOUNT > 0 and [DynEntityConfigUid] = scope_identity()

set @typeId = @typeUid;

update [dbo].[DynEntityConfig]
set [DynEntityConfigId] = @typeId
where ([DynEntityConfigUid] = @typeUid)


IF NOT EXISTS  ( SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = @tableName ) CREATE TABLE ADynEntityActivity (
	[DynEntityUid] BigInt NOT NULL  PRIMARY KEY IDENTITY(1,1),
	[DynEntityId] BigInt NOT NULL  DEFAULT 0,
	[ActiveVersion] bit NOT NULL  DEFAULT 0,
	[IsDeleted] bit NOT NULL  DEFAULT 0,
	[DynEntityConfigUid] BigInt NOT NULL  DEFAULT 0,
	[Name] NVarChar (max)  NOT NULL  DEFAULT '',
	[Revision] Int NULL ,
	[UpdateUserId] BigInt NOT NULL  DEFAULT 0,
	[UpdateDate] DateTime NOT NULL  DEFAULT '',
	[Assignee] BigInt NOT NULL  DEFAULT 0,
	[Due_date] DateTime NOT NULL  DEFAULT '',
	[Status] BigInt NOT NULL  DEFAULT 0 );

exec [dbo].[_cust_RebuildTriggers] @dynEntityConfigUid=@typeUid

-- screen
declare @screenId bigint;
insert [dbo].[AssetTypeScreen]([DynEntityConfigUid], [Name], [Status], [Title], [Subtitle], [PageText], [Comment], [UpdateUserId], [UpdateDate], [LayoutId], [IsDefault], [ScreenUid], [IsMobile])
values (@typeUid, N'Default', 0, N'Default', N'', N'', N'Default screen', 1, @updateDate, 2, 1, '00000000-0000-0000-0000-000000000000',0)
select @screenId = [ScreenId]
from [dbo].[AssetTypeScreen]
where @@ROWCOUNT > 0 and [ScreenId] = scope_identity()

declare @panelUid bigint;
insert [dbo].[AttributePanel]([AttributePanelId], [DynEntityConfigUId], [Name], [NameTranslationId], [DisplayOrder], [Description], [HeaderLabel], [HeaderLabelTranslationId], [UpdateUserId], [UpdateDate], [ScreenId], [IsChildAssets], [ChildAssetAttrId])
values (0, @typeUid, N'General', null, 0, N'', null, null, 0, @updateDate, @screenId, 0, null)
select @panelUid = [AttributePanelUid]
from [dbo].[AttributePanel]
where @@ROWCOUNT > 0 and [AttributePanelUid] = scope_identity()

-- Attributes
declare @userConfigId bigint;
declare @userAttribConfigId bigint;

select @userConfigId = [DynEntityConfigID], @userAttribConfigId = [DynEntityAttribConfigID]
from [PredefinedAttributes]
where [Name] = 'User'

declare @attrUid bigint;
declare @attrNameId bigint;
-- DynEntityUid attribute
insert [dbo].[DynEntityAttribConfig](
	[DynEntityConfigUid], [DynEntityAttribConfigId], [Name], [NameTranslationId], [DataTypeUid], [DynListUid],
	[DBTableFieldname], [ContextId], [IsDynListValue], [IsFinancialInfo], [IsRequired], [IsKeyword], [Format], [IsFullTextInidex], [DisplayOnResultList],
	[DisplayOrderResultList], [DisplayOnExtResultList], [UpdateUserId], [UpdateDate], [Label], [LabelTranslationId], [Revision], [ActiveVersion], [DisplayOrderExtResultList],
	[Comment], [Active], [RelatedAssetTypeID], [RelatedAssetTypeAttributeID], [ValidationExpr], [IsDescription], [IsShownInGrid], [IsShownOnPanel],
	[AllowEditConfig], [AllowEditValue], [ValidationMessage], [IsUsedForNames], [NameGenOrder], [DisplayOrder], [CalculationFormula])
values (@typeUid, 0, N'DynEntityUid', null, @longTypeId, null,
	N'DynEntityUid', null, 0, 0, 1, 0, null, 0, 0,
	0, 0, 1, @updateDate, N'', null, 1, 1, 0,
	null, 1, null, null, null, 0, 0, 0,
	0, 0, null, 0, null, 0, null)

-- DynEntityId attribute
insert [dbo].[DynEntityAttribConfig](
	[DynEntityConfigUid], [DynEntityAttribConfigId], [Name], [NameTranslationId], [DataTypeUid], [DynListUid],
	[DBTableFieldname], [ContextId], [IsDynListValue], [IsFinancialInfo], [IsRequired], [IsKeyword], [Format], [IsFullTextInidex], [DisplayOnResultList],
	[DisplayOrderResultList], [DisplayOnExtResultList], [UpdateUserId], [UpdateDate], [Label], [LabelTranslationId], [Revision], [ActiveVersion], [DisplayOrderExtResultList],
	[Comment], [Active], [RelatedAssetTypeID], [RelatedAssetTypeAttributeID], [ValidationExpr], [IsDescription], [IsShownInGrid], [IsShownOnPanel],
	[AllowEditConfig], [AllowEditValue], [ValidationMessage], [IsUsedForNames], [NameGenOrder], [DisplayOrder], [CalculationFormula])
values (@typeUid, 0, N'DynEntityId', null, @longTypeId, null,
	N'DynEntityId', null, 0, 0, 1, 0, null, 0, 0,
	0, 0, 1, @updateDate, N'', null, 1, 1, 0,
	null, 1, null, null, null, 0, 0, 0,
	0, 0, null, 0, null, 0, null)

-- ActiveVersion attribute
insert [dbo].[DynEntityAttribConfig](
	[DynEntityConfigUid], [DynEntityAttribConfigId], [Name], [NameTranslationId], [DataTypeUid], [DynListUid],
	[DBTableFieldname], [ContextId], [IsDynListValue], [IsFinancialInfo], [IsRequired], [IsKeyword], [Format], [IsFullTextInidex], [DisplayOnResultList],
	[DisplayOrderResultList], [DisplayOnExtResultList], [UpdateUserId], [UpdateDate], [Label], [LabelTranslationId], [Revision], [ActiveVersion], [DisplayOrderExtResultList],
	[Comment], [Active], [RelatedAssetTypeID], [RelatedAssetTypeAttributeID], [ValidationExpr], [IsDescription], [IsShownInGrid], [IsShownOnPanel],
	[AllowEditConfig], [AllowEditValue], [ValidationMessage], [IsUsedForNames], [NameGenOrder], [DisplayOrder], [CalculationFormula])
values (@typeUid, 0, N'ActiveVersion', null, @boolTypeId, null,
	N'ActiveVersion', null, 0, 0, 1, 0, null, 0, 0,
	0, 0, 1, @updateDate, N'', null, 1, 1, 0,
	null, 1, null, null, null, 0, 0, 0,
	0, 0, null, 0, null, 0, null)

-- IsDeleted attribute
insert [dbo].[DynEntityAttribConfig](
	[DynEntityConfigUid], [DynEntityAttribConfigId], [Name], [NameTranslationId], [DataTypeUid], [DynListUid],
	[DBTableFieldname], [ContextId], [IsDynListValue], [IsFinancialInfo], [IsRequired], [IsKeyword], [Format], [IsFullTextInidex], [DisplayOnResultList],
	[DisplayOrderResultList], [DisplayOnExtResultList], [UpdateUserId], [UpdateDate], [Label], [LabelTranslationId], [Revision], [ActiveVersion], [DisplayOrderExtResultList],
	[Comment], [Active], [RelatedAssetTypeID], [RelatedAssetTypeAttributeID], [ValidationExpr], [IsDescription], [IsShownInGrid], [IsShownOnPanel],
	[AllowEditConfig], [AllowEditValue], [ValidationMessage], [IsUsedForNames], [NameGenOrder], [DisplayOrder], [CalculationFormula])
values (@typeUid, 0, N'IsDeleted', null, @boolTypeId, null,
	N'IsDeleted', null, 0, 0, 1, 0, null, 0, 0,
	0, 0, 1, @updateDate, N'', null, 1, 1, 0,
	null, 1, null, null, null, 0, 0, 0,
	0, 0, null, 0, null, 0, null)

-- DynEntityConfigUid attribute
insert [dbo].[DynEntityAttribConfig](
	[DynEntityConfigUid], [DynEntityAttribConfigId], [Name], [NameTranslationId], [DataTypeUid], [DynListUid],
	[DBTableFieldname], [ContextId], [IsDynListValue], [IsFinancialInfo], [IsRequired], [IsKeyword], [Format], [IsFullTextInidex], [DisplayOnResultList],
	[DisplayOrderResultList], [DisplayOnExtResultList], [UpdateUserId], [UpdateDate], [Label], [LabelTranslationId], [Revision], [ActiveVersion], [DisplayOrderExtResultList],
	[Comment], [Active], [RelatedAssetTypeID], [RelatedAssetTypeAttributeID], [ValidationExpr], [IsDescription], [IsShownInGrid], [IsShownOnPanel],
	[AllowEditConfig], [AllowEditValue], [ValidationMessage], [IsUsedForNames], [NameGenOrder], [DisplayOrder], [CalculationFormula])
values (@typeUid, 0, N'DynEntityConfigUid', null, @longTypeId, null,
	N'DynEntityConfigUid', null, 0, 0, 1, 0, null, 0, 0,
	0, 0, 1, @updateDate, N'', null, 1, 1, 0,
	null, 1, null, null, null, 0, 0, 0,
	0, 0, null, 0, null, 0, null)

-- Name attribute
insert [dbo].[DynEntityAttribConfig](
	[DynEntityConfigUid], [DynEntityAttribConfigId], [Name], [NameTranslationId], [DataTypeUid], [DynListUid],
	[DBTableFieldname], [ContextId], [IsDynListValue], [IsFinancialInfo], [IsRequired], [IsKeyword], [Format], [IsFullTextInidex], [DisplayOnResultList],
	[DisplayOrderResultList], [DisplayOnExtResultList], [UpdateUserId], [UpdateDate], [Label], [LabelTranslationId], [Revision], [ActiveVersion], [DisplayOrderExtResultList],
	[Comment], [Active], [RelatedAssetTypeID], [RelatedAssetTypeAttributeID], [ValidationExpr], [IsDescription], [IsShownInGrid], [IsShownOnPanel],
	[AllowEditConfig], [AllowEditValue], [ValidationMessage], [IsUsedForNames], [NameGenOrder], [DisplayOrder], [CalculationFormula])
values (@typeUid, 0, N'Name', null, @stringTypeId, null,
	N'Name', null, 0, 0, 1, 1, null, 1, 0,
	0, 0, 1, @updateDate, N'Name', null, 1, 1, 0,
	null, 1, null, null, null, 0, 1, 1,
	0, 1, null, 0, null, 10, null)
select @attrUid = [DynEntityAttribConfigUid], @attrNameId = [DynEntityAttribConfigId]
from [dbo].[DynEntityAttribConfig]
where @@ROWCOUNT > 0 and [DynEntityAttribConfigUid] = scope_identity()

insert [dbo].[AttributePanelAttribute]([AttributePanelUid], [DynEntityAttribConfigUId], [DisplayOrder], [UpdateUserId], [UpdateDate], [ReferencingDynEntityAttribConfigId], [ScreenFormula])
values (@panelUid, @attrUid, 0, 1, @updateDate, 0, null)


-- Revision attribute
insert [dbo].[DynEntityAttribConfig](
	[DynEntityConfigUid], [DynEntityAttribConfigId], [Name], [NameTranslationId], [DataTypeUid], [DynListUid],
	[DBTableFieldname], [ContextId], [IsDynListValue], [IsFinancialInfo], [IsRequired], [IsKeyword], [Format], [IsFullTextInidex], [DisplayOnResultList],
	[DisplayOrderResultList], [DisplayOnExtResultList], [UpdateUserId], [UpdateDate], [Label], [LabelTranslationId], [Revision], [ActiveVersion], [DisplayOrderExtResultList],
	[Comment], [Active], [RelatedAssetTypeID], [RelatedAssetTypeAttributeID], [ValidationExpr], [IsDescription], [IsShownInGrid], [IsShownOnPanel],
	[AllowEditConfig], [AllowEditValue], [ValidationMessage], [IsUsedForNames], [NameGenOrder], [DisplayOrder], [CalculationFormula])
values (@typeUid, 0, N'Revision', null, @revisionTypeId, null,
	N'Revision', null, 0, 0, 0, 0, null, 0, 0,
	0, 0, 1, @updateDate, N'Revision', null, 1, 1, 0,
	null, 1, null, null, null, 0, 0, 1,
	0, 0, null, 0, null, 20, null)

-- Update User attribute
insert [dbo].[DynEntityAttribConfig](
	[DynEntityConfigUid], [DynEntityAttribConfigId], [Name], [NameTranslationId], [DataTypeUid], [DynListUid],
	[DBTableFieldname], [ContextId], [IsDynListValue], [IsFinancialInfo], [IsRequired], [IsKeyword], [Format], [IsFullTextInidex], [DisplayOnResultList],
	[DisplayOrderResultList], [DisplayOnExtResultList], [UpdateUserId], [UpdateDate], [Label], [LabelTranslationId], [Revision], [ActiveVersion], [DisplayOrderExtResultList],
	[Comment], [Active], [RelatedAssetTypeID], [RelatedAssetTypeAttributeID], [ValidationExpr], [IsDescription], [IsShownInGrid], [IsShownOnPanel],
	[AllowEditConfig], [AllowEditValue], [ValidationMessage], [IsUsedForNames], [NameGenOrder], [DisplayOrder], [CalculationFormula])
values (@typeUid, 0, N'Update User', null, @assetTypeId, null,
	N'UpdateUserId', null, 0, 0, 0, 0, null, 0, 0,
	0, 0, 1, @updateDate, N'UpdateUser', null, 1, 1, 0,
	null, 1, @userConfigId, @userAttribConfigId, null, 0, 0, 1,
	0, 0, null, 0, null, 100, null)
select @attrUid = [DynEntityAttribConfigUid]
from [dbo].[DynEntityAttribConfig]
where @@ROWCOUNT > 0 and [DynEntityAttribConfigUid] = scope_identity()

insert [dbo].[AttributePanelAttribute]([AttributePanelUid], [DynEntityAttribConfigUId], [DisplayOrder], [UpdateUserId], [UpdateDate], [ReferencingDynEntityAttribConfigId], [ScreenFormula])
values (@panelUid, @attrUid, 4, 1, @updateDate, 0, null)

-- Update Date attribute
insert [dbo].[DynEntityAttribConfig](
	[DynEntityConfigUid], [DynEntityAttribConfigId], [Name], [NameTranslationId], [DataTypeUid], [DynListUid],
	[DBTableFieldname], [ContextId], [IsDynListValue], [IsFinancialInfo], [IsRequired], [IsKeyword], [Format], [IsFullTextInidex], [DisplayOnResultList],
	[DisplayOrderResultList], [DisplayOnExtResultList], [UpdateUserId], [UpdateDate], [Label], [LabelTranslationId], [Revision], [ActiveVersion], [DisplayOrderExtResultList],
	[Comment], [Active], [RelatedAssetTypeID], [RelatedAssetTypeAttributeID], [ValidationExpr], [IsDescription], [IsShownInGrid], [IsShownOnPanel],
	[AllowEditConfig], [AllowEditValue], [ValidationMessage], [IsUsedForNames], [NameGenOrder], [DisplayOrder], [CalculationFormula])
values (@typeUid, 0, N'Update Date', null, @currentdateTypeId, null,
	N'UpdateDate', null, 0, 0, 1, 0, null, 0, 0,
	0, 0, 1, @updateDate, N'UpdateDate', null, 1, 1, 0,
	null, 1, null, null, null, 0, 0, 1,
	0, 0, null, 0, null, 110, null)
select @attrUid = [DynEntityAttribConfigUid]
from [dbo].[DynEntityAttribConfig]
where @@ROWCOUNT > 0 and [DynEntityAttribConfigUid] = scope_identity()

insert [dbo].[AttributePanelAttribute]([AttributePanelUid], [DynEntityAttribConfigUId], [DisplayOrder], [UpdateUserId], [UpdateDate], [ReferencingDynEntityAttribConfigId], [ScreenFormula])
values (@panelUid, @attrUid, 5, 1, @updateDate, 0, null)

-- Assignee attribute
insert [dbo].[DynEntityAttribConfig](
	[DynEntityConfigUid], [DynEntityAttribConfigId], [Name], [NameTranslationId], [DataTypeUid], [DynListUid],
	[DBTableFieldname], [ContextId], [IsDynListValue], [IsFinancialInfo], [IsRequired], [IsKeyword], [Format], [IsFullTextInidex], [DisplayOnResultList],
	[DisplayOrderResultList], [DisplayOnExtResultList], [UpdateUserId], [UpdateDate], [Label], [LabelTranslationId], [Revision], [ActiveVersion], [DisplayOrderExtResultList],
	[Comment], [Active], [RelatedAssetTypeID], [RelatedAssetTypeAttributeID], [ValidationExpr], [IsDescription], [IsShownInGrid], [IsShownOnPanel],
	[AllowEditConfig], [AllowEditValue], [ValidationMessage], [IsUsedForNames], [NameGenOrder], [DisplayOrder], [CalculationFormula])
values (@typeUid, 0, N'Assignee', null, @assetTypeId, null,
	N'Assignee', null, 0, 0, 1, 0, null, 1, 1,
	null, 1, 1, @updateDate, N'Assignee', null, 1, 1, 0,
	null, 1, @userConfigId, @userAttribConfigId, null, 0, 0, 1,
	0, 1, null, 0, null, 111, null)
select @attrUid = [DynEntityAttribConfigUid]
from [dbo].[DynEntityAttribConfig]
where @@ROWCOUNT > 0 and [DynEntityAttribConfigUid] = scope_identity()

insert [dbo].[AttributePanelAttribute]([AttributePanelUid], [DynEntityAttribConfigUId], [DisplayOrder], [UpdateUserId], [UpdateDate], [ReferencingDynEntityAttribConfigId], [ScreenFormula])
values (@panelUid, @attrUid, 1, 1, @updateDate, 0, null)

-- Status attribute
insert [dbo].[DynEntityAttribConfig](
	[DynEntityConfigUid], [DynEntityAttribConfigId], [Name], [NameTranslationId], [DataTypeUid], [DynListUid],
	[DBTableFieldname], [ContextId], [IsDynListValue], [IsFinancialInfo], [IsRequired], [IsKeyword], [Format], [IsFullTextInidex], [DisplayOnResultList],
	[DisplayOrderResultList], [DisplayOnExtResultList], [UpdateUserId], [UpdateDate], [Label], [LabelTranslationId], [Revision], [ActiveVersion], [DisplayOrderExtResultList],
	[Comment], [Active], [RelatedAssetTypeID], [RelatedAssetTypeAttributeID], [ValidationExpr], [IsDescription], [IsShownInGrid], [IsShownOnPanel],
	[AllowEditConfig], [AllowEditValue], [ValidationMessage], [IsUsedForNames], [NameGenOrder], [DisplayOrder], [CalculationFormula])
values (@typeUid, 0, N'Status', null, @dynlistTypeId, @statusListUid,
	N'Status', null, 1, 0, 1, 0, null, 0, 1,
	null, 1, 1, @updateDate, N'Status', null, 1, 1, 0,
	null, 1, null, null, null, 0, 0, 1,
	0, 1, null, 0, null, 113, null)
select @attrUid = [DynEntityAttribConfigUid]
from [dbo].[DynEntityAttribConfig]
where @@ROWCOUNT > 0 and [DynEntityAttribConfigUid] = scope_identity()

insert [dbo].[AttributePanelAttribute]([AttributePanelUid], [DynEntityAttribConfigUId], [DisplayOrder], [UpdateUserId], [UpdateDate], [ReferencingDynEntityAttribConfigId], [ScreenFormula])
values (@panelUid, @attrUid, 3, 1, @updateDate, 0, null)

-- Due date attribute
insert [dbo].[DynEntityAttribConfig](
	[DynEntityConfigUid], [DynEntityAttribConfigId], [Name], [NameTranslationId], [DataTypeUid], [DynListUid],
	[DBTableFieldname], [ContextId], [IsDynListValue], [IsFinancialInfo], [IsRequired], [IsKeyword], [Format], [IsFullTextInidex], [DisplayOnResultList],
	[DisplayOrderResultList], [DisplayOnExtResultList], [UpdateUserId], [UpdateDate], [Label], [LabelTranslationId], [Revision], [ActiveVersion], [DisplayOrderExtResultList],
	[Comment], [Active], [RelatedAssetTypeID], [RelatedAssetTypeAttributeID], [ValidationExpr], [IsDescription], [IsShownInGrid], [IsShownOnPanel],
	[AllowEditConfig], [AllowEditValue], [ValidationMessage], [IsUsedForNames], [NameGenOrder], [DisplayOrder], [CalculationFormula])
values (@typeUid, 0, N'Due date', null, @datetimeTypeId, @statusListUid,
	N'Due_date', null, 1, 0, 1, 0, null, 0, 1,
	null, 1, 1, @updateDate, N'Due date', null, 1, 1, 0,
	null, 1, null, null, null, 0, 0, 1,
	0, 1, null, 0, null, 112, null)
select @attrUid = [DynEntityAttribConfigUid]
from [dbo].[DynEntityAttribConfig]
where @@ROWCOUNT > 0 and [DynEntityAttribConfigUid] = scope_identity()

insert [dbo].[AttributePanelAttribute]([AttributePanelUid], [DynEntityAttribConfigUId], [DisplayOrder], [UpdateUserId], [UpdateDate], [ReferencingDynEntityAttribConfigId], [ScreenFormula])
values (@panelUid, @attrUid, 2, 1, @updateDate, 0, null)

insert [PredefinedAttributes]
(Name, DynEntityConfigID, DynEntityAttribConfigID)
values (@typeName, @typeId, @attrNameId)

declare @viewId bigint;
SELECT @viewId = (MAX(ViewId) + 1) FROM Rights
insert [dbo].[Rights]([ViewId], [UserId], [DynEntityConfigId], [CategoryId], [DepartmentId], [Rights1], [IsDeny], [UpdateDate], [UpdateUserId])
values (@viewId, 1, @typeUid, 0, 0, 31, 0, @updateDate, 1)

exec [dbo].[_cust_RebuildTriggers] @dynEntityConfigUid=@typeUid

exec CreateView @DynEntityConfigUid=@typeUid,@TableName=@tableName