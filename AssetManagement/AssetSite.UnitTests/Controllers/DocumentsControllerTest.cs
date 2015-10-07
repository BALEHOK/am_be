using AssetSite.UnitTests.Fixtures;
using Moq;
using Ploeh.AutoFixture.Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;
using Xunit;
using Xunit.Extensions;
using AssetManager.WebApi.Controllers.Api;

namespace AssetSite.UnitTests.Controllers
{
    public class DocumentsControllerTest
    {
        [Theory(Skip="Cannot test claims?"), AutoDomainData]
        public void DocumentsController_ReturnsCollectionOfAssetModels(
            long userId,
            List<AssetModel> documents,
            [Frozen]Mock<IDocumentService> documentServiceMock,
            DocumentsController sut)
        {
            // Arrange
            documentServiceMock.Setup(x => x.GetDocuments(userId, null, null, null))
                .Returns(documents);

            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Sid, "1") });
            Thread.CurrentPrincipal = new GenericPrincipal(identity, null);

            // Act
            var result = sut.Get();
            // Assert
            Assert.NotEmpty(result);
        }
    }
}
