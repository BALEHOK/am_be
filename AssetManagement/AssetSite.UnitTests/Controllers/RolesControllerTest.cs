using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Services;
using AssetManager.WebApi.Controllers.Api;
using AssetSite.UnitTests.Fixtures;
using Moq;
using Ploeh.AutoFixture.Xunit;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace AssetSite.UnitTests.Controllers
{
    public class RolesControllerTest
    {
        [Theory, AutoDomainData]
        public void RolesController_ReturnsCollectionOfRoles(
            List<PredefinedRoles> roles,
            [Frozen]Mock<IRoleService> roleServiceMock,
            RolesController sut)
        {
            // Arrange
            roleServiceMock.Setup(x => x.GetAllRoles())
                .Returns(roles);
            // Act
            var result = sut.Get();
            // Assert
            Assert.NotEmpty(result);
        }
    }
}
