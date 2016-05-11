using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.DataTypes;
using AppFramework.DataProxy;
using AppFramework.Entities;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;

namespace AppFramework.UnitTests.Common.Fixtures
{
    public class AssetCustomization : ICustomization
    {
        protected Asset Asset { get; private set; }        

        private readonly IUnitOfWork _unitOfWork;

        public AssetCustomization(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Asset = new Asset
            {
                Configuration = new AssetType(new DynEntityConfig(), _unitOfWork)
                {
                    ID = 1
                },
                Attributes = new List<AssetAttribute>
                {
                    new AssetAttribute
                    {
                        Configuration = new AssetTypeAttribute(
                            new DynEntityAttribConfig(), _unitOfWork)
                        {
                            DBTableFieldName = AttributeNames.DynEntityId,
                            DataType = new CustomDataType
                            {
                                Code = Enumerators.DataType.Long
                            }
                        },
                        Value = "1"
                    },
                    new AssetAttribute
                    {
                        Configuration = new AssetTypeAttribute(
                            new DynEntityAttribConfig(), _unitOfWork)
                        {
                            DBTableFieldName = AttributeNames.DynEntityUid,
                            DataType = new CustomDataType
                            {
                                Code = Enumerators.DataType.Long
                            }
                        },
                        Value = "1"
                    },
                    new AssetAttribute
                    {
                        Configuration = new AssetTypeAttribute(
                            new DynEntityAttribConfig(), _unitOfWork)
                        {
                            DBTableFieldName = AttributeNames.DynEntityConfigId,
                            DataType = new CustomDataType
                            {
                                Code = Enumerators.DataType.Long
                            }
                        },
                        Value = "1"
                    },
                    new AssetAttribute
                    {
                        Configuration = new AssetTypeAttribute(
                            new DynEntityAttribConfig(), _unitOfWork)
                        {
                            DBTableFieldName = AttributeNames.DynEntityConfigUid,
                            DataType = new CustomDataType
                            {
                                Code = Enumerators.DataType.Long
                            }
                        },
                        Value = "1"
                    },
                    new AssetAttribute
                    {
                        Configuration = new AssetTypeAttribute(
                            new DynEntityAttribConfig(), _unitOfWork)
                        {
                            DBTableFieldName = AttributeNames.Revision,
                            DataType = new CustomDataType
                            {
                                Code = Enumerators.DataType.Int
                            }
                        },
                        Value = "1"
                    },
                    new AssetAttribute
                    {
                        Configuration = new AssetTypeAttribute(
                            new DynEntityAttribConfig(), _unitOfWork)
                        {
                            DBTableFieldName = AttributeNames.UpdateDate,
                            DataType = new CustomDataType
                            {
                                Code = Enumerators.DataType.DateTime
                            }
                        },
                        Value = DateTime.Now.ToString()
                    },
                    new AssetAttribute
                    {
                        Configuration = new AssetTypeAttribute(
                            new DynEntityAttribConfig(), _unitOfWork)
                        {
                            DBTableFieldName = "asset",
                            DataType = new CustomDataType
                            {
                                Code = Enumerators.DataType.Asset
                            },
                            RelatedAssetTypeID = 1,
                            RelatedAssetTypeAttributeID = 1,
                        },
                    },
                    new AssetAttribute
                    {
                        Configuration = new AssetTypeAttribute(
                            new DynEntityAttribConfig(), _unitOfWork)
                        {
                            DBTableFieldName = "assets",
                            DataType = new CustomDataType
                            {
                                Code = Enumerators.DataType.Assets
                            },
                            RelatedAssetTypeID = 1,
                            RelatedAssetTypeAttributeID = 1
                        },
                    },
                    new AssetAttribute
                    {
                        Configuration = new AssetTypeAttribute(
                            new DynEntityAttribConfig(), _unitOfWork)
                        {
                            DBTableFieldName = AttributeNames.Name,
                            IsShownOnPanel = true,
                            DataType = new CustomDataType
                            {
                                Code = Enumerators.DataType.String
                            },
                        },
                    },
                    new AssetAttribute
                    {
                        Configuration = new AssetTypeAttribute(
                            new DynEntityAttribConfig()
                            {
                                DBTableFieldname  = AttributeNames.UpdateUserId,
                            }, _unitOfWork)
                        {
                            IsShownOnPanel = true,
                            DataType = new CustomDataType
                            {
                                Code = Enumerators.DataType.String
                            },
                        },
                        Value = "admin",
                        ValueAsId = 1
                    },
                    new AssetAttribute
                    {
                        Configuration = new AssetTypeAttribute(
                            new DynEntityAttribConfig()
                            {
                                DBTableFieldname  = AttributeNames.ActiveVersion,
                            }, _unitOfWork)
                        {
                            DataType = new CustomDataType
                            {
                                Code = Enumerators.DataType.Bool
                            },
                        },
                        Value = "true",
                    },
                    new AssetAttribute
                    {
                        Configuration = new AssetTypeAttribute(
                            new DynEntityAttribConfig()
                            {
                                DBTableFieldname  = AttributeNames.IsDeleted,
                            }, _unitOfWork)
                        {
                            DataType = new CustomDataType
                            {
                                Code = Enumerators.DataType.Bool
                            },
                        },
                        Value = "false",
                    },
                    new AssetAttribute
                    {
                        Configuration = new AssetTypeAttribute(
                            new DynEntityAttribConfig(), _unitOfWork)
                        {
                            DBTableFieldName = AttributeNames.Role,
                            DataType = new CustomDataType
                            {
                                Code = Enumerators.DataType.Role
                            },
                        },
                    },
                }
            };
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<Asset>(composer =>
            {
                return composer.FromFactory(() => { return Asset; });
            });
        }
    }
}
