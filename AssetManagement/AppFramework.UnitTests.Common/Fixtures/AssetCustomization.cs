using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.DataTypes;
using AppFramework.DataProxy;
using AppFramework.Entities;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework.UnitTests.Common.Fixtures
{
    public class AssetCustomization : ICustomization
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssetCustomization(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<AppFramework.Core.Classes.Asset>(composer =>
                   {
                       return composer.FromFactory(CreateAsset);
                   });
        }

        private AppFramework.Core.Classes.Asset CreateAsset()
        {
            return new AppFramework.Core.Classes.Asset
            {
                IsDeleted = false,
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
    }
}
