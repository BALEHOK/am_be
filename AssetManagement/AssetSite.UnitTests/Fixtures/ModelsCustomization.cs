using AssetManager.Infrastructure.Models;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSite.UnitTests.Fixtures
{
    class ModelsCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<AssetModel>(c => c.Without(p => p.Taxonomy));
        }
    }
}
