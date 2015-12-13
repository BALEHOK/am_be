using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.DAL;
using AppFramework.Core.DataTypes;
using AppFramework.DataProxy;
using AppFramework.Entities;
using Ploeh.AutoFixture;
using System.Linq;

namespace AppFramework.Core.UnitTests.Fixtures.AssetTypes
{
    class AssetTypeFixture : AssetType
    {
        public AssetTypeFixture(IUnitOfWork unitOfWork)
            : base(new DynEntityConfig(), unitOfWork)
        {
            var fixture = new Fixture();
            fixture.Customize<DynEntityAttribConfig>(deac =>
                deac.OmitAutoProperties());
            Attributes.Add(new AssetTypeAttribute(fixture.Create<DynEntityAttribConfig>(), unitOfWork) { Name = AttributeNames.DynEntityId, DataType = new LongDataType(), ID = 1 });
            Attributes.Add(new AssetTypeAttribute(fixture.Create<DynEntityAttribConfig>(), unitOfWork) { Name = AttributeNames.DynEntityConfigUid, DataType = new LongDataType(), ID = 2 });
            Attributes.Add(new AssetTypeAttribute(fixture.Create<DynEntityAttribConfig>(), unitOfWork) { Name = AttributeNames.DynEntityUid, DataType = new LongDataType(), ID = 3 });
            Attributes.Add(new AssetTypeAttribute(fixture.Create<DynEntityAttribConfig>(), unitOfWork) { Name = AttributeNames.Revision, DataType = new LongDataType(), ID = 4 });
            Attributes.Add(new AssetTypeAttribute(fixture.Create<DynEntityAttribConfig>(), unitOfWork) { Name = AttributeNames.ActiveVersion, DataType = new BoolDataType(), ID = 5 });
            Attributes.Add(new AssetTypeAttribute(fixture.Create<DynEntityAttribConfig>(), unitOfWork) { Name = AttributeNames.UpdateUserId, DataType = new AssetDataType(), ID = 6 });
            Attributes.Add(new AssetTypeAttribute(fixture.Create<DynEntityAttribConfig>(), unitOfWork) { Name = AttributeNames.UpdateDate, DataType = new DateTimeDataType(), ID = 7 });
            Attributes.Add(new AssetTypeAttribute(fixture.Create<DynEntityAttribConfig>(), unitOfWork) { Name = AttributeNames.Name, DataType = new StringDataType(), ID = 8 });
            AllAttributes = Attributes.ToList();
            Panels.Add(new Panel());
            ID = UID = 1;
        }
    }
}
