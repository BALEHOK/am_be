using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.DataTypes;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AppFramework.UnitTests.Common.Fixtures
{
    public class UserAssetCustomization : AssetCustomization
    {
        public UserAssetCustomization(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            Asset.Attributes.Add(new AssetAttribute
            {
                Configuration = new AssetTypeAttribute(
                            new DynEntityAttribConfig(), unitOfWork)
                {
                    DBTableFieldName = "PermissionOnUsers",
                    DataType = new CustomDataType
                    {
                        Code = Enumerators.DataType.Permission
                    },
                },
            });
        }
    }
}
