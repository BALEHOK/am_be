using AppFramework.Entities;
using AssetManager.Infrastructure.Services;
using AssetSite.UnitTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace AssetSite.UnitTests.Services
{
    public class ExportServiceTests
    {
        [Theory, AutoDomainData]
        public void ExportService_ExportSearchResultToTxt_ReturnsString(
            List<f_cust_SearchByKeywords_Result> searchResults,
            ExportService sut)
        {
            // Arrange
            // Act
            var result = sut.ExportSearchResultToTxt(searchResults);
            // Assert
            Assert.NotEqual(string.Empty, result);
        }

        [Theory, AutoDomainData]
        public void ExportService_ExportSearchResultToXml_ReturnsString(
            List<f_cust_SearchByKeywords_Result> searchResults,
            ExportService sut)
        {
            // Arrange
            // Act
            var result = sut.ExportSearchResultToXml(searchResults);
            // Assert
            Assert.NotEqual(string.Empty, result);
        }

        [Theory, AutoDomainData]
        public void ExportService_ExportSearchResultToExcel_ReturnsString(
            List<f_cust_SearchByKeywords_Result> searchResults,
            ExportService sut)
        {
            // Arrange
            // Act
            var result = sut.ExportSearchResultToExcel(searchResults);
            // Assert
            Assert.NotNull(result);
        }
    }
}
