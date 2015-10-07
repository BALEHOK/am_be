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
    public class AssetTypeCustomization : ICustomization
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssetTypeCustomization(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<AppFramework.Core.Classes.AssetType>(composer =>
                   {
                       return composer.FromFactory(CreateAssetType);
                   });
        }

        private AppFramework.Core.Classes.AssetType CreateAssetType()
        {
            return new AppFramework.Core.Classes.AssetType(new DynEntityConfig(), _unitOfWork)
            {
                ID = 1,
                Name = "anonymous asset type",
                IsActiveVersion = true
            };
        }
    }
}
