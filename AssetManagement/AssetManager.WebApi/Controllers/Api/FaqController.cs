using AppFramework.Core.Classes;
using AssetManager.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AssetManager.WebApi.Controllers.Api
{
    [AllowAnonymous]
    public class FaqController : ApiController
    {
        private readonly IAssetsService _assetsService;

        public FaqController(IAssetsService assetsService)
        {
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
        }

        // GET api/faq
        public IEnumerable<FaqModel> Get()
        {
            var items = _assetsService.GetFaqItems();
            return items.Select(i => new FaqModel
            {
                Question = i["Question"].Value,
                Answer = i["Answer"].Value
            });
        }
    }
}