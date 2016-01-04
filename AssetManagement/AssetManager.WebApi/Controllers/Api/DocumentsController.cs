using System.Collections.Generic;
using System.Web.Http;
using AssetManager.Infrastructure.Extensions;
using AssetManager.Infrastructure.Services;
using AssetManager.Infrastructure.Models;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/documents")]
    public class DocumentsController : ApiController
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            if (documentService == null)
                throw new System.ArgumentNullException("documentService");
            _documentService = documentService;
        }

        [Route("")]
        public IEnumerable<AssetModel> Get(string query = null, int? rowStart = 1, int? rowsNumber = 20)
        {
            var userId = User.GetId();
            return _documentService.GetDocuments(userId, query, rowStart, rowsNumber);
        }
    }
}
