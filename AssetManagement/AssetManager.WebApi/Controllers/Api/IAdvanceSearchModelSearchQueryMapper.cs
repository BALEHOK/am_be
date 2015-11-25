using AppFramework.Entities;
using AssetManager.WebApi.Models.Search;

namespace AssetManager.WebApi.Controllers.Api
{
    public interface IAdvanceSearchModelSearchQueryMapper
    {
        SearchQuery GetSearchQuery(AdvanceSearchModel model);
        AdvanceSearchModel GetAdvanceSearchModel(SearchQuery searchQuery);
    }
}