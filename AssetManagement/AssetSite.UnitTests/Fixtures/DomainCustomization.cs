using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSite.UnitTests.Fixtures
{
    class DomainCustomization : CompositeCustomization
    {
        public DomainCustomization()
            : base(
                new ApiControllerCustomization(),
                new ModelsCustomization(),
                new AutoMoqCustomization()
            )
        {
        }
    }
}
