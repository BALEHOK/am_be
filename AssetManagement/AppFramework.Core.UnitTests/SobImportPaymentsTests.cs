using AppFramework.Core.Classes.Tasks;
using AppFramework.DataProxy;
using AppFramework.DataProxy.Providers;
using Moq;
using Xunit;

namespace AppFramework.Core.UnitTests
{
    public class SobImportPaymentsTests
    {
        [Fact(Skip="It's not a unit test. Everything must be mocked!")]
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
