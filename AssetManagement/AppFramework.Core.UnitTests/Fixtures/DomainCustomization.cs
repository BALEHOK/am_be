using AppFramework.UnitTests.Common.Fixtures;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.UnitTests.Fixtures
{
    class DomainCustomization : CompositeCustomization
    {
        public DomainCustomization()
             : base(
                new EntitiesCustomization(),
                new AutoMoqCustomization())
        {
        }        
    }
}
