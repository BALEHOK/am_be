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
    public class RolesControllerTest
    {
        [Theory, AutoDomainData]
        public void RolesController_ReturnsCollectionOfRoles(
            List<AppFramework.Entities.Role> roles,
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            RolesController sut)
        {
            // Arrange
            unitOfWorkMock.Setup(x => x.RoleRepository.Get(null, null, null, null, null))
                .Returns(roles.AsQueryable());
            // Act
            var result = sut.Get();
            // Assert
            Assert.NotEmpty(result);
        }
    }
}
