using AssetManager.Infrastructure;
using AssetManager.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace AssetManager.WebApi
{
    /// <summary>
    /// Handler for asset's files downloading
    /// </summary>
    public class FileHandler : IHttpHandler
    {
        private readonly IFileService _fileService;
        private readonly IAssetService _assetService;

        public FileHandler(IFileService fileService, IAssetService assetService)
        {
            if (fileService == null)
                throw new ArgumentNullException("fileService");
            _fileService = fileService;
            if (assetService == null)
                throw new ArgumentNullException("assetService");
            _assetService = assetService;
        }

        public void ProcessRequest(HttpContext context)
        {
            string filePath;
            if (context.Request["assetTypeId"] != null &&
                context.Request["attributeId"] != null &&
                context.Request["assetId"] != null)
            {
                long assetTypeId = long.Parse(context.Request["assetTypeId"]);
                long attributeId = long.Parse(context.Request["attributeId"]);
                long assetId = long.Parse(context.Request["assetId"]);

                var attribute = _assetService.GetAssetAttribute(
                    assetTypeId, assetId, attributeId);
                filePath = _fileService.GetRelativeFilepath(
                    assetTypeId, 
                    attribute.Id,
                    attribute.Datatype,
                    attribute.Value.ToString());
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.End();
                return;
            }

            context.Response.AddHeader(
                "Content-Disposition",
                "attachment; filename=" +
                    Path.GetFileName(filePath));

            var filepath = Path.Combine(
                context.Server.MapPath(filePath));

            context.Response.TransmitFile(filepath);
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}