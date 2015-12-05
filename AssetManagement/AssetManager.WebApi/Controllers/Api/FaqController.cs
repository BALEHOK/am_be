using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AssetManager.WebApi.Controllers.Api
{
    [AllowAnonymous]
    [RoutePrefix("api/faq")]
    public class FaqController : ApiController
    {
        private readonly IFaqService _faqService;

        public FaqController(IFaqService faqService)
        {
            if (faqService == null)
                throw new ArgumentNullException("faqService");
            _faqService = faqService;
        }

        [Route("")]
        public IEnumerable<FaqModel> Get()
        {
            var items = _faqService.GetFaqItems();
            return items.Select(i => new FaqModel
            {
                Question = i["Question"].Value,
                Answer = i["Answer"].Value
            });
        }

        [Route("assettype")]
        public long GetFaqAssetTypeId()
        {
            return _faqService.GetFaqAssetTypeId();
        }
    }
}