using Xunit;
using Xunit.Extensions;

namespace AppFramework.Reports.UnitTests
{
    public class ReportFactoryTest
    {
        [Theory]
        [InlineData(ReportType.AssetReport)]
        [InlineData(ReportType.AssetsListReport)]
        [InlineData(ReportType.SearchResultReport)]
        public void ReportFactory_BuildReportByType_ReturnsObject(ReportType reportType)
        {
            // Arrange
            // Act
            var result = ReportFactory.BuildReport(reportType);
            // Assert
            Assert.NotNull(result);
        }
    }
}
