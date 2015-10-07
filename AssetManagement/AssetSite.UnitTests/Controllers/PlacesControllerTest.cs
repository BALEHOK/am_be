using AppFramework.DataProxy;
using AssetManager.WebApi.Controllers.Api;
using AssetSite.UnitTests.Fixtures;
using Moq;
using Ploeh.AutoFixture.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace AssetSite.UnitTests.Controllers
{
    public class PlacesControllerTest
    {
        private IQueryable<AppFramework.Entities.Place> _anonymousPlaces =
            new List<AppFramework.Entities.Place> 
                { 
                    new AppFramework.Entities.Place { PlaceName = "Amougies" },
                    new AppFramework.Entities.Place { PlaceName = "Andenne" },
                    new AppFramework.Entities.Place { PlaceName = "Anderlecht" },
                    new AppFramework.Entities.Place { PlaceName = "Appels" },
                    new AppFramework.Entities.Place { PlaceName = "Ganshoren" },
                }.AsQueryable();

        [Theory, AutoDomainData]
        public void PlacesController_ReturnsCollectionOfPlaces(
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            PlacesController sut)
        {
            // Arrange
            unitOfWorkMock.Setup(x => x.PlaceRepository.AsQueryable())
                .Returns(_anonymousPlaces);
            // Act
            var result = sut.Get(null, null, null);
            // Assert
            Assert.NotEmpty(result);
        }

        [Theory, AutoDomainData]
        public void PlacesController_GetPlacesWithFilter_ReturnsMatchedPlaces(
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            PlacesController sut)
        {
            // Arrange
            var filter = "an";
            unitOfWorkMock.Setup(x => x.PlaceRepository.AsQueryable())
                .Returns(_anonymousPlaces);
            // Act
            var result = sut.Get(filter, null, null);
            // Assert
            Assert.Equal(2, result.Count());
            Assert.True(result.All(p => p.Name.ToLower().StartsWith(filter)));
        }

        [Theory, AutoDomainData]
        public void PlacesController_GetPlacesWithRowStartAndRowNumber_ReturnsSubsetOfPlaces(
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            PlacesController sut)
        {
            // Arrange
            unitOfWorkMock.Setup(x => x.PlaceRepository.AsQueryable())
                .Returns(_anonymousPlaces);
            // Act
            var result = sut.Get(null, 1, 1);
            // Assert
            Assert.Equal(1, result.Count());
            Assert.Equal("Andenne", result.Single().Name);
        }
    }
}
