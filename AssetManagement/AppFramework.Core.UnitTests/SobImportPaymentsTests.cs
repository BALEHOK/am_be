using AppFramework.Core.UnitTests.Fixtures;
using AppFramework.DataProxy;
using AppFramework.Tasks;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace AppFramework.Core.UnitTests
{
    public class SobImportPaymentsTests
    {
        [Theory(Skip="TBD"), AutoDomainData]
        public void SobImportPayments_DoImport_NoExceptionOccures()
        {
            // Fixture setup
            var unitOfWorkMock = new UnitOfWork();
            var sut = new SobImportPayments(unitOfWorkMock, 353, 360,
                @"c:\temp\Payments", @"c:\temp\Payments\History");
            // Exercise system
            Assert.DoesNotThrow(sut.DoImport);
            // Verify outcome
            // Teardown
        }
    }
}
