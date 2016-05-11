using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using AppFramework.Entities;
using Ploeh.AutoFixture;

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
            fixture.Customize<AssetType>(composer =>
                   {
                       return composer.FromFactory(CreateAssetType);
                   });
        }

        private AssetType CreateAssetType()
        {
            return new AssetType(new DynEntityConfig(), _unitOfWork)
            {
                ID = 1,
                Name = "anonymous asset type",
                IsActiveVersion = true
            };
        }
    }
}
