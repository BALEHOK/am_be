using AppFramework.Core.Classes;
using AppFramework.Core.Classes.IE;
using AppFramework.Core.DAL;
using AppFramework.Core.DataTypes;
using AppFramework.DataProxy;
using AppFramework.Entities;
using Ploeh.AutoFixture;
using System.Collections.Generic;

namespace AppFramework.Core.UnitTests.Fixtures.AssetTypes
{
    class WerkloosheidAssetTypeFixture : AssetTypeFixture
    {
        public WerkloosheidAssetTypeFixture(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            var fixture = new Fixture();
            fixture.Customize<DynEntityAttribConfig>(deac =>
               deac.OmitAutoProperties());
            var attribute = new AssetTypeAttribute(fixture.Create<DynEntityAttribConfig>(), unitOfWork)
                {
                    Name = "person",
                    DataType = new AssetDataType(),
                    ID = 9,
                    RelatedAssetTypeID = 123,
                    RelatedAssetTypeAttributeID = 123
                };
            Attributes.Add(attribute);
            AllAttributes.Add(attribute);
        }
    }

    class WerkloosheidBindingsFixture : BindingInfo
    {
        public WerkloosheidBindingsFixture(WerkloosheidAssetTypeFixture assetTypeFixture)
        {
            Bindings.Add(new ImportBinding
            {
                DestinationAttributeId = assetTypeFixture["ActiveVersion"].ID,
                DefaultValue = "true"
            });
            Bindings.Add(new ImportBinding
            {
                DestinationAttributeId = assetTypeFixture["Name"].ID,
                DataSourceFieldName = "Name"
            });
            Bindings.Add(new ImportBinding
            {
                DestinationAttributeId = assetTypeFixture["person"].ID,
                DataSourceFieldName = "PersonName",
                DestinationRelatedAttributeId = assetTypeFixture["person"].RelatedAssetTypeAttributeID
            });
        }
    }
}
