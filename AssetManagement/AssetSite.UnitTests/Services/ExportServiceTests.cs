using AppFramework.Entities;
using AssetManager.Infrastructure.Services;
using AssetSite.UnitTests.Fixtures;
using Xunit;
using Xunit.Extensions;

namespace AssetSite.UnitTests.Services
{
    public class ExportServiceTests
    {
        [Theory, AutoDomainData]
        public void ExportService_ExportSearchResultToTxt_ReturnsString(
            SearchTracking searchTracking,
            long userId,
            ExportSearchResultService sut)
        {
            // Arrange
            // Act
            var result = sut.ExportToTxt(searchTracking, userId);
            // Assert
            Assert.NotEqual(0, result.Length);
        }

        [Theory, AutoDomainData]
        public void ExportService_ExportSearchResultToXml_ReturnsString(
            SearchTracking searchTracking,
            long userId,
            ExportSearchResultService sut)
        {
            // Arrange
            // Act
            var result = sut.ExportToXml(searchTracking, userId);
            // Assert
            Assert.NotEqual(0, result.Length);
        }

        [Theory, AutoDomainData]
        public void ExportService_ExportSearchResultToExcel_ReturnsString(
            SearchTracking searchTracking,
            long userId,
            ExportSearchResultService sut)
        {
            // Arrange
            // Act
            var result = sut.ExportToExcel(searchTracking, userId);
            // Assert
            Assert.NotNull(result);
        }
    }
}