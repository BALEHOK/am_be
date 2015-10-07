using AppFramework.Core.Classes;
using AppFramework.Core.DataTypes;
using AppFramework.DataProxy;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.UnitTests.Common.Fixtures
{
    public class EntitiesCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.OmitAutoProperties = true;
            fixture.Customize(new AutoMoqCustomization());
            var unitOfWork = fixture.Create<IUnitOfWork>();
            fixture.Customize(new AssetCustomization(unitOfWork))
                   .Customize(new AssetTypeCustomization(unitOfWork));
            fixture.Register<DataTypeBase>(() => 
            {
                return new CustomDataType();
            });
        }
    }
}
