using Ploeh.AutoFixture.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Extensions;

namespace AppFramework.Core.UnitTests.Fixtures
{
    public class InlineAutoDomainDataAttribute : CompositeDataAttribute
    {
        public InlineAutoDomainDataAttribute(params object[] values)
            : base(new DataAttribute[] { new InlineDataAttribute(values), new AutoDomainDataAttribute() })
        {

        }
    }
}
