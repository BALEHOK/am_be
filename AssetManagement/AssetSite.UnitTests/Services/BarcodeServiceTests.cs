using AppFramework.Core.Classes.Barcode;
using AssetManager.Infrastructure;
using AssetManager.Infrastructure.Services;
using AssetSite.UnitTests.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace AssetSite.UnitTests.Services
{
    public class BarcodeServiceTests
    {
        [Theory, AutoDomainData]
        public void BarcodeService_GenerateBarcode_ReturnsBarcodeString(
            string barcode,
            IEnvironmentSettings envSettings,
            Mock<IBarcodeProvider> barcodeProviderMock)
        {
            // Arrange
            barcodeProviderMock.Setup(x => x.GenerateBarcode()).Returns(barcode);
            var sut = new BarcodeService(envSettings, barcodeProviderMock.Object);
            // Act 
            var result = sut.GenerateBarcode();
            // Assert
            Assert.NotNull(result);
            Assert.Equal(barcode, result);
        }
    }
}
