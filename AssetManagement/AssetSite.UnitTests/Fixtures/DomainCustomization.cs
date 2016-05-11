using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

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
